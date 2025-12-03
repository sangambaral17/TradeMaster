using System.Windows;
using TradeMaster.Core.Entities;
using TradeMaster.Core.Interfaces;

namespace TradeMaster.Desktop.Views
{
    public partial class CustomerEditDialog : Window
    {
        private readonly Customer? _customer;
        private readonly IRepository<Customer> _customerRepository;
        private readonly bool _isEditMode;

        public CustomerEditDialog(Customer? customer, IRepository<Customer> customerRepository)
        {
            InitializeComponent();
            _customer = customer;
            _customerRepository = customerRepository;
            _isEditMode = customer != null;

            if (_isEditMode && _customer != null)
            {
                HeaderText.Text = "Edit Customer";
                NameTextBox.Text = _customer.Name;
                EmailTextBox.Text = _customer.Email;
                PhoneTextBox.Text = _customer.Phone;
                AddressTextBox.Text = _customer.Address;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Customer name is required.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_isEditMode && _customer != null)
                {
                    // Update existing customer
                    _customer.Name = NameTextBox.Text.Trim();
                    _customer.Email = string.IsNullOrWhiteSpace(EmailTextBox.Text) ? null : EmailTextBox.Text.Trim();
                    _customer.Phone = string.IsNullOrWhiteSpace(PhoneTextBox.Text) ? null : PhoneTextBox.Text.Trim();
                    _customer.Address = string.IsNullOrWhiteSpace(AddressTextBox.Text) ? null : AddressTextBox.Text.Trim();

                    await _customerRepository.UpdateAsync(_customer);
                    MessageBox.Show("Customer updated successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Create new customer
                    var newCustomer = new Customer
                    {
                        Name = NameTextBox.Text.Trim(),
                        Email = string.IsNullOrWhiteSpace(EmailTextBox.Text) ? null : EmailTextBox.Text.Trim(),
                        Phone = string.IsNullOrWhiteSpace(PhoneTextBox.Text) ? null : PhoneTextBox.Text.Trim(),
                        Address = string.IsNullOrWhiteSpace(AddressTextBox.Text) ? null : AddressTextBox.Text.Trim(),
                        CreatedDate = DateTime.Now
                    };

                    await _customerRepository.AddAsync(newCustomer);
                    MessageBox.Show("Customer added successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving customer: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
