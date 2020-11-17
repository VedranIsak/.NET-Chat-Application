using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using TDDD49.Views;

namespace TDDD49.ViewModels
{
    public class ConfigureViewModel : ViewModel
    {
        private ChatViewModel chatViewModel;
        private bool validInternalUserName = true;
        private bool validInternalPort = true;


        public ConfigureViewModel(ChatViewModel chatViewModel)
        {
            this.chatViewModel = chatViewModel;
        }

        public bool ValidInternalPort
        {
            get { return validInternalPort; }
            set
            {
                validInternalPort = value;
                OnPropertyChanged(nameof(ValidInternalPort));
            }
        }

        public bool ValidInternalUserName
        {
            get { return validInternalUserName; }
            set
            {
                validInternalUserName = value;
                OnPropertyChanged(nameof(ValidInternalUserName));
            }
        }

        public string InternalUserName
        {
            get { return chatViewModel.InternalUser.Name; }
            set
            {
                chatViewModel.InternalUser.Name = value;
                if (value.Length >= 3)
                {
                    for (int i = 0; i < notAllowedCharacters.Length; i++)
                    {
                        if (value.IndexOf(notAllowedCharacters[i]) != -1)
                        {
                            ValidInternalUserName = false;
                            break;
                        }
                        else { ValidInternalUserName = true; }
                    }
                    if (ValidInternalUserName) { chatViewModel.InternalUser.Name = value; }
                    OnPropertyChanged(nameof(InternalUserName));
                }
            }
        }

        public string InternalIpAddress
        {
            get { return chatViewModel.InternalUser.IpAddress; }
            set
            {
                chatViewModel.InternalUser.IpAddress = value;
                OnPropertyChanged(nameof(InternalIpAddress));
            }
        }

        public int InternalPort
        {
            get { return chatViewModel.InternalUser.Port; }
            set
            {
                if (value > 1023 && value < 65353) { ValidInternalPort = true; }
                else { ValidInternalPort = false; }
                chatViewModel.InternalUser.Port = value;
                if (ValidInternalPort) { chatViewModel.InternalUser.Port = value; }
                OnPropertyChanged(nameof(InternalPort));
            }
        }
    }
}
