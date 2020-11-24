using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDDD49.Models;
using TDDD49.ViewModels;

namespace TDDD49.Communication
{
    public class InternalCommunicator
    {
        private ChatViewModel chatViewModel;
        public InternalCommunicator(ChatViewModel chatViewModel) { this.chatViewModel = chatViewModel; }

        private ObservableCollection<User> GetUsersFromJson()
        {
            List<User> tmp;
            using (StreamReader usersReader = new StreamReader("../../UsersStorage.json"))
            {
                tmp = JsonConvert.DeserializeObject<List<User>>(usersReader.ReadToEnd()).ToList<User>();
            }

            ObservableCollection<User> usersReceived = new ObservableCollection<User>();
            foreach (var user in tmp)
            {
                usersReceived.Add(user);
            }
            return usersReceived ?? null;
        }

        private User GetUserFromJson()
        {
            User userReceived;
            using(StreamReader reader = new StreamReader("../../UserStorage.json"))
            {
                userReceived = JsonConvert.DeserializeObject<User>(reader.ReadToEnd());
            }
            return userReceived ?? null;
        }

        public void ReadFromJson()
        {
            //List<User> tmp;
            //using (StreamReader usersReader = new StreamReader("../../UsersStorage.json"))
            //{
            //    string inputUsersString = usersReader.ReadToEnd();
            //    tmp = JsonConvert.DeserializeObject<List<User>>(inputUsersString).ToList<User>();

            //}

            //ObservableCollection<User> tmpObservable = new ObservableCollection<User>();
            //foreach (var user in tmp)
            //{
            //    tmpObservable.Add(user);
            //}
            ObservableCollection<User> tmp = GetUsersFromJson();
            if (tmp?.Any() == true)
            {
                chatViewModel.Users = tmp;
                chatViewModel.VisibleUser = chatViewModel.Users.ElementAt(0);
                chatViewModel.VisibleMessages = chatViewModel.VisibleUser.Messages;
            }

            chatViewModel.InternalUser = GetUserFromJson() ?? null;
            //using (StreamReader userReader = new StreamReader("../../UserStorage.json"))
            //{
            //    string inputUserString = userReader.ReadToEnd();
            //    chatViewModel.InternalUser = JsonConvert.DeserializeObject<User>(inputUserString);

            //}


            //Borde det inte vara UserStorage.json här??
            if (chatViewModel.InternalUser == null) { return; }
            if (chatViewModel.InternalUser.ID == null)
            {
                chatViewModel.InternalUser.ID = GetHashCode();
                using (StreamWriter writer = new StreamWriter("../../UsersStorage.json", false))
                {
                    writer.Write(JsonConvert.SerializeObject(chatViewModel.InternalUser));
                }
            }
        }

        public void WriteMessageToJson(Message newMessage)
        {
            //List<User> tmp;
            //using (StreamReader usersReader = new StreamReader("../../UsersStorage.json"))
            //{
            //    tmp = JsonConvert.DeserializeObject<List<User>>(usersReader.ReadToEnd()).ToList();

            //}

            //if (tmp?.Any() == true)
            //{
            //    tmp = new List<User>();
            //    if (this.chatViewModel.ExternalUser.Messages == null)
            //    {
            //        this.chatViewModel.ExternalUser.Messages = new ObservableCollection<Message>();
            //    }
            //    chatViewModel.ExternalUser.Messages.Add(newMessage);
            //    tmp.Add(chatViewModel.ExternalUser);
            //}

            ObservableCollection<User> tmp = GetUsersFromJson();


            if (tmp?.Any() == false)
            {
                tmp = new ObservableCollection<User>();
                if (this.chatViewModel.VisibleUser.Messages == null)
                {
                    this.chatViewModel.VisibleUser.Messages = new ObservableCollection<Message>();
                }
                chatViewModel.VisibleUser.Messages.Add(newMessage);
                tmp.Add(chatViewModel.VisibleUser);
            }
            else
            {
                if (!tmp.Any(item => item.ID == this.chatViewModel.VisibleUser.ID))
                {
                    if (this.chatViewModel.VisibleUser.Messages == null)
                    {
                        this.chatViewModel.VisibleUser.Messages = new ObservableCollection<Message>();
                    }
                    this.chatViewModel.VisibleUser.Messages.Add(newMessage);
                    tmp.Add(this.chatViewModel.VisibleUser);
                }
                else
                {
                    foreach (User u in tmp)
                    {
                        if (u.ID == this.chatViewModel.VisibleUser.ID)
                        {
                            u.Messages.Add(newMessage);
                            break;
                        }
                    }
                }
            }

            using (StreamWriter writer = new StreamWriter("../../UsersStorage.json", false))
            {
                writer.Write(JsonConvert.SerializeObject(tmp));
            }

            //Borde vara tmp?.Any() == false och inte == true?? 
            //if (tmp?.Any() == false)
            //{
            //    tmp = new List<User>();
            //    if (this.chatViewModel.ExternalUser.Messages == null)
            //    {
            //        this.chatViewModel.ExternalUser.Messages = new ObservableCollection<Message>();
            //    }
            //    chatViewModel.ExternalUser.Messages.Add(newMessage);
            //    tmp.Add(chatViewModel.ExternalUser);
            //}
            //else
            //{
            //    if (!tmp.Any(item => item.ID == this.chatViewModel.ExternalUser.ID))
            //    {
            //        if (this.chatViewModel.ExternalUser.Messages == null)
            //        {
            //            this.chatViewModel.ExternalUser.Messages = new ObservableCollection<Message>();
            //        }
            //        this.chatViewModel.ExternalUser.Messages.Add(newMessage);
            //        tmp.Add(this.chatViewModel.ExternalUser);
            //    }
            //    else
            //    {
            //        foreach (User u in tmp)
            //        {
            //            if (u.ID == this.chatViewModel.ExternalUser.ID)
            //            {
            //                u.Messages.Add(newMessage);
            //                break;
            //            }
            //        }
            //    }
            //}

            //string jsonOut = JsonConvert.SerializeObject(tmp);

            //using (StreamWriter writer = new StreamWriter("../../UsersStorage.json", false))
            //{
            //    writer.Write(jsonOut);
            //}
        }

        public void WriteUsersToJson()
        {
            using(StreamWriter writer = new StreamWriter("../../UsersStorage.json", false))
            {
                writer.Write(JsonConvert.SerializeObject(chatViewModel.Users));
            }
        }

        public void WriteUserToJson()
        {
            using (StreamWriter writer = new StreamWriter("../../UserStorage.json", false))
            {
                writer.Write(JsonConvert.SerializeObject(chatViewModel.InternalUser));
            }
        }
    }
}
