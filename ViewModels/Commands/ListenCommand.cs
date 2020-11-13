using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TDDD49.ViewModels.Commands
{
    public class ListenCommand : ICommand
    {
        private ConnectUserViewModel connectUserViewModel;
        public ListenCommand(ConnectUserViewModel connectUserViewModel) { this.connectUserViewModel = connectUserViewModel; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
             if(connectUserViewModel.ValidExternalPort) { return true; }
            return false;
        }

        public void Execute(object parameter)
        {
            //Här ska den interna användaren kunnna börja lyssna på port & ip
        }
    }
}
