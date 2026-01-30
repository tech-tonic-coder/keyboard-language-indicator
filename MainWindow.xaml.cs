using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Hardcodet.Wpf.TaskbarNotification;
using KeyboardLanguageIndicator.Services;
using MessageBox = System.Windows.MessageBox;

namespace KeyboardLanguageIndicator;

public partial class MainWindow : HandyControl.Controls.Window
{
    private const int HOTKEY_ID = 9000;
    private const uint MOD_CONTROL = 0x0002;
    private const uint VK_Q = 0x51;

    private readonly SettingsService _settings = new();
    private readonly StartupService _startup = new();
    private readonly UpdateService _updateService = new();
    private TaskbarIcon? _trayIcon;
    private List<NotificationWindow> _notificationWindows = new();
    private CursorFollowerWindow? _cursorFollower;
    private InputLanguageMonitor? _monitor;
    private bool _isInitialized;

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public MainWindow()
    {
        InitializeComponent();
        HandyControl.Tools.ConfigHelper.Instance.SetLang("en");

        InitializeTray();
        InitializeMonitors();
        InitializeLanguageSelector();
        LoadSettings();

        _isInitialized = true;
        StartLanguageMonitoring();

        if (App.StartMinimized)
            MinimizeToTray();
        else
            _ = CheckForUpdatesOnStartupAsync();
    }

