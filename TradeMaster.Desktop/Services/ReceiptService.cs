using System.Text;
using TradeMaster.Core.Entities;

namespace TradeMaster.Desktop.Services
{
    /// <summary>
    /// Service for generating and printing receipts.
    /// </summary>
    public class ReceiptService
    {
        private readonly SettingsService _settingsService;

        public ReceiptService(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        /// <summary>
        /// Generates receipt content for a sale.
        /// </summary>
        public ReceiptContent GenerateReceipt(Sale sale, decimal amountPaid = 0, decimal change = 0)
        {
            var settings = _settingsService.Settings;
            var width = settings.ReceiptWidth;
            var lines = new List<string>();

            // Header
            lines.Add(CenterText(settings.CompanyName, width));
            lines.Add(CenterText(settings.CompanyAddress, width));
            lines.Add(CenterText($"Tel: {settings.CompanyPhone}", width));
            lines.Add(new string('=', width));

            // Receipt Info
            lines.Add($"Receipt #: {sale.Id}");
            lines.Add($"Date: {sale.SaleDate:yyyy-MM-dd HH:mm}");
            if (!string.IsNullOrEmpty(sale.CustomerName))
            {
                lines.Add($"Customer: {sale.CustomerName}");
            }
            lines.Add(new string('-', width));

            // Column Headers
            lines.Add(FormatItemLine("Item", "Qty", "Price", "Total", width));
            lines.Add(new string('-', width));

            // Items
            foreach (var item in sale.Items)
            {
                var itemName = TruncateText(item.ProductName, 18);
                var qty = item.Quantity.ToString();
                var price = $"{settings.CurrencySymbol}{item.UnitPrice:N0}";
                var total = $"{settings.CurrencySymbol}{(item.Quantity * item.UnitPrice):N0}";
                lines.Add(FormatItemLine(itemName, qty, price, total, width));
            }

            lines.Add(new string('-', width));

            // Totals
            lines.Add(RightAlignText($"Subtotal: {settings.CurrencySymbol}{sale.TotalAmount:N2}", width));
            lines.Add(RightAlignText($"Tax (0%): {settings.CurrencySymbol}0.00", width));
            lines.Add(new string('=', width));
            lines.Add(RightAlignText($"TOTAL: {settings.CurrencySymbol}{sale.TotalAmount:N2}", width));
            lines.Add(new string('=', width));

            // Payment Info
            if (amountPaid > 0)
            {
                lines.Add(RightAlignText($"Cash: {settings.CurrencySymbol}{amountPaid:N2}", width));
                lines.Add(RightAlignText($"Change: {settings.CurrencySymbol}{change:N2}", width));
            }

            lines.Add("");
            lines.Add(CenterText("Thank you for your purchase!", width));
            lines.Add(CenterText("Please visit again.", width));
            lines.Add("");
            lines.Add(CenterText("--- End of Receipt ---", width));

            return new ReceiptContent
            {
                Lines = lines,
                PlainText = string.Join(Environment.NewLine, lines),
                Sale = sale,
                GeneratedAt = DateTime.Now
            };
        }

        /// <summary>
        /// Generates a simplified receipt for display.
        /// </summary>
        public string GenerateReceiptText(Sale sale, decimal amountPaid = 0, decimal change = 0)
        {
            return GenerateReceipt(sale, amountPaid, change).PlainText;
        }

        /// <summary>
        /// Prints receipt to the default or specified printer.
        /// </summary>
        public void PrintReceipt(ReceiptContent receipt, string? printerName = null)
        {
            var settings = _settingsService.Settings;
            var printer = printerName ?? settings.DefaultPrinterName;

            // For now, we'll use the default print dialog
            // In a real implementation, you would use System.Drawing.Printing
            // or a thermal printer library

            var printDialog = new System.Windows.Controls.PrintDialog();
            if (string.IsNullOrEmpty(printer) || printDialog.ShowDialog() == true)
            {
                var document = new System.Windows.Documents.FlowDocument();
                var paragraph = new System.Windows.Documents.Paragraph();
                paragraph.FontFamily = new System.Windows.Media.FontFamily("Consolas");
                paragraph.FontSize = 10;
                paragraph.Inlines.Add(new System.Windows.Documents.Run(receipt.PlainText));
                document.Blocks.Add(paragraph);

                // Print using the document viewer
                var paginator = ((System.Windows.Documents.IDocumentPaginatorSource)document).DocumentPaginator;
                printDialog.PrintDocument(paginator, "TradeMaster Receipt");
            }
        }

        #region Helper Methods

        private static string CenterText(string text, int width)
        {
            if (text.Length >= width) return text.Substring(0, width);
            var padding = (width - text.Length) / 2;
            return new string(' ', padding) + text;
        }

        private static string RightAlignText(string text, int width)
        {
            if (text.Length >= width) return text;
            var padding = width - text.Length;
            return new string(' ', padding) + text;
        }

        private static string TruncateText(string text, int maxLength)
        {
            if (text.Length <= maxLength) return text;
            return text.Substring(0, maxLength - 2) + "..";
        }

        private static string FormatItemLine(string name, string qty, string price, string total, int width)
        {
            // Format: Name(20) Qty(4) Price(10) Total(14)
            var nameWidth = 18;
            var qtyWidth = 4;
            var priceWidth = 12;
            var totalWidth = width - nameWidth - qtyWidth - priceWidth;

            var line = new StringBuilder();
            line.Append(name.PadRight(nameWidth).Substring(0, nameWidth));
            line.Append(qty.PadLeft(qtyWidth).Substring(0, qtyWidth));
            line.Append(price.PadLeft(priceWidth).Substring(0, priceWidth));
            line.Append(total.PadLeft(totalWidth));

            return line.ToString();
        }

        #endregion
    }

    public class ReceiptContent
    {
        public List<string> Lines { get; set; } = new();
        public string PlainText { get; set; } = string.Empty;
        public Sale Sale { get; set; } = null!;
        public DateTime GeneratedAt { get; set; }
    }
}
