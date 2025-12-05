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

        private void PaymentMethod_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.RadioButton rb && DataContext is PosViewModel vm)
            {
                vm.SelectedPaymentMethod = rb.Tag?.ToString() ?? "Cash";
            }
        }
    }
}
