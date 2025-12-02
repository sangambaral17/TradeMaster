using System.Windows;
using TradeMaster.Desktop.ViewModels;

namespace TradeMaster.Desktop.Views
{
    public partial class PosView : Window
    {
        public PosView(PosViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
