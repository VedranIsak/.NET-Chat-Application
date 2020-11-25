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
    class SwitchPageCommand : ICommand
    {
        private ShellViewModel shellViewModel;
        private SettingsPage settingsPage;
        private ChatPage chatPage;
        private ConnectPage connectPage;
        private HistoryPage historyPage;
        public SwitchPageCommand(ShellViewModel shellViewModel, SettingsPage configurePage, ChatPage chatPage, ConnectPage connectPage, HistoryPage historyPage)
        {
            this.shellViewModel = shellViewModel;
            this.settingsPage = configurePage;
            this.chatPage = chatPage;
            this.connectPage = connectPage;
            this.historyPage = historyPage;
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
                    shellViewModel.CurrentPage = settingsPage;
                    break;
                case "Chat":
                    shellViewModel.CurrentPage = chatPage;
                    break;
                case "History":
                    shellViewModel.CurrentPage = historyPage;
                    break;
                case "Connect":
                    shellViewModel.CurrentPage = connectPage;
                    break;
                default:
                    break;
            }
        }
    }
}
