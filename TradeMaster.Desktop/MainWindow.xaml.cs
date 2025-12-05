using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Input;
using TradeMaster.Core.Entities;
using TradeMaster.Core.Interfaces;
using TradeMaster.Desktop.Views;
using TradeMaster.Desktop.Services;

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
            
            // Register keyboard shortcuts
            RegisterKeyboardShortcuts();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                StatusText.Text = "⏳ Loading...";
                ProductCountText.Text = "...";
                
                var products = await _productRepository.GetAllAsync();
                ProductCountText.Text = products.Count().ToString();
                StatusText.Text = "✅ Connected";
                
                ErrorLogger.LogInfo("MainWindow loaded successfully");
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("Error loading MainWindow stats", ex);
                
                StatusText.Text = $"❌ Error";
                StatusText.Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#e74c3c"));
                ProductCountText.Text = "N/A";
            }
        }

        private void RegisterKeyboardShortcuts()
        {
            // Ctrl+P - Product Management
            var productShortcut = new KeyBinding(
                new RelayCommand(_ => ProductManagementButton_Click(this, new RoutedEventArgs())),
                Key.P,
                ModifierKeys.Control);
            InputBindings.Add(productShortcut);

            // Ctrl+O - Point of Sale (O for Order)
            var posShortcut = new KeyBinding(
                new RelayCommand(_ => PosButton_Click(this, new RoutedEventArgs())),
                Key.O,
                ModifierKeys.Control);
            InputBindings.Add(posShortcut);

            // Ctrl+C - Customer Management
            var customerShortcut = new KeyBinding(
                new RelayCommand(_ => CustomerManagementButton_Click(this, new RoutedEventArgs())),
                Key.C,
                ModifierKeys.Control);
            InputBindings.Add(customerShortcut);

            // Ctrl+R - Reports
            var reportsShortcut = new KeyBinding(
                new RelayCommand(_ => SalesReportsButton_Click(this, new RoutedEventArgs())),
                Key.R,
                ModifierKeys.Control);
            InputBindings.Add(reportsShortcut);

            // Ctrl+T - Settings (T for Tools)
            var settingsShortcut = new KeyBinding(
                new RelayCommand(_ => SettingsButton_Click(this, new RoutedEventArgs())),
                Key.T,
                ModifierKeys.Control);
            InputBindings.Add(settingsShortcut);
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            // Already on dashboard - refresh stats
            RefreshStats();
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
                StatusText.Text = "⏳ Refreshing...";
                var products = await _productRepository.GetAllAsync();
                ProductCountText.Text = products.Count().ToString();
                StatusText.Text = "✅ Connected";
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("Error refreshing stats", ex);
                ProductCountText.Text = "N/A";
                StatusText.Text = "❌ Error";
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

    // Helper class for keyboard shortcuts
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object? parameter) => _execute(parameter);
    }
}