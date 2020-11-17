using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Input;

using TDDD49.Models;
using TDDD49.ViewModels.Commands;
using System.IO;
using Newtonsoft.Json;

namespace TDDD49.ViewModels
{
    public class ChatViewModel : ViewModel
    {
        private string searchQuery;
        private User internalUser;
        private User externalUser;
        private string externalUserName;
        private ObservableCollection<User> users;
        private ObservableCollection<User> filteredUsers;
        private ObservableCollection<Models.Message> messages;
        private Communicator communicator;
        private Thread recieveMessageThread;
        public bool CanRecieve { get; set; } = false;

        public ChatViewModel(Communicator c)
        {
            SendCommand = new SendButtonCommand(this);
            SwitchUserCommand = new SwitchUserCommand(this);
            DisconnectButtonCommand = new DisconnectButtonCommand();
            ReadFromJSON();
            communicator = c;
            ReadMessage();
        }

        public ICommand SendCommand { get; set; }
        public ICommand SwitchUserCommand { get; set; }
        public ICommand DisconnectButtonCommand { get; set; }

        private void ReadFromJSON()
        {
            using (StreamReader usersReader = new StreamReader("../../UsersStorage.json"))
            {
                string inputUsersString = usersReader.ReadToEnd();
                List<User> tmp = JsonConvert.DeserializeObject<List<User>>(inputUsersString).ToList<User>();
                ObservableCollection<User> tmpObservable = new ObservableCollection<User>();
                foreach (var user in tmp)
                {
                    tmpObservable.Add(user);
                }
                Users = tmpObservable;
                ExternalUser = Users.ElementAt(0);
                Messages = ExternalUser.Messages;
            }
            using (StreamReader userReader = new StreamReader("../../UserStorage.json"))
            {
                string inputUserString = userReader.ReadToEnd();
                InternalUser = JsonConvert.DeserializeObject<User>(inputUserString);
            }
        }

        private void WriteToJSON(Models.Message newMessage)
        {
            var json = File.ReadAllText("../../UsersStorage.json");
            List<User> tmp = JsonConvert.DeserializeObject<List<User>>(json).ToList<User>();

            if (!tmp.Any(item => item.ID == this.externalUser.ID))
            {
                this.externalUser.Messages.Add(newMessage);
                tmp.Add(this.externalUser);
            }
            else
            {
                foreach (User u in tmp)
                {
                    if (u.ID == this.externalUser.ID)
                    {
                        u.Messages.Add(newMessage);
                        break;
                    }
                }
            }

            string jsonOut = JsonConvert.SerializeObject(tmp);

            using (StreamWriter writer = new StreamWriter("../../UsersStorage.json", false))
            {
                writer.Write(jsonOut);
            }
        }

        public void AddMessage(Models.Message newMessage)
        {
            Messages.Add(newMessage);
            WriteToJSON(newMessage);
        }

        public string SearchQuery
        {
            get { return searchQuery; }
            set
            {
                searchQuery = value;
                OnPropertyChanged(SearchQuery);
                FilterSearchQuery(searchQuery);
            }
        }

        private void FilterSearchQuery(string query)
        {
            ObservableCollection<User> userTmp = Users;
            List<User> tmp = userTmp.Where((user) => user.Name.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) >= 0).ToList();
            ObservableCollection<User> observableTmp = new ObservableCollection<User>();
            foreach(var user in tmp)
            {
                observableTmp.Add(user);
            }
            FilteredUsers = observableTmp;
        }

        public ObservableCollection<User> FilteredUsers
        {
            get { return filteredUsers; }
            set
            {
                filteredUsers = value;
                OnPropertyChanged(nameof(FilteredUsers));
            }
        }

        public ObservableCollection<User> Users
        {
            get { return users; }
            set
            {
                users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        public ObservableCollection<Models.Message> Messages
        {
            get 
            { 
                return messages; 
            }
            set
            {
                messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        public User InternalUser
        {
            get { return internalUser; }
            set { internalUser = value; }
        }

        public User ExternalUser
        {
            get { return externalUser; }
            set
            {
                externalUser = value;
                ExternalUserName = externalUser.Name;
                if (externalUser.Messages == null)
                {
                    Messages = new ObservableCollection<Models.Message>();
                }
                else
                {
                    Messages = externalUser.Messages;
                }
            }
        }

        public string ExternalUserName
        {
            get { return externalUserName; }
            set
            {
                externalUserName = value;
                OnPropertyChanged(nameof(ExternalUserName));
            }
        }

        public void WriteMessage(string message)
        {
            Console.WriteLine(this.externalUser.Name);
            Models.Message mes = new Models.Message()
            {
                Content = message,
                Sender = this.internalUser,
                TimePosted = DateTime.Now,
                MessageType = "message",
                IsInternalUserMessage = false
            };

            // vid merge okommentera nedan
            Console.WriteLine("Write message");
            this.AddMessage(mes);
            mes.IsInternalUserMessage = false;
            try
            {
                Thread t = new Thread(() =>
                {
                    communicator.sendMessage(mes);
                });
                t.IsBackground = true;
                t.Start();
                
            }
            catch (SocketException err)
            {
                System.Windows.MessageBox.Show("Connection lost, try connnecting again!", "Lost connection");
                communicator.disconnectStream();
                Console.WriteLine(err);
            }
        }

        private void ReadMessage()
        {
            try
            {
                this.recieveMessageThread = new Thread(() =>
                {
                    Models.Message message = null;
                    while (true)
                    {
                        
                        if (CanRecieve)
                        {
                            try
                            {
                                message = this.communicator.recieveMessage();

                            }
                            catch (ObjectDisposedException e1)
                            {
                                System.Windows.MessageBox.Show("Connection lost");
                                CanRecieve = false;
                            }

                            if (message == null)
                            {
                                continue;
                            }

                            else if (message.MessageType == "disconnect")
                            {
                                CanRecieve = false;
                            }
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                AddMessage(message);
                            });
                        }
                    }
                });
                this.recieveMessageThread.IsBackground = true;
                this.recieveMessageThread.Start();
            }
            catch (SocketException e1)
            {
                System.Windows.MessageBox.Show("Connection lost, try connnecting again!", "Lost connection");
                CanRecieve = false;
                communicator.disconnectStream();
                Console.WriteLine(e1);
            }
            catch (JsonReaderException e2)
            {
                Console.WriteLine(e2);
            }
            
        }
    }
}
