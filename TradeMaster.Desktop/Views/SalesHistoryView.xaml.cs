using System.Windows;
using TradeMaster.Desktop.ViewModels;

namespace TradeMaster.Desktop.Views
{
    public partial class SalesHistoryView : Window
    {
        public SalesHistoryView(SalesHistoryViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
