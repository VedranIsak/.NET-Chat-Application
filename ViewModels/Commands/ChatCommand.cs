using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TDDD49.Models;

namespace TDDD49.ViewModels.Commands
{
    public class ChatCommand : ICommand
    {
        private ChatViewModel chatViewModel;
        public ChatCommand(ChatViewModel chatViewModel) { this.chatViewModel = chatViewModel; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            //Här borde man ha att man kan bara chatta med personen man är uppkopplad med
            return true;
        }

        public void Execute(object parameter)
        {
            User userToChat = parameter as User;
            chatViewModel.ChattingUser = userToChat;
        }
    }
}
