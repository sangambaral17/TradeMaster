using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using TradeMaster.Core.Interfaces;
using TradeMaster.Infrastructure.Data;
using TradeMaster.Desktop.ViewModels;
using TradeMaster.Desktop.Views;

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

                    // ViewModels
                    services.AddTransient<ProductListViewModel>();
                    services.AddTransient<PosViewModel>();
                    services.AddTransient<CustomerListViewModel>();
                    services.AddTransient<SalesHistoryViewModel>();

                    // Windows
                    services.AddSingleton<MainWindow>();
                    services.AddTransient<ProductListView>();
                    services.AddTransient<PosView>();
                    services.AddTransient<CustomerListView>();
                    services.AddTransient<SalesHistoryView>();
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
