# Keyboard Language Indicator

![License](https://img.shields.io/badge/license-GPL--3.0-green.svg)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey.svg)
![Version](https://img.shields.io/badge/version-4.0.0-blue.svg)

A powerful Windows utility that not only displays on-screen notifications when you switch keyboard input languages, but also includes intelligent text conversion to fix typing mistakes when you forget to switch keyboard layouts. Choose between traditional screen-positioned notifications or a dynamic cursor-following indicator that stays right at your fingertips.

## ✨ Features

### Core Functionality
- **Real-time language change notifications** - Instantly see which language you've switched to
- **🆕 Intelligent Text Conversion** - Automatically convert mistyped text to the correct language layout
- **Cursor follower mode** - Language indicator follows your mouse cursor
- **Multi-monitor support** - Choose which monitor displays notifications, or show on all monitors
- **Cursor monitor detection** - Automatically display on the monitor where your cursor is located
- **Customizable colors per language** - Set unique colors for each language you use
- **Language configuration editor** - Edit and manage your configured languages with ease
- **Dark mode support** - Comfortable viewing in any lighting condition
- **System tray integration** - Runs quietly in the background
- **Automatic updates** - Stay up-to-date with the latest features
- **Persistent mode** - Keep notifications visible until dismissed with Ctrl+Q
- **Flexible positioning** - Place notifications anywhere on screen
- **Adjustable appearance** - Customize size, opacity, and duration
- **Live color preview** - See notification appearance instantly before saving
- **Toggle notifications** - Enable/disable screen notifications independently from the cursor follower

### 🆕 Text Conversion Feature (New in v4.0.0!)

Ever typed in the wrong keyboard layout? Now you can fix it instantly!

**How it works:**
1. Select the mistyped text in any application
2. Press `Ctrl+Shift+Space`
3. The text is automatically converted to the correct language layout

**Supported conversions:**
- **English ↔ Persian/Farsi** (both Standard & Legacy layouts)
- **English ↔ Arabic**
- **English ↔ Russian**
- **English ↔ Turkish**
- **English ↔ Hebrew**

**Smart Detection:**
- Automatically detects source and target languages
- Tries multiple layout variations (e.g., Persian Standard vs Legacy)
- Works in any Windows application (Word, Notepad, browsers, IDEs, etc.)
- Preserves clipboard contents after conversion

**Example:**
- You meant to type "سلام" (hello in Persian)
- But typed "sghl" because the keyboard was in English
- Select "sghl" → Press `Ctrl+Shift+Space` → Get "سلام"!

### Notification Modes

#### Traditional Mode
Classic screen-positioned notifications that appear at your chosen location (corners, edges, or center). Perfect for users who prefer a fixed notification area.

#### Cursor Follower Mode
A revolutionary way to view language changes - the indicator follows your cursor in real-time! This mode:
- Positions the language indicator near your mouse cursor
- Updates position dynamically as you move the mouse
- Automatically scales based on your screen resolution
- Supports customizable offset adjustments
- Can be persistent or auto-hide after a set duration
- Adapts to DPI scaling on different monitors

## 🚀 Getting Started

### Prerequisites

- Windows 10 (1809 or later) / Windows 11
- .NET 10.0 Runtime (included in installer and portable versions)

### Installation

#### Option 1: Installer (Recommended)
1. Download `KeyboardLanguageIndicator-Setup.exe` from the [Releases](https://github.com/tech-tonic-coder/keyboard-language-indicator/releases) page
2. Run the installer
3. Follow the installation wizard
4. The application will start automatically and minimize to the system tray

#### Option 2: Portable Version
1. Download either `KeyboardLanguageIndicator-x64.exe` or `KeyboardLanguageIndicator-x86.exe` from the [Releases](https://github.com/tech-tonic-coder/keyboard-language-indicator/releases) page
2. Place the executable in your preferred location
3. Run the application - no installation required!

### Quick Start

1. **Launch the app** - It will run in the system tray
2. **Switch languages** - Press your keyboard language switch shortcut (e.g., Alt+Shift)
3. **See the notification** - A visual indicator will appear showing your current language
4. **Try text conversion** - Type in wrong layout, select text, press `Ctrl+Shift+Space`
5. **Configure settings** - Double-click the tray icon to open settings

## ⚙️ Configuration

### General Settings

#### Enable/Disable Notifications
- **Enable screen notifications** - Toggle on-screen position-based notifications
- Works independently from the cursor follower mode

#### Display Options
- **Display Monitor** - Select which monitor shows notifications (multi-monitor setups)
- **Show on All Monitors** - Display notifications on every connected monitor simultaneously
- **Show on Cursor Monitor** - Automatically detect and show on the monitor containing your cursor
- **Notification Position** - Choose from 7 positions (corners, centers, middle)

#### Appearance
- **Duration** - Set how long notifications stay visible (0.5-10 seconds)
- **Persistent Mode** - Keep notifications visible until you press Ctrl+Q
- **Font Size** - Adjust text size (12-48pt)
- **Opacity** - Control notification transparency (10-100%)

### Cursor Follower Settings

Enable cursor follower mode for a dynamic, mouse-tracking language indicator:

- **Enable Cursor Follower** - Toggle cursor-following mode on/off
- **Persistent Mode** - Keep the cursor follower always visible (or auto-hide)
- **Duration** - How long to show before auto-hiding (when not persistent)
- **Offset X** - Horizontal distance from cursor (pixels)
- **Offset Y** - Vertical distance from cursor (pixels)
- **Opacity** - Transparency level of the cursor follower

The cursor follower is automatically:
- Scales font size based on your screen resolution
- Adjusts for DPI scaling on multi-monitor setups
- Updates position in real-time (10ms intervals)
- Recalculates dimensions when moving between monitors

### Appearance Settings

#### Application Theme
- **Enable Dark Mode** - Toggle between light and dark application themes

#### Notification Colors with Live Preview
- **Default Colors** - Set default background and text colors for all languages
- **Live Preview** - See how your notification will look before saving
- **Test Current Colors** - Click to preview notification with selected colors
- **Language-Specific Colors** - Assign unique colors to individual languages
  - Select a language from the dropdown or type a custom code
  - Choose your preferred colors using the color pickers
  - Preview the colors before adding
  - Click "Add Language with Current Colors"
  - Edit existing language configurations
  - Remove languages you no longer need

### Language Configuration Editor

Manage your configured languages with an intuitive interface:

1. **View configured languages** - See all languages with custom colors
2. **Add new languages** - Select from 35+ presets or add custom codes
3. **Edit existing languages** - Click "Edit" to modify colors for any configured language
4. **Remove languages** - Delete language configurations you no longer use
5. **Visual preview** - See color swatches for each configured language

### 🆕 Text Conversion Settings (New!)

Configure the text conversion feature:

- **Keyboard Shortcut** - `Ctrl+Shift+Space` (fixed for now, customization coming soon)
- **Supported Languages** - EN, FA, AR, RU, TR, HE
- **Test Guide** - Click "Text Conversion Guide" to see detailed examples

### Behavior Settings

- **Start with Windows** - Launch automatically on system startup
- **Check for Updates** - Enable automatic update checking (daily)
- **Manual Update Check** - Check for updates on demand
- **Minimize to Tray** - Hide to system tray when minimized
- **Close to Tray** - Keep running in the background when the window is closed

## 🎯 Keyboard Shortcuts

- **Ctrl+Q** - Dismiss persistent notifications and cursor follower
- **🆕 Ctrl+Shift+Space** - Convert selected text to correct language layout

## 🎨 Color Customization

You can customize colors in three ways:

### 1. Default Colors
Applied to all languages without specific settings:
- Select colors in the color pickers
- Click "Test Current Colors" to preview
- Click "Set as Default Colors" to save

### 2. Language-Specific Colors
Override defaults for specific languages:
- Select colors in the color pickers
- Click "Test Current Colors" to preview
- Choose a language from the dropdown
- Click "Add Language with Current Colors"
- The language will be added to your configured list
- Edit colors later using the "Edit" button

### 3. Live Preview
See exactly how your notification will look:
- Adjust colors using the color pickers
- Click the "Test Current Colors" button
- A sample notification appears on your screen
- Fine-tune colors until perfect

### Supported Languages

The app includes presets for 35+ common languages, including:

**European Languages:**
- English, Spanish, French, German, Italian, Portuguese
- Dutch, Swedish, Danish, Norwegian, Finnish
- Polish, Czech, Slovak, Hungarian, Romanian
- Croatian, Slovenian, Estonian, Latvian, Lithuanian
- Bulgarian, Greek

**Middle Eastern & Asian Languages:**
- Arabic, Persian, Hebrew, Turkish
- Russian, Ukrainian
- Chinese, Japanese, Korean
- Thai, Vietnamese, Indonesian

You can also add custom language codes manually for any language not in the preset list.

## 🔧 Text Conversion Examples

### Example 1: English to Persian
**Scenario:** You wanted to type "سلام", but the keyboard was in English mode

1. You typed: `sghl`
2. Select the text: `sghl`
3. Press: `Ctrl+Shift+Space`
4. Result: `سلام`

### Example 2: Persian to English
**Scenario:** You wanted to type "hello" but the keyboard was in Persian mode

1. You typed: `اثممخ` (looks like gibberish)
2. Select the text
3. Press: `Ctrl+Shift+Space`
4. Result: `hello`

### Example 3: Russian to English
**Scenario:** You wanted to type "привет" but the keyboard was in English mode

1. You typed: `ghbdtn`
2. Select the text
3. Press: `Ctrl+Shift+Space`
4. Result: `привет`

### Example 4: Arabic to English
**Scenario:** Text typed in the wrong layout

1. Select mistyped text
2. Press: `Ctrl+Shift+Space`
3. Text automatically converts to the correct layout

## 💡 Tips & Best Practices

### Using Text Conversion
- Works in any application: Notepad, Word, browsers, messaging apps, IDEs, etc.
- The conversion is smart - it tries multiple layout variations automatically
- Clipboard contents are preserved (restored after conversion)
- If conversion doesn't work, check if the language pair is supported
- For Persian: Both Standard and Legacy keyboard layouts are supported

### Using Cursor Follower Mode
- Enable persistent mode if you want constant visibility
- Use non-persistent mode to reduce screen clutter
- Adjust offsets to find your ideal position relative to the cursor
- Lower opacity if you find it distracting during work

### Multi-Monitor Setups
- Use "Show on Cursor Monitor" for automatic monitor detection
- Use "Show on All Monitors" if you switch focus frequently
- Cursor follower works seamlessly across monitors with different DPIs

### Language Colors
- Use contrasting colors for better visibility
- Match colors to your language preferences (e.g., green for English, blue for Spanish)
- Use the preview feature to test colors before committing
- Edit existing configurations to fine-tune your setup

### Notifications vs Cursor Follower
- Both can be enabled simultaneously
- Disable screen notifications if you only want the cursor follower
- Use screen notifications for important language switches
- Use cursor follower for subtle, always-available language indication

## 📋 System Requirements

- **Operating System**: Windows 10 (1809 or later) / Windows 11
- **Framework**: .NET 10.0 Runtime (included in installer and portable versions)
- **Memory**: 50 MB RAM (60 MB with cursor follower active)
- **Storage**: 
  - Installer: ~30 MB download, ~50 MB installed
  - Portable x64: ~25 MB
  - Portable x86: ~22 MB
- **Display**: Any resolution (optimized for multi-monitor setups)
- **DPI Scaling**: Fully supported on all monitors

## 🔄 Updates

The application automatically checks for updates on startup (once per day). You can also manually check for updates from the Behavior or About tabs.

When an update is available, you'll receive a notification with:
- Current version number
- Latest version number
- Direct download link

## 🐛 Troubleshooting

### Notification not appearing

- Ensure the app is running (check system tray)
- Verify your language switching shortcut is working
- Check that the selected monitor is connected
- Try adjusting the notification position
- If using cursor follower, ensure it's enabled in settings
- Check if "Enable screen notifications" is turned on

### Text Conversion not working

- Make sure you select the text before pressing `Ctrl+Shift+Space`
- Verify the language pair is supported (check supported conversions list)
- Ensure the text contains characters from a supported keyboard layout
- Try in a different application to rule out app-specific issues
- Check Windows clipboard access permissions

### Cursor follower not tracking properly

- Check that "Enable Cursor Follower" is toggled on
- Verify cursor follower opacity is not set to 0
- Try adjusting X/Y offset values
- Restart the application

### Colors not changing per language

- Make sure you've added the language in the Appearance tab
- Verify the language code matches what Windows uses
- Click "Save" or "Add Language" after making changes
- Test with the "Show Test Notification" button

### Cursor follower appears in the wrong position

- Adjust the Offset X and Offset Y values in settings
- The follower appears above the cursor by default
- Positive X moves it right, positive Y moves it further up
- Values are in pixels and can be negative

### App not starting with Windows

- Re-enable "Start with Windows" in Behavior settings
- Check Windows Task Manager > Startup tab
- Ensure you have proper user permissions

### Text conversion converts to the wrong language

- The app uses the current and previous keyboard language to determine conversion direction
- Switch to your target language keyboard before converting
- For bidirectional pairs (like EN↔FA), make sure you're on the target keyboard layout

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the GNU General Public License v3.0 - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built with [WPF](https://github.com/dotnet/wpf) and [.NET 10](https://dotnet.microsoft.com/)
- UI components by [HandyControl](https://github.com/HandyOrg/HandyControl)
- System tray integration by [Hardcodet.NotifyIcon.Wpf](https://github.com/hardcodet/wpf-notifyicon)

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/tech-tonic-coder/keyboard-language-indicator/issues)
- **Discussions**: [GitHub Discussions](https://github.com/tech-tonic-coder/keyboard-language-indicator/discussions)
- **Email**: Contact the maintainer through GitHub

## 🗺️ Roadmap

### Planned Features
- [ ] Custom keyboard shortcut configuration for text conversion
- [ ] More language pairs for text conversion (Greek, Ukrainian, etc.)
- [ ] Text conversion history/undo
- [ ] Custom notification sounds per language
- [ ] Animation effects for transitions
- [ ] Multiple notification style presets
- [ ] Mouse badge indicator option
- [ ] Export/import settings profiles
- [ ] Auto-detect and convert on paste

### Completed in v4.0.0
- [x] Intelligent text conversion with Ctrl+Shift+Space
- [x] Support for EN↔FA, EN↔AR, EN↔RU conversions
- [x] Smart layout detection (Persian Standard vs Legacy)
- [x] Live color preview before saving
- [x] Enable/disable notifications toggle
- [x] Extensive keyboard layout detection (150+ layouts)
- [x] Toast notifications for conversion status

### Completed in v3.0.0
- [x] Cursor follower mode with real-time tracking
- [x] Language configuration editor
- [x] Multi-monitor display options (all monitors, cursor monitor)
- [x] Dynamic DPI scaling across different monitors
- [x] Enhanced language color management

## 📊 Changelog

### Version 4.0.0 (Current) - 🆕 Intelligent Text Conversion Release
**Major New Features:**
- **Intelligent Text Conversion**: Automatically convert mistyped text with Ctrl+Shift+Space
  - Support for EN↔FA, EN↔AR, EN↔RU, EN↔TR, EN↔HE
  - Smart layout detection (Persian Standard vs Legacy)
  - Works in any Windows application
  - Preserves clipboard contents
- **Live Color Preview**: Test notification colors before saving
- **Notification Toggle**: Enable/disable screen notifications independently
- **Enhanced Keyboard Layout Detection**: Support for 150+ Windows keyboard layouts
- **Toast Notifications**: Visual feedback for conversion status

**Improvements:**
- Better About section with updated feature list
- Comprehensive text conversion guide
- Improved UI organization
- Enhanced error handling for text conversion
- Better multi-language support detection

**Bug Fixes:**
- Fixed Persian keyboard layout detection
- Improved clipboard handling
- Better hotkey registration

### Version 3.0.1
- Fixed "0x0000" display issue in terminal applications
- Added explicit mappings for 150+ Windows keyboard layouts
- Enhanced language detection algorithm
- Improved fallback for unknown layouts

### Version 3.0.0
- Added cursor follower mode
- Implemented language configuration editor
- Added multi-monitor display options
- Improved DPI scaling
- Enhanced language color management

### Version 2.0.0
- Multi-monitor support
- Customizable colors per language
- Automatic updates
- Dark mode support

---

**Made with ❤️ by [tech-tonic-coder](https://github.com/tech-tonic-coder)**

⭐ If you find this project useful, please consider giving it a star!

## 🎯 Use Cases

- **Multilingual Writers** - Fix typing mistakes instantly when you forget to switch keyboards
- **Translators** - Quickly convert mistyped text without retyping
- **Developers** - Convert code comments or documentation typed in the wrong layout
- **Students** - Stay aware of the language mode during language learning and fix mistakes easily
- **Content Creators** - Ensure correct language input for multilingual content
- **Customer Support** - Handle multi-language support tickets efficiently with instant text conversion
- **Anyone who types in multiple languages** - Save time by converting instead of retyping!
