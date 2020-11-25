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
        private ChatViewModel chatViewModel;

        public SwitchUserCommand(ChatViewModel chatViewModel)
        {
            this.chatViewModel = chatViewModel;
        }

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
            User user = parameter as User;
            chatViewModel.VisibleUser = user;
        }
    }
}
