using System.Windows;
using System.Windows.Controls;
using TradeMaster.Core.Entities;
using TradeMaster.Core.Interfaces;

namespace TradeMaster.Desktop.Views
{
    public partial class CategoryManagementDialog : Window
    {
        private readonly IRepository<Category> _categoryRepository;
        private List<Category> _categories;

        public bool CategoriesChanged { get; private set; } = false;

        public CategoryManagementDialog(IRepository<Category> categoryRepository)
        {
            InitializeComponent();
            _categoryRepository = categoryRepository;
            _categories = new List<Category>();
            LoadCategories();
        }

        private async void LoadCategories()
        {
            try
            {
                _categories = (await _categoryRepository.GetAllAsync()).ToList();
                CategoriesListBox.ItemsSource = null;
                CategoriesListBox.ItemsSource = _categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            var name = CategoryNameTextBox.Text?.Trim();
            
            if (string.IsNullOrWhiteSpace(name))
            {
                ShowValidation("Category name is required.");
                return;
            }

            // Check for duplicate
            if (_categories.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                ShowValidation("A category with this name already exists.");
                return;
            }

            try
            {
                var category = new Category
                {
                    Name = name,
                    Description = CategoryDescriptionTextBox.Text?.Trim()
                };

                await _categoryRepository.AddAsync(category);
                CategoriesChanged = true;

                // Clear form
                CategoryNameTextBox.Text = string.Empty;
                CategoryDescriptionTextBox.Text = string.Empty;
                ValidationBorder.Visibility = Visibility.Collapsed;

                // Reload list
                LoadCategories();

                MessageBox.Show("Category added successfully!", 
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding category: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not Category category)
                return;

            var dialog = new CategoryEditDialog(category);
            if (dialog.ShowDialog() == true && dialog.UpdatedCategory != null)
            {
                try
                {
                    // Check for duplicate (excluding current category)
                    if (_categories.Any(c => c.Id != category.Id && 
                        c.Name.Equals(dialog.UpdatedCategory.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        MessageBox.Show("A category with this name already exists.", 
                            "Duplicate Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    await _categoryRepository.UpdateAsync(dialog.UpdatedCategory);
                    CategoriesChanged = true;
                    LoadCategories();

                    MessageBox.Show("Category updated successfully!", 
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating category: {ex.Message}", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not Category category)
                return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete '{category.Name}'?\n\nThis action cannot be undone.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _categoryRepository.DeleteAsync(category);
                    CategoriesChanged = true;
                    LoadCategories();

                    MessageBox.Show("Category deleted successfully!", 
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting category: {ex.Message}\n\nNote: Cannot delete categories with existing products.", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ShowValidation(string message)
        {
            ValidationText.Text = message;
            ValidationBorder.Visibility = Visibility.Visible;
        }
    }

    // Simple edit dialog for categories
    public class CategoryEditDialog : Window
    {
        public Category? UpdatedCategory { get; private set; }

        public CategoryEditDialog(Category category)
        {
            Title = "Edit Category";
            Width = 400;
            Height = 250;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Background = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#f5f6fa"));

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var formPanel = new StackPanel { Margin = new Thickness(20) };
            
            formPanel.Children.Add(new TextBlock 
            { 
                Text = "Category Name *", 
                FontSize = 14, 
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 8)
            });
            
            var nameBox = new TextBox 
            { 
                Text = category.Name, 
                Padding = new Thickness(10, 8, 10, 8),
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 15)
            };
            formPanel.Children.Add(nameBox);

            formPanel.Children.Add(new TextBlock 
            { 
                Text = "Description", 
                FontSize = 14, 
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 8)
            });
            
            var descBox = new TextBox 
            { 
                Text = category.Description, 
                Padding = new Thickness(10, 8, 10, 8),
                FontSize = 14,
                Height = 60,
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true
            };
            formPanel.Children.Add(descBox);

            grid.Children.Add(formPanel);

            var buttonPanel = new StackPanel 
            { 
                Orientation = Orientation.Horizontal, 
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(20)
            };
            Grid.SetRow(buttonPanel, 1);

            var cancelBtn = new Button 
            { 
                Content = "Cancel", 
                Padding = new Thickness(20, 10, 20, 10),
                Margin = new Thickness(0, 0, 10, 0),
                MinWidth = 100
            };
            cancelBtn.Click += (s, e) => { DialogResult = false; Close(); };

            var saveBtn = new Button 
            { 
                Content = "Save", 
                Padding = new Thickness(20, 10, 20, 10),
                MinWidth = 100
            };
            saveBtn.Click += (s, e) => 
            {
                if (string.IsNullOrWhiteSpace(nameBox.Text))
                {
                    MessageBox.Show("Category name is required.", "Validation Error", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                UpdatedCategory = new Category
                {
                    Id = category.Id,
                    Name = nameBox.Text.Trim(),
                    Description = descBox.Text?.Trim()
                };
                DialogResult = true;
                Close();
            };

            buttonPanel.Children.Add(cancelBtn);
            buttonPanel.Children.Add(saveBtn);
            grid.Children.Add(buttonPanel);

            Content = grid;
        }
    }
}
