using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using TDDD49.Models;

namespace TDDD49.ViewModels.Commands
{
    class SaveCommand : ICommand
    {
        private SettingsViewModel settingsViewModel;
        private ChatViewModel chatViewModel;
        public SaveCommand(SettingsViewModel settingsViewModel, ChatViewModel chatViewModel)
        {
            this.settingsViewModel = settingsViewModel;
            this.chatViewModel = chatViewModel;
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if(!settingsViewModel.ValidInternalPort && !settingsViewModel.ValidInternalUserName) { return false; }
            if(chatViewModel.ChattingUser != null) { return false; }
            if(chatViewModel.IsListening) { return false; }
            return true;
         }

        public void Execute(object parameter)
        {
            if(chatViewModel.InternalUser == null)
            {
                chatViewModel.InternalUser = new User() { Name = settingsViewModel.InternalUserName, Port = settingsViewModel.InternalPort, IpAddress = settingsViewModel.InternalIpAddress };
            }
            else
            {
                chatViewModel.InternalUser.Name = settingsViewModel.InternalUserName;
                chatViewModel.InternalUser.Port = settingsViewModel.InternalPort;
                chatViewModel.InternalUser.IpAddress = settingsViewModel.InternalIpAddress;
            }
            chatViewModel.WriteUserToJSON();
        }
    }
}
