using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using KeyboardLanguageIndicator.Services;

namespace KeyboardLanguageIndicator
{
    public partial class CursorFollowerWindow : Window
    {
        private const int CURSOR_UPDATE_INTERVAL_MS = 10;
        private const int OPTIMAL_CURSOR_OFFSET = 8; // Closer to cursor (was 2, which is too close and can overlap)

        // Reference resolution and font size (1920x1080 @ 15pt)
        private const int REFERENCE_WIDTH = 1920;
        private const int REFERENCE_HEIGHT = 1080;
        private const double REFERENCE_FONT_SIZE = 15.0;

        private DispatcherTimer? _cursorTimer;
        private DispatcherTimer? _hideTimer;
        private readonly SettingsService _settings;
        private IntPtr _hwnd;
        private double _dpiScaleX = 1.0;
        private double _dpiScaleY = 1.0;
        private bool _isInitialized = false; // Track if window has been shown at least once

        // Calculated dimensions
        private double _optimalFontSize;
        private double _maxWidth;
        private double _maxHeight;
        private double _padding;

        // Track current monitor for dimension updates
        private IntPtr _currentMonitor = IntPtr.Zero;

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetDpiForWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern bool GetPhysicalCursorPos(out POINT lpPoint);

        [DllImport("shcore.dll")]
        private static extern int GetDpiForMonitor(
            IntPtr hmonitor,
            int dpiType,
            out uint dpiX,
            out uint dpiY
        );

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        private const uint MONITOR_DEFAULTTONEAREST = 2;

        public CursorFollowerWindow(SettingsService settings)
        {
            InitializeComponent();
            _settings = settings;
            CalculateOptimalDimensions();
            ConfigureAppearance();
        }

        private void CalculateOptimalDimensions(IntPtr? monitor = null)
        {
            int screenWidth = REFERENCE_WIDTH;
            int screenHeight = REFERENCE_HEIGHT;

            if (monitor.HasValue && monitor.Value != IntPtr.Zero)
            {
                // Get monitor resolution from specific monitor
                MONITORINFO monitorInfo = new MONITORINFO();
                monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);

                if (GetMonitorInfo(monitor.Value, ref monitorInfo))
                {
                    screenWidth = monitorInfo.rcMonitor.Right - monitorInfo.rcMonitor.Left;
                    screenHeight = monitorInfo.rcMonitor.Bottom - monitorInfo.rcMonitor.Top;
                }
            }
            else
            {
                // Fallback to primary screen
                var screen = Screen.PrimaryScreen;
                screenWidth = screen?.Bounds.Width ?? REFERENCE_WIDTH;
                screenHeight = screen?.Bounds.Height ?? REFERENCE_HEIGHT;
            }

            // Calculate font size based on screen diagonal
            double referenceDiagonal = Math.Sqrt(
                (REFERENCE_WIDTH * REFERENCE_WIDTH) + (REFERENCE_HEIGHT * REFERENCE_HEIGHT)
            );
            double currentDiagonal = Math.Sqrt(
                (screenWidth * screenWidth) + (screenHeight * screenHeight)
            );
            double scaleFactor = currentDiagonal / referenceDiagonal;

            _optimalFontSize = REFERENCE_FONT_SIZE * scaleFactor;
            // Clamp between reasonable bounds
            _optimalFontSize = Math.Max(10, Math.Min(24, _optimalFontSize));
            _optimalFontSize = Math.Round(_optimalFontSize, 1);

            // Calculate window dimensions based on font size
            _maxWidth = Math.Round(_optimalFontSize * 2.2); // Accommodate 2-3 characters
            _maxHeight = Math.Round(_optimalFontSize * 1.6); // Slightly taller than font
            _padding = Math.Round(_optimalFontSize * 0.2, 1); // Proportional padding
        }

        private void ConfigureAppearance()
        {
            // Apply calculated dimensions
            LanguageText.FontSize = _optimalFontSize;
            FollowerBorder.MaxWidth = _maxWidth;
            FollowerBorder.MaxHeight = _maxHeight;
            FollowerBorder.Padding = new Thickness(_padding);
            FollowerBorder.Opacity = _settings.CursorFollowerOpacity;

            // Use default colors
            FollowerBorder.Background = new SolidColorBrush(_settings.BackgroundColor);
            LanguageText.Foreground = new SolidColorBrush(_settings.TextColor);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _hwnd = new WindowInteropHelper(this).Handle;
            UpdateDpiScale();

            // Don't start tracking yet - wait for first UpdateLanguage call
            // This ensures we have actual content before positioning
        }

