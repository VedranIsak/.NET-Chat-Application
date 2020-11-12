using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using TDDD49.ViewModels;

namespace TDDD49.Views
{
    /// <summary>
    /// Interaction logic for ConfigurePage.xaml
    /// </summary>
    public partial class ConfigurePage : Page
    {
        public ConfigurePage(ChatViewModel chatViewModel)
        {
            DataContext = new ConfigureViewModel(chatViewModel); 
            InitializeComponent();
        }
    }
}
