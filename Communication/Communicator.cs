using Newtonsoft.Json;
using System;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using TDDD49.Models;
using TDDD49.ViewModels;

namespace TDDD49
{
    public class Communicator
    {
        public User externalUser;
        public User internalUser;
        public ChatViewModel chatViewModel;
        public TcpListener Server { get; set; }
        public TcpClient Client { get; set; }
        public NetworkStream Stream { get; set; }
        public bool CanRecieve { get; private set; } = false;

        private const string Pattern = @"^(([0-9]{1,3}.){3}([0-9]{1,3})|localhost)";
        private Regex regex = new Regex(Pattern, RegexOptions.Compiled);
        SoundPlayer audio = new SoundPlayer(Properties.Resources.pling);
        private Thread recieveMessageThread;

        public Communicator() { }

        public class BadServerException : Exception
        {
            public BadServerException(string message, Exception inner)
                :base(message, inner)
            {
            }

            public BadServerException(string message)
                : base(message)
            {
            }
            
            public BadServerException() { }


        }
        
        // https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient?view=netcore-3.1
        public void ConnectToOtherPerson(Int32 port, string server, User internalUser, ChatViewModel cvm)
        {
            this.internalUser = internalUser;
            this.chatViewModel = cvm;

            if (!regex.Match(server).Success) {
                Console.WriteLine("bad server");
                string s = string.Format("{0} is invalid server ip", server);
                throw new BadServerException(s);
            }

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

                        CanRecieve = true;
                        break;
                    }
                }
            }
        }

        public void ListenToPort(Int32 port, User internalUser, ChatViewModel cvm)
        {

            this.internalUser = internalUser;
            this.chatViewModel = cvm;
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
                            CanRecieve = true;
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
            externalUser = null;
        }

        public void recieveMessage()
        {
            //Console.WriteLine("recmes");
            if (Stream != null)
            {
                while (true)
                {
                    if (Stream == null)
                    {
                        return;
                    }
                    if (Stream.DataAvailable)
                    {
                        var res = new byte[9999999];
                        int readBytes = Stream.Read(res, 0, res.Length);
                        Message response = JsonConvert.DeserializeObject<Message>(Encoding.ASCII.GetString(res, 0, readBytes));
                        response.IsInternalUserMessage = false;

                        if (response.MessageType == "disconnect")
                        {
                            Console.WriteLine("disconnected");
                            String s = String.Format("{0} har kopplat bort.", response.Sender);
                            MessageBox.Show(s);
                            disconnectStream();
                            break;
                        }
                        else if (response.MessageType == "message")
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                chatViewModel.AddMessage(response);
                            });
                        }
                        else if (response.MessageType == "buzz")
                        {
                            audio.Play();
                        }
                    }
                }
            }
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
            Console.WriteLine("sending message");
            byte[] send = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(m));
            Stream.Write(send, 0, send.Length);
        }

        public void ReadMessage()
        {
            this.recieveMessageThread = new Thread(() =>
            {
                while (true)
                {
                    if (CanRecieve)
                    {
                        try
                        {
                            recieveMessage();
                        }
                        catch (ObjectDisposedException e1)
                        {
                            MessageBox.Show("Connection lost, try connnecting again!", "Lost connection");
                            CanRecieve = false;
                            Console.WriteLine(e1);
                        }  
                        catch (SocketException e1)
                        {
                            MessageBox.Show("Connection lost, try connnecting again!", "Lost connection");
                            CanRecieve = false;
                            disconnectStream();
                            Console.WriteLine(e1);
                        }
                        catch (JsonReaderException e2)
                        {
                            Console.WriteLine(e2);
                        }
                    }
                }
            });
            this.recieveMessageThread.IsBackground = true;
            this.recieveMessageThread.Start();
            
        }

        public void WriteMessage(string message, User sender)
        {
            Message mes = new Message()
            {
                Content = message,
                Sender = sender,
                TimePosted = DateTime.Now,
                MessageType = "message",
                IsInternalUserMessage = true
            };

            Thread t = new Thread(() =>
            {
                try
                {
                    sendMessage(mes);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.chatViewModel.AddMessage(mes);
                    });
                }
                catch (NullReferenceException e)
                {
                    MessageBox.Show("No connection, try connecting again", "No connection");
                    disconnectStream();
                    CanRecieve = false;
                    Console.WriteLine(e);
                }
                catch (SocketException e2)
                {
                    MessageBox.Show("Connection lost, try connnecting again!", "Lost connection");
                    disconnectStream();
                    CanRecieve = false;
                    Console.WriteLine(e2);
                }

        });
            t.IsBackground = true;
            t.Start();

        }

    }
}


