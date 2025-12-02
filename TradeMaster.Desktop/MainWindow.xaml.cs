using System.Windows;
using TradeMaster.Core.Entities;
using TradeMaster.Core.Interfaces;

namespace TradeMaster.Desktop
{
    public partial class MainWindow : Window
    {
        private readonly IRepository<Product> _productRepository;

        public MainWindow(IRepository<Product> productRepository)
        {
            InitializeComponent();
            _productRepository = productRepository;
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                StatusText.Text = "✅ Database Connected Successfully";
                
                var products = await _productRepository.GetAllAsync();
                ProductCountText.Text = $"Total Products in DB: {products.Count()}";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"❌ Error: {ex.Message}";
                ProductCountText.Text = "N/A";
            }
        }
    }
}