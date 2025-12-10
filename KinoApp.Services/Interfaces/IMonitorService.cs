namespace KinoApp.Services.Interfaces
{
    public interface IMonitorService
    {
        /// <summary>
        /// Sprawdza, czy dostępny jest drugi monitor.
        /// </summary>
        bool HasSecondMonitor();

        /// <summary>
        /// Zwraca index drugiego monitora (lub 0 jeśli tylko jeden).
        /// </summary>
        int GetSecondMonitorIndex();
    }
}
