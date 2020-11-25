using System.Windows.Controls;
using System.Windows.Input;
using TDDD49.Views;
using TDDD49.ViewModels.Commands;
using TDDD49.Models;

namespace TDDD49.ViewModels
{
    class ShellViewModel : ViewModel
    {
        private Page currentPage;
        private InternalCommunicator internalCommunicator;
        private ChatViewModel chatViewModel;
        private HistoryViewModel historyViewModel;
        private SettingsPage settingsPage;
        private ChatPage chatPage;
        private HistoryPage historyPage;
        private ConnectPage connectPage;
        private Communicator communicator = new Communicator();

        public ShellViewModel()
        {
            historyViewModel = new HistoryViewModel();
            historyPage = new HistoryPage(historyViewModel);
            chatViewModel = new ChatViewModel(communicator, historyViewModel);
            chatPage = new ChatPage(chatViewModel);
            settingsPage = new SettingsPage(chatViewModel);
            connectPage = new ConnectPage(chatViewModel, communicator);
            internalCommunicator = new InternalCommunicator(chatViewModel);
            SwitchPageCommand = new SwitchPageCommand(this, settingsPage, chatPage, connectPage, historyPage);
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
