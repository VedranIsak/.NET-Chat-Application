using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using TDDD49.Models;

namespace TDDD49.ViewModels
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        protected char[] notAllowedCharacters = { '@', '#', '.', ',', '-', '~', ' ' };
        private bool validIpAddress = false;
        private bool validPort = false;

        public ViewModel() { }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool ValidIpAddress
        {
            get { return validIpAddress; }
            set
            {
                validIpAddress = value;
                OnPropertyChanged(nameof(ValidIpAddress));
            }
        }

        public bool ValidPort
        {
            get { return validPort; }
            set
            {
                validPort = value;
                OnPropertyChanged(nameof(ValidPort));
            }
        }

        public bool CheckIpAddress(string address)
        {
            if (address == null) { return false; }
            else
            {
                bool hasThreeDots = (address.Count(character => character == '.') == 3);
                bool hasDigits = true;
                foreach (var character in address)
                {
                    if ((character < '0' || character > '9') && character != '.') { hasDigits = false; }
                }
                if ((hasThreeDots && hasDigits && (address.IndexOf('.') > 0 && address.IndexOf('.') < address.Length - 1))
                    || address == "localhost") { return true; }
                else { return false; }
            }
        }

        public bool CheckPort(int port)
        {
            if (port > 1023 && port < 65353) { return true; }
            else { return false; }
        }
    }
}
