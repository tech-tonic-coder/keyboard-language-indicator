using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Media;

namespace KeyboardLanguageIndicator.Services;

public static class MonitorHelper
{
    public record MonitorInfo(int Index, string DisplayName, Screen Screen);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    public static List<MonitorInfo> GetAllMonitors()
    {
        var screens = Screen.AllScreens;
        var monitors = new List<MonitorInfo>();

        for (int i = 0; i < screens.Length; i++)
        {
            var screen = screens[i];
            var name = screen.Primary
                ? $"Monitor {i + 1} (Primary) - {screen.Bounds.Width}x{screen.Bounds.Height}"
                : $"Monitor {i + 1} - {screen.Bounds.Width}x{screen.Bounds.Height}";

            monitors.Add(new MonitorInfo(i, name, screen));
        }

        return monitors;
    }

    public static Screen GetMonitor(int index)
    {
        var screens = Screen.AllScreens;
        return (index >= 0 && index < screens.Length) ? screens[index] : Screen.PrimaryScreen!;
    }

    public static int GetCursorMonitorIndex()
    {
        if (!GetCursorPos(out POINT point))
            return 0;

        var screens = Screen.AllScreens;
        for (int i = 0; i < screens.Length; i++)
        {
            var bounds = screens[i].Bounds;
            if (
                point.X >= bounds.Left
                && point.X < bounds.Right
                && point.Y >= bounds.Top
                && point.Y < bounds.Bottom
            )
            {
                return i;
            }
        }

        return 0; // Default to primary
    }

    public static (double left, double top) CalculatePosition(
        int monitorIndex,
        string position,
        double windowWidth,
        double windowHeight,
        double margin,
        Visual visual
    )
    {
        var screen = GetMonitor(monitorIndex);
        var workingArea = screen.WorkingArea;

        // Convert to WPF coordinates (handle DPI scaling)
        var dpi = VisualTreeHelper.GetDpi(visual);
        var left = workingArea.Left / dpi.DpiScaleX;
        var top = workingArea.Top / dpi.DpiScaleY;
        var width = workingArea.Width / dpi.DpiScaleX;
        var height = workingArea.Height / dpi.DpiScaleY;

        return position switch
        {
            "TopLeft" => (left + margin, top + margin),
            "TopCenter" => (left + ((width - windowWidth) / 2), top + margin),
            "TopRight" => (left + width - windowWidth - margin, top + margin),
            "BottomLeft" => (left + margin, top + height - windowHeight - margin),
            "BottomCenter" => (
                left + ((width - windowWidth) / 2),
                top + height - windowHeight - margin
            ),
            "BottomRight" => (
                left + width - windowWidth - margin,
                top + height - windowHeight - margin
            ),
            "Center" => (left + ((width - windowWidth) / 2), top + ((height - windowHeight) / 2)),
            _ => (left + width - windowWidth - margin, top + margin),
        };
    }
}
