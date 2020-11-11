using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using Newtonsoft.Json;
using TDDD49.Models;

namespace WpfApp2
{
    class Communicator
    {
        public Communicator() { }

        private TcpListener Server { get; set; }
        private TcpClient Client { get; set; }
        private NetworkStream Stream { get; set; }


        // https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient?view=netcore-3.1
        public void ConnectToOtherPerson(Int32 port, string server, String name)
        {
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

                    if(readBytes == 0)
                    {
                        break;
                    }
                    //Console.WriteLine(Encoding.ASCII.GetString(res, 0, readBytes));
                    Message response = JsonConvert.DeserializeObject<Message>(Encoding.ASCII.GetString(res, 0, readBytes));
                    if (response.MessageType == "decline")
                    {
                        String s = String.Format("{0} ville inte chatta med dig.", response.Sender);
                        MessageBox.Show(s);

                        sendMessage(name, "decline");
                        disconnectStream();

                        break;
                    }
                    else if (response.MessageType == "accept")
                    {
                        String s = String.Format("{0} ville chatta med dig.", response.Sender);
                        MessageBox.Show(s);

                        sendMessage(name, "accept");
                        
                        break;
                    }
                }
            }
        }

        public void ListenToPort(Int32 port, String name)
        {
            try
            {
                // Set the TcpListener on port 13000.
                // Int32 port = 13000;
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
                        sendMessage(name, "decline");
                        break;
                    }
                    if (result == MessageBoxResult.Yes)
                    {
                        // Respond and accept chat.
                        sendMessage(name, "accept");
                        
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
                            string s = string.Format("You are now chatting with {0}", res.Sender);
                            MessageBox.Show(s);
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
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                Server.Stop();
            }
        }

        public void stopChatting(String name)
        {
            sendMessage(name, "disconnect");
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
        }

        public Message recieveMessage()
        {
            //Console.WriteLine("recmes");
            while (true)
            {
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
                        return response;
                    }
                }
            }
        }

        public void sendMessage(string name, string messageType, string message = "")
        {
            if (Stream != null)
            {
                Message mes = new Message() { Content = message, MessageType = messageType, Sender = name, IsInternalUserMessage = false, TimePosted = DateTime.Now };
                byte[] send = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(mes));
                Stream.Write(send, 0, send.Length);
            }
        }
    }
}

