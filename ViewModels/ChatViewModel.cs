using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TDDD49.Models;
using TDDD49.ViewModels.Commands;

namespace TDDD49.ViewModels
{
    public class ChatViewModel : ViewModel
    {
        private string searchQuery;
        private User externalUser;
        public User chattingUser;
        private string externalUserName;
        private ObservableCollection<User> users;
        private ObservableCollection<User> filteredUsers;
        private ObservableCollection<Message> messages;
        private Communicator communicator;
        private InternalCommunicator internalCommunicator;

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
            internalCommunicator.ReadFromJson();
            ReadMessage();

            if (InternalUser == null) { InternalUser = new User(); }
        }

        public ICommand SendCommand { get; set; }
        public ICommand SwitchUserCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }
        public ICommand BuzzCommand { get; set; }

        private void WriteMessageToJson(Message newMessage) { internalCommunicator.WriteMessageToJson(newMessage); }

        public void WriteUserToJSON()
        {
            internalCommunicator.WriteUserToJson();
        }

        public void AddUser(User newUser)
        {
            Users.Add(newUser);
            internalCommunicator.WriteUsersToJson();
        }

        public void AddMessage(Message newMessage)
        {
            Messages.Add(newMessage);
            internalCommunicator.WriteMessageToJson(newMessage);
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
            get { return messages ?? new ObservableCollection<Message>(); }
            set
            {
                messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        public User InternalUser { get; set; }

        public User ChattingUser
        {
            get { return chattingUser; }
            set { chattingUser = value; }
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
