using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using TDDD49.Models;
using TDDD49.ViewModels.Commands;

namespace TDDD49.ViewModels
{
    public class ConnectViewModel : ViewModel
    {
        private ChatViewModel chatViewModel;
        private bool validExternalPort = true;
        private bool isListening = false;
        private int externalPort;
        private string externalIpAddress;
        private Communicator communicator;

        public ConnectViewModel(ChatViewModel chatViewModel, Communicator c)
        {
            this.chatViewModel = chatViewModel;
            AddUserCommand = new AddUserCommand(this, chatViewModel, c);
            ListenCommand = new ListenCommand(this, c, chatViewModel);
            communicator = c;
        }

        public bool ValidExternalPort
        {
            get { return validExternalPort; }
            set
            {
                validExternalPort = value;
                OnPropertyChanged(nameof(ValidExternalPort));
            }
        }

        public ICommand ListenCommand { get; set; }
        public ICommand AddUserCommand { get; set; }

        public string ExternalIpAddress
        {
            get { return externalIpAddress; }
            set
            {
                externalIpAddress = value;
                OnPropertyChanged(ExternalIpAddress);
            }
        }

        public int ExternalPort
        {
            get { return externalPort; }
            set
            {
                if (value > 1023 && value < 65353) { ValidExternalPort = true; }
                else { ValidExternalPort = false; }
                externalPort = value;
                OnPropertyChanged(nameof(ExternalPort));
            }
        }

        public bool IsListening
        {
            get { return isListening; }
            set
            {
                isListening = value;
                OnPropertyChanged(nameof(IsListening));
            }
        }
    }
}
