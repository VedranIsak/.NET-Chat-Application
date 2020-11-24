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
            if (connectViewModel.ValidExternalPort 
                && chatViewModel.InternalUser != null
                && chatViewModel.InternalUser.Name != null
                && chatViewModel.InternalUser.Port > 1023
                && chatViewModel.InternalUser.Port < 65353
                && chatViewModel.InternalUser.IpAddress != null) { return true; }
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
                    communicator.ListenToPort(internalUser: this.chatViewModel.InternalUser, port: this.chatViewModel.InternalUser.Port, cvm: chatViewModel);
                    
                    if (communicator.externalUser != null)
                    {
                        User newUser = new User()
                        {
                            ID = communicator.externalUser.ID,
                            Name = communicator.externalUser.Name,
                            IpAddress = communicator.externalUser.IpAddress,
                            Port = communicator.externalUser.Port,
                            Messages = new System.Collections.ObjectModel.ObservableCollection<Message>()
                        };

                        if (!chatViewModel.Users.Any(item => item.ID == newUser.ID))
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                chatViewModel.AddUser(newUser);
                            });
                            chatViewModel.ChattingUser = newUser;
                        }
                        else
                        {
                            chatViewModel.ChattingUser = chatViewModel.Users.Single(item => item.ID == newUser.ID);
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
                    MessageBox.Show("Kopplingen bröts, försök igen");
                    Console.WriteLine("SocketException: {0}", e1);
                }
                catch (ThreadAbortException e2)
                {
                    Console.WriteLine(e2);
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
