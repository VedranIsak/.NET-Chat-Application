using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using TDDD49.Views;
using TDDD49.ViewModels.Commands;
using System.Net;

namespace TDDD49.ViewModels
{
    public class SettingsViewModel : ViewModel
    {
        private ChatViewModel chatViewModel;
        private bool validInternalIpAddress = true;
        private bool validInternalUserName = true;
        private bool validInternalPort = true;
        private string internalIpAddress;
        private int internalPort;


        public SettingsViewModel(ChatViewModel chatViewModel)
        {
            this.chatViewModel = chatViewModel;
            SaveCommand = new SaveCommand(this, chatViewModel);
        }

        public ICommand SaveCommand { get; set; }

        public bool ValidInternalIpAddress
        {
            get { return validInternalIpAddress; }
            set
            {
                validInternalIpAddress = value;
                OnPropertyChanged(nameof(ValidInternalIpAddress));
            }
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
            get
            {
                if(chatViewModel.InternalUser == null) { return null; }
                return chatViewModel.InternalUser.Name;
            }
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
                    if (ValidInternalUserName) { chatViewModel.InternalUser.Name = value; chatViewModel.InternalUser.ID = GetHashCode(); }
                    OnPropertyChanged(nameof(InternalUserName));
                }
            }
        }

        public string InternalIpAddress
        {
            get
            {
                if (chatViewModel.InternalUser == null) { return null; }
                else if (!ValidIpAddress) { return internalIpAddress; }
                else { return chatViewModel.InternalUser.IpAddress; }
            }
            set
            {
                internalIpAddress = value;
                ValidIpAddress = CheckIpAddress(internalIpAddress);
                if (ValidIpAddress) { chatViewModel.InternalUser.IpAddress = internalIpAddress; }
                OnPropertyChanged(nameof(InternalIpAddress));
            }
        }

        public int InternalPort
        {
            get
            {
                if (chatViewModel.InternalUser == null) { return 0; }
                else if(!ValidPort) { return internalPort; }
                else { return chatViewModel.InternalUser.Port; }
            }
            set
            {
                internalPort = value;
                ValidPort = CheckPort(internalPort);
                if (ValidPort) { chatViewModel.InternalUser.Port = internalPort; }
                OnPropertyChanged(nameof(InternalPort));
            }
        }
    }
}
