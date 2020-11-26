using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TDDD49.Models;
using TDDD49.ViewModels.Commands;

namespace TDDD49.ViewModels
{
    public class HistoryViewModel : ViewModel
    {
        private string currentUserName;
        private User currentUser;
        private bool isChattingUser;
        private ObservableCollection<User> users;
        public HistoryViewModel() { }

        public ICommand SwitchUserCommand { get; set; }

        public string CurrentUserName
        {
            get { return currentUserName; }
            set
            {
                currentUserName = value;
                OnPropertyChanged(nameof(CurrentUserName));
            }
        }

        public User CurrentUser
        {
            get { return currentUser; }
            set
            {
                currentUser = value;
                if(currentUser != null)
                {
                    CurrentUserName = currentUser.Name;
                    IsChattingUser = CheckCurrentUser();
                }
                OnPropertyChanged(nameof(CurrentUser));
            }
        }

        public bool CheckCurrentUser()
        {
            if(CurrentUser != null && Users != null)
            {
                if (Users.Any(user => user.ID == CurrentUser.ID)) { return true; ; }
                else { return false; }
            }
            return false;
        }

        public bool IsChattingUser
        {
            get { return isChattingUser; }
            set
            {
                isChattingUser = value;
                OnPropertyChanged(nameof(IsChattingUser));
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
    }
}
