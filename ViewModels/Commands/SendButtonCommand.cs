using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using TDDD49.ViewModels;
using TDDD49.Models;
using System.Net.Sockets;

namespace TDDD49.ViewModels.Commands
{
    class SendButtonCommand : ICommand
    {
        private ChatViewModel chatViewModel;
        private TcpClient tcpClient;
        public SendButtonCommand(ChatViewModel chatViewModel)
        {
            this.chatViewModel = chatViewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            string input = parameter as string;
            if(input == null) { return false; }
            if (input.Length == 0) return false;
            return true;
        }

        public void Execute(object parameter)
        {
            //try { tcpClient = new TcpClient("localhost", 8080); }
            //catch (Exception e) { return; }

            //string message = parameter as string;
            //int byteCount = Encoding.ASCII.GetByteCount(message + 1);
            //byte[] sendData = new byte[byteCount];
            //sendData = Encoding.ASCII.GetBytes(message + ";");

            //NetworkStream stream = tcpClient.GetStream();
            //stream.Write(sendData, 0, sendData.Length);

            //stream.Close();
            //tcpClient.Close();

            //chatViewModel.CurrentUser.Messages.Add(new Message() { Content = message, TimePosted = DateTime.Now, IsInternalUserMessage = true });
            string messageToWrite = parameter as string;
            chatViewModel.WriteMessage(messageToWrite);
        }
    }
}
