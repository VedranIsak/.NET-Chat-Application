using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using TDDD49.ViewModels;

namespace TDDD49.ViewModels.Commands
{
    public class BuzzCommand : ICommand
    {
        private Communicator c;
        private ChatViewModel chatViewModel;

        public BuzzCommand(Communicator c, ChatViewModel chatViewModel)
        {
            this.c = c;
            this.chatViewModel = chatViewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) { return true; }

        public void Execute(object parameter)
        {
            //Här är koden som ska kallas när man ska buzza
        }
    }
}
