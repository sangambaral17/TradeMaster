using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using TradeMaster.Core.Entities;
using TradeMaster.Core.Interfaces;
using TradeMaster.Desktop.Views;
using TradeMaster.Desktop.Services;

namespace TradeMaster.Desktop.ViewModels
{
    public partial class ProductListViewModel : ObservableObject
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private List<Product> _allProducts = new();
        private System.Timers.Timer? _searchDebounceTimer;

        [ObservableProperty]
        private ObservableCollection<Product> _products = new();

        [ObservableProperty]
        private Product? _selectedProduct;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageSize = 50;

        [ObservableProperty]
        private int _totalPages = 1;

        [ObservableProperty]
        private int _totalProducts;

        [ObservableProperty]
        private string _sortColumn = "Name";

        [ObservableProperty]
        private bool _sortAscending = true;

        public ProductListViewModel(
            IRepository<Product> productRepository,
            IRepository<Category> categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            
            // Setup search debounce timer
            _searchDebounceTimer = new System.Timers.Timer(300); // 300ms delay
            _searchDebounceTimer.Elapsed += (s, e) =>
            {
                _searchDebounceTimer?.Stop();
                Application.Current.Dispatcher.Invoke(async () => await ApplyFiltersAndPagination());
            };

            Task.Run(async () => await LoadProducts());
        }

        partial void OnSearchTextChanged(string value)
        {
            // Debounce search input
            _searchDebounceTimer?.Stop();
            _searchDebounceTimer?.Start();
        }

        [RelayCommand]
        private async Task LoadProducts()
        {
            IsLoading = true;
            try
            {
                ErrorLogger.LogInfo("Loading products...");
                var productList = await _productRepository.GetAllAsync();
                _allProducts = productList.ToList();
                TotalProducts = _allProducts.Count;
                
                await ApplyFiltersAndPagination();
                ErrorLogger.LogInfo($"Loaded {TotalProducts} products successfully");
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("Error loading products", ex);
                MessageBox.Show($"Error loading products: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ApplyFiltersAndPagination()
        {
            await Task.Run(() =>
            {
                // Filter products based on search text
                var filtered = _allProducts.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    var searchLower = SearchText.ToLower();
                    filtered = filtered.Where(p =>
                        p.Name.ToLower().Contains(searchLower) ||
                        (p.Sku != null && p.Sku.ToLower().Contains(searchLower)));
                }

                // Sort products
                filtered = SortColumn switch
                {
                    "Name" => SortAscending ? filtered.OrderBy(p => p.Name) : filtered.OrderByDescending(p => p.Name),
                    "Price" => SortAscending ? filtered.OrderBy(p => p.Price) : filtered.OrderByDescending(p => p.Price),
                    "StockQuantity" => SortAscending ? filtered.OrderBy(p => p.StockQuantity) : filtered.OrderByDescending(p => p.StockQuantity),
                    _ => filtered.OrderBy(p => p.Name)
                };

                var filteredList = filtered.ToList();
                TotalProducts = filteredList.Count;

                // Calculate pagination
                TotalPages = (int)Math.Ceiling((double)TotalProducts / PageSize);
                if (CurrentPage > TotalPages && TotalPages > 0)
                {
                    CurrentPage = TotalPages;
                }

                // Apply pagination
                var paginatedProducts = filteredList
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                // Update UI on dispatcher thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Products = new ObservableCollection<Product>(paginatedProducts);
                });
            });
        }

        [RelayCommand]
        private async Task SortBy(string columnName)
        {
            if (SortColumn == columnName)
            {
                SortAscending = !SortAscending;
            }
            else
            {
                SortColumn = columnName;
                SortAscending = true;
            }

            await ApplyFiltersAndPagination();
        }

        [RelayCommand]
        private async Task NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                await ApplyFiltersAndPagination();
            }
        }

        [RelayCommand]
        private async Task PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await ApplyFiltersAndPagination();
            }
        }

        [RelayCommand]
        private async Task GoToPage(int pageNumber)
        {
            if (pageNumber >= 1 && pageNumber <= TotalPages)
            {
                CurrentPage = pageNumber;
                await ApplyFiltersAndPagination();
            }
        }

        [RelayCommand]
        private async Task AddProduct()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync();
                var dialog = new ProductEditDialog(categories.ToList(), null, _categoryRepository);
                
                if (dialog.ShowDialog() == true && dialog.Product != null)
                {
                    await _productRepository.AddAsync(dialog.Product);
                    await LoadProducts(); // Reload all products
                    
                    ErrorLogger.LogInfo($"Product added: {dialog.Product.Name}");
                    MessageBox.Show("Product added successfully!", 
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("Error adding product", ex);
                MessageBox.Show("Error adding product. Please check your input.", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task EditProduct()
        {
            if (SelectedProduct == null) return;

            try
            {
                var categories = await _categoryRepository.GetAllAsync();
                var dialog = new ProductEditDialog(categories.ToList(), SelectedProduct, _categoryRepository);
                
                if (dialog.ShowDialog() == true && dialog.Product != null)
                {
                    await _productRepository.UpdateAsync(dialog.Product);
                    await LoadProducts(); // Reload to reflect changes
                    
                    ErrorLogger.LogInfo($"Product updated: {dialog.Product.Name}");
                    MessageBox.Show("Product updated successfully!", 
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("Error updating product", ex);
                MessageBox.Show("Error updating product. Please check your input.", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task DeleteProduct()
        {
            if (SelectedProduct == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete '{SelectedProduct.Name}'?\n\nThis action cannot be undone.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _productRepository.DeleteAsync(SelectedProduct);
                    await LoadProducts(); // Reload products
                    
                    ErrorLogger.LogInfo($"Product deleted: {SelectedProduct.Name}");
                    MessageBox.Show("Product deleted successfully!", 
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    ErrorLogger.LogError("Error deleting product", ex);
                    MessageBox.Show("Error deleting product.", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private async Task ClearSearch()
        {
            SearchText = string.Empty;
            await ApplyFiltersAndPagination();
        }
    }
}
