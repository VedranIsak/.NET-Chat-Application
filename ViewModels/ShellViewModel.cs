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
        private SettingsPage settingsPage;
        private ChatPage chatPage;
        private ConnectPage connectPage;
        private Communicator communicator = new Communicator();

        public ShellViewModel()
        {
            chatViewModel = new ChatViewModel(communicator);
            chatPage = new ChatPage(chatViewModel);
            settingsPage = new SettingsPage(chatViewModel);
            connectPage = new ConnectPage(chatViewModel, communicator);
            SwitchPageCommand = new SwitchPageCommand(this, settingsPage, chatPage, connectPage);
            CurrentPage = settingsPage;
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

        public ICommand SwitchPageCommand { get; private set; }
    }
}
