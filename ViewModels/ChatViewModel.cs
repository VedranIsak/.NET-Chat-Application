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
        private User visibleUser;
        public User chattingUser;
        private string visibleUserName;
        private ObservableCollection<User> users;
        private ObservableCollection<User> filteredUsers;
        private ObservableCollection<Message> visibleMessages;
        private ObservableCollection<Message> chattingMessages;
        private Communicator communicator;
        private InternalCommunicator internalCommunicator;
        private Thread recieveMessageThread;
        public bool CanRecieve { get; set; } = false;

        public ChatViewModel(Communicator c)
        {
            ChatCommand = new ChatCommand(this);
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

        public ICommand ChatCommand { get; set; }
        public ICommand SendCommand { get; set; }
        public ICommand SwitchUserCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }
        public ICommand BuzzCommand { get; set; }

        private void WriteMessageToJson(Message newMessage) { internalCommunicator.WriteMessageToJson(newMessage); }

        public void WriteUserToJSON()  { internalCommunicator.WriteUserToJson(); }

        public void AddUser(User newUser)
        {
            Users.Add(newUser);
            internalCommunicator.WriteUsersToJson();
        }

        public void AddChattingMessage(Message newMessage)
        {
            ChattingMessages.Add(newMessage);
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

        public ObservableCollection<Message> VisibleMessages
        {
            get { return visibleMessages ?? new ObservableCollection<Message>(); }
            set
            {
                visibleMessages = value;
                OnPropertyChanged(nameof(VisibleMessages));
            }
        }

        public ObservableCollection<Message> ChattingMessages
        {
            get { return chattingMessages ?? new ObservableCollection<Message>(); }
            set
            {
                chattingMessages = value;
                OnPropertyChanged(nameof(ChattingMessages));
            }
        }

        public User InternalUser
        {
            get { return internalUser; }
            set { internalUser = value; }
        }

        public User ChattingUser
        {
            get { return chattingUser; }
            set
            {
                chattingUser = value;
                VisibleUserName = chattingUser.Name;
                VisibleMessages = chattingUser.Messages;
                ChattingMessages = chattingUser.Messages ?? new ObservableCollection<Message>();
            }
        }

        public User VisibleUser
        {
            get { return visibleUser; }
            set
            {
                visibleUser = value;
                VisibleUserName = visibleUser.Name;
                VisibleMessages = visibleUser.Messages ?? new ObservableCollection<Message>();
            }
        }

        public string VisibleUserName
        {
            get { return visibleUserName; }
            set
            {
                visibleUserName = value;
                OnPropertyChanged(nameof(VisibleUserName));
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
