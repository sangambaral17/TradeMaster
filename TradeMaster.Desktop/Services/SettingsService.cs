using System.IO;
using System.Text.Json;

namespace TradeMaster.Desktop.Services
{
    /// <summary>
    /// Service for managing application settings with JSON persistence.
    /// </summary>
    public class SettingsService
    {
        private static readonly string SettingsFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

        private AppSettings _settings;

        public SettingsService()
        {
            _settings = LoadSettings();
        }

        public AppSettings Settings => _settings;

        /// <summary>
        /// Loads settings from JSON file or creates default settings if file doesn't exist.
        /// </summary>
        private AppSettings LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    var json = File.ReadAllText(SettingsFilePath);
                    return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch (Exception)
            {
                // If loading fails, return default settings
            }

            var defaultSettings = new AppSettings();
            SaveSettings(defaultSettings);
            return defaultSettings;
        }

        /// <summary>
        /// Saves current settings to JSON file.
        /// </summary>
        public void SaveSettings()
        {
            SaveSettings(_settings);
        }

        private void SaveSettings(AppSettings settings)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch (Exception)
            {
                // Handle save errors gracefully
            }
        }

        /// <summary>
        /// Updates a specific setting and saves.
        /// </summary>
        public void UpdateSetting(Action<AppSettings> updateAction)
        {
            updateAction(_settings);
            SaveSettings();
        }
    }

    /// <summary>
    /// Application settings model.
    /// </summary>
    public class AppSettings
    {
        // General Settings
        public string CompanyName { get; set; } = "Walsong Nepal";
        public string CompanyAddress { get; set; } = "Kathmandu, Nepal";
        public string CompanyPhone { get; set; } = "+977-1-XXXXXXX";
        public string Currency { get; set; } = "NPR";
        public string CurrencySymbol { get; set; } = "Rs.";

        // Language Settings
        public string Language { get; set; } = "en-US"; // en-US or ne-NP

        // Inventory Settings
        public int DefaultLowStockThreshold { get; set; } = 5;
        public int DefaultReorderQuantity { get; set; } = 20;
        public bool ShowLowStockAlerts { get; set; } = true;

        // Printing Settings
        public string DefaultPrinterName { get; set; } = "";
        public bool AutoPrintReceipt { get; set; } = false;
        public bool ShowReceiptPreview { get; set; } = true;
        public int ReceiptWidth { get; set; } = 48; // Characters for 80mm thermal

        // Backup Settings
        public bool AutoBackupEnabled { get; set; } = true;
        public int BackupIntervalHours { get; set; } = 24;
        public string BackupLocation { get; set; } = "";
        public int MaxBackupsToKeep { get; set; } = 7;
        public DateTime? LastBackupDate { get; set; }

        // User Settings
        public bool RequireLogin { get; set; } = false;
        public int SessionTimeoutMinutes { get; set; } = 30;

        // POS Settings
        public bool EnableBarcodeScanning { get; set; } = true;
        public bool PlaySoundOnScan { get; set; } = true;
    }
}
