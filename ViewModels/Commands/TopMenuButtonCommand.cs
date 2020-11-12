using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using TDDD49.ViewModels;
using TDDD49.Views;

namespace TDDD49.ViewModels.Commands
{
    class TopMenuButtonCommand : ICommand
    {
        private ShellViewModel shellViewModel;
        private ConfigurePage configurePage;
        private ChatPage chatPage;
        private ConnectUserPage connectUserPage;
        public TopMenuButtonCommand(ShellViewModel shellViewModel, ConfigurePage configurePage, ChatPage chatPage, ConnectUserPage connectUserPage)
        {
            this.shellViewModel = shellViewModel;
            this.configurePage = configurePage;
            this.chatPage = chatPage;
            this.connectUserPage = connectUserPage;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) { return true; }

        public void Execute(object parameter)
        {
            string direction = parameter as string;

            switch(direction)
            {
                case "Settings":
                    shellViewModel.CurrentPage = configurePage;
                    break;
                case "Chat":
                    shellViewModel.CurrentPage = chatPage;
                    break;
                case "Add Friend":
                    shellViewModel.CurrentPage = connectUserPage;
                    break;
                default:
                    break;
            }
        }
    }
}
