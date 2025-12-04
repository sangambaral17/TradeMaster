using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TradeMaster.Core.Entities;
using TradeMaster.Core.Interfaces;

namespace TradeMaster.Desktop.ViewModels
{
    public partial class SalesHistoryViewModel : ObservableObject
    {
        private readonly ISaleRepository _saleRepository;

        [ObservableProperty]
        private ObservableCollection<Sale> _sales = new();

        [ObservableProperty]
        private ObservableCollection<Sale> _filteredSales = new();

        [ObservableProperty]
        private Sale? _selectedSale;

        [ObservableProperty]
        private DateTime? _startDate;

        [ObservableProperty]
        private DateTime? _endDate;

        [ObservableProperty]
        private decimal _totalRevenue;

        public SalesHistoryViewModel(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
            
            // Default to current month
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            EndDate = DateTime.Now;

            Task.Run(async () => await LoadSales());
        }

        [RelayCommand]
        private async Task LoadSales()
        {
            var sales = await _saleRepository.GetSalesWithItemsAsync();
            Sales = new ObservableCollection<Sale>(sales);
            FilterSales();
        }

        partial void OnStartDateChanged(DateTime? value)
        {
            FilterSales();
        }

        partial void OnEndDateChanged(DateTime? value)
        {
            FilterSales();
        }

        private void FilterSales()
        {
            var query = Sales.AsEnumerable();

            if (StartDate.HasValue)
            {
                query = query.Where(s => s.SaleDate.Date >= StartDate.Value.Date);
            }

            if (EndDate.HasValue)
            {
                query = query.Where(s => s.SaleDate.Date <= EndDate.Value.Date);
            }

            FilteredSales = new ObservableCollection<Sale>(query);
            TotalRevenue = FilteredSales.Sum(s => s.TotalAmount);
        }
    }
}
