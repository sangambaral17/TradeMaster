using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using TradeMaster.Core.Entities;
using TradeMaster.Core.Interfaces;
using TradeMaster.Desktop.Views;

namespace TradeMaster.Desktop.ViewModels
{
    public partial class ProductListViewModel : ObservableObject
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;

        [ObservableProperty]
        private ObservableCollection<Product> _products = new();

        [ObservableProperty]
        private Product? _selectedProduct;

        [ObservableProperty]
        private bool _isLoading;

        public ProductListViewModel(
            IRepository<Product> productRepository,
            IRepository<Category> categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            Task.Run(async () => await LoadProducts());
        }

        [RelayCommand]
        private async Task LoadProducts()
        {
            IsLoading = true;
            try
            {
                var productList = await _productRepository.GetAllAsync();
                Products = new ObservableCollection<Product>(productList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task AddProduct()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var dialog = new ProductEditDialog(categories.ToList());
            
            if (dialog.ShowDialog() == true && dialog.Product != null)
            {
                try
                {
                    await _productRepository.AddAsync(dialog.Product);
                    Products.Add(dialog.Product);
                    MessageBox.Show("Product added successfully!", 
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error adding product. Please check your input.", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private async Task EditProduct()
        {
            if (SelectedProduct == null) return;

            var categories = await _categoryRepository.GetAllAsync();
            var dialog = new ProductEditDialog(categories.ToList(), SelectedProduct);
            
            if (dialog.ShowDialog() == true && dialog.Product != null)
            {
                try
                {
                    await _productRepository.UpdateAsync(dialog.Product);
                    await LoadProducts(); // Reload to reflect changes
                    MessageBox.Show("Product updated successfully!", 
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error updating product. Please check your input.", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
                    Products.Remove(SelectedProduct);
                    MessageBox.Show("Product deleted successfully!", 
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error deleting product.", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
