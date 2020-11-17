using System.Windows.Controls;
using System.Windows.Input;

using TDDD49.Views;
using TDDD49.ViewModels.Commands;

namespace TDDD49.ViewModels
{
    class ShellViewModel : ViewModel
    {
        private Page currentPage;
        private ChatViewModel chatViewModel;
        private ConfigurePage configurePage;
        private ChatPage chatPage;
        private ConnectUserPage connectUserPage;
        private Communicator communicator = new Communicator();

        public ShellViewModel()
        {
            chatViewModel = new ChatViewModel(communicator);
            chatPage = new ChatPage(chatViewModel);
            configurePage = new ConfigurePage(chatViewModel);
            connectUserPage = new ConnectUserPage(chatViewModel, communicator);
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
