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
using TDDD49.Communication;

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
        private InternalCommunicator internalCommunicator;
        private Thread recieveMessageThread;
        public bool CanRecieve { get; set; } = false;
        public User chattingWith;

        public ChatViewModel(Communicator c)
        {
            SendCommand = new SendCommand(this);
            SwitchUserCommand = new SwitchUserCommand(this);
            DisconnectCommand = new DisconnectCommand(c, this);
            BuzzCommand = new BuzzCommand(c, this);
            Users = new ObservableCollection<User>();
            FilteredUsers = new ObservableCollection<User>();
            communicator = c;
            internalCommunicator = new InternalCommunicator(this);
            ReadFromJSON();
            ReadMessage();

            if (InternalUser == null) { InternalUser = new User(); }
        }

        public ICommand SendCommand { get; set; }
        public ICommand SwitchUserCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }
        public ICommand BuzzCommand { get; set; }

        private void ReadFromJSON()
        {
<<<<<<< HEAD
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
            if (tmp?.Any() == true)
            {
                Users = tmpObservable;
                ExternalUser = Users.ElementAt(0);
                Messages = ExternalUser.Messages;
            }
=======
            internalCommunicator.ReadFromJSON();
            //List<User> tmp;
            //using (StreamReader usersReader = new StreamReader("../../UsersStorage.json"))
            //{
            //    string inputUsersString = usersReader.ReadToEnd();
            //    tmp = JsonConvert.DeserializeObject<List<User>>(inputUsersString).ToList<User>();

            //}
>>>>>>> 7e321f9efdc13d66efd50e631b87c04f290f1c6e

            //ObservableCollection<User> tmpObservable = new ObservableCollection<User>();
            //foreach (var user in tmp)
            //{
            //    tmpObservable.Add(user);
            //}
            //if (tmp?.Any() == true)
            //{
            //    Users = tmpObservable;
            //    ExternalUser = Users.ElementAt(0);
            //    Messages = ExternalUser.Messages;
            //}

            //using (StreamReader userReader = new StreamReader("../../UserStorage.json"))
            //{
            //    string inputUserString = userReader.ReadToEnd();
            //    InternalUser = JsonConvert.DeserializeObject<User>(inputUserString);

<<<<<<< HEAD
            if (InternalUser == null) { return; }
            if (InternalUser.ID == null)
            {
                InternalUser.ID = GetHashCode();
                using (StreamWriter writer = new StreamWriter("../../UsersStorage.json", false))
                {
                    writer.Write(JsonConvert.SerializeObject(InternalUser));
                }
            }
=======
            //}

            //if (InternalUser == null) { return; }
            //if (InternalUser.ID == null)
            //{
            //    InternalUser.ID = GetHashCode();
            //    using (StreamWriter writer = new StreamWriter("../../UsersStorage.json", false))
            //    {
            //        writer.Write(JsonConvert.SerializeObject(InternalUser));
            //    }
            //}
>>>>>>> 7e321f9efdc13d66efd50e631b87c04f290f1c6e
        }

        private void WriteUsersToJSON(Message newMessage)
        {
<<<<<<< HEAD
            List<User> tmp;
            using (StreamReader usersReader = new StreamReader("../../UsersStorage.json"))
            {
                tmp = JsonConvert.DeserializeObject<List<User>>(usersReader.ReadToEnd()).ToList();
            }

            if (tmp?.Any() == true)
            {
                tmp = new List<User>();
                if (this.chattingWith.Messages == null)
                {
                    this.chattingWith.Messages = new ObservableCollection<Message>();
                }
                chattingWith.Messages.Add(newMessage);
                tmp.Add(chattingWith);
            }
            else
            {
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
            }
=======
            internalCommunicator.WriteUsersToJSON(newMessage);
        //    List<User> tmp;
        //    using (StreamReader usersReader = new StreamReader("../../UsersStorage.json"))
        //    {
        //        tmp = JsonConvert.DeserializeObject<List<User>>(usersReader.ReadToEnd()).ToList();

        //    }

        //    if (tmp?.Any() == true)
        //    {
        //        tmp = new List<User>();
        //        if (this.externalUser.Messages == null)
        //        {
        //            this.externalUser.Messages = new ObservableCollection<Message>();
        //        }
        //        ExternalUser.Messages.Add(newMessage);
        //        tmp.Add(ExternalUser);
        //    }
        //    else
        //    {
        //        if (!tmp.Any(item => item.ID == this.externalUser.ID))
        //        {
        //            if (this.externalUser.Messages == null)
        //            {
        //                this.externalUser.Messages = new ObservableCollection<Message>();
        //            }
        //            this.externalUser.Messages.Add(newMessage);
        //            tmp.Add(this.externalUser);
        //        }
        //        else
        //        {
        //            foreach (User u in tmp)
        //            {
        //                if (u.ID == this.externalUser.ID)
        //                {
        //                    u.Messages.Add(newMessage);
        //                    break;
        //                }
        //            }
        //        }
        //    }
>>>>>>> 7e321f9efdc13d66efd50e631b87c04f290f1c6e

        //    string jsonOut = JsonConvert.SerializeObject(tmp);

        //    using (StreamWriter writer = new StreamWriter("../../UsersStorage.json", false))
        //    {
        //        writer.Write(jsonOut);
        //    }
        }

        public void WriteUserToJSON()
        {
            internalCommunicator.WriteUserToJSON();
            //using (StreamWriter writer = new StreamWriter("../../UserStorage.json", false))
            //{
            //    writer.Write(JsonConvert.SerializeObject(InternalUser));
            //}
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
            foreach (var user in tmp)
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
            communicator.WriteMessage(message, InternalUser);
        }

        private void ReadMessage()
        {
            communicator.ReadMessage();
        }
    }
}
