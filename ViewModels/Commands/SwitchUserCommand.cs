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
    public class SwitchUserCommand : ICommand
    {

        public SwitchUserCommand(ChatViewModel chatViewModel)
        {
            this.ChatViewModel = chatViewModel;
        }

        public ChatViewModel ChatViewModel { get; set; }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            string inp = parameter as string;
            User newUser = ChatViewModel.Users.Single(newCurrentUser => newCurrentUser.Name.Equals(inp));
            ChatViewModel.ExternalUser = newUser;
            ChatViewModel.ExternalUserName = newUser.Name;
            ChatViewModel.ExternalPort = newUser.Port;
            ChatViewModel.ExternalIpAddress = newUser.IpAddress;
        }
    }
}
