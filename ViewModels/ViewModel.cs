using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TDDD49.ViewModels
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        private bool validPort = false;
        private int externalPort;
        private int internalPort;

        public ViewModel() { }

        public bool ValidInternalPort
        {
            get { return validPort; }
            set
            {
                validPort = value;
                OnPropertyChanged(nameof(ValidInternalPort));
            }
        }

        public int InternalPort
        {
            get { return internalPort; }
            set
            {
                if (value > 1023 && value < 65353) { ValidInternalPort = true; }
                else { ValidInternalPort = false; }
                internalPort = value;
                OnPropertyChanged(nameof(InternalPort));
            }
        }

        public int ExternalPort
        {
            get { return externalPort; }
            set
            {
                externalPort = value;
                OnPropertyChanged(nameof(ExternalPort));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
