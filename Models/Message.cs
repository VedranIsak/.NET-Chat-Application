using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDDD49.Models
{
    public class Message
    {
        public string Content { get; set; }     // The message
        public DateTime TimePosted { get; set; }
        public bool IsInternalUserMessage { get; set; }   // false om skickas och true annars
        public string MessageType { get; set; } // if a regular message this is "message". decline == "decline", 
                                                // accept == "accept" and disconnect == "disconnect".
        public string Sender { get; set; }      // Name of the person who sent the message
    }
}
