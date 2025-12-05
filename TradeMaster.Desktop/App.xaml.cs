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
            // Global exception handlers
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

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
            try
            {
                ErrorLogger.LogInfo("Application starting...");

                await AppHost!.StartAsync();

                // Initialize Database
                using (var scope = AppHost.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<TradeMasterDbContext>();
                    DbInitializer.Initialize(context);
                }

                ErrorLogger.LogInfo("Database initialized successfully");

                // Show Main Window
                var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
                startupForm.Show();

                ErrorLogger.LogInfo("Application started successfully");

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("Critical error during application startup", ex);
                
                MessageBox.Show(
                    $"Failed to start Walsong TradeMaster.\n\n" +
                    $"Error: {ex.Message}\n\n" +
                    $"Please check the log file for details or contact support.",
                    "Startup Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Shutdown(1);
            }
        }


        protected override async void OnExit(ExitEventArgs e)
        {
            ErrorLogger.LogInfo("Application shutting down...");
            await AppHost!.StopAsync();
            base.OnExit(e);
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            ErrorLogger.LogError("Unhandled exception", exception, "AppDomain.UnhandledException");

            if (e.IsTerminating)
            {
                MessageBox.Show(
                    "A critical error occurred and the application must close.\n\n" +
                    "Error details have been logged. Please contact support.",
                    "Critical Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ErrorLogger.LogError("Dispatcher unhandled exception", e.Exception, "Dispatcher");

            MessageBox.Show(
                $"An unexpected error occurred:\n\n{e.Exception.Message}\n\n" +
                "The application will attempt to continue. If problems persist, please restart.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            e.Handled = true; // Prevent application crash
        }

        private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            ErrorLogger.LogError("Unobserved task exception", e.Exception, "TaskScheduler");
            e.SetObserved(); // Prevent application crash
        }
    }
}
