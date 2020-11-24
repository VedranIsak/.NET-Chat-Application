using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using TDDD49.ViewModels;
using TDDD49.Models;
using System.Net.Sockets;

namespace TDDD49.ViewModels.Commands
{
    class SendCommand : ICommand
    {
        private ChatViewModel chatViewModel;
        public SendCommand(ChatViewModel chatViewModel)
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
            string input = parameter as string;
            if(input == null) { return false; }
            if (input.Length == 0) { return false; }
            if(input.Length > 50) { return false; }
            if(chatViewModel.ChattingUser == null) { return false; } 
            return true;
        }

        public void Execute(object parameter)
        {
            string messageToWrite = parameter as string;
            chatViewModel.WriteMessage(messageToWrite);
        }
    }
}
