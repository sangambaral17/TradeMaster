using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using TradeMaster.Core.Interfaces;
using TradeMaster.Infrastructure.Data;
using TradeMaster.Desktop.ViewModels;
using TradeMaster.Desktop.Views;
using TradeMaster.Desktop.Services;

namespace TradeMaster.Desktop
{
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    // Database
                    services.AddDbContext<TradeMasterDbContext>(options =>
                    {
                        options.UseSqlite("Data Source=trademaster.db");
                    });

                    // Repositories
                    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
                    services.AddScoped<ISaleRepository, SaleRepository>();

                    // Services (Singletons - shared across app)
                    services.AddSingleton<SettingsService>();
                    services.AddSingleton<LocalizationService>();

                    // Services (Scoped - per request)
                    services.AddScoped<BackupService>();
                    services.AddScoped<AuthenticationService>();
                    services.AddScoped<InventoryAlertService>();
                    services.AddScoped<ReportingService>();
                    services.AddScoped<ReceiptService>();
                    services.AddScoped<BarcodeService>();

                    // ViewModels
                    services.AddTransient<ProductListViewModel>();
                    services.AddTransient<PosViewModel>();
                    services.AddTransient<CustomerListViewModel>();
                    services.AddTransient<SalesHistoryViewModel>();
                    services.AddTransient<SettingsViewModel>();

                    // Windows
                    services.AddSingleton<MainWindow>();
                    services.AddTransient<ProductListView>();
                    services.AddTransient<PosView>();
                    services.AddTransient<CustomerListView>();
                    services.AddTransient<SalesHistoryView>();
                    services.AddTransient<SettingsView>();
                    services.AddTransient<LoginWindow>();
                    // Note: ReceiptPreviewDialog takes runtime params, create manually
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            // Initialize Database
            using (var scope = AppHost.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TradeMasterDbContext>();
                DbInitializer.Initialize(context);
            }

            // Show Main Window
            var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
            startupForm.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            base.OnExit(e);
        }
    }
}
