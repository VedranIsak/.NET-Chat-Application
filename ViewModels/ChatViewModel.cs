using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;

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
        private ObservableCollection<User> users;
        private ObservableCollection<User> filteredUsers;
        private ObservableCollection<Models.Message> messages;
        private Communicator communicator;
        private Thread recieveMessageThread;

        public ChatViewModel(Communicator c)
        {
            //Users = new ObservableCollection<User>();
            /*Users.Add(new User() { Name = "Paulie", Port = 8080, IpAddress = "localhost",
                Messages = new ObservableCollection<Models.Message>()
                { new Models.Message() { Content="sdsjdaasjd", IsInternalUserMessage=false, TimePosted=DateTime.Now },
                new Models.Message() { Content="ajasjasjasasj", IsInternalUserMessage=false, TimePosted=DateTime.Now }
                } });*/
            //FilteredUsers = new ObservableCollection<User>();
            SendCommand = new SendButtonCommand(this);
            SwitchUserCommand = new SwitchUserCommand(this);
            ReadFromJSON();
            //ExternalUser = Users.ElementAt(0);
            //InternalUser = Users.ElementAt(0);
            communicator = c;
        }

        public ICommand SendCommand { get; set; }
        public ICommand SwitchUserCommand { get; set; }

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
                externalUser = Users.ElementAt(0);
                Messages = externalUser.Messages;
            }
            using (StreamReader userReader = new StreamReader("../../UserStorage.json"))
            {
                string inputUserString = userReader.ReadToEnd();
                internalUser = JsonConvert.DeserializeObject<User>(inputUserString);
            }
        }

        private void WriteToJSON(Models.Message newMessage)
        {
            using (StreamReader usersReader = new StreamReader("../../UsersStorage.json"))
            {
                string inputUsersString = usersReader.ReadToEnd();
                List<User> tmp = JsonConvert.DeserializeObject<List<User>>(inputUsersString).ToList<User>();
                bool userIsStored = false;
                foreach (var user in tmp)
                {
                    if (user.ID == ExternalUser.ID)
                    {
                        user.Messages.Add(newMessage);
                        userIsStored = true;
                    }
                }
                //userIsStored är true ifall användaren redan är sparad i jsonfilen, då lägger man bara till det nya meddelandet till användarens Messages
                //Annars måste man lägga till en ny användare till listan som hämtas från jsonfilen
                if(!userIsStored)
                {
                    tmp.Add(ExternalUser);
                }
                string jsonList = JsonConvert.SerializeObject(tmp);
                File.WriteAllText("../../UsersStorage.json", jsonList);
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

        public ObservableCollection<TDDD49.Models.Message> Messages
        {
            get { return messages; }
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

        //public string InternalUserName
        //{
        //    get { return InternalUser.Name; }
        //    set { InternalUser.Name = value; }
        //}

        //public int InternalPort
        //{
        //    get { return InternalUser.Port; }
        //    set { InternalUser.Port = value; }
        //}

        //public string InternalIpAddress
        //{
        //    get { return InternalUser.IpAddress; }
        //    set { InternalUser.IpAddress = value; }
        //}

        public User ExternalUser
        {
            get { return externalUser; }
            set
            {
                externalUser = value;
                Messages = externalUser.Messages;
            }
        }

        public void WriteMessage(string message)
        {
            Models.Message mes = new Models.Message()
            {
                Content = message,
                Sender = this.internalUser.Name,
                TimePosted = DateTime.Now,
                MessageType = "message",
                IsInternalUserMessage = true
            };
            mes.IsInternalUserMessage = false;
            try
            {
                new Thread(() =>
                {
                    communicator.sendMessage(mes);
                });
                
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
                        message = this.communicator.recieveMessage();

                        if (message != null && message.MessageType == "disconnect")
                        {
                            break;
                        }
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            //när merge, kommentera bort under detta
                            AddMessage(message);
                        });
                    }
                });
                this.recieveMessageThread.IsBackground = true;
                this.recieveMessageThread.Start();
            }
            catch (SocketException e1)
            {
                System.Windows.MessageBox.Show("Connection lost, try connnecting again!", "Lost connection");
                communicator.disconnectStream();
                Console.WriteLine(e1);
            }
            catch (Newtonsoft.Json.JsonReaderException e2)
            {
                Console.WriteLine(e2);
            }
            
        }
    }
}
