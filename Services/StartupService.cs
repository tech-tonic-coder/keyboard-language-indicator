using Microsoft.Win32;

namespace KeyboardLanguageIndicator.Services;

public class StartupService
{
    private const string APP_NAME = "KeyboardLanguageIndicator";
    private const string STARTUP_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    public bool IsEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(STARTUP_KEY, false);
        return key?.GetValue(APP_NAME) != null;
    }

    public void Enable()
    {
        var exePath = GetExecutablePath();
        if (exePath == null)
            return;

        using var key = Registry.CurrentUser.OpenSubKey(STARTUP_KEY, true);
        key?.SetValue(APP_NAME, $"\"{exePath}\" --minimized");
    }

    public void Disable()
    {
        using var key = Registry.CurrentUser.OpenSubKey(STARTUP_KEY, true);
        key?.DeleteValue(APP_NAME, false);
    }

    private static string? GetExecutablePath() =>
        System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
}
