using KeyboardLanguageIndicator.Services;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using AppSettings = KeyboardLanguageIndicator.Properties.Settings;

namespace KeyboardLanguageIndicator;

public partial class NotificationWindow : Window
{
    private const int FADE_DURATION_MS = 200;
    private const double MARGIN = 20;

    private DispatcherTimer? _timer;
    private readonly int _monitorIndex;
    private readonly string _position;

    public NotificationWindow(
        string text,
        string position,
        double duration,
        double opacity,
        Color backgroundColor,
        Color textColor,
        double fontSize,
        int monitorIndex = 0
    )
    {
        InitializeComponent();

        _monitorIndex = monitorIndex;
        _position = position;

        ConfigureAppearance(text, backgroundColor, textColor, fontSize, opacity);

        var settings = AppSettings.Default;
        if (!settings.IsPersistent && duration > 0)
            InitializeAutoCloseTimer(duration);
    }

    private void ConfigureAppearance(
        string text,
        Color bg,
        Color textColor,
        double fontSize,
        double opacity
    )
    {
        LanguageText.Text = text;
        LanguageText.FontSize = fontSize;
        LanguageText.Foreground = new SolidColorBrush(textColor);
        NotificationBorder.Background = new SolidColorBrush(bg);
        NotificationBorder.Opacity = opacity;
    }

    private void InitializeAutoCloseTimer(double seconds)
    {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(seconds) };
        _timer.Tick += (s, e) => Close();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        PositionWindow();
        FadeIn();
        _timer?.Start();
    }

    private void PositionWindow()
    {
        var (left, top) = MonitorHelper.CalculatePosition(
            _monitorIndex,
            _position,
            ActualWidth,
            ActualHeight,
            MARGIN,
            this
        );

        Left = left;
        Top = top;
    }

    public new void Close()
    {
        _timer?.Stop();
        FadeOut(() => base.Close());
    }

    public void EnableTimer(double duration)
    {
        if (_timer == null && duration > 0)
        {
            InitializeAutoCloseTimer(duration);
            _timer?.Start();
        }
        else
        {
            _timer?.Start();
        }
    }

    private void FadeIn() => Animate(0, 1);

    private void FadeOut(Action onComplete) => Animate(1, 0, onComplete);

    private void Animate(double from, double to, Action? onComplete = null)
    {
        var animation = new DoubleAnimation(from, to, TimeSpan.FromMilliseconds(FADE_DURATION_MS));
        if (onComplete != null)
            animation.Completed += (s, e) => onComplete();
        BeginAnimation(OpacityProperty, animation);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.Q && Keyboard.Modifiers == ModifierKeys.Control)
            Close();
    }
}
