using KinoApp.UI.ViewModels;
using KinoApp.Services.Interfaces;
using System;
using System.Windows;

namespace KinoApp.UI.Views
{
    public partial class SeatSelectionWindow : Window
    {
        // Konstruktor bezparametrowy - potrzebny dla projektanta XAML
        public SeatSelectionWindow()
        {
            InitializeComponent();
        }

        // Konstruktor używany w aplikacji (DI): wstrzyknij ViewModel i monitorService (opcjonalnie)
        public SeatSelectionWindow(SeatSelectionViewModel vm, IMonitorService? monitorService = null)
        {
            InitializeComponent();

            if (vm != null)
            {
                DataContext = vm;
            }

            try
            {
                // Jeżeli mamy platformowy monitorService, spróbuj przenieść okno na drugi monitor
                if (monitorService != null && monitorService.HasSecondMonitor())
                {
                    // Nasza platformowa implementacja (w UI) to MonitorServiceWin i posiada TryGetMonitorWorkArea
                    // Spróbuj rzutować — jeśli nie jest MonitorServiceWin, maxymalizujemy jako fallback.
                    if (monitorService is KinoApp.UI.Services.MonitorServiceWin winMon &&
                        winMon.TryGetMonitorWorkArea(1, out var rect))
                    {
                        this.WindowStartupLocation = WindowStartupLocation.Manual;
                        this.Left = rect.Left;
                        this.Top = rect.Top;
                        this.Width = rect.Width;
                        this.Height = rect.Height;
                    }
                    else
                    {
                        // fallback: otwórz okno maksymalizowane na dostępny ekran
                        this.WindowState = WindowState.Maximized;
                    }
                }
            }
            catch (Exception ex)
            {
                // Nie przerywamy działania aplikacji z powodu pozycji okna — logujemy/wyświetlamy diagnostykę
                MessageBox.Show($"Błąd ustawiania pozycji SeatSelectionWindow:\n{ex.Message}", "Diagnostyka SeatSelection");
            }
        }
    }
}
