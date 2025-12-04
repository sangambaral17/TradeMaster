using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using TradeMaster.Desktop.Services;

namespace TradeMaster.Desktop.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly SettingsService _settingsService;

        public SettingsViewModel(SettingsService settingsService)
        {
            _settingsService = settingsService;
            LoadSettings();
        }

        // General Settings
        [ObservableProperty]
        private string _companyName = string.Empty;

        [ObservableProperty]
        private string _companyAddress = string.Empty;

        [ObservableProperty]
        private string _companyPhone = string.Empty;

        [ObservableProperty]
        private string _currencySymbol = string.Empty;

        // Language Settings
        [ObservableProperty]
        private string _selectedLanguage = "en-US";

        public ObservableCollection<LanguageOption> Languages { get; } = new()
        {
            new LanguageOption { Code = "en-US", Name = "English", Flag = "🇺🇸" },
            new LanguageOption { Code = "ne-NP", Name = "नेपाली (Nepali)", Flag = "🇳🇵" }
        };

        public ObservableCollection<CurrencyOption> CurrencyOptions { get; } = new()
        {
            new CurrencyOption { Code = "USD", Symbol = "$", Display = "USD - $" },
            new CurrencyOption { Code = "AUD", Symbol = "A$", Display = "AUD - A$" },
            new CurrencyOption { Code = "NPR", Symbol = "Rs.", Display = "NPR - Rs." }
        };

        // Inventory Settings
        [ObservableProperty]
        private int _defaultLowStockThreshold = 5;

        [ObservableProperty]
        private int _defaultReorderQuantity = 20;

        [ObservableProperty]
        private bool _showLowStockAlerts = true;

        // Printing Settings
        [ObservableProperty]
        private string _defaultPrinterName = string.Empty;

        [ObservableProperty]
        private bool _autoPrintReceipt = false;

        [ObservableProperty]
        private bool _showReceiptPreview = true;

        // Backup Settings
        [ObservableProperty]
        private bool _autoBackupEnabled = true;

        [ObservableProperty]
        private int _backupIntervalHours = 24;

        [ObservableProperty]
        private string _backupLocation = string.Empty;

        [ObservableProperty]
        private int _maxBackupsToKeep = 7;

        [ObservableProperty]
        private string _lastBackupInfo = "Never";

        // User Settings
        [ObservableProperty]
        private bool _requireLogin = false;

        [ObservableProperty]
        private int _sessionTimeoutMinutes = 30;

        // POS Settings
        [ObservableProperty]
        private bool _enableBarcodeScanning = true;

        [ObservableProperty]
        private bool _playSoundOnScan = true;

        // Status
        [ObservableProperty]
        private string _statusMessage = string.Empty;

        private void LoadSettings()
        {
            var settings = _settingsService.Settings;

            // General
            CompanyName = settings.CompanyName;
            CompanyAddress = settings.CompanyAddress;
            CompanyPhone = settings.CompanyPhone;
            CurrencySymbol = settings.CurrencySymbol;

            // Language
            SelectedLanguage = settings.Language;

            // Inventory
            DefaultLowStockThreshold = settings.DefaultLowStockThreshold;
            DefaultReorderQuantity = settings.DefaultReorderQuantity;
            ShowLowStockAlerts = settings.ShowLowStockAlerts;

            // Printing
            DefaultPrinterName = settings.DefaultPrinterName;
            AutoPrintReceipt = settings.AutoPrintReceipt;
            ShowReceiptPreview = settings.ShowReceiptPreview;

            // Backup
            AutoBackupEnabled = settings.AutoBackupEnabled;
            BackupIntervalHours = settings.BackupIntervalHours;
            BackupLocation = string.IsNullOrEmpty(settings.BackupLocation)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TradeMasterBackups")
                : settings.BackupLocation;
            MaxBackupsToKeep = settings.MaxBackupsToKeep;
            LastBackupInfo = settings.LastBackupDate?.ToString("yyyy-MM-dd HH:mm") ?? "Never";

            // User
            RequireLogin = settings.RequireLogin;
            SessionTimeoutMinutes = settings.SessionTimeoutMinutes;

            // POS
            EnableBarcodeScanning = settings.EnableBarcodeScanning;
            PlaySoundOnScan = settings.PlaySoundOnScan;
        }

        [RelayCommand]
        private void SaveSettings()
        {
            _settingsService.UpdateSetting(s =>
            {
                // General
                s.CompanyName = CompanyName;
                s.CompanyAddress = CompanyAddress;
                s.CompanyPhone = CompanyPhone;
                s.CurrencySymbol = CurrencySymbol;

                // Language
                s.Language = SelectedLanguage;

                // Inventory
                s.DefaultLowStockThreshold = DefaultLowStockThreshold;
                s.DefaultReorderQuantity = DefaultReorderQuantity;
                s.ShowLowStockAlerts = ShowLowStockAlerts;

                // Printing
                s.DefaultPrinterName = DefaultPrinterName;
                s.AutoPrintReceipt = AutoPrintReceipt;
                s.ShowReceiptPreview = ShowReceiptPreview;

                // Backup
                s.AutoBackupEnabled = AutoBackupEnabled;
                s.BackupIntervalHours = BackupIntervalHours;
                s.BackupLocation = BackupLocation;
                s.MaxBackupsToKeep = MaxBackupsToKeep;

                // User
                s.RequireLogin = RequireLogin;
                s.SessionTimeoutMinutes = SessionTimeoutMinutes;

                // POS
                s.EnableBarcodeScanning = EnableBarcodeScanning;
                s.PlaySoundOnScan = PlaySoundOnScan;
            });

            StatusMessage = "✅ Settings saved successfully!";
            MessageBox.Show("Settings saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void BrowseBackupLocation()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Select Backup Folder",
                FileName = "Select Folder",
                Filter = "Folder|*.folder",
                CheckFileExists = false
            };

            // Workaround to select folder using SaveFileDialog
            if (dialog.ShowDialog() == true)
            {
                BackupLocation = Path.GetDirectoryName(dialog.FileName) ?? BackupLocation;
            }
        }

        [RelayCommand]
        private void ResetToDefaults()
        {
            var result = MessageBox.Show(
                "Are you sure you want to reset all settings to their default values?",
                "Confirm Reset",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var defaults = new AppSettings();
                CompanyName = defaults.CompanyName;
                CompanyAddress = defaults.CompanyAddress;
                CompanyPhone = defaults.CompanyPhone;
                CurrencySymbol = defaults.CurrencySymbol;
                SelectedLanguage = defaults.Language;
                DefaultLowStockThreshold = defaults.DefaultLowStockThreshold;
                DefaultReorderQuantity = defaults.DefaultReorderQuantity;
                ShowLowStockAlerts = defaults.ShowLowStockAlerts;
                DefaultPrinterName = defaults.DefaultPrinterName;
                AutoPrintReceipt = defaults.AutoPrintReceipt;
                ShowReceiptPreview = defaults.ShowReceiptPreview;
                AutoBackupEnabled = defaults.AutoBackupEnabled;
                BackupIntervalHours = defaults.BackupIntervalHours;
                MaxBackupsToKeep = defaults.MaxBackupsToKeep;
                RequireLogin = defaults.RequireLogin;
                SessionTimeoutMinutes = defaults.SessionTimeoutMinutes;
                EnableBarcodeScanning = defaults.EnableBarcodeScanning;
                PlaySoundOnScan = defaults.PlaySoundOnScan;

                StatusMessage = "Settings reset to defaults. Click Save to apply.";
            }
        }
    }

    public class LanguageOption
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Flag { get; set; } = string.Empty;
        public string DisplayName => $"{Flag} {Name}";
    }

    public class CurrencyOption
    {
        public string Code { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string Display { get; set; } = string.Empty;
    }
}
