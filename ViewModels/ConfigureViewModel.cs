using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;


namespace TDDD49.ViewModels
{
    public class ConfigureViewModel : ViewModel
    {
        private char[] notAllowedCharacters = { '@', '#', '.', ',', '-', '~', ' ' };
        private bool validUserName;
        private string userName;

        public ConfigureViewModel() { }

        public bool ValidUserName
        {
            get { return validUserName; }
            set
            {
                validUserName = value;
                OnPropertyChanged(nameof(ValidUserName));
            }
        }

        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                if (userName.Length >= 3)
                {
                    for (int i = 0; i < notAllowedCharacters.Length; i++)
                    {
                        if (userName.IndexOf(notAllowedCharacters[i]) != -1)
                        {
                            ValidUserName = false;
                            return;
                        }
                    }
                    ValidUserName = true;
                    OnPropertyChanged(nameof(UserName));
                }
            }
        }
    }
}