        private void UpdateDpiScale()
        {
            try
            {
                if (GetCursorPos(out POINT cursorPos))
                {
                    IntPtr hMonitor = MonitorFromPoint(cursorPos, MONITOR_DEFAULTTONEAREST);

                    // Check if monitor changed - recalculate dimensions if so
                    if (hMonitor != _currentMonitor && hMonitor != IntPtr.Zero)
                    {
                        _currentMonitor = hMonitor;

                        // Recalculate dimensions for this monitor
                        CalculateOptimalDimensions(hMonitor);

                        // Update appearance with new dimensions
                        Dispatcher.Invoke(() =>
                        {
                            LanguageText.FontSize = _optimalFontSize;
                            FollowerBorder.MaxWidth = _maxWidth;
                            FollowerBorder.MaxHeight = _maxHeight;
                            FollowerBorder.Padding = new Thickness(_padding);
                            UpdateLayout();
                        });
                    }

                    if (
                        hMonitor != IntPtr.Zero
                        && GetDpiForMonitor(hMonitor, 0, out uint dpiX, out uint dpiY) == 0
                    )
                    {
                        _dpiScaleX = dpiX / 96.0;
                        _dpiScaleY = dpiY / 96.0;
                        return;
                    }
                }

                if (_hwnd != IntPtr.Zero)
                {
                    uint dpi = GetDpiForWindow(_hwnd);
                    _dpiScaleX = dpi / 96.0;
                    _dpiScaleY = dpi / 96.0;
                    return;
                }
            }
            catch { }

            var source = PresentationSource.FromVisual(this);
            if (source?.CompositionTarget != null)
            {
                _dpiScaleX = source.CompositionTarget.TransformToDevice.M11;
                _dpiScaleY = source.CompositionTarget.TransformToDevice.M22;
            }
        }

        private void StartCursorTracking()
        {
            _cursorTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(CURSOR_UPDATE_INTERVAL_MS),
            };
            _cursorTimer.Tick += (s, e) => UpdatePosition();
            _cursorTimer.Start();
        }

        private void UpdatePosition()
        {
            bool success = GetPhysicalCursorPos(out POINT point);

            if (!success)
            {
                success = GetCursorPos(out point);
            }

            if (success)
            {
                // Check for monitor changes every 100ms
                if (DateTime.Now.Millisecond % 100 < CURSOR_UPDATE_INTERVAL_MS)
                {
                    UpdateDpiScale();
                }

                double logicalX = point.X / _dpiScaleX;
                double logicalY = point.Y / _dpiScaleY;

                // Apply optimal offset + user adjustments
                // X: positive offset to the right
                // Y: negative offset to go UP (subtract window height + offset)
                double offsetX = OPTIMAL_CURSOR_OFFSET + _settings.CursorFollowerOffsetX;
                double offsetY = OPTIMAL_CURSOR_OFFSET + _settings.CursorFollowerOffsetY;

                Left = logicalX + offsetX;

                // Use ActualHeight if available, otherwise DesiredSize
                double windowHeight = ActualHeight > 0 ? ActualHeight : DesiredSize.Height;
                // Position above cursor: subtract window height and offset
                Top = logicalY - windowHeight - offsetY;
            }
        }

        public void UpdateLanguage(string language)
        {
            LanguageText.Text = language;

            if (_settings.TryGetLanguageColor(language, out var bgColor, out var textColor))
            {
                FollowerBorder.Background = new SolidColorBrush(bgColor);
                LanguageText.Foreground = new SolidColorBrush(textColor);
            }
            else
            {
                FollowerBorder.Background = new SolidColorBrush(_settings.BackgroundColor);
                LanguageText.Foreground = new SolidColorBrush(_settings.TextColor);
            }

            // Force immediate layout update with actual content
            Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Arrange(new Rect(0, 0, DesiredSize.Width, DesiredSize.Height));
            UpdateLayout();

            // On first call, start cursor tracking now that we have content
            if (!_isInitialized)
            {
                _isInitialized = true;
                StartCursorTracking();
                // Give it one frame to position correctly before showing
                Dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        UpdatePosition(); // Position immediately with correct size
                        ShowWindow();
                    }),
                    System.Windows.Threading.DispatcherPriority.Loaded
                );
            }
            else
            {
                ShowWindow();
            }
        }

        private void ShowWindow()
        {
            if (!_settings.CursorFollowerPersistent)
            {
                _hideTimer?.Stop();
                Show();
                Opacity = 1;

                _hideTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(_settings.CursorFollowerDuration),
                };
                _hideTimer.Tick += (s, e) =>
                {
                    Hide();
                    _hideTimer?.Stop();
                };
                _hideTimer.Start();
            }
            else
            {
                Show();
                Opacity = 1;
            }
        }

        public new void Close()
        {
            _cursorTimer?.Stop();
            _hideTimer?.Stop();
            base.Close();
        }
    }
}
