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
        private Communicator communicator;
        private Thread recieveMessageThread;
        public bool CanRecieve { get; set; } = false;

        public ChatViewModel(Communicator c)
        {
            Users = new ObservableCollection<User>();
            ReadFromJSON();
            SendCommand = new SendButtonCommand(this);
            SwitchUserCommand = new SwitchUserCommand(this);

            communicator = c;
            ReadMessage();
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
            var json = File.ReadAllText("../../UsersStorage.json");
            Console.WriteLine(json);
            List<Models.User> tmp = JsonConvert.DeserializeObject<List<Models.User>>(json).ToList<Models.User>();
            foreach (var user in tmp)
            {
                if (user.ID == this.ExternalUser.ID)
                {
                    user.Messages.Add(newMessage);
                }
            }
            var jsonOut = JsonConvert.SerializeObject(tmp);
            Console.WriteLine(jsonOut);
            File.WriteAllText("../../UsersStorage.json", jsonOut);
            /*
            using (var usersReader = File.Open("../../UsersStorage.json", FileMode.Open))
            {
                string inputUsersString = usersReader.Read();
                List<Models.User> tmp = JsonConvert.DeserializeObject<List<Models.User>>(inputUsersString).ToList<Models.User>();
                foreach (var user in tmp)
                {
                    if (user.ID == ExternalUser.ID)
                    {
                        user.Messages.Add(newMessage);
                    }
                }
                usersReader.write()
                string jsonList = JsonConvert.SerializeObject(tmp);
                Console.WriteLine(1);
                var file = File.Create("../../UsersStorage.json");
                byte[] send = Encoding.ASCII.GetBytes(jsonList);
                file.Write(send, 0, jsonList.Length);
                //File.WriteAllText("../../UsersStorage.json", jsonList);
                file.Close();
                Console.WriteLine(2);
            
            }
            */
        }

        public void AddMessage(Models.Message newMessage)
        {
            Console.WriteLine("{0}, {1}, {2}" ,newMessage.Content, newMessage.Sender, newMessage.IsInternalUserMessage);
            Console.WriteLine(Messages);
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

        public ObservableCollection<Models.Message> Messages
        {
            get 
            { 
                if (messages == null)
                {
                    return new ObservableCollection<Models.Message>();
                }
                return messages; 
            }
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
                if (externalUser.Messages == null)
                {
                    Messages = new ObservableCollection<Models.Message>();
                }
                else
                {
                    Messages = externalUser.Messages;
                }
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
            Console.WriteLine(this.externalUser.Name);
            Models.Message mes = new Models.Message()
            {
                Content = message,
                Sender = this.internalUser.Name,
                TimePosted = DateTime.Now,
                MessageType = "message",
                IsInternalUserMessage = false
            };
            // vid merge okommentera nedan
            this.AddMessage(mes);
            mes.IsInternalUserMessage = false;
            try
            {
                Thread t = new Thread(() =>
                {
                    communicator.sendMessage(mes);
                });
                t.IsBackground = true;
                t.Start();
                
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
            Console.WriteLine("Readmessage");
            try
            {
                this.recieveMessageThread = new Thread(() =>
                {
                    Models.Message message = null;
                    while (true)
                    {
                        
                        if (CanRecieve)
                        {
                            Console.WriteLine(CanRecieve);
                            message = this.communicator.recieveMessage();

                            if (message == null)
                            {
                                continue;
                            }

                            else if (message.MessageType == "disconnect")
                            {
                                CanRecieve = false;
                            }
                            //this.AddMessage(message);
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                // när merge, kommentera bort under detta
                                //AddMessage.Add(message);
                                AddMessage(message);
                            });
                        }
                    }
                });
                this.recieveMessageThread.IsBackground = true;
                this.recieveMessageThread.Start();
            }
            catch (SocketException e1)
            {
                System.Windows.MessageBox.Show("Connection lost, try connnecting again!", "Lost connection");
                CanRecieve = false;
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
