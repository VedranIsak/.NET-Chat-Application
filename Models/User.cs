using Newtonsoft.Json;
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

        [JsonProperty("ID")]
        public int ID { get; private set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Port")]
        public int Port { get; set; }

        [JsonProperty("IpAddress")]
        public string IpAddress { get; set; } = "127.0.0.1";

        [JsonProperty("Messages")]
        public ObservableCollection<Message> Messages { get; set; }
    }
}

