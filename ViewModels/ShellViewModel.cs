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

namespace TDDD49.ViewModels
{
    class ShellViewModel : ViewModel
    {
        private Page currentPage;
        private ConfigurePage configurePage;
        private ChatPage chatPage;
        private ICommand topMenuButtonCommand;

        public ShellViewModel()
        {
            configurePage = new ConfigurePage();
            chatPage = new ChatPage();
            topMenuButtonCommand = new TopMenuButtonCommand(this, configurePage, chatPage);
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

        public ICommand TopMenuButtonCommand
        {
            get { return topMenuButtonCommand; }
            private set
            {
                topMenuButtonCommand = value;
            }
        }
    }
}
