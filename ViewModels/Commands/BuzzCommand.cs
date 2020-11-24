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
using TDDD49.ViewModels;

namespace TDDD49.ViewModels.Commands
{
    public class BuzzCommand : ICommand
    {
        private Communicator communicator;
        private ChatViewModel chatViewModel;

        public BuzzCommand(Communicator c, ChatViewModel chatViewModel)
        {
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
            if (chatViewModel.ChattingUser == null) { return false; }
            return true;
        }

        public void Execute(object parameter)
        {
            //Här är koden som ska kallas när man ska buzza
            Message m = new Message()
            {
                MessageType = "buzz",
                Sender = chatViewModel.InternalUser,
            };
            try
            {
                Thread t = new Thread(() => communicator.sendMessage(m));
                t.IsBackground = true;
                t.Start();
            }
            catch (SocketException err)
            {
                MessageBox.Show("Connection lost, try connnecting again!", "Lost connection");
                communicator.disconnectStream();
                Console.WriteLine(err);
            }

        }
    }
}
