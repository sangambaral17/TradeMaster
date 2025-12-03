using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using TradeMaster.Core.Entities;
using TradeMaster.Core.Interfaces;

namespace TradeMaster.Desktop.ViewModels
{
    public partial class PosViewModel : ObservableObject
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Sale> _saleRepository;

        [ObservableProperty]
        private ObservableCollection<Product> _products = new();

        [ObservableProperty]
        private ObservableCollection<Product> _filteredProducts = new();

        [ObservableProperty]
        private ObservableCollection<SaleItem> _cartItems = new();

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private decimal _totalAmount;

        [ObservableProperty]
        private decimal _subTotal;

        [ObservableProperty]
        private decimal _taxAmount;

        private const decimal TaxRate = 0.0m; // 0% tax for now

        public PosViewModel(
            IRepository<Product> productRepository,
            IRepository<Sale> saleRepository)
        {
            _productRepository = productRepository;
            _saleRepository = saleRepository;
            
            // Load products on init
            Task.Run(async () => await LoadProducts());
        }

        partial void OnSearchQueryChanged(string value)
        {
            FilterProducts();
        }

        private void FilterProducts()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                FilteredProducts = new ObservableCollection<Product>(Products);
            }
            else
            {
                var filtered = Products.Where(p => 
                    p.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) || 
                    (p.Sku != null && p.Sku.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)));
                FilteredProducts = new ObservableCollection<Product>(filtered);
            }
        }

        [RelayCommand]
        private async Task LoadProducts()
        {
            var products = await _productRepository.GetAllAsync();
            Products = new ObservableCollection<Product>(products);
            FilteredProducts = new ObservableCollection<Product>(Products);
        }

        [RelayCommand]
        private void AddToCart(Product product)
        {
            var existingItem = CartItems.FirstOrDefault(i => i.ProductId == product.Id);

            if (existingItem != null)
            {
                existingItem.Quantity++;
                existingItem.TotalPrice = existingItem.UnitPrice * existingItem.Quantity;
                // Trigger update manually since SaleItem doesn't implement INotifyPropertyChanged yet
                // For a real app, SaleItem should be observable or wrapped in a ViewModel
                var index = CartItems.IndexOf(existingItem);
                CartItems[index] = existingItem; 
            }
            else
            {
                CartItems.Add(new SaleItem
                {
                    ProductId = product.Id,
                    Product = product,
                    UnitPrice = product.Price,
                    Quantity = 1,
                    TotalPrice = product.Price
                });
            }

            CalculateTotals();
        }

        [RelayCommand]
        private void RemoveFromCart(SaleItem item)
        {
            CartItems.Remove(item);
            CalculateTotals();
        }

        [RelayCommand]
        private void ClearCart()
        {
            CartItems.Clear();
            CalculateTotals();
        }

        private void CalculateTotals()
        {
            SubTotal = CartItems.Sum(i => i.TotalPrice);
            TaxAmount = SubTotal * TaxRate;
            TotalAmount = SubTotal + TaxAmount;
        }

        [RelayCommand]
        private async Task Checkout()
        {
            if (!CartItems.Any()) return;

            try
            {
                // 1. Create the Sale entity
                var sale = new Sale
                {
                    SaleDate = DateTime.Now,
                    TotalAmount = TotalAmount,
                    // CustomerId will be added later when we integrate customer selection in POS
                };

                // 2. Create SaleItems and Update Stock
                foreach (var item in CartItems)
                {
                    // Create a clean SaleItem for the database (avoid attaching the full Product object)
                    var saleItem = new SaleItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    };
                    sale.Items.Add(saleItem);

                    // Update Stock
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity -= item.Quantity;
                        await _productRepository.UpdateAsync(product);
                    }
                }

                // 3. Save the Sale
                await _saleRepository.AddAsync(sale);

                MessageBox.Show($"Transaction Completed!\nTotal: {TotalAmount:C2}", 
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                ClearCart();
                
                // Refresh products to show updated stock
                await LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing transaction: {ex.Message}\n\n{ex.InnerException?.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
