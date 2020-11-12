using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using TDDD49.ViewModels;
using TDDD49.Models;

namespace TDDD49.ViewModels.Commands
{
    class AddUserCommand : ICommand
    {
        private ConnectUserViewModel connectUserViewModel;
        private ChatViewModel chatViewModel;
        public AddUserCommand(ConnectUserViewModel connectUserViewModel, ChatViewModel chatViewModel)
        {
            this.connectUserViewModel = connectUserViewModel;
            this.chatViewModel = chatViewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if(connectUserViewModel.ValidExternalPort && connectUserViewModel.ValidExternalUserName) { return true; }
            return false;
        }

        public void Execute(object parameter)
        {
            chatViewModel.Users.Add(new User()
            {
                Name = connectUserViewModel.ExternalUserName,
                Port = connectUserViewModel.ExternalPort,
                IpAddress = connectUserViewModel.ExternalIpAddress
            });
        }
    }
}
