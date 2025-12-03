using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using TradeMaster.Core.Entities;
using TradeMaster.Core.Interfaces;
using TradeMaster.Desktop.Views;

namespace TradeMaster.Desktop.ViewModels
{
    public partial class CustomerListViewModel : ObservableObject
    {
        private readonly IRepository<Customer> _customerRepository;

        [ObservableProperty]
        private ObservableCollection<Customer> _customers = new();

        [ObservableProperty]
        private ObservableCollection<Customer> _filteredCustomers = new();

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private Customer? _selectedCustomer;

        // Add optional parameter to control automatic background loading (useful for tests)
        public CustomerListViewModel(IRepository<Customer> customerRepository, bool autoLoad = true)
        {
            _customerRepository = customerRepository;
            if (autoLoad)
            {
                _ = Task.Run(async () => await LoadCustomers());
            }
        }

        partial void OnSearchQueryChanged(string value)
        {
            FilterCustomers();
        }

        private void FilterCustomers()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                FilteredCustomers = new ObservableCollection<Customer>(Customers);
            }
            else
            {
                var filtered = Customers.Where(c =>
                    c.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    (c.Email != null && c.Email.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Phone != null && c.Phone.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)));
                FilteredCustomers = new ObservableCollection<Customer>(filtered);
            }
        }

        [RelayCommand]
        private async Task LoadCustomers()
        {
            var customers = await _customerRepository.GetAllAsync();
            Customers = new ObservableCollection<Customer>(customers);
            FilteredCustomers = new ObservableCollection<Customer>(Customers);
        }

        [RelayCommand]
        private void AddCustomer()
        {
            var dialog = new CustomerEditDialog(null, _customerRepository);
            if (dialog.ShowDialog() == true)
            {
                _ = LoadCustomers();
            }
        }

        [RelayCommand]
        private void EditCustomer(Customer? customer)
        {
            if (customer == null) return;

            var dialog = new CustomerEditDialog(customer, _customerRepository);
            if (dialog.ShowDialog() == true)
            {
                _ = LoadCustomers();
            }
        }

        [RelayCommand]
        private async Task DeleteCustomer(Customer? customer)
        {
            if (customer == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete customer '{customer.Name}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _customerRepository.DeleteAsync(customer);
                    await LoadCustomers();
                    MessageBox.Show("Customer deleted successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting customer: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
