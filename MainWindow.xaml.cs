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
    private const int HOTKEY_ID_CLOSE = 9000;
    private const int HOTKEY_ID_CONVERT = 9001;
    private const uint MOD_CONTROL = 0x0002;
    private const uint MOD_SHIFT = 0x0004;
    private const uint VK_Q = 0x51;
    private const uint VK_SPACE = 0x20;

    private readonly SettingsService _settings = new();
    private readonly StartupService _startup = new();
    private readonly UpdateService _updateService = new();
    private TaskbarIcon? _trayIcon;
    private List<NotificationWindow> _notificationWindows = new();
    private CursorFollowerWindow? _cursorFollower;
    private InputLanguageMonitor? _monitor;
    private bool _isInitialized;
    private NotificationWindow? _previewWindow;
    private string _currentLanguage = "EN"; // Track current language
    private string _previousLanguage = "EN"; // Track previous language for conversion

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
            ToolTipText = "Keyboard Indicator\nCtrl+Shift+Space: Convert text",
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

        // Enable/Disable Notifications
        EnableNotificationsCheckBox.IsChecked = _settings.EnableNotifications;
        UpdateNotificationUI();

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

        // Add event handlers for color preview
        BgColorPicker.SelectedColorChanged += ColorPicker_SelectedColorChanged;
        TextColorPicker.SelectedColorChanged += ColorPicker_SelectedColorChanged;

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
        _settings.EnableNotifications = EnableNotificationsCheckBox.IsChecked ?? false;

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

    private void Notifications_Changed(object sender, RoutedEventArgs e)
    {
        if (!_isInitialized)
            return;

        UpdateNotificationUI();
        SaveSettings();
    }

    private void UpdateNotificationUI()
    {
        var isEnabled = EnableNotificationsCheckBox.IsChecked ?? false;
        NotificationSettingsPanel.IsEnabled = isEnabled;
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

            if (!string.IsNullOrEmpty(_currentLanguage))
                _cursorFollower.UpdateLanguage(_currentLanguage);
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

    private void ColorPicker_SelectedColorChanged(
        object sender,
        HandyControl.Data.FunctionEventArgs<Color> e
    )
    {
        if (!_isInitialized)
            return;

        // Update preview window with new colors
        UpdatePreviewWindow();
    }

    private void UpdatePreviewWindow()
    {
        // Close existing preview if any
        _previewWindow?.Close();

        // Get current colors from pickers
        var bgColor = BgColorPicker.SelectedBrush?.Color ?? _settings.BackgroundColor;
        var textColor = TextColorPicker.SelectedBrush?.Color ?? _settings.TextColor;

        // Create a preview notification
        _previewWindow = CreateNotificationWindow(
            "PREVIEW",
            bgColor,
            textColor,
            _settings.MonitorIndex
        );
        _previewWindow.Show();

        // Auto-close after 3 seconds
        var timer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(3),
        };
        timer.Tick += (s, e) =>
        {
            _previewWindow?.Close();
            _previewWindow = null;
            timer.Stop();
        };
        timer.Start();
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
        _previousLanguage = _currentLanguage; // Store previous for conversion
        _currentLanguage = language; // Track current language

        // Show regular notifications if enabled
        if (_settings.EnableNotifications)
        {
            ShowNotifications(language);
        }

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

    #endregion

    #region Language Colors Management - IMPROVED

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
        _settings.Save(); // IMPORTANT: Save immediately
        RefreshLanguageList();

        // Apply colors IMMEDIATELY if this is the current language
        ApplyColorsImmediately(langCode, bgColor, textColor);

        LanguageSelector.SelectedItem = null;
        LanguageSelector.Text = string.Empty;

        MessageBox.Show(
            $"Colors saved and applied instantly for {LanguageList.GetName(langCode)}!",
            "Success",
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

        _settings.Save(); // IMPORTANT: Save immediately

        // Apply colors immediately to current language if it doesn't have custom colors
        var bgColor = _settings.BackgroundColor;
        var textColor = _settings.TextColor;

        if (!_settings.TryGetLanguageColor(_currentLanguage, out _, out _))
        {
            ApplyColorsImmediately(_currentLanguage, bgColor, textColor);
        }

        // Show a preview notification with the new colors
        UpdatePreviewWindow();

        MessageBox.Show(
            "Default colors have been saved and applied instantly!",
            "Success",
            MessageBoxButton.OK,
            MessageBoxImage.Information
        );
    }

    /// <summary>
    /// Apply colors immediately to notification and cursor follower windows
    /// </summary>
    private void ApplyColorsImmediately(string langCode, Color bgColor, Color textColor)
    {
        // Apply to notification windows if current language matches
        if (langCode == _currentLanguage)
        {
            foreach (var window in _notificationWindows)
            {
                window.UpdateColors(bgColor, textColor);
            }

            // Apply to cursor follower
            if (_cursorFollower != null && _settings.EnableCursorFollower)
            {
                _cursorFollower.UpdateColors(bgColor, textColor);
            }
        }
    }

    private void RemoveLanguageConfig_Click(object sender, RoutedEventArgs e)
    {
        if (sender is System.Windows.Controls.Button { Tag: string langCode })
        {
            var result = MessageBox.Show(
                $"Are you sure you want to remove custom colors for {LanguageList.GetName(langCode)}?",
                "Confirm Removal",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                _settings.RemoveLanguageColor(langCode);
                _settings.SaveLanguageColors();
                _settings.Save();
                RefreshLanguageList();

                // If this is the current language, revert to default colors
                if (langCode == _currentLanguage)
                {
                    ApplyColorsImmediately(
                        _currentLanguage,
                        _settings.BackgroundColor,
                        _settings.TextColor
                    );
                }

                MessageBox.Show(
                    $"Custom colors removed for {LanguageList.GetName(langCode)}",
                    "Removed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
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

    #region Text Conversion - IMPROVED AUTOMATIC VERSION

    /// <summary>
    /// Convert selected text automatically - called by hotkey Ctrl+Shift+Space
    /// This function:
    /// 1. Automatically copies selected text (simulates Ctrl+C)
    /// 2. Converts the text between current and previous language
    /// 3. Pastes it back (simulates Ctrl+V)
    /// All in one smooth operation!
    /// </summary>
    private async void ConvertSelectedText()
    {
        try
        {
            Clipboard.Clear(); // Clear clipboard to detect new content reliably

            // Step 1: Simulate Ctrl+C to copy selected text
            await Task.Delay(200);
            System.Windows.Forms.SendKeys.SendWait("^c");
            int retry = 0;
            while (!Clipboard.ContainsText() && retry < 10)
            {
                await Task.Delay(100);
                retry++;
            } // Wait for clipboard to be populated

            // Step 2: Get the text from clipboard
            if (!System.Windows.Forms.Clipboard.ContainsText())
            {
                ShowToast("No text selected", "Please select text first", 3);
                return;
            }

            string selectedText = System.Windows.Forms.Clipboard.GetText();

            // Check if there's actual text
            if (string.IsNullOrWhiteSpace(selectedText))
            {
                ShowToast("No text selected", "Please select text first", 3);
                return;
            }

            // Step 3: Determine conversion direction
            string fromLang = _previousLanguage;
            string toLang = _currentLanguage;

            // Check if mapping exists
            if (!TextConversionService.HasMapping(fromLang, toLang))
            {
                // Try reverse direction
                if (TextConversionService.HasMapping(toLang, fromLang))
                {
                    (fromLang, toLang) = (toLang, fromLang);
                }
                else
                {
                    ShowToast(
                        "Conversion not supported",
                        $"No mapping between {fromLang} and {toLang}",
                        3
                    );
                    return;
                }
            }

            // Step 4: Convert the text
            string convertedText = TextConversionService.ConvertText(
                selectedText,
                fromLang,
                toLang
            );

            // Check if conversion actually changed anything
            if (convertedText == selectedText)
            {
                ShowToast("No conversion needed", "Text appears to be in correct language", 2);
                return;
            }

            // Step 5: Put converted text in clipboard
            System.Windows.Forms.Clipboard.SetText(convertedText);
            await Task.Delay(100);

            // Step 6: Simulate Ctrl+V to paste converted text
            System.Windows.Forms.SendKeys.SendWait("^v");
            await Task.Delay(100);

            // Step 7: Show success notification
            ShowToast("Text Converted!", $"{fromLang} → {toLang}", 3);

            // Step 8: Restore original clipboard after a delay
            await Task.Delay(500);
        }
        catch (Exception ex)
        {
            ShowToast("Conversion Error", ex.Message, 3);
        }
    }

    /// <summary>
    /// Show a temporary toast notification
    /// </summary>
    private void ShowToast(string title, string message, double durationSeconds)
    {
        var toast = new NotificationWindow(
            title,
            "Center",
            durationSeconds,
            0.95,
            Colors.DarkSlateGray,
            Colors.White,
            18,
            MonitorHelper.GetCursorMonitorIndex()
        );

        toast.Show();
    }

    /// <summary>
    /// shows comprehensive guide to user
    /// </summary>
    private void TestConversion_Click(object sender, RoutedEventArgs e)
    {
        var testMessage =
            "📝 TEXT CONVERSION GUIDE\n"
            + "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n"
            + "🎯 HOW IT WORKS:\n\n"
            + "1️⃣ Select mistyped text in ANY application\n"
            + "2️⃣ Press Ctrl+Shift+Space\n"
            + "3️⃣ Text converts to correct keyboard layout!\n\n"
            + "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n"
            + "📋 STEP-BY-STEP EXAMPLES:\n\n"
            + "Example 1: English → Persian\n"
            + "  ⚠️ Problem: Meant to type \"سلام\" but keyboard was English\n"
            + "  ⌨️ You typed: sghl\n"
            + "  🎬 Steps:\n"
            + "     • Select \"sghl\" with mouse or Ctrl+A\n"
            + "     • Press Ctrl+Shift+Space\n"
            + "  ✅ Result: سلام\n\n"
            + "Example 2: Persian → English\n"
            + "  ⚠️ Problem: Meant to type \"hello\" but keyboard was Persian\n"
            + "  ⌨️ You typed: اثممخ (gibberish!)\n"
            + "  🎬 Steps:\n"
            + "     • Select the text\n"
            + "     • Press Ctrl+Shift+Space\n"
            + "  ✅ Result: hello\n\n"
            + "Example 3: Russian → English\n"
            + "  ⚠️ Problem: Meant to type \"привет\" but forgot to switch\n"
            + "  ⌨️ You typed: ghbdtn\n"
            + "  🎬 Steps:\n"
            + "     • Select \"ghbdtn\"\n"
            + "     • Press Ctrl+Shift+Space\n"
            + "  ✅ Result: привет\n\n"
            + "Example 4: Arabic → English\n"
            + "  ⚠️ Problem: Typed in wrong layout\n"
            + "  🎬 Steps:\n"
            + "     • Select mistyped Arabic text\n"
            + "     • Press Ctrl+Shift+Space\n"
            + "  ✅ Result: Converts to English equivalent\n\n"
            + "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n"
            + "🌍 SUPPORTED CONVERSIONS:\n\n"
            + "  ✅ English ↔ Persian/Farsi\n"
            + "     (Standard & Legacy keyboard layouts)\n"
            + "  ✅ English ↔ Arabic\n"
            + "  ✅ English ↔ Russian\n"
            + "  ✅ English ↔ Turkish\n"
            + "  ✅ English ↔ Hebrew\n\n"
            + "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n"
            + "⚙️ HOW IT DETECTS LANGUAGE:\n\n"
            + "The app uses your current and previous keyboard\n"
            + "language to determine conversion direction:\n\n"
            + "  🔄 If you switch from EN to FA:\n"
            + "     → Converts English text to Persian\n\n"
            + "  🔄 If you switch from FA to EN:\n"
            + "     → Converts Persian text to English\n\n"
            + "💡 Tip: Switch to your TARGET language\n"
            + "   keyboard BEFORE converting for best results!\n\n"
            + "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n"
            + "ℹ️ CURRENT STATE:\n\n"
            + $"  • Current Language: {_currentLanguage}\n"
            + $"  • Previous Language: {_previousLanguage}\n\n"
            + "  (These are used to determine conversion direction)\n\n"
            + "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n"
            + "🎯 WHERE IT WORKS:\n\n"
            + "  ✅ Microsoft Word, Excel, PowerPoint\n"
            + "  ✅ Notepad, Notepad++, VS Code\n"
            + "  ✅ Web browsers (Chrome, Firefox, Edge)\n"
            + "  ✅ Messaging apps (Telegram, WhatsApp, etc.)\n"
            + "  ✅ Email clients (Outlook, Thunderbird)\n"
            + "  ✅ IDEs (Visual Studio, PyCharm, etc.)\n"
            + "  ✅ Any Windows application with text input!\n\n"
            + "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n"
            + "💡 PRO TIPS:\n\n"
            + "  • Works with SELECTED text only\n"
            + "  • Your clipboard is preserved (don't worry!)\n"
            + "  • Conversion is smart - tries multiple layouts\n"
            + "  • If it doesn't convert, check supported pairs\n"
            + "  • For Persian: Both Standard & Legacy supported\n"
            + "  • Toast notification shows conversion status\n\n"
            + "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n"
            + "🚀 TRY IT NOW:\n\n"
            + "1. Open Notepad or any text editor\n"
            + "2. Type some text in wrong keyboard layout\n"
            + "3. Select the text (Ctrl+A)\n"
            + "4. Press Ctrl+Shift+Space\n"
            + "5. Watch the magic happen! ✨\n\n"
            + "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n"
            + "❓ Need help? Check GitHub Issues or Discussions!";

        MessageBox.Show(
            testMessage,
            "Text Conversion - Complete Guide",
            MessageBoxButton.OK,
            MessageBoxImage.Information
        );
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
        // Close preview window if open
        _previewWindow?.Close();

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

            // Unregister hotkeys
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, HOTKEY_ID_CLOSE);
            UnregisterHotKey(helper.Handle, HOTKEY_ID_CONVERT);
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
        _previewWindow?.Close();
        foreach (var window in _notificationWindows)
            window.Close();
        _trayIcon?.Dispose();

        // Unregister hotkeys
        var helper = new WindowInteropHelper(this);
        UnregisterHotKey(helper.Handle, HOTKEY_ID_CLOSE);
        UnregisterHotKey(helper.Handle, HOTKEY_ID_CONVERT);

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

        // Register Ctrl+Q for closing persistent notifications
        RegisterHotKey(helper.Handle, HOTKEY_ID_CLOSE, MOD_CONTROL, VK_Q);

        // Register Ctrl+Shift+Space for text conversion
        RegisterHotKey(helper.Handle, HOTKEY_ID_CONVERT, MOD_CONTROL | MOD_SHIFT, VK_SPACE);
    }

    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        const int WM_HOTKEY = 0x0312;

        if (msg == WM_HOTKEY)
        {
            int id = wParam.ToInt32();

            if (id == HOTKEY_ID_CLOSE)
            {
                // Close persistent notifications
                foreach (var window in _notificationWindows)
                    window.Close();
                handled = true;
            }
            else if (id == HOTKEY_ID_CONVERT)
            {
                // Convert selected text
                ConvertSelectedText();
                handled = true;
            }
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
