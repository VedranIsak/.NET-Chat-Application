using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using TDDD49.ViewModels;
using TDDD49.Models;
using System.Threading;
using System.Net.Sockets;
using System.Windows;
using System.IO;

namespace TDDD49.ViewModels.Commands
{
    class AddUserCommand : ICommand
    {
        private ConnectViewModel connectViewModel;
        private ChatViewModel chatViewModel;
        private Thread addThread;
        private Communicator communicator;
        public AddUserCommand(ConnectViewModel connectViewModel, ChatViewModel chatViewModel, Communicator c)
        {
            this.connectViewModel = connectViewModel;
            this.chatViewModel = chatViewModel;
            this.communicator = c;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if(connectViewModel.ValidExternalPort
                && chatViewModel.InternalUser != null 
                && chatViewModel.InternalUser.Name != null
                && chatViewModel.InternalUser.Port > 1023 
                && chatViewModel.InternalUser.Port < 65353
                && chatViewModel.InternalUser.IpAddress != null) { return true; }
            return false;
        }

        public void Execute(object parameter)
        {
            if (addThread != null)
            {
                if (addThread.IsAlive)
                {
                    communicator.disconnectStream();
                    // communicator.Server.Stop();
                    addThread.Abort();
                }
                else
                {
                    communicator.stopChatting(chatViewModel.InternalUser);
                }
                addThread.Abort();
            }

            addThread = new Thread(() =>
            {
                chatViewModel.IsConnecting = true;

                try
                {
                    communicator.ConnectToOtherPerson(internalUser: chatViewModel.InternalUser, port: connectViewModel.ExternalPort, server: connectViewModel.ExternalIpAddress, cvm: this.chatViewModel);
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
                    }
                    else
                    {
                        MessageBox.Show("Failed to connect");
                    }
                }
                catch (SocketException e)
                {
                    MessageBox.Show("No one listening to that address");
                    Console.WriteLine(e);
                }
                catch (ArgumentNullException e2)
                {
                    MessageBox.Show("Bad IP address, try again", "Bad IP address");
                    Console.WriteLine(e2);
                }
                catch (Communicator.BadServerException e3)
                {
                    MessageBox.Show("Bad IP address, try again", "Bad IP address");
                    Console.WriteLine(e3);
                }
                catch (ObjectDisposedException e4)
                {
                    Console.WriteLine(e4);
                }
                catch (IOException e4)
                {
                    Console.WriteLine(e4);
                }
                finally
                {
                    chatViewModel.IsConnecting = false;
                }
            });
            addThread.IsBackground = true;
            addThread.Start();
            
        }
    }
}
