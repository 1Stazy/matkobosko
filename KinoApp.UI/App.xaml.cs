using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using KinoApp.Services.Interfaces;

namespace KinoApp.UI
{
    public partial class App : Application
    {
        // publiczny dostęp dla reszty aplikacji (np. ViewModel-e odwołujące się do App.AppHost)
        public static IServiceProvider? AppHost { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            // przykład rejestracji - dopasuj do swojej konfiguracji
            services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=kino.db"));
            services.AddScoped<IShowService, ShowService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddSingleton<IMonitorService, KinoApp.UI.Services.MonitorServiceWin>();

            AppHost = services.BuildServiceProvider();

            // uruchomienie okna głównego - możesz pobierać przez AppHost jeśli chcesz
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
