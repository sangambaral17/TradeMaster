using System.Windows;
using TradeMaster.Desktop.ViewModels;

namespace TradeMaster.Desktop.Views
{
    public partial class SettingsView : Window
    {
        public SettingsView(SettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
