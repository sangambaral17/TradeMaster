using System.Text;
using TradeMaster.Core.Entities;

namespace TradeMaster.Desktop.Services;

/// <summary>
/// Formats sales data into shareable text-based bills/receipts
/// </summary>
public static class BillFormatter
{
    public static string FormatBill(Sale sale, List<SaleItem> items, string companyName = "WALSONG TRADEMASTER")
    {
        var sb = new StringBuilder();
        
        // Header
        sb.AppendLine("═══════════════════════════════");
        sb.AppendLine($"    {companyName}");
        sb.AppendLine("═══════════════════════════════");
        sb.AppendLine();
        
        // Sale Info
        sb.AppendLine($"Date: {sale.SaleDate:yyyy-MM-dd hh:mm tt}");
        sb.AppendLine($"Invoice: #INV-{sale.Id:D6}");
        
        if (!string.IsNullOrEmpty(sale.CustomerName))
        {
            sb.AppendLine($"Customer: {sale.CustomerName}");
        }
        
        sb.AppendLine();
        sb.AppendLine("ITEMS:");
        sb.AppendLine("───────────────────────────────");
        
        // Items
        foreach (var item in items)
        {
            var productName = item.Product?.Name ?? "Unknown Product";
            var qty = item.Quantity;
            var price = item.TotalPrice;
            
            // Format: "2x Product Name    Rs. 200.00"
            var line = $"{qty}x {productName}";
            var priceStr = $"Rs. {price:F2}";
            
            // Pad to align prices on the right
            var padding = 31 - line.Length - priceStr.Length;
            if (padding > 0)
            {
                line += new string(' ', padding);
            }
            line += priceStr;
            
            sb.AppendLine(line);
        }
        
        sb.AppendLine("───────────────────────────────");
        
        // Calculate totals
        var subtotal = items.Sum(i => i.TotalPrice);
        var tax = sale.TotalAmount - subtotal;
        
        // Subtotal
        sb.AppendLine($"Subtotal:       Rs. {subtotal,10:F2}");
        
        if (tax > 0)
        {
            sb.AppendLine($"Tax:            Rs. {tax,10:F2}");
        }
        
        sb.AppendLine("───────────────────────────────");
        sb.AppendLine($"TOTAL:          Rs. {sale.TotalAmount,10:F2}");
        sb.AppendLine();
        
        // Payment Method
        sb.AppendLine($"Payment: {sale.PaymentMethod}");
        sb.AppendLine();
        
        // Footer
        sb.AppendLine("Thank you for shopping!");
        sb.AppendLine("═══════════════════════════════");
        
        return sb.ToString();
    }
    
    /// <summary>
    /// Format bill for WhatsApp/Viber (with emojis)
    /// </summary>
    public static string FormatBillForSocial(Sale sale, List<SaleItem> items)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("🧾 *WALSONG TRADEMASTER*");
        sb.AppendLine();
        sb.AppendLine($"📅 {sale.SaleDate:MMM dd, yyyy hh:mm tt}");
        sb.AppendLine($"🔖 Invoice: #INV-{sale.Id:D6}");
        
        if (!string.IsNullOrEmpty(sale.CustomerName))
        {
            sb.AppendLine($"👤 {sale.CustomerName}");
        }
        
        sb.AppendLine();
        sb.AppendLine("📦 *ITEMS:*");
        
        foreach (var item in items)
        {
            var productName = item.Product?.Name ?? "Unknown";
            sb.AppendLine($"  {item.Quantity}x {productName} - Rs. {item.TotalPrice:F2}");
        }
        
        sb.AppendLine();
        sb.AppendLine($"💰 *TOTAL: Rs. {sale.TotalAmount:F2}*");
        sb.AppendLine($"💳 Payment: {sale.PaymentMethod}");
        sb.AppendLine();
        sb.AppendLine("✨ Thank you for shopping!");
        
        return sb.ToString();
    }
}
