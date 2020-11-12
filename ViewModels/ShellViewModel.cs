using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using TDDD49.Views;
using TDDD49.ViewModels.Commands;
using TDDD49.Models;
using Newtonsoft.Json;
using TDDD49.Models;
using System.IO;
using System.Collections.ObjectModel;

namespace TDDD49.ViewModels
{
    class ShellViewModel : ViewModel
    {
        private Page currentPage;
        private ChatViewModel chatViewModel;
        private ConfigurePage configurePage;
        private ChatPage chatPage;
        private ConnectUserPage connectUserPage;

        public ShellViewModel()
        {
            chatViewModel = new ChatViewModel();
            chatPage = new ChatPage(chatViewModel);
            configurePage = new ConfigurePage(chatViewModel);
            connectUserPage = new ConnectUserPage(chatViewModel);
            TopMenuButtonCommand = new TopMenuButtonCommand(this, configurePage, chatPage, connectUserPage);
            CurrentPage = configurePage;
        }

        public Page CurrentPage
        {
            get { return currentPage; }
            set
            {
                if(value != currentPage)
                {
                    currentPage = value;
                    OnPropertyChanged(nameof(CurrentPage));
                }
            }
        }

        public ICommand TopMenuButtonCommand { get; private set; }
    }
}
