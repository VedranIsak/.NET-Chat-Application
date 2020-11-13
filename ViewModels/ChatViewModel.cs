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
using System.IO;
using Newtonsoft.Json;

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
        private ObservableCollection<Models.Message> messages;
        private const string ipAddress = "localhost";

        public ChatViewModel()
        {
            Users = new ObservableCollection<User>();
            ReadFromJSON();
            SendCommand = new SendButtonCommand(this);
            SwitchUserCommand = new SwitchUserCommand(this);

            var listenerThread = new Thread(ReadMessage);
            listenerThread.Start();
        }

        public ICommand SendCommand { get; set; }
        public ICommand SwitchUserCommand { get; set; }

        private void ReadFromJSON()
        {
            using (StreamReader usersReader = new StreamReader("../../UsersStorage.json"))
            {
                string inputUsersString = usersReader.ReadToEnd();
                List<Models.User> tmp = JsonConvert.DeserializeObject<List<Models.User>>(inputUsersString).ToList<Models.User>();
                foreach (var user in tmp) { users.Add(user); }
                externalUser = users.ElementAt(0);
            }

            using (StreamReader userReader = new StreamReader("../../UserStorage.json"))
            {
                string inputUserString = userReader.ReadToEnd();
                internalUser = JsonConvert.DeserializeObject<Models.User>(inputUserString);
            }
        }

        private void WriteToJSON(Models.Message newMessage)
        {
            using (StreamReader usersReader = new StreamReader("../../UsersStorage.json"))
            {
                string inputUsersString = usersReader.ReadToEnd();
                List<Models.User> tmp = JsonConvert.DeserializeObject<List<Models.User>>(inputUsersString).ToList<Models.User>();
                foreach (var user in tmp)
                {
                    if (user.ID == ExternalUser.ID)
                    {
                        user.Messages.Add(newMessage);
                    }
                }
                string jsonList = JsonConvert.SerializeObject(tmp);
                File.WriteAllText("../../UsersStorage.json", jsonList);
            }
        }

        public void AddMessage(Models.Message newMessage)
        {
            Messages.Add(newMessage);
            WriteToJSON(newMessage);
        }

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
                Messages = externalUser.Messages;
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
                    ExternalUser.Messages.Add(new TDDD49.Models.Message() { Content = message, TimePosted = DateTime.Now, IsInternalUserMessage = true });
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
                            ExternalUser.Messages.Add(new TDDD49.Models.Message() { Content = msg.ToString(), TimePosted = DateTime.Now, IsInternalUserMessage = false });
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
