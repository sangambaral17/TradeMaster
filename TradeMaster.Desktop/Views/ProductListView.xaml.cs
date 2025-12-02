using System.Windows;
using TradeMaster.Desktop.ViewModels;

namespace TradeMaster.Desktop.Views
{
    public partial class ProductListView : Window
    {
        public ProductListView(ProductListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
