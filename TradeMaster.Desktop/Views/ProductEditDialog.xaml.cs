using System.Windows;
using TradeMaster.Core.Entities;

namespace TradeMaster.Desktop.Views
{
    public partial class ProductEditDialog : Window
    {
        public Product? Product { get; private set; }

        private readonly bool _isEditMode;

        public ProductEditDialog(List<Category> categories, Product? existingProduct = null)
        {
            InitializeComponent();

            // Populate categories
            CategoryComboBox.ItemsSource = categories;

            // Edit mode vs Add mode
            _isEditMode = existingProduct != null;

            if (_isEditMode && existingProduct != null)
            {
                HeaderText.Text = "✏️ Edit Product";
                LoadProductData(existingProduct);
            }
        }

        private void LoadProductData(Product product)
        {
            NameTextBox.Text = product.Name;
            SkuTextBox.Text = product.Sku;
            CategoryComboBox.SelectedValue = product.CategoryId;
            PriceTextBox.Text = product.Price.ToString("F2");
            StockTextBox.Text = product.StockQuantity.ToString();
            
            // Store the product ID for updates
            Product = product;
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
