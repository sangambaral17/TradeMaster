using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TradeMaster.Core.Entities;
using TradeMaster.Core.Interfaces;
using TradeMaster.Desktop.Views;

namespace TradeMaster.Desktop
{
    public partial class MainWindow : Window
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IServiceProvider _serviceProvider;

        public MainWindow(IRepository<Product> productRepository, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _productRepository = productRepository;
            _serviceProvider = serviceProvider;
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                StatusText.Text = "✅ Connected";
                
                var products = await _productRepository.GetAllAsync();
                ProductCountText.Text = products.Count().ToString();
            }
            catch (Exception)
            {
                StatusText.Text = $"❌ Error";
                StatusText.Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#e74c3c"));
                ProductCountText.Text = "N/A";
            }
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            // Already on dashboard - could refresh stats here
            MessageBox.Show("You are already on the Dashboard!", "Info", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ProductManagementButton_Click(object sender, RoutedEventArgs e)
        {
            var productListView = _serviceProvider.GetRequiredService<ProductListView>();
            productListView.ShowDialog();
            
            // Refresh product count after closing the product management window
            RefreshStats();
        }

        private void PosButton_Click(object sender, RoutedEventArgs e)
        {
            var posView = _serviceProvider.GetRequiredService<PosView>();
            posView.ShowDialog();
            
            // Refresh stats as sales might have affected stock
            RefreshStats();
        }

        private async void RefreshStats()
        {
            try
            {
                var products = await _productRepository.GetAllAsync();
                ProductCountText.Text = products.Count().ToString();
            }
            catch
            {
                // Ignore errors during refresh
            }
        }

        private void CustomerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            var customerListView = _serviceProvider.GetRequiredService<CustomerListView>();
            customerListView.ShowDialog();
        }

        private void SalesReportsButton_Click(object sender, RoutedEventArgs e)
        {
            var salesHistoryView = _serviceProvider.GetRequiredService<SalesHistoryView>();
            salesHistoryView.ShowDialog();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsView = _serviceProvider.GetRequiredService<SettingsView>();
            settingsView.ShowDialog();
        }
    }
}