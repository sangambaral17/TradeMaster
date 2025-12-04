using System.Windows;
using TradeMaster.Desktop.Services;

namespace TradeMaster.Desktop.Views
{
    public partial class ReceiptPreviewDialog : Window
    {
        private readonly ReceiptService _receiptService;
        private readonly ReceiptContent _receiptContent;

        public ReceiptPreviewDialog(ReceiptService receiptService, ReceiptContent receiptContent)
        {
            InitializeComponent();
            _receiptService = receiptService;
            _receiptContent = receiptContent;

            ReceiptTextBlock.Text = receiptContent.PlainText;
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _receiptService.PrintReceipt(_receiptContent);
                MessageBox.Show("Receipt printed successfully!", "Print", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Print failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
