using System.Windows;
using TradeMaster.Desktop.ViewModels;

namespace TradeMaster.Desktop.Views
{
    public partial class CustomerListView : Window
    {
        public CustomerListView(CustomerListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
