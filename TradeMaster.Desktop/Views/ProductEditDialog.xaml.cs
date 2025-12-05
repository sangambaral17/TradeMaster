using System.Windows;
using System.Windows.Media;
using TradeMaster.Core.Entities;
using TradeMaster.Core.Interfaces;
using TradeMaster.Desktop.Services;

namespace TradeMaster.Desktop.Views
{
    public partial class ProductEditDialog : Window
    {
        public Product? Product { get; private set; }

        private readonly bool _isEditMode;
        private readonly IRepository<Category>? _categoryRepository;
        private List<Category> _categories;

        public ProductEditDialog(List<Category> categories, Product? existingProduct = null, IRepository<Category>? categoryRepository = null)
        {
            InitializeComponent();

            _categoryRepository = categoryRepository;
            _categories = categories;

            // Initialize currency options
            CurrencyComboBox.ItemsSource = new[]
            {
                new { Code = "USD", Display = "USD $" },
                new { Code = "AUD", Display = "AUD A$" },
                new { Code = "NPR", Display = "NPR Rs." }
            };
            CurrencyComboBox.SelectedValue = "USD";

            // Populate categories
            CategoryComboBox.ItemsSource = _categories;

            // Edit mode vs Add mode
            _isEditMode = existingProduct != null;

            if (_isEditMode && existingProduct != null)
            {
                HeaderText.Text = "✏️ Edit Product";
                LoadProductData(existingProduct);
            }
            
            // Attach real-time validation handlers
            NameTextBox.TextChanged += NameTextBox_TextChanged;
            PriceTextBox.TextChanged += PriceTextBox_TextChanged;
            StockTextBox.TextChanged += StockTextBox_TextChanged;
        }

        private void LoadProductData(Product product)
        {
            NameTextBox.Text = product.Name;
            SkuTextBox.Text = product.Sku;
            CategoryComboBox.SelectedValue = product.CategoryId;
            CurrencyComboBox.SelectedValue = product.Currency ?? "USD";
            PriceTextBox.Text = product.Price.ToString("F2");
            StockTextBox.Text = product.StockQuantity.ToString();
            
            // Store the product ID for updates
            Product = product;
        }

        private async void ManageCategories_Click(object sender, RoutedEventArgs e)
        {
            if (_categoryRepository == null)
            {
                MessageBox.Show("Category management is not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new CategoryManagementDialog(_categoryRepository);
            dialog.ShowDialog();

            // Reload categories if changed
            if (dialog.CategoriesChanged)
            {
                _categories = (await _categoryRepository.GetAllAsync()).ToList();
                var selectedId = CategoryComboBox.SelectedValue;
                CategoryComboBox.ItemsSource = _categories;
                CategoryComboBox.SelectedValue = selectedId;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate inputs
            if (!ValidateInputs())
            {
                return;
            }

            try
            {
                // Create or update product
                if (_isEditMode && Product != null)
                {
                    // Update existing product
                    Product.Name = NameTextBox.Text.Trim();
                    Product.Sku = string.IsNullOrWhiteSpace(SkuTextBox.Text) ? null : SkuTextBox.Text.Trim();
                    Product.CategoryId = (int)CategoryComboBox.SelectedValue;
                    Product.Currency = CurrencyComboBox.SelectedValue?.ToString() ?? "USD";
                    Product.Price = decimal.Parse(PriceTextBox.Text);
                    Product.StockQuantity = int.Parse(StockTextBox.Text);
                }
                else
                {
                    // Create new product
                    Product = new Product
                    {
                        Name = NameTextBox.Text.Trim(),
                        Sku = string.IsNullOrWhiteSpace(SkuTextBox.Text) ? null : SkuTextBox.Text.Trim(),
                        CategoryId = (int)CategoryComboBox.SelectedValue,
                        Currency = CurrencyComboBox.SelectedValue?.ToString() ?? "USD",
                        Price = decimal.Parse(PriceTextBox.Text),
                        StockQuantity = int.Parse(StockTextBox.Text)
                    };
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ShowValidationError($"Error saving product: {ex.Message}");
            }
        }

        private bool ValidateInputs()
        {
            // Product Name
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                ShowValidationError("Product name is required.");
                NameTextBox.Focus();
                return false;
            }

            // Category
            if (CategoryComboBox.SelectedValue == null)
            {
                ShowValidationError("Please select a category.");
                CategoryComboBox.Focus();
                return false;
            }

            // Price
            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price < 0)
            {
                ShowValidationError("Please enter a valid price (must be 0 or greater).");
                PriceTextBox.Focus();
                return false;
            }

            // Stock Quantity
            if (!int.TryParse(StockTextBox.Text, out int stock) || stock < 0)
            {
                ShowValidationError("Please enter a valid stock quantity (must be 0 or greater).");
                StockTextBox.Focus();
                return false;
            }

            // Hide validation error if all is good
            ValidationBorder.Visibility = Visibility.Collapsed;
            return true;
        }

        private void ShowValidationError(string message)
        {
            ValidationText.Text = message;
            ValidationBorder.Visibility = Visibility.Visible;
        }
        
        // Real-time validation event handlers
        private void NameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                NameTextBox.BorderBrush = new SolidColorBrush(Color.FromRgb(231, 76, 60)); // Red
                NameTextBox.BorderThickness = new Thickness(2);
            }
            else
            {
                NameTextBox.BorderBrush = new SolidColorBrush(Color.FromRgb(189, 195, 199)); // Normal
                NameTextBox.BorderThickness = new Thickness(1);
            }
        }
        
        private void PriceTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price < 0)
            {
                PriceTextBox.BorderBrush = new SolidColorBrush(Color.FromRgb(231, 76, 60)); // Red
                PriceTextBox.BorderThickness = new Thickness(2);
            }
            else
            {
                PriceTextBox.BorderBrush = new SolidColorBrush(Color.FromRgb(189, 195, 199)); // Normal
                PriceTextBox.BorderThickness = new Thickness(1);
            }
        }
        
        private void StockTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!int.TryParse(StockTextBox.Text, out int stock) || stock < 0)
            {
                StockTextBox.BorderBrush = new SolidColorBrush(Color.FromRgb(231, 76, 60)); // Red
                StockTextBox.BorderThickness = new Thickness(2);
            }
            else
            {
                StockTextBox.BorderBrush = new SolidColorBrush(Color.FromRgb(189, 195, 199)); // Normal
                StockTextBox.BorderThickness = new Thickness(1);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
