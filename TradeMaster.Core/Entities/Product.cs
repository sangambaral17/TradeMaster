using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradeMaster.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Sku { get; set; } // Stock Keeping Unit

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [MaxLength(10)]
        public string Currency { get; set; } = "USD";

        public int StockQuantity { get; set; }

        public int CategoryId { get; set; }

        // Inventory Alert Properties
        public int LowStockThreshold { get; set; } = 5;
        public int ReorderQuantity { get; set; } = 20;

        // Navigation property
        public Category? Category { get; set; }

        // Computed property for checking low stock
        [NotMapped]
        public bool IsLowStock => StockQuantity <= LowStockThreshold;
    }
}
