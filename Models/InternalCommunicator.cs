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

        public void ReadFromJSON()
        {
            List<User> tmp;
            using (StreamReader usersReader = new StreamReader("../../UsersStorage.json"))
            {
                string inputUsersString = usersReader.ReadToEnd();
                tmp = JsonConvert.DeserializeObject<List<User>>(inputUsersString).ToList<User>();

            }

            ObservableCollection<User> tmpObservable = new ObservableCollection<User>();
            foreach (var user in tmp)
            {
                tmpObservable.Add(user);
            }
            if (tmp?.Any() == true)
            {
                chatViewModel.Users = tmpObservable;
                chatViewModel.ExternalUser = chatViewModel.Users.ElementAt(0);
                chatViewModel.Messages = chatViewModel.ExternalUser.Messages;
            }

            using (StreamReader userReader = new StreamReader("../../UserStorage.json"))
            {
                string inputUserString = userReader.ReadToEnd();
                chatViewModel.InternalUser = JsonConvert.DeserializeObject<User>(inputUserString);

            }

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

        public void WriteUsersToJSON(Message newMessage)
        {
            List<User> tmp;
            using (StreamReader usersReader = new StreamReader("../../UsersStorage.json"))
            {
                tmp = JsonConvert.DeserializeObject<List<User>>(usersReader.ReadToEnd()).ToList();

            }

            if (tmp?.Any() == true)
            {
                tmp = new List<User>();
                if (this.chatViewModel.ExternalUser.Messages == null)
                {
                    this.chatViewModel.ExternalUser.externalUser.Messages = new ObservableCollection<Message>();
                }
                chatViewModel.ExternalUser.Messages.Add(newMessage);
                tmp.Add(chatViewModel.ExternalUser);
            }
            else
            {
                if (!tmp.Any(item => item.ID == this.chatViewModel.ExternalUser.ID))
                {
                    if (this.chatViewModel.ExternalUser.Messages == null)
                    {
                        this.chatViewModel.ExternalUser.Messages = new ObservableCollection<Message>();
                    }
                    this.chatViewModel.ExternalUser.Messages.Add(newMessage);
                    tmp.Add(this.chatViewModel.ExternalUser);
                }
                else
                {
                    foreach (User u in tmp)
                    {
                        if (u.ID == this.chatViewModel.ExternalUser.ID)
                        {
                            u.Messages.Add(newMessage);
                            break;
                        }
                    }
                }
            }

            string jsonOut = JsonConvert.SerializeObject(tmp);

            using (StreamWriter writer = new StreamWriter("../../UsersStorage.json", false))
            {
                writer.Write(jsonOut);
            }
        }

        public void WriteUserToJSON()
        {
            using (StreamWriter writer = new StreamWriter("../../UserStorage.json", false))
            {
                writer.Write(JsonConvert.SerializeObject(chatViewModel.InternalUser));
            }
        }
    }
}
