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
    public class ConnectUserViewModel : ViewModel
    {
        private ChatViewModel chatViewModel;
        private bool validExternalPort = true;
        //private bool validExternalUserName = true;
        //private string externalUserName;
        private int externalPort;
        private string externalIpAddress;
        private Communicator communicator;

        public ConnectUserViewModel(ChatViewModel chatViewModel, Communicator c)
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

        //public bool ValidExternalUserName
        //{
        //    get { return validExternalUserName; }
        //    set
        //    {
        //        validExternalUserName = value;
        //        OnPropertyChanged(nameof(validExternalUserName));
        //    }
        //}

        public ICommand ListenCommand { get; set; }
        public ICommand AddUserCommand { get; set; }

        //public string ExternalUserName
        //{
        //    get { return externalUserName; }
        //    set
        //    {
        //        externalUserName = value;
        //        if (value.Length >= 3)
        //        {
        //            for (int i = 0; i < notAllowedCharacters.Length; i++)
        //            {
        //                if (value.IndexOf(notAllowedCharacters[i]) != -1)
        //                {
        //                    ValidExternalUserName = false;
        //                    break;
        //                }
        //                else { ValidExternalUserName = true; }
        //            }
        //            if(validExternalUserName) { externalUserName = value; }
        //            OnPropertyChanged(nameof(ExternalUserName));
        //        }
        //    }
        //}

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
    }
}
