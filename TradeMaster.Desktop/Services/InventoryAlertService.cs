using TradeMaster.Core.Entities;
using TradeMaster.Core.Interfaces;

namespace TradeMaster.Desktop.Services
{
    /// <summary>
    /// Service for monitoring inventory levels and generating alerts.
    /// </summary>
    public class InventoryAlertService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly SettingsService _settingsService;

        public InventoryAlertService(IRepository<Product> productRepository, SettingsService settingsService)
        {
            _productRepository = productRepository;
            _settingsService = settingsService;
        }

        /// <summary>
        /// Gets all products that are low on stock.
        /// </summary>
        public async Task<IEnumerable<LowStockAlert>> GetLowStockAlertsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            var alerts = products
                .Where(p => p.StockQuantity <= p.LowStockThreshold)
                .Select(p => new LowStockAlert
                {
                    Product = p,
                    CurrentStock = p.StockQuantity,
                    Threshold = p.LowStockThreshold,
                    ReorderQuantity = p.ReorderQuantity,
                    Severity = GetSeverity(p.StockQuantity, p.LowStockThreshold)
                })
                .OrderBy(a => a.Severity)
                .ThenBy(a => a.CurrentStock)
                .ToList();

            return alerts;
        }

        /// <summary>
        /// Gets count of products with low stock.
        /// </summary>
        public async Task<int> GetLowStockCountAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Count(p => p.StockQuantity <= p.LowStockThreshold);
        }

        /// <summary>
        /// Gets products that are completely out of stock.
        /// </summary>
        public async Task<IEnumerable<Product>> GetOutOfStockProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Where(p => p.StockQuantity == 0).ToList();
        }

        /// <summary>
        /// Generates reorder suggestions based on current stock levels.
        /// </summary>
        public async Task<IEnumerable<ReorderSuggestion>> GetReorderSuggestionsAsync()
        {
            var alerts = await GetLowStockAlertsAsync();
            return alerts.Select(a => new ReorderSuggestion
            {
                ProductId = a.Product.Id,
                ProductName = a.Product.Name,
                Sku = a.Product.Sku ?? "",
                CurrentStock = a.CurrentStock,
                SuggestedOrderQuantity = a.ReorderQuantity,
                EstimatedCost = a.Product.Price * a.ReorderQuantity,
                Priority = a.Severity
            }).ToList();
        }

        /// <summary>
        /// Updates the low stock threshold for a product.
        /// </summary>
        public async Task UpdateThresholdAsync(int productId, int newThreshold)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product != null)
            {
                product.LowStockThreshold = newThreshold;
                await _productRepository.UpdateAsync(product);
            }
        }

        /// <summary>
        /// Updates the reorder quantity for a product.
        /// </summary>
        public async Task UpdateReorderQuantityAsync(int productId, int newQuantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product != null)
            {
                product.ReorderQuantity = newQuantity;
                await _productRepository.UpdateAsync(product);
            }
        }

        private static AlertSeverity GetSeverity(int currentStock, int threshold)
        {
            if (currentStock == 0) return AlertSeverity.Critical;
            if (currentStock <= threshold / 2) return AlertSeverity.High;
            if (currentStock <= threshold) return AlertSeverity.Medium;
            return AlertSeverity.Low;
        }
    }

    public class LowStockAlert
    {
        public Product Product { get; set; } = null!;
        public int CurrentStock { get; set; }
        public int Threshold { get; set; }
        public int ReorderQuantity { get; set; }
        public AlertSeverity Severity { get; set; }

        public string SeverityDisplay => Severity switch
        {
            AlertSeverity.Critical => "🔴 Critical",
            AlertSeverity.High => "🟠 High",
            AlertSeverity.Medium => "🟡 Medium",
            AlertSeverity.Low => "🟢 Low",
            _ => "Unknown"
        };
    }

    public class ReorderSuggestion
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int SuggestedOrderQuantity { get; set; }
        public decimal EstimatedCost { get; set; }
        public AlertSeverity Priority { get; set; }

        public string EstimatedCostFormatted => $"Rs. {EstimatedCost:N2}";
    }

    public enum AlertSeverity
    {
        Critical = 0,
        High = 1,
        Medium = 2,
        Low = 3
    }
}
