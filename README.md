# Keyboard Language Indicator

![License](https://img.shields.io/badge/license-GPL--3.0-green.svg)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey.svg)
![Version](https://img.shields.io/badge/version-3.0.0-blue.svg)

A lightweight utility that displays on-screen notifications when you switch keyboard input languages. Choose between traditional screen-positioned notifications or a dynamic cursor-following indicator that stays right at your fingertips.

## ✨ Features

### Core Functionality
- **Real-time language change notifications** - Instantly see which language you've switched to
- **Cursor follower mode** - NEW! Language indicator follows your mouse cursor
- **Multi-monitor support** - Choose which monitor displays notifications, or show on all monitors
- **Cursor monitor detection** - Automatically display on the monitor where your cursor is located
- **Customizable colors per language** - Set unique colors for each language you use
- **Language configuration editor** - NEW! Edit and manage your configured languages with ease
- **Dark mode support** - Comfortable viewing in any lighting condition
- **System tray integration** - Runs quietly in the background
- **Automatic updates** - Stay up-to-date with the latest features
- **Persistent mode** - Keep notifications visible until dismissed with Ctrl+Q
- **Flexible positioning** - Place notifications anywhere on screen
- **Adjustable appearance** - Customize size, opacity, and duration

### Notification Modes

#### Traditional Mode
Classic screen-positioned notifications that appear at your chosen location (corners, edges, or center). Perfect for users who prefer a fixed notification area.

#### Cursor Follower Mode (NEW!)
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
- .NET 10.0 Runtime (included in the installer)

### Installation

1. Download the latest release from the [Releases](https://github.com/tech-tonic-coder/keyboard-language-indicator/releases) page
2. Run the `KeyboardLanguageIndicator.exe` installer
3. The application will start automatically and minimize to the system tray

### Quick Start

1. **Launch the app** - It will run in the system tray
2. **Switch languages** - Press your keyboard language switch shortcut (e.g., Alt+Shift)
3. **See the notification** - A visual indicator will appear showing your current language
4. **Configure settings** - Double-click the tray icon to open settings

## ⚙️ Configuration

### General Settings

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

### Cursor Follower Settings (NEW!)

Enable cursor follower mode for a dynamic, mouse-tracking language indicator:

- **Enable Cursor Follower** - Toggle cursor-following mode on/off
- **Persistent Mode** - Keep the cursor follower always visible (or auto-hide)
- **Duration** - How long to show before auto-hiding (when not persistent)
- **Offset X** - Horizontal distance from cursor (pixels)
- **Offset Y** - Vertical distance from cursor (pixels)
- **Opacity** - Transparency level of the cursor follower

The cursor follower automatically:
- Scales font size based on your screen resolution
- Adjusts for DPI scaling on multi-monitor setups
- Updates position in real-time (10ms intervals)
- Recalculates dimensions when moving between monitors

### Appearance Settings

#### Application Theme
- **Enable Dark Mode** - Toggle between light and dark application themes

#### Notification Colors
- **Default Colors** - Set default background and text colors for all languages
- **Language-Specific Colors** - Assign unique colors to individual languages
  - Select a language from the dropdown or type a custom code
  - Choose your preferred colors using the color pickers
  - Click "Add Language with Current Colors"
  - Edit existing language configurations
  - Remove languages you no longer need

### Language Configuration Editor (NEW!)

Manage your configured languages with an intuitive interface:

1. **View configured languages** - See all languages with custom colors
2. **Add new languages** - Select from 35+ presets or add custom codes
3. **Edit existing languages** - Click "Edit" to modify colors for any configured language
4. **Remove languages** - Delete language configurations you no longer use
5. **Visual preview** - See color swatches for each configured language

### Behavior Settings

- **Start with Windows** - Launch automatically on system startup
- **Check for Updates** - Enable automatic update checking (daily)
- **Manual Update Check** - Check for updates on demand
- **Minimize to Tray** - Hide to system tray when minimized
- **Close to Tray** - Keep running in background when window is closed

## 🎨 Color Customization

You can customize colors in two ways:

### 1. Default Colors
Applied to all languages without specific settings:
- Select colors in the color pickers
- Click "Set as Default Colors"

### 2. Language-Specific Colors
Override defaults for specific languages:
- Select colors in the color pickers
- Choose a language from the dropdown
- Click "Add Language with Current Colors"
- The language will be added to your configured list
- Edit colors later using the "Edit" button

### Supported Languages

The app includes presets for 35+ common languages including:

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

## 🔧 Keyboard Shortcuts

- **Ctrl+Q** - Dismiss persistent notifications and cursor follower

## 📋 System Requirements

- **Operating System**: Windows 10 (1809 or later) / Windows 11
- **Framework**: .NET 10.0 Runtime (included)
- **Memory**: 50 MB RAM
- **Storage**: 20 MB available space
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

### Cursor follower not tracking properly

- Check that "Enable Cursor Follower" is toggled on
- Verify cursor follower opacity is not set to 0
- Try adjusting X/Y offset values
- Restart the application

### Colors not changing per language

- Make sure you've added the language in the Appearance tab
- Verify the language code matches what Windows uses
- Click "Save" after adding or editing language colors
- Test with the "Show Test Notification" button

### Cursor follower appears in wrong position

- Adjust the Offset X and Offset Y values in settings
- The follower appears above the cursor by default
- Positive X moves it right, positive Y moves it further up
- Values are in pixels and can be negative

### App not starting with Windows

- Re-enable "Start with Windows" in Behavior settings
- Check Windows Task Manager > Startup tab
- Ensure you have proper user permissions

## 💡 Tips & Best Practices

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
- Edit existing configurations to fine-tune your setup

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
- [ ] Custom notification sounds per language
- [ ] Animation effects for transitions
- [ ] Multiple notification style presets
- [ ] Command-line interface for power users
- [ ] Mouse badge indicator option
- [ ] Keyboard shortcut customization
- [ ] Export/import settings profiles

### Completed in v3.0.0
- [x] Cursor follower mode
- [x] Language configuration editor
- [x] Show on all monitors option
- [x] Show on cursor monitor option
- [x] Dynamic DPI scaling for cursor follower
- [x] Edit existing language configurations

## 📊 Changelog

### Version 3.0.0 (Current)
- Added cursor follower mode with real-time tracking
- Implemented language configuration editor
- Added multi-monitor display options (all monitors, cursor monitor)
- Improved DPI scaling across different monitors
- Enhanced language color management
- Performance optimizations for cursor tracking

### Version 2.0.0
- Multi-monitor support
- Customizable colors per language
- Automatic updates
- Dark mode fixes
- UI improvements

---

**Made with ❤️ by [tech-tonic-coder](https://github.com/tech-tonic-coder)**

⭐ If you find this project useful, please consider giving it a star!

## 🎯 Use Cases

- **Multilingual Writers** - Instantly know which language you're typing in
- **Translators** - Keep track of language switches during translation work
- **Developers** - Monitor input language when coding in different languages
- **Students** - Stay aware of language mode during language learning
- **Content Creators** - Ensure correct language input for multilingual content
- **Customer Support** - Handle multi-language support tickets efficiently
