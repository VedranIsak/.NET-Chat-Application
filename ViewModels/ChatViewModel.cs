using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;

using TDDD49.Models;
using TDDD49.ViewModels.Commands;

// IPAddresserna och portnumren som används är fortfarande hårdkodade
// TO DO -> Fixa JSON 
// TO DO -> Fixa searchbar högst upp
// ConnectUser sidan bindar inte bra till ChatViewModel

namespace TDDD49.ViewModels
{
    public class ChatViewModel : ViewModel
    {
        private User internalUser;
        private User externalUser;
        private ObservableCollection<User> users;
        private ObservableCollection<TDDD49.Models.Message> messages;
        private const string ipAddress = "localhost";

        public ChatViewModel()
        {
            SendCommand = new SendButtonCommand(this);
            SwitchUserCommand = new SwitchUserCommand(this);

            Users = new ObservableCollection<User>
            {
            new User() { Name = "George", Port=8080, IpAddress="localhost", IsOnline = false },
            new User() { Name = "Steven", IsOnline = true },
            new User() { Name = "Julia", IsOnline = true },
            new User() { Name = "Sarah", IsOnline = false },
            new User() { Name = "Alex", IsOnline = true }
            };

            Users.ElementAt(0).Messages = new ObservableCollection<TDDD49.Models.Message>()
            {
                new TDDD49.Models.Message() { TimePosted=DateTime.Now, Content="Hi whats up!", IsInternalUserMessage = false },
                new TDDD49.Models.Message() { TimePosted = DateTime.Now, Content="Nothing much, you?", IsInternalUserMessage = true},
                new TDDD49.Models.Message() { TimePosted=DateTime.Now, Content="Nothing much", IsInternalUserMessage = false },
                new TDDD49.Models.Message() { TimePosted = DateTime.Now, Content="Good!", IsInternalUserMessage = true }

            };
            Users.ElementAt(1).Messages = new ObservableCollection<TDDD49.Models.Message>()
            {
                new TDDD49.Models.Message() { TimePosted=DateTime.Now, Content="Whats up!", IsInternalUserMessage = false },
                new TDDD49.Models.Message() { TimePosted = DateTime.Now, Content="Whaddup bruh?", IsInternalUserMessage = true},
            };

            InternalUser = Users.ElementAt(0);
            ExternalUser = Users.ElementAt(1);
            List<Models.Message> listMessages = InternalUser.Messages.Concat(ExternalUser.Messages).ToList<Models.Message>();
            listMessages.Sort((x, y) => x.TimePosted.CompareTo(y.TimePosted));
            Messages = new ObservableCollection<Models.Message>();
            foreach (var msg in listMessages)
            {
                Messages.Add(msg);
            }

            var listenerThread = new Thread(ReadMessage);
            listenerThread.Start();
        }

        public void LoadMockData()
        {
            Users = new ObservableCollection<User>
            {
            new User() { Name = "George", Port=8080, IpAddress="localhost", IsOnline = false },
            new User() { Name = "Steven", IsOnline = true },
            new User() { Name = "Julia", IsOnline = true },
            new User() { Name = "Sarah", IsOnline = false },
            new User() { Name = "Alex", IsOnline = true }
            };

            Users.ElementAt(0).Messages = new ObservableCollection<TDDD49.Models.Message>()
            {
                new TDDD49.Models.Message() { TimePosted=DateTime.Now, Content="Hi whats up!", IsInternalUserMessage = false },
                new TDDD49.Models.Message() { TimePosted = DateTime.Now, Content="Nothing much, you?", IsInternalUserMessage = true},
                new TDDD49.Models.Message() { TimePosted=DateTime.Now, Content="Nothing much", IsInternalUserMessage = false },
                new TDDD49.Models.Message() { TimePosted = DateTime.Now, Content="Good!", IsInternalUserMessage = true }

            };
            Users.ElementAt(1).Messages = new ObservableCollection<TDDD49.Models.Message>()
            {
                new TDDD49.Models.Message() { TimePosted = DateTime.Now, Content="Whats up!", IsInternalUserMessage = false },
                new TDDD49.Models.Message() { TimePosted = DateTime.Now, Content="Whaddup bruh?", IsInternalUserMessage = true},
            };
        }

        public ICommand SendCommand { get; set; }
        public ICommand SwitchUserCommand { get; set; }

        public ObservableCollection<User> Users
        {
            get { return users; }
            set
            {
                users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        public ObservableCollection<TDDD49.Models.Message> Messages
        {
            get { return messages; }
            set
            {
                messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        public User InternalUser
        {
            get { return internalUser; }
            set { internalUser = value; }
        }

        public string InternalUserName
        {
            get { return InternalUser.Name; }
            set { InternalUser.Name = value; }
        }

        public int InternalPort
        {
            get { return InternalUser.Port; }
            set { InternalUser.Port = value; }
        }

        public string InternalIpAddress
        {
            get { return InternalUser.IpAddress; }
            set { InternalUser.IpAddress = value; }
        }

        public User ExternalUser
        {
            get { return externalUser; }
            set
            {
                externalUser = value;
            }
        }

        public string ExternalUserName
        {
            get { return ExternalUser.Name; }
            set
            {
                ExternalUser.Name = value;
                OnPropertyChanged(nameof(ExternalUserName));
            }
        }

        public int ExternalPort
        {
            get { return ExternalUser.Port; }
            set { ExternalUser.Port = value; }
        }

        public string ExternalIpAddress
        {
            get { return ExternalUser.IpAddress; }
            set { ExternalUser.IpAddress = value; }
        }

        public void WriteMessage(string message)
        {
            Task.Run(() =>
            {
                TcpClient tcpClient;
                NetworkStream writeStream;
                try
                {
                    tcpClient = new TcpClient("localhost", 8080);
                    writeStream = tcpClient.GetStream();
                }
                catch (Exception e) { return; }

                int byteCount = Encoding.ASCII.GetByteCount(message + 1);
                byte[] sendData = new byte[byteCount];
                sendData = Encoding.ASCII.GetBytes(message + ";");

                writeStream.Write(sendData, 0, sendData.Length);

                writeStream.Close();
                tcpClient.Close();

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    InternalUser.Messages.Add(new TDDD49.Models.Message() { Content = message, TimePosted = DateTime.Now, IsInternalUserMessage = true });
                });
            });
        }

        private void ReadMessage()
        {
            Task.Run(() =>
            {
                IPAddress ip = Dns.GetHostEntry("localhost").AddressList[0];
                TcpListener tcpListener = new TcpListener(ip, 8080);
                tcpListener.Start();

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    NetworkStream stream = tcpClient.GetStream();
                    byte[] receivedBuffer = new byte[100];

                    int bytesRead = 0;
                    bytesRead = stream.Read(receivedBuffer, 0, receivedBuffer.Length);
                    if (bytesRead > 0)
                    {
                        try { bytesRead = stream.Read(receivedBuffer, 0, receivedBuffer.Length); }
                        catch (SocketException se)
                        {
                            System.Windows.Forms.MessageBox.Show("Error! Cannot establish a connection!");
                            break;
                        }

                        StringBuilder msg = new StringBuilder();

                        foreach (byte b in receivedBuffer)
                        {
                            if (b.Equals(59)){ break; }
                            else { msg.Append(Convert.ToChar(b).ToString()); }
                        }

                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            InternalUser.Messages.Add(new TDDD49.Models.Message() { Content = msg.ToString(), TimePosted = DateTime.Now, IsInternalUserMessage = false });
                        });
                        stream.Close();
                        tcpClient.Close();
                    }
                }
                tcpListener.Stop();
            });
        }
    }
}
