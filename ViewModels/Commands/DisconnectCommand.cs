using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Input;
using TDDD49.Models;

namespace TDDD49.ViewModels.Commands
{
    class DisconnectCommand : ICommand
    {
        Communicator communicator;
        ChatViewModel chatViewModel;
        public DisconnectCommand(Communicator c, ChatViewModel cvm)
        {
            this.chatViewModel = cvm;
            this.communicator = c;
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if(chatViewModel.ChattingUser == null) { return false; }
            return true;
        }

        public void Execute(object parameter)
        {
            Console.WriteLine("trying to disconnect");
            try
            {
                Thread t = new Thread(() =>
                {
                    communicator.stopChatting(this.chatViewModel.InternalUser);
                    this.chatViewModel.ChattingUser = null;
                });
                t.IsBackground = true;
                t.Start();
            }
            catch (NullReferenceException e1)
            {
                Console.WriteLine(e1);
            }
            catch (SocketException e2)
            {
                Console.WriteLine(e2);
            }
            catch (ThreadAbortException e3)
            {
                Console.WriteLine(e3);
            }
            catch (IOException e4)
            {
                Console.WriteLine(e4);
            }
            catch (ObjectDisposedException e4)
            {
                Console.WriteLine(e4);
            }
        }
    }
}
