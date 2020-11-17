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

namespace TDDD49.ViewModels.Commands
{
    class AddUserCommand : ICommand
    {
        private ConnectUserViewModel connectUserViewModel;
        private ChatViewModel chatViewModel;
        private Thread addThread;
        private Communicator communicator;
        public AddUserCommand(ConnectUserViewModel connectUserViewModel, ChatViewModel chatViewModel, Communicator c)
        {
            this.connectUserViewModel = connectUserViewModel;
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
            if(connectUserViewModel.ValidExternalPort) { return true; }
            return false;
        }

        public void Execute(object parameter)
        {
            if (addThread != null)
            {
                if (addThread.IsAlive)
                {/*
                    if (communicator.Client != null)
                    {
                        communicator.Client.Close();
                    }
                    if (communicator.Server != null)
                    {
                        communicator.Server.Stop();
                    }*/
                    communicator.disconnectStream();
                    // communicator.Server.Stop();
                    addThread.Abort();
                }
                //MessageBox.Show("Du blir bortkopplad från din nuvarande chatt!", "kopplar bort");
                communicator.stopChatting(chatViewModel.InternalUser);
                addThread.Abort();
            }

            addThread = new Thread(() =>
            {
                chatViewModel.CanRecieve = false;
                try
                {
                    communicator.ConnectToOtherPerson(internalUser: chatViewModel.InternalUser, port: connectUserViewModel.ExternalPort, server: connectUserViewModel.ExternalIpAddress);
                    
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        chatViewModel.Users.Add(new User()
                        {
                            //Här måste man få tag i Namnet via anslutning och sätta Name propertyn till det
                            //Name = connectUserViewModel.ExternalUserName, 
                            Name = communicator.Name,
                            Port = connectUserViewModel.ExternalPort,
                            IpAddress = connectUserViewModel.ExternalIpAddress
                        });
                        chatViewModel.CanRecieve = true;
                    });

                    /*
                    communicator.connectPerson(name: chatViewModel.InternalUser.Name, port: connectUserViewModel.ExternalPort, server: connectUserViewModel.ExternalIpAddress);
                    chatViewModel.Users.Add(new User()
                    {
                        //Här måste man få tag i Namnet via anslutning och sätta Name propertyn till det
                        //Name = connectUserViewModel.ExternalUserName, 
                        Name = communicator.Name,
                        Port = connectUserViewModel.ExternalPort,
                        IpAddress = connectUserViewModel.ExternalIpAddress
                    });*/
                }
                catch (SocketException e)
                {
                    //MessageBox.Show("Noone listening to that address.");
                    Console.WriteLine("Noone listening to that address");
                    Console.WriteLine(e);
                }

                Console.WriteLine("donE");
            });
            addThread.IsBackground = true;
            addThread.Start();
            
        }
    }
}
