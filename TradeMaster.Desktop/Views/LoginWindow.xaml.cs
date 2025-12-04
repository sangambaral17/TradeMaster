using System.Windows;
using TradeMaster.Desktop.Services;

namespace TradeMaster.Desktop.Views
{
    public partial class LoginWindow : Window
    {
        private readonly AuthenticationService _authService;

        public LoginWindow(AuthenticationService authService)
        {
            InitializeComponent();
            _authService = authService;
            UsernameTextBox.Focus();
        }

        public bool LoginSuccessful { get; private set; }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text.Trim();
            var password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Please enter both username and password.");
                return;
            }

            LoginButton.IsEnabled = false;
            LoginButton.Content = "Signing in...";

            try
            {
                var result = await _authService.LoginAsync(username, password);
                if (result)
                {
                    LoginSuccessful = true;
                    DialogResult = true;
                    Close();
                }
                else
                {
                    ShowError("Invalid username or password.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Login failed: {ex.Message}");
            }
            finally
            {
                LoginButton.IsEnabled = true;
                LoginButton.Content = "Login";
            }
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            // For development - skip login
            LoginSuccessful = true;
            DialogResult = true;
            Close();
        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorMessage.Visibility = Visibility.Visible;
        }
    }
}
