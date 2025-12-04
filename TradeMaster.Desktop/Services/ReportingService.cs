using TradeMaster.Core.Entities;
using TradeMaster.Core.Interfaces;

namespace TradeMaster.Desktop.Services
{
    /// <summary>
    /// Service for generating various business reports.
    /// </summary>
    public class ReportingService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Customer> _customerRepository;

        public ReportingService(
            ISaleRepository saleRepository,
            IRepository<Product> productRepository,
            IRepository<Customer> customerRepository)
        {
            _saleRepository = saleRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
        }

        /// <summary>
        /// Generates a daily sales summary for a specific date.
        /// </summary>
        public async Task<DailySalesReport> GetDailySummaryAsync(DateTime date)
        {
            var sales = await _saleRepository.GetSalesWithItemsAsync();
            var dailySales = sales.Where(s => s.SaleDate.Date == date.Date).ToList();

            var report = new DailySalesReport
            {
                ReportDate = date,
                TotalSales = dailySales.Count,
                TotalRevenue = dailySales.Sum(s => s.TotalAmount),
                AverageOrderValue = dailySales.Any() ? dailySales.Average(s => s.TotalAmount) : 0,
                ItemsSold = dailySales.SelectMany(s => s.Items).Sum(i => i.Quantity),
                TopSellingProducts = GetTopProducts(dailySales, 5),
                HourlySales = GetHourlySales(dailySales)
            };

            return report;
        }

        /// <summary>
        /// Generates a weekly sales summary.
        /// </summary>
        public async Task<WeeklySalesReport> GetWeeklySummaryAsync(DateTime weekStart)
        {
            var weekEnd = weekStart.AddDays(7);
            var sales = await _saleRepository.GetSalesWithItemsAsync();
            var weeklySales = sales.Where(s => s.SaleDate.Date >= weekStart.Date && s.SaleDate.Date < weekEnd.Date).ToList();

            var dailyBreakdown = new List<DailySummary>();
            for (int i = 0; i < 7; i++)
            {
                var day = weekStart.AddDays(i);
                var daySales = weeklySales.Where(s => s.SaleDate.Date == day.Date).ToList();
                dailyBreakdown.Add(new DailySummary
                {
                    Date = day,
                    DayName = day.ToString("dddd"),
                    SalesCount = daySales.Count,
                    Revenue = daySales.Sum(s => s.TotalAmount)
                });
            }

            return new WeeklySalesReport
            {
                WeekStart = weekStart,
                WeekEnd = weekEnd.AddDays(-1),
                TotalSales = weeklySales.Count,
                TotalRevenue = weeklySales.Sum(s => s.TotalAmount),
                AverageOrderValue = weeklySales.Any() ? weeklySales.Average(s => s.TotalAmount) : 0,
                DailyBreakdown = dailyBreakdown,
                TopSellingProducts = GetTopProducts(weeklySales, 10)
            };
        }

        /// <summary>
        /// Generates a monthly sales summary.
        /// </summary>
        public async Task<MonthlySalesReport> GetMonthlySummaryAsync(int month, int year)
        {
            var monthStart = new DateTime(year, month, 1);
            var monthEnd = monthStart.AddMonths(1);
            var sales = await _saleRepository.GetSalesWithItemsAsync();
            var monthlySales = sales.Where(s => s.SaleDate >= monthStart && s.SaleDate < monthEnd).ToList();

            var weeklyBreakdown = new List<WeeklySummary>();
            var currentWeekStart = monthStart;
            int weekNum = 1;
            while (currentWeekStart < monthEnd)
            {
                var weekEnd = currentWeekStart.AddDays(7);
                if (weekEnd > monthEnd) weekEnd = monthEnd;
                
                var weekSales = monthlySales
                    .Where(s => s.SaleDate >= currentWeekStart && s.SaleDate < weekEnd)
                    .ToList();

                weeklyBreakdown.Add(new WeeklySummary
                {
                    WeekNumber = weekNum++,
                    StartDate = currentWeekStart,
                    EndDate = weekEnd.AddDays(-1),
                    SalesCount = weekSales.Count,
                    Revenue = weekSales.Sum(s => s.TotalAmount)
                });

                currentWeekStart = weekEnd;
            }

            return new MonthlySalesReport
            {
                Month = month,
                Year = year,
                MonthName = monthStart.ToString("MMMM"),
                TotalSales = monthlySales.Count,
                TotalRevenue = monthlySales.Sum(s => s.TotalAmount),
                AverageOrderValue = monthlySales.Any() ? monthlySales.Average(s => s.TotalAmount) : 0,
                WeeklyBreakdown = weeklyBreakdown,
                TopSellingProducts = GetTopProducts(monthlySales, 15),
                TopCustomers = await GetTopCustomersAsync(monthlySales, 10)
            };
        }

