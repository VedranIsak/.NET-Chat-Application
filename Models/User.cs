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
        public bool IsOnline { get; set; } // Kanske ta bort elr använd som "current chat" typ för att se vilken man chattar med just nu
        public string Ip { get; set; } = "127.0.0.1";
        public ObservableCollection<Message> Messages { get; set; }
    }
}

//Kolla in binding till base class!