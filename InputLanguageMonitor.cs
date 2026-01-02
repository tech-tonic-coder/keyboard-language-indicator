using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace KeyboardLanguageIndicator;

public class InputLanguageMonitor
{
    private const int CHECK_INTERVAL_MS = 100;
    private DispatcherTimer? _timer;
    private string _currentLanguage = string.Empty;

    public event EventHandler<string>? LanguageChanged;

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);

    [DllImport("user32.dll")]
    private static extern IntPtr GetKeyboardLayout(uint idThread);

    public void Start()
    {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(CHECK_INTERVAL_MS) };
        _timer.Tick += (s, e) => CheckLanguage();
        _timer.Start();
        CheckLanguage();
    }

    public void Stop() => _timer?.Stop();

    private void CheckLanguage()
    {
        try
        {
            var layout = GetCurrentKeyboardLayout();
            var language = GetLanguageCode(layout);

            if (language != _currentLanguage && !string.IsNullOrEmpty(language))
            {
                _currentLanguage = language;
                LanguageChanged?.Invoke(this, language);
            }
        }
        catch { }
    }

    private static IntPtr GetCurrentKeyboardLayout()
    {
        var window = GetForegroundWindow();
        var threadId = GetWindowThreadProcessId(window, IntPtr.Zero);
        return GetKeyboardLayout(threadId);
    }

    private static string GetLanguageCode(IntPtr layout)
    {
        var layoutId = (int)layout & 0xFFFF;

        try
        {
            var culture = new CultureInfo(layoutId);
            return culture.TwoLetterISOLanguageName.ToUpperInvariant();
        }
        catch
        {
            // Fallback to manual mapping
            return layoutId switch
            {
                0x0409 => "EN",
                0x0429 => "FA",
                0x0401 => "AR",
                0x040C => "FR",
                0x0407 => "DE",
                0x0410 => "IT",
                0x040A => "ES",
                0x0419 => "RU",
                0x041F => "TR",
                0x0804 => "ZH",
                0x0411 => "JA",
                0x0412 => "KO",
                0x0416 => "PT",
                _ => $"0x{layoutId:X4}",
            };
        }
    }
}
