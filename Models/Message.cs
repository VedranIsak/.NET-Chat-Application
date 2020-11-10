using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDDD49.Models
{
    public class Message
    {
        public string Content { get; set; }
        public DateTime TimePosted { get; set; }
        public bool IsInternalUserMessage { get; set; }
    }
}
