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
        public string IpAddress { get; set; }
        public bool IsOnline { get; set; }
        public ObservableCollection<Message> Messages { get; set; }
    }
}

