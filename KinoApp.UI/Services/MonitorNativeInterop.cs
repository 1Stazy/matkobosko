using System;
using System.Runtime.InteropServices;

namespace KinoApp.UI.Services
{
    internal static class MonitorNativeInterop
    {
        // GetSystemMetrics -> liczba monitorów
        private const int SM_CMONITORS = 80;

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        // MonitorFromPoint / MonitorFromWindow / MonitorFromRect
        private const uint MONITOR_DEFAULTTONULL = 0x00000000;
        private const uint MONITOR_DEFAULTTOPRIMARY = 0x00000001;
        private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct MONITORINFO
        {
            public uint cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        // Public helper: liczba monitorów
        public static int GetMonitorCount()
        {
            try
            {
                return GetSystemMetrics(SM_CMONITORS);
            }
            catch
            {
                return 1;
            }
        }

        // Public helper: pobierz prostokąt monitora (Working area) dla monitora zawierającego punkt (x,y)
        // Jeśli index==-1 => primary
        public static bool TryGetMonitorWorkAreaContainingPoint(int x, int y, out System.Windows.Rect rect)
        {
            rect = new System.Windows.Rect();
            try
            {
                var pt = new POINT { X = x, Y = y };
                IntPtr hMonitor = MonitorFromPoint(pt, MONITOR_DEFAULTTONEAREST);
                if (hMonitor == IntPtr.Zero) return false;

                MONITORINFO info = new MONITORINFO();
                info.cbSize = (uint)Marshal.SizeOf<MONITORINFO>();
                if (!GetMonitorInfo(hMonitor, ref info)) return false;

                rect = new System.Windows.Rect(
                    info.rcWork.Left, info.rcWork.Top,
                    info.rcWork.Right - info.rcWork.Left,
                    info.rcWork.Bottom - info.rcWork.Top
                );
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Public helper: get working area of primary monitor
        public static bool TryGetPrimaryMonitorWorkArea(out System.Windows.Rect rect)
        {
            rect = new System.Windows.Rect();
            try
            {
                // point (0,0) is in primary monitor
                return TryGetMonitorWorkAreaContainingPoint(0, 0, out rect);
            }
            catch
            {
                return false;
            }
        }
    }
}
