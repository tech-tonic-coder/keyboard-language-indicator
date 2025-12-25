using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Hardcodet.Wpf.TaskbarNotification;
using KeyboardLanguageIndiactor;
using Microsoft.Win32;
using AppSettings = KeyboardLanguageIndicator.Properties.Settings;

namespace KeyboardLanguageIndicator
{
    public partial class MainWindow : HandyControl.Controls.Window
    {
        private const string APP_NAME = "KeyboardLanguageIndicator";
        private const string STARTUP_REG_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private bool _isInitialized = false;
        private const int HOTKEY_ID = 9000;
        private const uint MOD_CONTROL = 0x0002;
        private const uint VK_Q = 0x51;

        private TaskbarIcon? _trayIcon;
        private NotificationWindow? _notificationWindow;
        private InputLanguageMonitor? _monitor;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public MainWindow()
        {
            InitializeComponent();
            HandyControl.Tools.ConfigHelper.Instance.SetLang("en");
            InitializeTray();
            LoadSettings();
            _isInitialized = true;
            StartMonitoring();

            if (App.StartMinimized)
                MinimizeToTray();
        }

        private void InitializeTray()
        {
            _trayIcon = new TaskbarIcon
            {
                IconSource = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri("pack://application:,,,/icon.ico")
                ),
                ToolTipText = "Keyboard Indicator",
                ContextMenu = (System.Windows.Controls.ContextMenu)FindResource("TrayMenu"),
            };
            _trayIcon.TrayMouseDoubleClick += TrayIcon_DoubleClick;
        }

        private void LoadSettings()
        {
            var s = AppSettings.Default;

            if (Application.Current is App app)
                app.SetTheme(s.IsDarkMode);
            DarkModeCheckBox.IsChecked = s.IsDarkMode;

            foreach (System.Windows.Controls.ComboBoxItem item in PositionComboBox.Items)
            {
                if (item.Tag?.ToString() == s.Position)
                {
                    PositionComboBox.SelectedItem = item;
                    break;
                }
            }

            DurationSlider.Value = s.Duration;
            OpacitySlider.Value = s.Opacity;
            FontSizeSlider.Value = s.FontSize;
            PersistentCheckBox.IsChecked = s.IsPersistent;
            MinimizeToTrayCheckBox.IsChecked = s.MinimizeToTray;
            CloseToTrayCheckBox.IsChecked = s.CloseToTray;

            try
            {
                BgColorPicker.SelectedBrush = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(s.BackgroundColor)
                );
                TextColorPicker.SelectedBrush = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(s.TextColor)
                );
            }
            catch { }

            StartWithWindowsCheckBox.IsChecked = IsInStartup();
        }

        private void SaveSettings()
        {
            if (!_isInitialized)
                return;
            var s = AppSettings.Default;

            if (PositionComboBox.SelectedItem is System.Windows.Controls.ComboBoxItem selectedItem)
                s.Position = selectedItem.Tag.ToString();

            s.Duration = DurationSlider.Value;
            s.Opacity = OpacitySlider.Value;
            s.FontSize = FontSizeSlider.Value;
            s.IsPersistent = PersistentCheckBox.IsChecked ?? false;
            s.IsDarkMode = DarkModeCheckBox.IsChecked ?? false;
            s.MinimizeToTray = MinimizeToTrayCheckBox.IsChecked ?? false;
            s.CloseToTray = CloseToTrayCheckBox.IsChecked ?? false;
            s.BackgroundColor = BgColorPicker.SelectedBrush?.ToString() ?? "#FF000000";
            s.TextColor = TextColorPicker.SelectedBrush?.ToString() ?? "#FFFFFFFF";
            s.Save();
        }

        private void Theme_Changed(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized)
                return;
            if (Application.Current is App app)
            {
                bool isDark = DarkModeCheckBox.IsChecked ?? false;
                app.SetTheme(isDark);
                SaveSettings();
            }
        }

        private void Settings_Changed(object sender, EventArgs e) => SaveSettings();

        private void Settings_Changed(object sender, RoutedPropertyChangedEventArgs<double> e) =>
            SaveSettings();

        private void Settings_Changed(object sender, RoutedEventArgs e) => SaveSettings();

        private void Window_Closing(object? sender, CancelEventArgs e)
        {
            if (CloseToTrayCheckBox.IsChecked == true)
            {
                e.Cancel = true;
                MinimizeToTray();
            }
            else
            {
                _trayIcon?.Dispose();
            }
        }

        private void Window_StateChanged(object? sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized && MinimizeToTrayCheckBox.IsChecked == true)
                Hide();
        }

        private void TrayIcon_DoubleClick(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            _isInitialized = false;
            _monitor?.Stop();
            _trayIcon?.Dispose();
            Application.Current.Shutdown();
        }

        private void StartMonitoring()
        {
            _monitor = new InputLanguageMonitor();
            _monitor.LanguageChanged += (s, lang) =>
                Dispatcher.Invoke(() => ShowNotification(lang));
            _monitor.Start();
        }

        private void ShowNotification(string text)
        {
            _notificationWindow?.Close();
            var s = AppSettings.Default;
            _notificationWindow = new NotificationWindow(
                text,
                s.Position,
                s.Duration,
                s.Opacity,
                s.BackgroundColor,
                s.TextColor,
                s.FontSize
            );
            _notificationWindow.Show();
        }

        private void Persistent_Changed(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            if (PersistentCheckBox.IsChecked == false && _notificationWindow != null)
                _notificationWindow.EnableTimer(AppSettings.Default.Duration);
        }

        private void MinimizeToTray()
        {
            WindowState = WindowState.Minimized;
            Hide();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            var source = HwndSource.FromHwnd(helper.Handle);
            source.AddHook(HwndHook);
            RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_CONTROL, VK_Q);
        }

        private IntPtr HwndHook(
            IntPtr hwnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam,
            ref bool handled
        )
        {
            if (msg == 0x0312 && wParam.ToInt32() == HOTKEY_ID)
            {
                _notificationWindow?.Close();
                handled = true;
            }
            return IntPtr.Zero;
        }

        private void StartWithWindows_Changed(object sender, RoutedEventArgs e)
        {
            if (StartWithWindowsCheckBox.IsChecked == true)
                AddToStartup();
            else
                RemoveFromStartup();
        }

        private bool IsInStartup()
        {
            using var key = Registry.CurrentUser.OpenSubKey(STARTUP_REG_KEY, false);
            return key?.GetValue(APP_NAME) != null;
        }

        private void AddToStartup()
        {
            using var key = Registry.CurrentUser.OpenSubKey(STARTUP_REG_KEY, true);
            var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
            if (exePath != null)
                key?.SetValue(APP_NAME, $"\"{exePath}\" --minimized");
        }

        private void RemoveFromStartup()
        {
            using var key = Registry.CurrentUser.OpenSubKey(STARTUP_REG_KEY, true);
            key?.DeleteValue(APP_NAME, false);
        }

        private void TestNotification_Click(object sender, RoutedEventArgs e) =>
            ShowNotification("TEST");
    }
}
