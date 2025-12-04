using System.Globalization;

namespace TradeMaster.Desktop.Services
{
    /// <summary>
    /// Service for managing application localization.
    /// </summary>
    public class LocalizationService
    {
        private readonly SettingsService _settingsService;
        private CultureInfo _currentCulture;

        public LocalizationService(SettingsService settingsService)
        {
            _settingsService = settingsService;
            _currentCulture = new CultureInfo(_settingsService.Settings.Language);
        }

        /// <summary>
        /// Event raised when the language is changed.
        /// </summary>
        public event EventHandler? LanguageChanged;

        /// <summary>
        /// Gets the current culture/language.
        /// </summary>
        public CultureInfo CurrentCulture => _currentCulture;

        /// <summary>
        /// Gets the current language code (e.g., "en-US" or "ne-NP").
        /// </summary>
        public string CurrentLanguageCode => _settingsService.Settings.Language;

        /// <summary>
        /// Indicates whether the current language is Nepali.
        /// </summary>
        public bool IsNepali => CurrentLanguageCode == "ne-NP";

        /// <summary>
        /// Changes the application language.
        /// </summary>
        public void SetLanguage(string cultureCode)
        {
            _settingsService.UpdateSetting(s => s.Language = cultureCode);
            _currentCulture = new CultureInfo(cultureCode);

            // Update thread culture
            Thread.CurrentThread.CurrentCulture = _currentCulture;
            Thread.CurrentThread.CurrentUICulture = _currentCulture;

            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets a localized string by key.
        /// </summary>
        public string GetString(string key)
        {
            return Strings.TryGetValue(key, out var value) ? value : key;
        }

        /// <summary>
        /// Localized strings dictionary.
        /// In a production app, these would be loaded from .resx files.
        /// </summary>
        private Dictionary<string, string> Strings => IsNepali ? NepaliStrings : EnglishStrings;

        private static readonly Dictionary<string, string> EnglishStrings = new()
        {
            // App
            { "AppTitle", "Walsong TradeMaster Enterprise" },
            { "Subtitle", "Inventory & Sales Management System" },

            // Navigation
            { "Dashboard", "Dashboard" },
            { "Products", "Products" },
            { "ProductManagement", "Product Management" },
            { "PointOfSale", "Point of Sale" },
            { "Customers", "Customers" },
            { "CustomerManagement", "Customer Management" },
            { "SalesReports", "Sales Reports" },
            { "Settings", "Settings" },

            // Dashboard
            { "WelcomeMessage", "Welcome to Walsong TradeMaster" },
            { "TotalProducts", "Total Products" },
            { "SystemStatus", "System Status" },
            { "Connected", "Connected" },
            { "OpenModule", "Open Module" },

            // Product
            { "ProductName", "Product Name" },
            { "SKU", "SKU" },
            { "Price", "Price" },
            { "StockQuantity", "Stock Quantity" },
            { "Category", "Category" },
            { "AddProduct", "Add Product" },
            { "EditProduct", "Edit Product" },
            { "DeleteProduct", "Delete Product" },
            { "LowStock", "Low Stock" },

            // POS
            { "Cart", "Cart" },
            { "AddToCart", "Add to Cart" },
            { "RemoveFromCart", "Remove" },
            { "ClearCart", "Clear Cart" },
            { "Checkout", "Checkout" },
            { "Total", "Total" },
            { "Subtotal", "Subtotal" },
            { "Tax", "Tax" },
            { "AmountPaid", "Amount Paid" },
            { "Change", "Change" },
            { "SearchProducts", "Search Products" },

            // Customer
            { "CustomerName", "Customer Name" },
            { "Email", "Email" },
            { "Phone", "Phone" },
            { "Address", "Address" },
            { "AddCustomer", "Add Customer" },
            { "EditCustomer", "Edit Customer" },

            // Sales
            { "SalesHistory", "Sales History" },
            { "Date", "Date" },
            { "TotalAmount", "Total Amount" },
            { "ViewDetails", "View Details" },

            // Settings
            { "CompanyInfo", "Company Information" },
            { "CompanyName", "Company Name" },
            { "Language", "Language" },
            { "BackupRestore", "Backup & Restore" },
            { "CreateBackup", "Create Backup" },
            { "RestoreBackup", "Restore Backup" },
            { "SaveSettings", "Save Settings" },

            // Common
            { "Save", "Save" },
            { "Cancel", "Cancel" },
            { "Delete", "Delete" },
            { "Edit", "Edit" },
            { "Add", "Add" },
            { "Search", "Search" },
            { "Close", "Close" },
            { "Yes", "Yes" },
            { "No", "No" },
            { "Confirm", "Confirm" },
            { "Success", "Success" },
            { "Error", "Error" },
            { "Warning", "Warning" },
            { "Loading", "Loading..." }
        };

        private static readonly Dictionary<string, string> NepaliStrings = new()
        {
            // App
            { "AppTitle", "वाल्सोंग ट्रेडमास्टर इन्टरप्राइज" },
            { "Subtitle", "सामान र बिक्री व्यवस्थापन प्रणाली" },

            // Navigation
            { "Dashboard", "ड्यासबोर्ड" },
            { "Products", "उत्पादनहरू" },
            { "ProductManagement", "उत्पादन व्यवस्थापन" },
            { "PointOfSale", "बिक्री बिन्दु" },
            { "Customers", "ग्राहकहरू" },
            { "CustomerManagement", "ग्राहक व्यवस्थापन" },
            { "SalesReports", "बिक्री रिपोर्टहरू" },
            { "Settings", "सेटिङहरू" },

            // Dashboard
            { "WelcomeMessage", "वाल्सोंग ट्रेडमास्टरमा स्वागत छ" },
            { "TotalProducts", "कुल उत्पादनहरू" },
            { "SystemStatus", "प्रणाली स्थिति" },
            { "Connected", "जोडिएको" },
            { "OpenModule", "खोल्नुहोस्" },

            // Product
            { "ProductName", "उत्पादनको नाम" },
            { "SKU", "SKU" },
            { "Price", "मूल्य" },
            { "StockQuantity", "स्टक मात्रा" },
            { "Category", "वर्ग" },
            { "AddProduct", "उत्पादन थप्नुहोस्" },
            { "EditProduct", "उत्पादन सम्पादन" },
            { "DeleteProduct", "उत्पादन मेटाउनुहोस्" },
            { "LowStock", "कम स्टक" },

            // POS
            { "Cart", "कार्ट" },
            { "AddToCart", "कार्टमा थप्नुहोस्" },
            { "RemoveFromCart", "हटाउनुहोस्" },
            { "ClearCart", "कार्ट खाली गर्नुहोस्" },
            { "Checkout", "चेकआउट" },
            { "Total", "कुल" },
            { "Subtotal", "उप-जम्मा" },
            { "Tax", "कर" },
            { "AmountPaid", "भुक्तान रकम" },
            { "Change", "फिर्ता" },
            { "SearchProducts", "उत्पादन खोज्नुहोस्" },

            // Customer
            { "CustomerName", "ग्राहकको नाम" },
            { "Email", "इमेल" },
            { "Phone", "फोन" },
            { "Address", "ठेगाना" },
            { "AddCustomer", "ग्राहक थप्नुहोस्" },
            { "EditCustomer", "ग्राहक सम्पादन" },

            // Sales
            { "SalesHistory", "बिक्री इतिहास" },
            { "Date", "मिति" },
            { "TotalAmount", "कुल रकम" },
            { "ViewDetails", "विवरण हेर्नुहोस्" },

            // Settings
            { "CompanyInfo", "कम्पनी जानकारी" },
            { "CompanyName", "कम्पनी नाम" },
            { "Language", "भाषा" },
            { "BackupRestore", "ब्याकअप र पुनर्स्थापना" },
            { "CreateBackup", "ब्याकअप बनाउनुहोस्" },
            { "RestoreBackup", "ब्याकअप पुनर्स्थापना गर्नुहोस्" },
            { "SaveSettings", "सेटिङहरू बचत गर्नुहोस्" },

            // Common
            { "Save", "बचत गर्नुहोस्" },
            { "Cancel", "रद्द गर्नुहोस्" },
            { "Delete", "मेटाउनुहोस्" },
            { "Edit", "सम्पादन" },
            { "Add", "थप्नुहोस्" },
            { "Search", "खोज्नुहोस्" },
            { "Close", "बन्द गर्नुहोस्" },
            { "Yes", "हो" },
            { "No", "होइन" },
            { "Confirm", "पुष्टि गर्नुहोस्" },
            { "Success", "सफल" },
            { "Error", "त्रुटि" },
            { "Warning", "चेतावनी" },
            { "Loading", "लोड हुँदैछ..." }
        };
    }
}
