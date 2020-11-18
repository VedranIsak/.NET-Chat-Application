using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TDDD49.ViewModels.Commands
{
    class DisconnectButtonCommand : ICommand
    {
        Communicator communicator;
        ChatViewModel chatViewModel;
        public DisconnectButtonCommand(Communicator c, ChatViewModel cvm)
        {
            this.chatViewModel = cvm;
            this.communicator = c;
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) { return true; }

        public void Execute(object parameter)
        {
            Console.WriteLine("trying to disconnect");
            Thread t = new Thread(() =>
            {
                try
                {
                    communicator.stopChatting(this.chatViewModel.InternalUser);
                    chatViewModel.CanRecieve = false;
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e);
                }
                catch (NullReferenceException e2)
                {
                    Console.WriteLine(e2);
                }
            });
            t.IsBackground = true;
            t.Start();
        }
    }
}
