using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDDD49.Models
{
    public class User
    {
        public User() { ID = GetHashCode(); }

        public int ID { get; private set; }
        public string Name { get; set; }
        public int Port { get; set; }
<<<<<<< HEAD
        public string IpAddress { get; set; }
        public bool IsOnline { get; set; }
=======
        public bool IsOnline { get; set; } // Kanske ta bort elr använd som "current chat" typ för att se vilken man chattar med just nu
        public string Ip { get; set; } = "127.0.0.1";
>>>>>>> fb4bcbd37986b2f07238d945f5236007063fbc21
        public ObservableCollection<Message> Messages { get; set; }
    }
}

