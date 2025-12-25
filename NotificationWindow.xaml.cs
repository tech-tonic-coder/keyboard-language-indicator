using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using AppSettings = KeyboardLanguageIndicator.Properties.Settings;

namespace KeyboardLanguageIndicator;

public partial class NotificationWindow : Window
{
    private DispatcherTimer? _timer;
    private const int FADE_DURATION_MS = 200;

    public NotificationWindow(
        string text,
        string position,
        double duration,
        double opacity,
        string bgColor,
        string textColor,
        double fontSize
    )
    {
        InitializeComponent();

        ConfigureAppearance(text, bgColor, textColor, fontSize, opacity);
        SetPosition(position);

        var s = AppSettings.Default;

        if (!s.IsPersistent && duration > 0)
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(duration) };
            _timer.Tick += (s, e) => Close();
        }
    }

    private void ConfigureAppearance(
        string text,
        string bgColor,
        string textColor,
        double fontSize,
        double opacity
    )
    {
        LanguageText.Text = text;
        LanguageText.FontSize = fontSize;
        LanguageText.Foreground = new SolidColorBrush(
            (Color)ColorConverter.ConvertFromString(textColor)
        );

        var background = (Color)ColorConverter.ConvertFromString(bgColor);
        NotificationBorder.Background = new SolidColorBrush(background);
        NotificationBorder.Opacity = opacity;
    }

    private void SetPosition(string position)
    {
        Loaded += (s, e) =>
        {
            var screen = SystemParameters.WorkArea;
            var margin = 20;

            (Left, Top) = position switch
            {
                "TopLeft" => (screen.Left + margin, screen.Top + margin),
                "TopCenter" => (
                    screen.Left + ((screen.Width - ActualWidth) / 2),
                    screen.Top + margin
                ),
                "TopRight" => (screen.Right - ActualWidth - margin, screen.Top + margin),
                "BottomLeft" => (screen.Left + margin, screen.Bottom - ActualHeight - margin),
                "BottomCenter" => (
                    screen.Left + ((screen.Width - ActualWidth) / 2),
                    screen.Bottom - ActualHeight - margin
                ),
                "BottomRight" => (
                    screen.Right - ActualWidth - margin,
                    screen.Bottom - ActualHeight - margin
                ),
                "Center" => (
                    screen.Left + ((screen.Width - ActualWidth) / 2),
                    screen.Top + ((screen.Height - ActualHeight) / 2)
                ),
                _ => (screen.Right - ActualWidth - margin, screen.Top + margin),
            };
        };
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        AnimateFade(0, 1);
        _timer?.Start();
    }

    private void Close()
    {
        _timer?.Stop();
        AnimateFade(1, 0, onComplete: () => base.Close());
    }

    private void AnimateFade(double from, double to, Action? onComplete = null)
    {
        var animation = new DoubleAnimation(from, to, TimeSpan.FromMilliseconds(FADE_DURATION_MS));
        if (onComplete != null)
            animation.Completed += (s, e) => onComplete();
        BeginAnimation(OpacityProperty, animation);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.N && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        {
            Close();
        }
    }

    public void EnableTimer(double duration)
    {
        if (_timer == null && duration > 0)
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(duration) };
            _timer.Tick += (s, e) => Close();
            _timer.Start();
        }
        else
            _timer?.Start();
    }
}
