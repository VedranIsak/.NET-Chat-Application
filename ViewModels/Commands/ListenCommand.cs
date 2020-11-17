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
        private ConnectUserViewModel connectUserViewModel;
        private Communicator communicator;
        private Thread listenThread;
        private ChatViewModel chatViewModel;
        public ListenCommand(ConnectUserViewModel connectUserViewModel, Communicator c, ChatViewModel chatViewModel) 
        { 
            this.connectUserViewModel = connectUserViewModel; 
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
             if(connectUserViewModel.ValidExternalPort) { return true; }
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
                    if (communicator.Server != null)
                    {
                        communicator.Server.Stop();
                    }
                    if (communicator.Client != null)
                    {
                        communicator.Client.Close();
                    }
                    // communicator.Server.Stop();
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
                    communicator.ListenToPort(internalUser: this.chatViewModel.InternalUser, port: this.chatViewModel.InternalUser.Port);

                    if (!chatViewModel.Users.Any(item => item.ID == communicator.externalUser.ID))
                    {
                        Console.WriteLine("new user");
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            chatViewModel.Users.Add(new User()
                            {
                                //Här måste man få tag i Namnet via anslutning och sätta Name propertyn till det
                                Name = communicator.externalUser.Name,
                                Port = connectUserViewModel.ExternalPort,
                                IpAddress = connectUserViewModel.ExternalIpAddress
                            });
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
            });
            listenThread.IsBackground = true;
            listenThread.Start();
        }
    }
}