        /// <summary>
        /// Gets top selling products for a date range.
        /// </summary>
        public async Task<List<TopProductReport>> GetTopSellingProductsAsync(DateTime start, DateTime end, int count = 10)
        {
            var sales = await _saleRepository.GetSalesWithItemsAsync();
            var periodSales = sales.Where(s => s.SaleDate >= start && s.SaleDate <= end).ToList();
            return GetTopProducts(periodSales, count);
        }

        /// <summary>
        /// Gets customer purchase history.
        /// </summary>
        public async Task<CustomerPurchaseHistoryReport> GetCustomerPurchaseHistoryAsync(int customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                throw new ArgumentException("Customer not found.");
            }

            var sales = await _saleRepository.GetSalesWithItemsAsync();
            var customerSales = sales.Where(s => s.CustomerId == customerId).OrderByDescending(s => s.SaleDate).ToList();

            return new CustomerPurchaseHistoryReport
            {
                Customer = customer,
                TotalPurchases = customerSales.Count,
                TotalSpent = customerSales.Sum(s => s.TotalAmount),
                AverageOrderValue = customerSales.Any() ? customerSales.Average(s => s.TotalAmount) : 0,
                FirstPurchase = customerSales.LastOrDefault()?.SaleDate,
                LastPurchase = customerSales.FirstOrDefault()?.SaleDate,
                Purchases = customerSales.Select(s => new PurchaseSummary
                {
                    SaleId = s.Id,
                    Date = s.SaleDate,
                    TotalAmount = s.TotalAmount,
                    ItemCount = s.Items.Sum(i => i.Quantity)
                }).ToList()
            };
        }

        /// <summary>
        /// Gets sales summary for a custom date range.
        /// </summary>
        public async Task<SalesRangeReport> GetSalesRangeReportAsync(DateTime start, DateTime end)
        {
            var sales = await _saleRepository.GetSalesWithItemsAsync();
            var periodSales = sales.Where(s => s.SaleDate.Date >= start.Date && s.SaleDate.Date <= end.Date).ToList();

            return new SalesRangeReport
            {
                StartDate = start,
                EndDate = end,
                TotalSales = periodSales.Count,
                TotalRevenue = periodSales.Sum(s => s.TotalAmount),
                AverageOrderValue = periodSales.Any() ? periodSales.Average(s => s.TotalAmount) : 0,
                TotalItemsSold = periodSales.SelectMany(s => s.Items).Sum(i => i.Quantity),
                UniqueCustomers = periodSales.Where(s => s.CustomerId.HasValue).Select(s => s.CustomerId).Distinct().Count(),
                TopProducts = GetTopProducts(periodSales, 10),
                Sales = periodSales.OrderByDescending(s => s.SaleDate).Take(100).ToList()
            };
        }

        private List<TopProductReport> GetTopProducts(List<Sale> sales, int count)
        {
            return sales
                .SelectMany(s => s.Items)
                .GroupBy(i => new { i.ProductId, i.ProductName })
                .Select(g => new TopProductReport
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    QuantitySold = g.Sum(i => i.Quantity),
                    TotalRevenue = g.Sum(i => i.Quantity * i.UnitPrice)
                })
                .OrderByDescending(p => p.QuantitySold)
                .Take(count)
                .ToList();
        }

        private Dictionary<int, decimal> GetHourlySales(List<Sale> sales)
        {
            var hourly = new Dictionary<int, decimal>();
            for (int i = 0; i < 24; i++) hourly[i] = 0;

            foreach (var sale in sales)
            {
                hourly[sale.SaleDate.Hour] += sale.TotalAmount;
            }

            return hourly;
        }

        private async Task<List<TopCustomerReport>> GetTopCustomersAsync(List<Sale> sales, int count)
        {
            var customerGroups = sales
                .Where(s => s.CustomerId.HasValue)
                .GroupBy(s => s.CustomerId!.Value)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    TotalSpent = g.Sum(s => s.TotalAmount),
                    OrderCount = g.Count()
                })
                .OrderByDescending(c => c.TotalSpent)
                .Take(count)
                .ToList();

            var result = new List<TopCustomerReport>();
            foreach (var cg in customerGroups)
            {
                var customer = await _customerRepository.GetByIdAsync(cg.CustomerId);
                result.Add(new TopCustomerReport
                {
                    CustomerId = cg.CustomerId,
                    CustomerName = customer?.Name ?? "Unknown",
                    TotalSpent = cg.TotalSpent,
                    OrderCount = cg.OrderCount
                });
            }

            return result;
        }
    }

    #region Report Models

    public class DailySalesReport
    {
        public DateTime ReportDate { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int ItemsSold { get; set; }
        public List<TopProductReport> TopSellingProducts { get; set; } = new();
        public Dictionary<int, decimal> HourlySales { get; set; } = new();

        public string TotalRevenueFormatted => $"Rs. {TotalRevenue:N2}";
        public string AverageOrderValueFormatted => $"Rs. {AverageOrderValue:N2}";
    }

    public class WeeklySalesReport
    {
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<DailySummary> DailyBreakdown { get; set; } = new();
        public List<TopProductReport> TopSellingProducts { get; set; } = new();

        public string TotalRevenueFormatted => $"Rs. {TotalRevenue:N2}";
    }

    public class MonthlySalesReport
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<WeeklySummary> WeeklyBreakdown { get; set; } = new();
        public List<TopProductReport> TopSellingProducts { get; set; } = new();
        public List<TopCustomerReport> TopCustomers { get; set; } = new();

        public string TotalRevenueFormatted => $"Rs. {TotalRevenue:N2}";
    }

    public class SalesRangeReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TotalItemsSold { get; set; }
        public int UniqueCustomers { get; set; }
        public List<TopProductReport> TopProducts { get; set; } = new();
        public List<Sale> Sales { get; set; } = new();

        public string TotalRevenueFormatted => $"Rs. {TotalRevenue:N2}";
        public string DateRangeDisplay => $"{StartDate:MMM dd, yyyy} - {EndDate:MMM dd, yyyy}";
    }

    public class CustomerPurchaseHistoryReport
    {
        public Customer Customer { get; set; } = null!;
        public int TotalPurchases { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime? FirstPurchase { get; set; }
        public DateTime? LastPurchase { get; set; }
        public List<PurchaseSummary> Purchases { get; set; } = new();

        public string TotalSpentFormatted => $"Rs. {TotalSpent:N2}";
    }

    public class DailySummary
    {
        public DateTime Date { get; set; }
        public string DayName { get; set; } = string.Empty;
        public int SalesCount { get; set; }
        public decimal Revenue { get; set; }
        public string RevenueFormatted => $"Rs. {Revenue:N2}";
    }

    public class WeeklySummary
    {
        public int WeekNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SalesCount { get; set; }
        public decimal Revenue { get; set; }
        public string RevenueFormatted => $"Rs. {Revenue:N2}";
    }

    public class TopProductReport
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public string TotalRevenueFormatted => $"Rs. {TotalRevenue:N2}";
    }

    public class TopCustomerReport
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalSpent { get; set; }
        public int OrderCount { get; set; }
        public string TotalSpentFormatted => $"Rs. {TotalSpent:N2}";
    }

    public class PurchaseSummary
    {
        public int SaleId { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
        public string TotalAmountFormatted => $"Rs. {TotalAmount:N2}";
    }

    #endregion
}
