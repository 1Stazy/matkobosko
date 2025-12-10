using KinoApp.Services.Interfaces;
using System.Windows; 
using System;

namespace KinoApp.UI.Services
{
    // Implementacja specyficzna dla Windows + WPF (nie używa System.Windows.Forms)
    public class MonitorServiceWin : IMonitorService
    {
        public bool HasSecondMonitor()
        {
            try
            {
                return MonitorNativeInterop.GetMonitorCount() >= 2;
            }
            catch
            {
                return false;
            }
        }

        public int GetSecondMonitorIndex()
        {
            try
            {
                var count = MonitorNativeInterop.GetMonitorCount();
                return count >= 2 ? 1 : 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Zwraca prostokąt WorkArea (WPF units = device-independent px) drugiego monitora.
        /// Jeśli nie ma drugiego monitora zwraca primary.
        /// </summary>
        public bool TryGetMonitorWorkArea(int monitorIndex, out System.Windows.Rect rect)
        {
            rect = new System.Windows.Rect();

            // prosta metoda: jeśli monitorIndex==1 użyj point z dużym offsetem (np 10000,10000),
            // ale lepiej: zwróć WorkArea nearest to a point (przykładowo: primary center vs far right)
            try
            {
                if (monitorIndex <= 0)
                    return MonitorNativeInterop.TryGetPrimaryMonitorWorkArea(out rect);

                // pick a point far to the right to hit the secondary monitor in typical setups
                // (alternatywnie: możesz iterować po monitorach — tu minimalna implementacja)
                return MonitorNativeInterop.TryGetMonitorWorkAreaContainingPoint(5000, 0, out rect)
                    || MonitorNativeInterop.TryGetPrimaryMonitorWorkArea(out rect);
            }
            catch
            {
                rect = new System.Windows.Rect(0, 0, SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
                return false;
            }
        }
    }
}
