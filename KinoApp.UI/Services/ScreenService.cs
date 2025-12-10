using KinoApp.Core.Services;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;

namespace KinoApp.UI.Services
{
    public class ScreenService : IScreenService
    {
        public MonitorInfoSimple[] GetMonitors()
        {
            var list = new List<MonitorInfoSimple>();

            bool enumResult = EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (hMonitor, hdcMonitor, lprcMonitor, dwData) =>
            {
                var info = new MONITORINFO();
                info.cbSize = Marshal.SizeOf(info);
                if (GetMonitorInfo(hMonitor, ref info))
                {
                    var left = info.rcWork.left;
                    var top = info.rcWork.top;
                    var right = info.rcWork.right;
                    var bottom = info.rcWork.bottom;
                    var width = right - left;
                    var height = bottom - top;
                    var isPrimary = (info.dwFlags & MONITORINFOF_PRIMARY) != 0;
                    list.Add(new MonitorInfoSimple(left, top, width, height, isPrimary));
                }
                return true;
            }, IntPtr.Zero);

            return list.ToArray();
        }

        public void PositionWindowOnMonitor(object window, int monitorIndex)
        {
            if (window is not Window w) return;

            var monitors = GetMonitors();
            if (monitors.Length == 0) return;
            if (monitorIndex < 0 || monitorIndex >= monitors.Length) monitorIndex = 0;

            var target = monitors[monitorIndex];

            // ustaw pozycję okna (Manual startup)
            w.WindowStartupLocation = WindowStartupLocation.Manual;
            w.Left = target.X + 50;
            w.Top = target.Y + 50;
            // Możesz też zmaksymalizować:
            // w.WindowState = WindowState.Maximized;
        }

        #region Win32 interop
        private const int MONITORINFOF_PRIMARY = 0x00000001;

        private delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, IntPtr lprcMonitor, IntPtr dwData);

        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left, top, right, bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public int dwFlags;
        }
        #endregion
    }
}
