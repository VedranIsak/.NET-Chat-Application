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
            if (listenThread != null)
            {
                if (listenThread.IsAlive)
                {
                    
                    communicator.stopChatting(chatViewModel.InternalUser);
                    listenThread.Abort();
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
                    
                    if (communicator.externalUser != null)
                    {
                        User newUser = new User()
                        {
                            Name = communicator.externalUser.Name,
                            IpAddress = communicator.externalUser.IpAddress,
                            Port = communicator.externalUser.Port,
                            Messages = new System.Collections.ObjectModel.ObservableCollection<Message>()
                        };

                        if (!chatViewModel.Users.Any(item => item.ID == communicator.externalUser.ID))
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                chatViewModel.Users.Add(newUser);
                            });
                            chatViewModel.chattingWith = newUser;
                        }
                        chatViewModel.CanRecieve = true;
                    }
                    else
                    {
                        MessageBox.Show("Failed to connect");
                    }

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
