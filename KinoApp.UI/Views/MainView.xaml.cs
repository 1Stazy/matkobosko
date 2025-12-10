using KinoApp.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;


namespace KinoApp.UI.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();

            try
            {
                // Pobieramy MainViewModel z DI (tak jak zaleca dokumentacja projektu)
                var vm = App.AppHost!.Services.GetRequiredService<MainViewModel>();
                this.DataContext = vm;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Błąd inicjalizacji MainView:\n{ex.Message}",
                    "MainView Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                // Minimalny fallback w razie awarii (rzadko używany)
                this.DataContext = new MainViewModel(
                    App.AppHost!.Services.GetRequiredService<KinoApp.Data.Context.KinoDbContext>()
                );
            }
        }
    }
}
