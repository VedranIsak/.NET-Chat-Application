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
        private User visibleUser;
        private User chattingUser;
        private string visibleUserName;
        private ObservableCollection<User> users;
        private ObservableCollection<User> filteredUsers;
        private ObservableCollection<Message> visibleMessages;
        private ObservableCollection<Message> chattingMessages;
        private Communicator communicator;
        private HistoryViewModel historyViewModel;
        private InternalCommunicator internalCommunicator;

        public ChatViewModel(Communicator communicator, HistoryViewModel historyViewModel)
        {
            this.communicator = communicator;
            this.historyViewModel = historyViewModel;
      
            SendCommand = new SendCommand(this);
            SwitchUserCommand = new SwitchUserCommand(this);
            historyViewModel.SwitchUserCommand = SwitchUserCommand;
            DisconnectCommand = new DisconnectCommand(communicator, this);
            BuzzCommand = new BuzzCommand(communicator, this);
            Users = new ObservableCollection<User>();
            FilteredUsers = new ObservableCollection<User>();
            internalCommunicator = new InternalCommunicator(this);
            internalCommunicator.ReadFromJson();
            ReadMessage();

            if (InternalUser == null) { InternalUser = new User(); }
        }

        public ICommand SendCommand { get; set; }
        public ICommand SwitchUserCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }
        public ICommand BuzzCommand { get; set; }

        public void WriteUserToJSON()  { internalCommunicator.WriteUserToJson(); }

        public void AddUser(User newUser)
        {
            Users.Add(newUser);
            internalCommunicator.WriteUsersToJson();
        }

        public void AddMessage(Message newMessage)
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
            if(query != null)
            {
                if (query != String.Empty) { FilteredUsers = observableTmp; }
                else { FilteredUsers = new ObservableCollection<User>(); }
            }
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
            get { return historyViewModel.Users; }
            set { historyViewModel.Users = value; }
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

        public User InternalUser { get; set; }

        public User ChattingUser
        {
            get { return chattingUser; }
            set
            {
                chattingUser = value;
                VisibleUser = value;
                if(ChattingUser != null)
                {
                    VisibleUserName = chattingUser.Name ?? String.Empty;
                    VisibleMessages = chattingUser.Messages ?? new ObservableCollection<Message>();
                    ChattingMessages = chattingUser.Messages ?? new ObservableCollection<Message>();
                }
            }
        }

        public User VisibleUser
        {
            get { return visibleUser; }
            set
            {
                visibleUser = value;
                if(visibleUser != null)
                {
                    VisibleUserName = visibleUser.Name ?? String.Empty;
                    VisibleMessages = visibleUser.Messages ?? new ObservableCollection<Message>();
                    historyViewModel.CurrentUserName = VisibleUserName;
                }
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

        public bool IsListening { get; set; } = false;

        public bool IsConnecting { get; set; } = false;

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
