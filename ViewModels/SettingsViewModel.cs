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
        private bool showSuccessfulSave = false;
        private bool validInternalUserName = false;
        private string internalIpAddress;
        private int internalPort;


        public SettingsViewModel(ChatViewModel chatViewModel)
        {
            this.chatViewModel = chatViewModel;
            InternalUserName = chatViewModel.InternalUser.Name;
            InternalIpAddress = chatViewModel.InternalUser.IpAddress;
            InternalPort = chatViewModel.InternalUser.Port;
            SaveCommand = new SaveCommand(this, chatViewModel);
        }

        public ICommand SaveCommand { get; set; }

        public bool ShowSuccessfulSave
        {
            get { return showSuccessfulSave; }
            set
            {
                showSuccessfulSave = value;
                OnPropertyChanged(nameof(ShowSuccessfulSave));
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
                if (chatViewModel.InternalUser == null) { return null; }
                return chatViewModel.InternalUser.Name;
            }
            set
            {
                chatViewModel.InternalUser.Name = value;
                if (value.Length < 3)
                {
                    ValidInternalUserName = false;
                }
                else
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
                }
                if (ValidInternalUserName) { chatViewModel.InternalUser.Name = value; chatViewModel.InternalUser.ID = GetHashCode(); }
                OnPropertyChanged(nameof(InternalUserName));
            }
        }

        public string InternalIpAddress
        {
            get
            {
                //if (chatViewModel.InternalUser == null) { return String.Empty; }
                if (!ValidIpAddress) { return internalIpAddress; }
                else if (chatViewModel.InternalUser != null) { return chatViewModel.InternalUser.IpAddress; }
                else { return String.Empty; }
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
                //if (chatViewModel.InternalUser == null) { return 0; }
                if (!ValidPort) { return internalPort; }
                else if (chatViewModel.InternalUser != null) { return chatViewModel.InternalUser.Port; }
                else { return 0; }
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
