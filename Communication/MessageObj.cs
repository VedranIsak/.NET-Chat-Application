using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    // What a message should contain
    class MessageObj
    {
        public string Sender { get; set; }
        public string Message { get; set; }
        public string MessageType { get; set; } // if a regular message this is "message". decline == "decline", 
                                                // accept == "accept" and disconnect == "disconnect".
        
    }
}
