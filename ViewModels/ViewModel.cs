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
        //private bool validExternalUserName = true;
        //private bool validInternalUserName = true;
        //private bool validInternalPort = true;
        //private bool validExternalPort = true;
        //private string externalUserName;
        //private string internalUserName;
        //private int externalPort;
        //private int internalPort;
        //private int externalIpAddress;
        //private int internalIpAddress;

        public ViewModel()
        {  
        }

        //public bool ValidInternalPort
        //{
        //    get { return validInternalPort; }
        //    set
        //    {
        //        validInternalPort = value;
        //        OnPropertyChanged(nameof(ValidInternalPort));
        //    }
        //}

        //public bool ValidExternalPort
        //{
        //    get { return validExternalPort; }
        //    set
        //    {
        //        validExternalPort = value;
        //        OnPropertyChanged(nameof(ValidExternalPort));
        //    }
        //}

        //public bool ValidInternalUserName
        //{
        //    get { return validInternalUserName; }
        //    set
        //    {
        //        validInternalUserName = value;
        //        OnPropertyChanged(nameof(ValidInternalUserName));
        //    }
        //}

        //public bool ValidExternalUserName
        //{
        //    get { return validExternalUserName; }
        //    set
        //    {
        //        validExternalUserName = value;
        //        OnPropertyChanged(nameof(validExternalUserName));
        //    }
        //}

        //public string ExternalUserName
        //{
        //    get { return externalUserName; }
        //    set
        //    {
        //        externalUserName = value;
        //        if (externalUserName.Length >= 3)
        //        {
        //            for (int i = 0; i < notAllowedCharacters.Length; i++)
        //            {
        //                if (externalUserName.IndexOf(notAllowedCharacters[i]) != -1)
        //                {
        //                    ValidExternalUserName = false;
        //                }
        //            }
        //            OnPropertyChanged(nameof(ExternalUserName));
        //        }
        //    }
        //}

        //public string InternalUserName
        //{
        //    get { return internalUserName; }
        //    set
        //    {
        //        internalUserName = value;
        //        if (internalUserName.Length >= 3)
        //        {
        //            for (int i = 0; i < notAllowedCharacters.Length; i++)
        //            {
        //                if (internalUserName.IndexOf(notAllowedCharacters[i]) != -1)
        //                {
        //                    ValidInternalUserName = false;
        //                }
        //            }
        //            if (ValidInternalUserName) { internalUser.Name = value; }
        //            OnPropertyChanged(nameof(InternalUserName));
        //        }
        //    }
        //}

        //public int ExternalIpAddress
        //{
        //    get { return externalIpAddress; }
        //    set
        //    {
        //        externalIpAddress = value;
        //    }
        //}

        //public int InternalIpAddress
        //{
        //    get { return internalIpAddress; }
        //    set
        //    {
        //        internalIpAddress = value;
        //        internalUser.IpAddress = value;
        //        OnPropertyChanged(nameof(InternalIpAddress));
        //    }
        //}

        //public int InternalPort
        //{
        //    get { return internalPort; }
        //    set
        //    {
        //        if (value > 1023 && value < 65353) { ValidInternalPort = true; }
        //        else { ValidInternalPort = false; }
        //        internalPort = value;
        //        if(ValidInternalPort) { internalUser.Port = value; }
        //        OnPropertyChanged(nameof(InternalPort));
        //    }
        //}

        //public int ExternalPort
        //{
        //    get { return externalPort; }
        //    set
        //    {
        //        if(value > 1023 && value < 65353) { ValidExternalPort = true; }
        //        else { ValidExternalPort = false; }
        //        externalPort = value;
        //        OnPropertyChanged(nameof(ExternalPort));
        //    }
        //}

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
