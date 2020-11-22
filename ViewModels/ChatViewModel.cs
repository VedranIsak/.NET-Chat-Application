using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Input;

using TDDD49.Models;
using TDDD49.ViewModels.Commands;
using System.IO;
using Newtonsoft.Json;
using System.Windows;

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
        private ObservableCollection<Message> messages;
        private Communicator communicator;
        private Thread recieveMessageThread;
        public bool CanRecieve { get; set; } = false;

        public ChatViewModel(Communicator c)
        {
            SendCommand = new SendCommand(this);
            SwitchUserCommand = new SwitchUserCommand(this);
            DisconnectCommand = new DisconnectCommand(c, this);
            BuzzCommand = new BuzzCommand(c, this);
            Users = new ObservableCollection<User>();
            FilteredUsers = new ObservableCollection<User>();
            ReadFromJSON();
            communicator = c;
            ReadMessage();
        }

        public ICommand SendCommand { get; set; }
        public ICommand SwitchUserCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }
        public ICommand BuzzCommand { get; set; }

        private void ReadFromJSON()
        {
            List<User> tmp;
            using (StreamReader usersReader = new StreamReader("../../UsersStorage.json"))
            {
                string inputUsersString = usersReader.ReadToEnd();
                tmp = JsonConvert.DeserializeObject<List<User>>(inputUsersString).ToList<User>();
               
            }

            ObservableCollection<User> tmpObservable = new ObservableCollection<User>();
            foreach (var user in tmp)
            {
                tmpObservable.Add(user);
            }
            Users = tmpObservable;
            ExternalUser = Users.ElementAt(0);
            Messages = ExternalUser.Messages;

            using (StreamReader userReader = new StreamReader("../../UserStorage.json"))
            {
                string inputUserString = userReader.ReadToEnd();
                InternalUser = JsonConvert.DeserializeObject<User>(inputUserString);

            }

            if(InternalUser == null) { return; }
            if (InternalUser.ID == null)
            {
                InternalUser.ID = GetHashCode();
                using (StreamWriter writer = new StreamWriter("../../UsersStorage.json", false))
                {
                    writer.Write(JsonConvert.SerializeObject(InternalUser));
                }
            }
        }

        private void WriteUsersToJSON(Message newMessage)
        {
            List<User> tmp;
            using (StreamReader usersReader = new StreamReader("../../UsersStorage.json"))
            {
                tmp = JsonConvert.DeserializeObject<List<User>>(usersReader.ReadToEnd()).ToList();

            }
            
            if (!tmp.Any(item => item.ID == this.externalUser.ID))
            {
                if (this.externalUser.Messages == null)
                {
                    this.externalUser.Messages = new ObservableCollection<Message>();
                }
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

        public void WriteUserToJSON()
        {
            using (StreamWriter writer = new StreamWriter("../../UserStorage.json", false))
            {
                writer.Write(JsonConvert.SerializeObject(InternalUser));
            }
        }

        public void AddMessage(Message newMessage)
        {
            Messages.Add(newMessage);
            WriteUsersToJSON(newMessage);
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

        public ObservableCollection<Message> Messages
        {
            get 
            {
                if (messages == null)
                {
                    messages = new ObservableCollection<Message>();
                }
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
                    Messages = new ObservableCollection<Message>();
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
            Message mes = new Message()
            {
                Content = message,
                Sender = this.internalUser,
                TimePosted = DateTime.Now,
                MessageType = "message",
                IsInternalUserMessage = true
            };

            // vid merge okommentera nedan
            Console.WriteLine("Write message");
            try
            {
                Thread t = new Thread(() =>
                {
                    try
                    {
                        communicator.sendMessage(mes);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            AddMessage(mes);
                        });
                    }
                    catch (NullReferenceException e)
                    {
                        MessageBox.Show("No connection, try connecting again", "No connection");
                        Console.WriteLine(e);
                    }
                    
                });
                t.IsBackground = true;
                t.Start();
                
            }
            catch (SocketException err)
            {
                MessageBox.Show("Connection lost, try connnecting again!", "Lost connection");
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
                    Message message;
                    while (true)
                    {
                        message = null;
                        if (CanRecieve)
                        {
                            try
                            {
                                message = this.communicator.recieveMessage();
                                message.IsInternalUserMessage = false;
                            }
                            catch (ObjectDisposedException e1)
                            {
                                MessageBox.Show("Connection lost");
                                CanRecieve = false;
                                continue;
                            }

                            if (message == null)
                            {
                                continue;
                            }
                            else if (message.MessageType == "disconnect")
                            {
                                CanRecieve = false;
                                continue;
                            }
                            else if (message.MessageType == "buzz")
                            {
                                continue;
                            }
                            else
                            {
                                Console.WriteLine("adding message");
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    AddMessage(message);
                                });
                            }
                        }
                    }
                });
                this.recieveMessageThread.IsBackground = true;
                this.recieveMessageThread.Start();
            }
            catch (SocketException e1)
            {
                MessageBox.Show("Connection lost, try connnecting again!", "Lost connection");
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
