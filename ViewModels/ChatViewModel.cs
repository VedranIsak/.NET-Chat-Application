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
        private ObservableCollection<TDDD49.Models.Message> messages;
        private const string ipAddress = "localhost";
        private Communicator communicator;
        private Thread recieveMessageThread;

        public ChatViewModel(Communicator c)
        {
            Users = new ObservableCollection<User>();
            LoadJSON();
            SendCommand = new SendButtonCommand(this);
            SwitchUserCommand = new SwitchUserCommand(this);

            communicator = c;

        }

        public ICommand SendCommand { get; set; }
        public ICommand SwitchUserCommand { get; set; }

        private void LoadJSON()
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
            Models.Message mes = new Models.Message()
            {
                Content = message,
                Sender = this.internalUser.Name,
                TimePosted = DateTime.Now,
                MessageType = "message",
                IsInternalUserMessage = true
            };
            // vid merge okommentera nedan
            // AddMessages.Add(mes);
            mes.IsInternalUserMessage = false;
            try
            {
                new Thread(() =>
                {
                    communicator.sendMessage(mes);
                });
                
            }
            catch (SocketException err)
            {
                System.Windows.MessageBox.Show("Connection lost, try connnecting again!", "Lost connection");
                communicator.disconnectStream();
                Console.WriteLine(err);
            }
        }

        private void ReadMessage()
        {
            try
            {
                this.recieveMessageThread = new Thread(() =>
                {
                    Models.Message message = null;
                    while (true)
                    {
                        message = this.communicator.recieveMessage();

                        if (message != null && message.MessageType == "disconnect")
                        {
                            break;
                        }
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            // när merge, kommentera bort under detta
                            // AddMessages.Add(message);
                        });
                    }
                });
                this.recieveMessageThread.IsBackground = true;
                this.recieveMessageThread.Start();
            }
            catch (SocketException e1)
            {
                System.Windows.MessageBox.Show("Connection lost, try connnecting again!", "Lost connection");
                communicator.disconnectStream();
                Console.WriteLine(e1);
            }
            catch (Newtonsoft.Json.JsonReaderException e2)
            {
                Console.WriteLine(e2);
            }
            
        }
    }
}
