using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using TDDD49.Models;
using Newtonsoft.Json;
using System.Threading;

namespace TDDD49
{
    public class Communicator
    {
        public User externalUser;
        public string Name { get; set; }
        public int ID { get; set; }
        public TcpListener Server { get; set; }
        public TcpClient Client { get; set; }
        public NetworkStream Stream { get; set; }

        public Communicator() { }
        
        // https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient?view=netcore-3.1
        public void ConnectToOtherPerson(Int32 port, string server, User internalUser)
        {
            Console.WriteLine("Connecting to person");
            
            Client = new TcpClient(server, port);
            Stream = Client.GetStream();

            Console.WriteLine("Connected to other person");
            // Get message to see if the other person wants to chat

            while (true)
            {
                if (Stream.DataAvailable)
                {
                    var res = new byte[9999999];
                    int readBytes = Stream.Read(res, 0, res.Length);

                    if (readBytes == 0)
                    {
                        break;
                    }
                    //Console.WriteLine(Encoding.ASCII.GetString(res, 0, readBytes));
                    Message response = JsonConvert.DeserializeObject<Message>(Encoding.ASCII.GetString(res, 0, readBytes));
                    if (response.MessageType == "decline")
                    {
                        String s = String.Format("{0} ville inte chatta med dig.", response.Sender.Name);
                        MessageBox.Show(s);

                        sendMessage(internalUser, "decline");
                        disconnectStream();

                        break;
                    }
                    else if (response.MessageType == "accept")
                    {
                        String s = String.Format("{0} ville chatta med dig.", response.Sender.Name);
                        MessageBox.Show(s);
                        externalUser = response.Sender;
                        sendMessage(internalUser, "accept");

                        break;
                    }
                }
            }
        }

        public void ListenToPort(Int32 port, User internalUser)
        {
            try
            {
                // Set the TcpListener on port 13000.
                // Int32 port = 13000;
                Console.WriteLine("Listen to port!!!!!");
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                Server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                Server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[25600];

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    Client = Server.AcceptTcpClient();
                    Console.WriteLine("Connected!");
                    
                    // Get a stream object for reading and writing
                    Stream = Client.GetStream();

                    var result = MessageBox.Show("Vill du chatta?", "Chatta?", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.No)
                    {
                        // Respond and decline chat.
                        sendMessage(internalUser, "decline");
                        break;
                    }
                    if (result == MessageBoxResult.Yes)
                    {
                        // Respond and accept chat.
                        sendMessage(internalUser, "accept");
                        
                    }

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = Stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        
                        if (i == 0)
                        {
                            break;
                        }
                        Message res = JsonConvert.DeserializeObject<Message>(Encoding.ASCII.GetString(bytes, 0, i));

                        if (res.MessageType == "accept")
                        {
                            string s = string.Format("You are now chatting with {0}", res.Sender.Name);
                            MessageBox.Show(s);
                            externalUser = res.Sender;
                            break;
                        }
                        else if (res.MessageType == "decline")
                        {
                            MessageBox.Show("Ville inte chatta");
                            disconnectStream();
                            break;
                        }
                        else if (res.MessageType == "disconnect")
                        {
                            MessageBox.Show("Koppling bruten");
                            disconnectStream();
                            break;
                        }
                        
                    }
                    break;
                }
            }
            catch (SocketException e)
            {
                MessageBox.Show("Kopplingen bröts, försök igen");
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                if (Server != null)
                {
                    Server.Stop();
                }
            }
        }

        public void stopChatting(User internalUser)
        {
            sendMessage(internalUser, "disconnect");
            disconnectStream();
        }

        public void disconnectStream()
        {
            if (this.Client != null)
            {
                this.Client.Close();
                this.Client = null;
            }
            if (this.Stream != null)
            {
                this.Stream.Close();
                this.Stream = null;
            }
            if (this.Server != null)
            {
                this.Server.Stop();
                this.Server = null;
            }
            Name = null;
        }

        public Message recieveMessage()
        {
            //Console.WriteLine("recmes");
            if (Stream != null)
            {
                while (true)
                {
                    if (Stream == null)
                    {
                        return null;
                    }
                    if (Stream.DataAvailable)
                    {
                        var res = new byte[9999999];
                        int readBytes = Stream.Read(res, 0, res.Length);
                        Message response = JsonConvert.DeserializeObject<Message>(Encoding.ASCII.GetString(res, 0, readBytes));

                        if (response.MessageType == "disconnect")
                        {
                            String s = String.Format("{0} har kopplat bort.", response.Sender);
                            MessageBox.Show(s);
                            disconnectStream();
                            return response;
                            // break;
                        }
                        else if (response.MessageType == "message")
                        {
                            String s = String.Format("{0}: {1}", response.Sender, response.Content);
                            Console.WriteLine(s);
                            response.IsInternalUserMessage = false;
                            return response;
                        }
                    }
                }
            }
            return null;
        }

        public void sendMessage(User internalUser, string messageType, string message = "")
        {
            if (Stream != null)
            {
                Message mes = new Message() { Content = message, MessageType = messageType, Sender = internalUser, IsInternalUserMessage = false, TimePosted = DateTime.Now };
                byte[] send = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(mes));
                Stream.Write(send, 0, send.Length);
            }
        }

        public void sendMessage(Message m)
        {
            if (Stream != null)
            {
                byte[] send = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(m));
                Stream.Write(send, 0, send.Length);
            }
        }
    }
}

