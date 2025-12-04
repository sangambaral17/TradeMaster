using System.Windows.Input;
using TradeMaster.Core.Entities;
using TradeMaster.Core.Interfaces;

namespace TradeMaster.Desktop.Services
{
    /// <summary>
    /// Service for handling barcode scanner input.
    /// Barcode scanners typically emulate keyboard input and end with Enter key.
    /// </summary>
    public class BarcodeService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly SettingsService _settingsService;

        private string _barcodeBuffer = "";
        private DateTime _lastKeyTime = DateTime.MinValue;
        private const int BarcodeTimeoutMs = 100; // Max time between keystrokes

        public BarcodeService(IRepository<Product> productRepository, SettingsService settingsService)
        {
            _productRepository = productRepository;
            _settingsService = settingsService;
        }

        /// <summary>
        /// Event raised when a product is successfully scanned.
        /// </summary>
        public event EventHandler<ProductScannedEventArgs>? ProductScanned;

        /// <summary>
        /// Event raised when a barcode is scanned but no product is found.
        /// </summary>
        public event EventHandler<string>? BarcodeNotFound;

        /// <summary>
        /// Indicates whether barcode scanning is enabled.
        /// </summary>
        public bool IsEnabled => _settingsService.Settings.EnableBarcodeScanning;

        /// <summary>
        /// Processes keyboard input for barcode detection.
        /// Call this from the KeyDown event of the POS view.
        /// </summary>
        public async void ProcessKeyInput(KeyEventArgs e)
        {
            if (!IsEnabled) return;

            var now = DateTime.Now;

            // Check if this is a new barcode (too much time passed since last key)
            if ((now - _lastKeyTime).TotalMilliseconds > BarcodeTimeoutMs && !string.IsNullOrEmpty(_barcodeBuffer))
            {
                _barcodeBuffer = "";
            }

            _lastKeyTime = now;

            // Handle Enter key - barcode complete
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(_barcodeBuffer))
            {
                var barcode = _barcodeBuffer;
                _barcodeBuffer = "";
                await LookupBarcodeAsync(barcode);
                e.Handled = true;
                return;
            }

            // Add character to buffer
            var key = e.Key;
            var character = KeyToChar(key, Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));

            if (character.HasValue)
            {
                _barcodeBuffer += character.Value;
            }
        }

        /// <summary>
        /// Manually looks up a product by SKU/barcode.
        /// </summary>
        public async Task<Product?> LookupBarcodeAsync(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode)) return null;

            barcode = barcode.Trim().ToUpperInvariant();

            var products = await _productRepository.FindAsync(p => 
                p.Sku != null && p.Sku.ToUpper() == barcode);

            var product = products.FirstOrDefault();

            if (product != null)
            {
                ProductScanned?.Invoke(this, new ProductScannedEventArgs
                {
                    Product = product,
                    Barcode = barcode
                });
            }
            else
            {
                BarcodeNotFound?.Invoke(this, barcode);
            }

            return product;
        }

        /// <summary>
        /// Resets the barcode buffer.
        /// </summary>
        public void Reset()
        {
            _barcodeBuffer = "";
        }

        private static char? KeyToChar(Key key, bool shift)
        {
            // Number keys
            if (key >= Key.D0 && key <= Key.D9)
            {
                return (char)('0' + (key - Key.D0));
            }

            // Numpad keys
            if (key >= Key.NumPad0 && key <= Key.NumPad9)
            {
                return (char)('0' + (key - Key.NumPad0));
            }

            // Letter keys
            if (key >= Key.A && key <= Key.Z)
            {
                return shift ? (char)('A' + (key - Key.A)) : (char)('a' + (key - Key.A));
            }

            // Common barcode characters
            return key switch
            {
                Key.OemMinus => '-',
                Key.OemPlus => '+',
                Key.OemPeriod => '.',
                Key.Space => ' ',
                _ => null
            };
        }

        /// <summary>
        /// Plays a beep sound for successful scan.
        /// </summary>
        public void PlayScanSound()
        {
            if (_settingsService.Settings.PlaySoundOnScan)
            {
                System.Media.SystemSounds.Beep.Play();
            }
        }

        /// <summary>
        /// Plays an error sound for failed scan.
        /// </summary>
        public void PlayErrorSound()
        {
            System.Media.SystemSounds.Hand.Play();
        }
    }

    public class ProductScannedEventArgs : EventArgs
    {
        public Product Product { get; set; } = null!;
        public string Barcode { get; set; } = string.Empty;
    }
}
