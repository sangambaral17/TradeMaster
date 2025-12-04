using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradeMaster.Core.Entities
{
    public class SaleItem
    {
        public int Id { get; set; }

        public int SaleId { get; set; }
        public Sale? Sale { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [MaxLength(200)]
        public string ProductName { get; set; } = string.Empty; // Store name at time of sale

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; } // Price at the time of sale

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; } // UnitPrice * Quantity
    }
}

