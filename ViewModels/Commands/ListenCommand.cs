using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TDDD49.Models;

namespace TDDD49.ViewModels.Commands
{
    public class ListenCommand : ICommand
    {
        private ConnectViewModel connectViewModel;
        private Communicator communicator;
        private Thread listenThread;
        private ChatViewModel chatViewModel;
        public ListenCommand(ConnectViewModel connectViewModel, Communicator c, ChatViewModel chatViewModel) 
        { 
            this.connectViewModel = connectViewModel; 
            this.communicator = c;
            this.chatViewModel = chatViewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
             if(connectViewModel.ValidExternalPort) { return true; }
            return false;
        }

        public void Execute(object parameter)
        {
            //Här ska den interna användaren kunnna börja lyssna på port & ip
            if (listenThread != null)
            {
                if (listenThread.IsAlive)
                {
                    //MessageBox.Show("Du kommer börja lyssna på nytt!", "kopplar bort");
                    /*if (communicator.Server != null)
                    {
                        communicator.Server.Stop();
                    }
                    if (communicator.Client != null)
                    {
                        communicator.Client.Close();
                    }*/
                    communicator.stopChatting(chatViewModel.InternalUser);
                    listenThread.Abort();
                    Console.WriteLine("aborted");
                }
                else
                {
                    communicator.stopChatting(chatViewModel.InternalUser);
                }

            }
            listenThread = new Thread(() =>
            {
                chatViewModel.CanRecieve = false;
                try
                {
                    Console.WriteLine(chatViewModel.InternalUser.ID);

                    communicator.ListenToPort(internalUser: this.chatViewModel.InternalUser, port: this.chatViewModel.InternalUser.Port);

                    if (!chatViewModel.Users.Any(item => item.ID == communicator.externalUser.ID))
                    {
                        Console.WriteLine("new user");
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            chatViewModel.Users.Add(communicator.externalUser);
                        });
                    }
                    chatViewModel.CanRecieve = true;

                }
                catch (SocketException e1)
                {
                    Console.WriteLine("e1");
                }
                catch (ThreadAbortException e2)
                {
                    Console.WriteLine("e2");
                }
                catch (NullReferenceException e3)
                {
                    Console.WriteLine(e3);
                }
            });
            listenThread.IsBackground = true;
            listenThread.Start();
        }
    }
}
