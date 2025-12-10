using KinoApp.Services.Interfaces;

namespace KinoApp.Services.Implementations
{
    // Lekki fallback: bez zależności od UI / Windows Forms.
    public class MonitorService : IMonitorService
    {
        public bool HasSecondMonitor()
        {
            // Warstwa Services nie powinna zależeć od UI — zwracamy bezpieczny fallback.
            return false;
        }

        public int GetSecondMonitorIndex()
        {
            return 0;
        }
    }
}
