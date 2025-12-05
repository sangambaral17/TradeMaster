using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradeMaster.Core.Entities
{
    public class Sale
    {
        public int Id { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(100)]
        public string? CustomerName { get; set; }

        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        // Payment method: Cash, Card, UPI, eSewa
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = "Cash";

        // Navigation property
        public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
    }
}