    #region Initialization

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
        _trayIcon.TrayMouseDoubleClick += (s, e) => RestoreWindow();
    }

    private void InitializeMonitors()
    {
        foreach (var monitor in MonitorHelper.GetAllMonitors())
        {
            MonitorComboBox.Items.Add(
                new System.Windows.Controls.ComboBoxItem
                {
                    Content = monitor.DisplayName,
                    Tag = monitor.Index,
                }
            );
        }
    }

    private void InitializeLanguageSelector()
    {
        foreach (var lang in LanguageList.Common)
        {
            LanguageSelector.Items.Add(
                new System.Windows.Controls.ComboBoxItem
                {
                    Content = $"{lang.Name} ({lang.Code})",
                    Tag = lang.Code,
                }
            );
        }
    }

    private void StartLanguageMonitoring()
    {
        _monitor = new InputLanguageMonitor();
        _monitor.LanguageChanged += (s, lang) => Dispatcher.Invoke(() => OnLanguageChanged(lang));
        _monitor.Start();
    }

    #endregion

    #region Settings Management

    private void LoadSettings()
    {
        _settings.LoadLanguageColors();

        // Theme
        if (Application.Current is App app)
            app.SetTheme(_settings.IsDarkMode);
        DarkModeCheckBox.IsChecked = _settings.IsDarkMode;

        // Monitor display mode
        if (_settings.ShowOnAllMonitors)
        {
            AllMonitorsRadio.IsChecked = true;
            MonitorComboBox.IsEnabled = false;
        }
        else if (_settings.ShowOnCursorMonitor)
        {
            CursorMonitorRadio.IsChecked = true;
            MonitorComboBox.IsEnabled = false;
        }
        else
        {
            SingleMonitorRadio.IsChecked = true;
            MonitorComboBox.IsEnabled = true;
        }

        // Monitor
        SelectComboBoxByTag(MonitorComboBox, _settings.MonitorIndex);

        // Position
        SelectComboBoxByTag(PositionComboBox, _settings.Position);

        // Sliders
        DurationSlider.Value = _settings.Duration;
        OpacitySlider.Value = _settings.Opacity;
        FontSizeSlider.Value = _settings.FontSize;

        // Checkboxes
        PersistentCheckBox.IsChecked = _settings.IsPersistent;
        MinimizeToTrayCheckBox.IsChecked = _settings.MinimizeToTray;
        CloseToTrayCheckBox.IsChecked = _settings.CloseToTray;
        StartWithWindowsCheckBox.IsChecked = _startup.IsEnabled();
        CheckForUpdatesCheckBox.IsChecked = _settings.CheckForUpdates;

        // Cursor Follower
        EnableCursorFollowerCheckBox.IsChecked = _settings.EnableCursorFollower;
        CursorFollowerPersistentCheckBox.IsChecked = _settings.CursorFollowerPersistent;
        CursorFollowerDurationSlider.Value = _settings.CursorFollowerDuration;
        CursorFollowerOpacitySlider.Value = _settings.CursorFollowerOpacity;
        CursorFollowerOffsetXSlider.Value = _settings.CursorFollowerOffsetX;
        CursorFollowerOffsetYSlider.Value = _settings.CursorFollowerOffsetY;

        UpdateCursorFollowerUI();

        // About
        VersionText.Text = $"Version {UpdateService.GetCurrentVersion()}";

        // Colors
        BgColorPicker.SelectedBrush = _settings.BackgroundColor.ToBrush();
        TextColorPicker.SelectedBrush = _settings.TextColor.ToBrush();

        RefreshLanguageList();
    }

    private void SaveSettings()
    {
        if (!_isInitialized)
            return;

        _settings.Position = GetSelectedTag<string>(PositionComboBox);
        _settings.MonitorIndex = GetSelectedTag<int>(MonitorComboBox);
        _settings.Duration = DurationSlider.Value;
        _settings.Opacity = OpacitySlider.Value;
        _settings.FontSize = FontSizeSlider.Value;
        _settings.IsPersistent = PersistentCheckBox.IsChecked ?? false;
        _settings.IsDarkMode = DarkModeCheckBox.IsChecked ?? false;
        _settings.MinimizeToTray = MinimizeToTrayCheckBox.IsChecked ?? false;
        _settings.CloseToTray = CloseToTrayCheckBox.IsChecked ?? false;
        _settings.CheckForUpdates = CheckForUpdatesCheckBox.IsChecked ?? false;

        // Monitor display mode
        _settings.ShowOnAllMonitors = AllMonitorsRadio.IsChecked ?? false;
        _settings.ShowOnCursorMonitor = CursorMonitorRadio.IsChecked ?? false;

        // Cursor Follower
        _settings.EnableCursorFollower = EnableCursorFollowerCheckBox.IsChecked ?? false;
        _settings.CursorFollowerPersistent = CursorFollowerPersistentCheckBox.IsChecked ?? false;
        _settings.CursorFollowerDuration = CursorFollowerDurationSlider.Value;
        _settings.CursorFollowerOpacity = CursorFollowerOpacitySlider.Value;
        _settings.CursorFollowerOffsetX = CursorFollowerOffsetXSlider.Value;
        _settings.CursorFollowerOffsetY = CursorFollowerOffsetYSlider.Value;

        _settings.Save();
    }

    #endregion

    #region UI Event Handlers

    private void Settings_Changed(object sender, EventArgs e) => SaveSettings();

    private void Theme_Changed(object sender, RoutedEventArgs e)
    {
        if (!_isInitialized)
            return;

        if (Application.Current is App app)
            app.SetTheme(DarkModeCheckBox.IsChecked ?? false);

        SaveSettings();
    }

    private void Persistent_Changed(object sender, RoutedEventArgs e)
    {
        SaveSettings();
        if (!_settings.IsPersistent)
        {
            foreach (var window in _notificationWindows)
                window.EnableTimer(_settings.Duration);
        }
    }

    private void MonitorMode_Changed(object sender, RoutedEventArgs e)
    {
        if (!_isInitialized)
            return;

        MonitorComboBox.IsEnabled = SingleMonitorRadio.IsChecked ?? false;
        SaveSettings();
    }

    private void CursorFollower_Changed(object sender, RoutedEventArgs e)
    {
        if (!_isInitialized)
            return;

        UpdateCursorFollowerUI();
        SaveSettings();

        // Restart cursor follower if settings changed
        if (_settings.EnableCursorFollower)
        {
            _cursorFollower?.Close();
            _cursorFollower = new CursorFollowerWindow(_settings);

            if (_monitor != null)
            {
                var currentLang = GetCurrentLanguage();
                if (!string.IsNullOrEmpty(currentLang))
                    _cursorFollower.UpdateLanguage(currentLang);
            }
        }
        else
        {
            _cursorFollower?.Close();
            _cursorFollower = null;
        }
    }

    private void UpdateCursorFollowerUI()
    {
        var isEnabled = EnableCursorFollowerCheckBox.IsChecked ?? false;
        CursorFollowerSettingsPanel.IsEnabled = isEnabled;

        var isPersistent = CursorFollowerPersistentCheckBox.IsChecked ?? false;
        CursorFollowerDurationPanel.Visibility = isPersistent
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    private void StartWithWindows_Changed(object sender, RoutedEventArgs e)
    {
        if (StartWithWindowsCheckBox.IsChecked == true)
            _startup.Enable();
        else
            _startup.Disable();
    }

    private void TestNotification_Click(object sender, RoutedEventArgs e) =>
        OnLanguageChanged("TEST");

    private void ViewOnGitHub_Click(object sender, RoutedEventArgs e) =>
        UpdateService.OpenReleasesPage();

    #endregion

    #region Language Change Handling

    private void OnLanguageChanged(string language)
    {
        // Show regular notifications
        ShowNotifications(language);

        // Update cursor follower
        if (_settings.EnableCursorFollower)
        {
            if (_cursorFollower == null)
            {
                _cursorFollower = new CursorFollowerWindow(_settings);
            }
            _cursorFollower.UpdateLanguage(language);
        }
    }

    private void ShowNotifications(string language)
    {
        // Close existing notifications
        foreach (var window in _notificationWindows)
            window.Close();
        _notificationWindows.Clear();

        // Get language-specific colors or use defaults
        if (!_settings.TryGetLanguageColor(language, out var bgColor, out var textColor))
        {
            bgColor = _settings.BackgroundColor;
            textColor = _settings.TextColor;
        }

        if (_settings.ShowOnAllMonitors)
        {
            // Show on all monitors
            var monitors = MonitorHelper.GetAllMonitors();
            foreach (var monitor in monitors)
            {
                var window = CreateNotificationWindow(language, bgColor, textColor, monitor.Index);
                _notificationWindows.Add(window);
                window.Show();
            }
        }
        else if (_settings.ShowOnCursorMonitor)
        {
            // Show on cursor's monitor
            var cursorMonitorIndex = MonitorHelper.GetCursorMonitorIndex();
            var window = CreateNotificationWindow(language, bgColor, textColor, cursorMonitorIndex);
            _notificationWindows.Add(window);
            window.Show();
        }
        else
        {
            // Show on selected monitor
            var window = CreateNotificationWindow(
                language,
                bgColor,
                textColor,
                _settings.MonitorIndex
            );
            _notificationWindows.Add(window);
            window.Show();
        }
    }

    private NotificationWindow CreateNotificationWindow(
        string language,
        Color bgColor,
        Color textColor,
        int monitorIndex
    )
    {
        return new NotificationWindow(
            language,
            _settings.Position,
            _settings.Duration,
            _settings.Opacity,
            bgColor,
            textColor,
            _settings.FontSize,
            monitorIndex
        );
    }

    private string GetCurrentLanguage()
    {
        // This is a helper to get current language for cursor follower initialization
        // You might want to track this in the InputLanguageMonitor
        return "EN"; // Default, will be updated on next language change
    }

    #endregion

    #region Language Colors Management

    private void AddLanguageConfig_Click(object sender, RoutedEventArgs e)
    {
        string langInput;
        if (LanguageSelector.SelectedItem is System.Windows.Controls.ComboBoxItem item)
        {
            langInput = item.Tag?.ToString() ?? LanguageSelector.Text;
        }
        else
        {
            langInput = LanguageSelector.Text;
        }

        var langCode = LanguageList.GetCode(langInput.Trim());

        if (string.IsNullOrEmpty(langCode))
        {
            MessageBox.Show(
                "Please select or enter a language.",
                "Invalid Input",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
            return;
        }

        var bgColor = BgColorPicker.SelectedBrush?.Color ?? Colors.Black;
        var textColor = TextColorPicker.SelectedBrush?.Color ?? Colors.White;

        _settings.SetLanguageColor(langCode, bgColor, textColor);
        _settings.SaveLanguageColors();
        RefreshLanguageList();

        LanguageSelector.SelectedItem = null;
        LanguageSelector.Text = string.Empty;

        MessageBox.Show(
            $"Colors saved for {LanguageList.GetName(langCode)}.",
            "Language Added",
            MessageBoxButton.OK,
            MessageBoxImage.Information
        );
    }

    private void EditLanguageConfig_Click(object sender, RoutedEventArgs e)
    {
        if (sender is System.Windows.Controls.Button { Tag: string langCode })
        {
            // Get existing colors for the language
            if (_settings.TryGetLanguageColor(langCode, out var bgColor, out var textColor))
            {
                // Set the color pickers to the existing colors
                BgColorPicker.SelectedBrush = new SolidColorBrush(bgColor);
                TextColorPicker.SelectedBrush = new SolidColorBrush(textColor);

                // Set the language selector to the language being edited
                var langName = LanguageList.GetName(langCode);
                foreach (System.Windows.Controls.ComboBoxItem item in LanguageSelector.Items)
                {
                    if (item.Tag?.ToString() == langCode)
                    {
                        LanguageSelector.SelectedItem = item;
                        break;
                    }
                }

                // If language not found in dropdown, set it manually in the editable field
                if (LanguageSelector.SelectedItem == null)
                {
                    LanguageSelector.Text = langCode;
                }

                MessageBox.Show(
                    $"Color pickers have been set to the current colors for {langName}.\n\n"
                        + "Adjust the colors above and click 'Add Language with Current Colors' to update.",
                    "Edit Language Colors",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }
    }

    private void SaveDefaultColors_Click(object sender, RoutedEventArgs e)
    {
        if (BgColorPicker.SelectedBrush is SolidColorBrush bgBrush)
            _settings.BackgroundColor = bgBrush.Color;

        if (TextColorPicker.SelectedBrush is SolidColorBrush textBrush)
            _settings.TextColor = textBrush.Color;

        _settings.Save();

        MessageBox.Show(
            "Default colors have been saved.",
            "Colors Saved",
            MessageBoxButton.OK,
            MessageBoxImage.Information
        );
    }

    private void RemoveLanguageConfig_Click(object sender, RoutedEventArgs e)
    {
        if (sender is System.Windows.Controls.Button { Tag: string langCode })
        {
            _settings.RemoveLanguageColor(langCode);
            SaveSettings();
            RefreshLanguageList();
        }
    }

    private void RefreshLanguageList()
    {
        var items = _settings
            .GetConfiguredLanguages()
            .Select(lang =>
            {
                _settings.TryGetLanguageColor(lang, out var bg, out var text);
                var displayName = LanguageList.GetName(lang);
                return new LanguageColorItem(lang, displayName, bg.ToBrush(), text.ToBrush());
            })
            .ToList();

        LanguageConfigList.ItemsSource = items;
    }

    #endregion

    #region Update Checking

    private async Task CheckForUpdatesOnStartupAsync()
    {
        if (!_settings.CheckForUpdates)
            return;

        var lastCheck = _settings.LastUpdateCheck;
        if (lastCheck.HasValue && (DateTime.Now - lastCheck.Value).TotalHours < 24)
            return;

        _settings.LastUpdateCheck = DateTime.Now;
        _settings.Save();

        await Task.Delay(5000);
        await CheckForUpdatesAsync(silent: true);
    }

    private async void CheckForUpdates_Click(object sender, RoutedEventArgs e)
    {
        await CheckForUpdatesAsync(silent: false);
    }

    private async Task CheckForUpdatesAsync(bool silent)
    {
        if (!silent)
        {
            CheckNowButton.IsEnabled = false;
            CheckNowButton.Content = "Checking...";
            CheckForUpdatesAboutButton.IsEnabled = false;
            CheckForUpdatesAboutButton.Content = "Checking...";
        }

        try
        {
            var updateInfo = await _updateService.CheckForUpdatesAsync();

            if (updateInfo == null)
            {
                if (!silent)
                    MessageBox.Show(
                        "Unable to check for updates. Please check your internet connection.",
                        "Update Check Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                return;
            }

            if (updateInfo.IsNewerVersion)
            {
                ShowUpdateNotification(updateInfo);
            }
            else if (!silent)
            {
                MessageBox.Show(
                    $"You are running the latest version ({UpdateService.GetCurrentVersion()}).",
                    "No Updates Available",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }
        finally
        {
            if (!silent)
            {
                CheckNowButton.IsEnabled = true;
                CheckNowButton.Content = "Check for Updates Now";
                CheckForUpdatesAboutButton.IsEnabled = true;
                CheckForUpdatesAboutButton.Content = "Check for Updates";
            }
        }
    }

    private void ShowUpdateNotification(UpdateService.UpdateInfo updateInfo)
    {
        var currentVersion = UpdateService.GetCurrentVersion();
        var message =
            $"A new version is available!\n\n"
            + $"Current Version: {currentVersion}\n"
            + $"Latest Version: {updateInfo.Version}\n\n"
            + $"Would you like to download it now?";

        var result = MessageBox.Show(
            message,
            "Update Available",
            MessageBoxButton.YesNo,
            MessageBoxImage.Information
        );

        if (result == MessageBoxResult.Yes)
        {
            UpdateService.OpenDownloadUrl(updateInfo.DownloadUrl);
        }
    }

    #endregion

    #region Window Management

    private void Window_Closing(object? sender, CancelEventArgs e)
    {
        if (_settings.CloseToTray)
        {
            e.Cancel = true;
            MinimizeToTray();
        }
        else
        {
            _monitor?.Stop();
            _cursorFollower?.Close();
            foreach (var window in _notificationWindows)
                window.Close();
            _trayIcon?.Dispose();
        }
    }

    private void Window_StateChanged(object? sender, EventArgs e)
    {
        if (WindowState == WindowState.Minimized && _settings.MinimizeToTray)
            Hide();
    }

    private void MinimizeToTray()
    {
        WindowState = WindowState.Minimized;
        Hide();
    }

    private void RestoreWindow()
    {
        Show();
        WindowState = WindowState.Normal;
        Activate();
    }

    private void TrayIcon_DoubleClick(object sender, RoutedEventArgs e) => RestoreWindow();

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        _monitor?.Stop();
        _cursorFollower?.Close();
        foreach (var window in _notificationWindows)
            window.Close();
        _trayIcon?.Dispose();
        Application.Current.Shutdown();
    }

    #endregion

    #region Hotkey Support

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        var helper = new WindowInteropHelper(this);
        var source = HwndSource.FromHwnd(helper.Handle);
        source.AddHook(HwndHook);
        RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_CONTROL, VK_Q);
    }

    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == 0x0312 && wParam.ToInt32() == HOTKEY_ID)
        {
            foreach (var window in _notificationWindows)
                window.Close();
            handled = true;
        }
        return IntPtr.Zero;
    }

    #endregion

    #region Helper Methods

    private static void SelectComboBoxByTag<T>(
        System.Windows.Controls.ComboBox comboBox,
        T tagValue
    )
    {
        foreach (System.Windows.Controls.ComboBoxItem item in comboBox.Items)
        {
            if (item.Tag?.Equals(tagValue) == true)
            {
                comboBox.SelectedItem = item;
                break;
            }
        }
    }

    private static T GetSelectedTag<T>(System.Windows.Controls.ComboBox comboBox)
    {
        if (comboBox.SelectedItem is System.Windows.Controls.ComboBoxItem item && item.Tag is T tag)
            return tag;
        return default!;
    }

    #endregion
}

public record LanguageColorItem(
    string Code,
    string Name,
    SolidColorBrush BackgroundBrush,
    SolidColorBrush TextBrush
);
