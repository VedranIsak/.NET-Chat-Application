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
        private ObservableCollection<User> users;
        public HistoryViewModel() { }

        public ICommand SwitchUserCommand { get; set; }

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
