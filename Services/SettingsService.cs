using System.Windows.Media;
using AppSettings = KeyboardLanguageIndicator.Properties.Settings;

namespace KeyboardLanguageIndicator.Services;

public class SettingsService
{
    private readonly AppSettings _settings = AppSettings.Default;

    // Position & Display
    public string Position
    {
        get => _settings.Position;
        set => _settings.Position = value;
    }

    public int MonitorIndex
    {
        get => _settings.MonitorIndex;
        set => _settings.MonitorIndex = value;
    }

    // Appearance
    public double Duration
    {
        get => _settings.Duration;
        set => _settings.Duration = value;
    }

    public double Opacity
    {
        get => _settings.Opacity;
        set => _settings.Opacity = value;
    }

    public double FontSize
    {
        get => _settings.FontSize;
        set => _settings.FontSize = value;
    }

    // Behavior
    public bool IsPersistent
    {
        get => _settings.IsPersistent;
        set => _settings.IsPersistent = value;
    }

    public bool IsDarkMode
    {
        get => _settings.IsDarkMode;
        set => _settings.IsDarkMode = value;
    }

    public bool MinimizeToTray
    {
        get => _settings.MinimizeToTray;
        set => _settings.MinimizeToTray = value;
    }

    public bool CloseToTray
    {
        get => _settings.CloseToTray;
        set => _settings.CloseToTray = value;
    }

    // Update Settings
    public bool CheckForUpdates
    {
        get => _settings.CheckForUpdates;
        set => _settings.CheckForUpdates = value;
    }

    public DateTime? LastUpdateCheck
    {
        get =>
            string.IsNullOrEmpty(_settings.LastUpdateCheck)
                ? null
                : DateTime.Parse(_settings.LastUpdateCheck);
        set => _settings.LastUpdateCheck = value?.ToString("o") ?? "";
    }

    // Colors
    public Color BackgroundColor
    {
        get => ColorHelper.Parse(_settings.BackgroundColor, Colors.Black);
        set => _settings.BackgroundColor = value.ToString();
    }

    public Color TextColor
    {
        get => ColorHelper.Parse(_settings.TextColor, Colors.White);
        set => _settings.TextColor = value.ToString();
    }

    // Language-specific colors
    private Dictionary<string, (Color bg, Color text)> _languageColors = new();

    public void LoadLanguageColors()
    {
        _languageColors.Clear();
        if (string.IsNullOrWhiteSpace(_settings.LanguageColors))
            return;

        try
        {
            var pairs = _settings.LanguageColors.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in pairs)
            {
                var parts = pair.Split(':', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 3)
                {
                    var lang = parts[0];
                    var bg = ColorHelper.Parse(parts[1], Colors.Black);
                    var text = ColorHelper.Parse(parts[2], Colors.White);
                    _languageColors[lang] = (bg, text);
                }
            }
        }
        catch
        { /* Invalid format, ignore */
        }
    }

    public void SaveLanguageColors()
    {
        var pairs = _languageColors.Select(kvp => $"{kvp.Key}:{kvp.Value.bg}:{kvp.Value.text}");
        _settings.LanguageColors = string.Join(";", pairs);
    }

    public void SetLanguageColor(string language, Color background, Color text)
    {
        _languageColors[language] = (background, text);
        SaveLanguageColors();
    }

    public void RemoveLanguageColor(string language)
    {
        _languageColors.Remove(language);
        SaveLanguageColors();
    }

    public bool TryGetLanguageColor(string language, out Color background, out Color text)
    {
        if (_languageColors.TryGetValue(language, out var colors))
        {
            background = colors.bg;
            text = colors.text;
            return true;
        }
        background = BackgroundColor;
        text = TextColor;
        return false;
    }

    public IEnumerable<string> GetConfiguredLanguages() => _languageColors.Keys;

    public void Save() => _settings.Save();
}

/// <summary>
/// Helper for color conversions
/// </summary>
public static class ColorHelper
{
    public static Color Parse(string colorString, Color fallback)
    {
        try
        {
            return (Color)ColorConverter.ConvertFromString(colorString);
        }
        catch
        {
            return fallback;
        }
    }

    public static SolidColorBrush ToBrush(this Color color) => new(color);
}
