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

        public ViewModel()
        {  
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
