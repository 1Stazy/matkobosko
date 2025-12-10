namespace KinoApp.Core.Services
{
    // Prosty, platform-agnostyczny opis monitora (bez System.Windows types)
    public record MonitorInfoSimple(double X, double Y, double Width, double Height, bool IsPrimary);

    public interface IScreenService
    {
        MonitorInfoSimple[] GetMonitors();
        /// <summary>
        /// Pozycjonuje okno - implementacja specyficzna dla platformy (UI projekt będzie przyjmować typ Window)
        /// W Core pozostawiamy abstrakcję: przekazujemy indeks monitora i preferencję (np. left/top),
        /// implementacja w UI ustawi okno na wskazanym monitorze.
        /// </summary>
        void PositionWindowOnMonitor(object window, int monitorIndex);
    }
}
