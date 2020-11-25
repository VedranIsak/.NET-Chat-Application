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

namespace TDDD49.Models
{
    public class InternalCommunicator
    {
        private ChatViewModel chatViewModel;
        public InternalCommunicator(ChatViewModel chatViewModel)
        {
            this.chatViewModel = chatViewModel;
        }

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
            ObservableCollection<User> tmp = GetUsersFromJson();
            if (tmp?.Any() == true)
            {
                chatViewModel.Users = tmp;
                chatViewModel.VisibleUser = chatViewModel.Users.ElementAt(0);
                chatViewModel.VisibleMessages = chatViewModel.VisibleUser.Messages;
            }

            chatViewModel.InternalUser = GetUserFromJson() ?? null;
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

            ObservableCollection<User> tmp = GetUsersFromJson();


            if (tmp?.Any() == false)
            {
                tmp = new ObservableCollection<User>();
                if (this.chatViewModel.ChattingUser.Messages == null)
                {
                    this.chatViewModel.ChattingUser.Messages = new ObservableCollection<Message>();
                }
                chatViewModel.ChattingUser.Messages.Add(newMessage);
                tmp.Add(chatViewModel.ChattingUser);
            }
            else
            {
                if (!tmp.Any(item => item.ID == this.chatViewModel.ChattingUser.ID))
                {
                    if (this.chatViewModel.ChattingUser.Messages == null)
                    {
                        this.chatViewModel.ChattingUser.Messages = new ObservableCollection<Message>();
                    }
                    this.chatViewModel.ChattingUser.Messages.Add(newMessage);
                    tmp.Add(this.chatViewModel.ChattingUser);
                }
                else
                {
                    foreach (User u in tmp)
                    {
                        if (u.ID == this.chatViewModel.ChattingUser.ID)
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
