using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{   
    // this is a user model, a user should have a name, ip and a port
    public class User : INotifyPropertyChanged
    {
        public User() { }
        private string name = "Namn";
        private Int32 port = 9000;
        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }
        public string Ip { get; set; } = "127.0.0.1";
        public int Port { get { return port; } set { port = value; OnPropertyChanged("Port"); } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String change)
        {
            // Happens only when Property value changes
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(change));
        }
    }
}
