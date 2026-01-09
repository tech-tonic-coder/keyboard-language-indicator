# Keyboard Language Indicator

![License](https://img.shields.io/badge/license-GPL--3.0-green.svg)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey.svg)

A lightweight utility that displays on-screen notifications when you switch keyboard input languages.

## ✨ Features

- **Real-time language change notifications** - Instantly see which language you've switched to
- **Multi-monitor support** - Choose which monitor displays the notification
- **Customizable colors per language** - Set unique colors for each language
- **Dark mode support** - Comfortable viewing in any lighting condition
- **System tray integration** - Runs quietly in the background
- **Automatic updates** - Stay up-to-date with the latest features
- **Persistent mode** - Keep notifications visible until dismissed with Ctrl+Q
- **Flexible positioning** - Place notifications anywhere on screen
- **Adjustable appearance** - Customize size, opacity, and duration

## 🚀 Getting Started

### Prerequisites

- Windows 10 or later
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

- **Display Monitor** - Select which monitor shows notifications (multi-monitor setups)
- **Notification Position** - Choose from 7 positions (corners, centers, middle)
- **Duration** - Set how long notifications stay visible (0.5-10 seconds)
- **Persistent Mode** - Keep notifications visible until you press Ctrl+Q
- **Font Size** - Adjust text size (12-48pt)
- **Opacity** - Control notification transparency (10-100%)

### Appearance Settings

- **Application Theme** - Toggle between light and dark modes
- **Default Colors** - Set default background and text colors
- **Language-Specific Colors** - Assign unique colors to individual languages
  - Select a language from the dropdown or type a custom code
  - Choose your preferred colors
  - Click "Add Language with Current Colors"

### Behavior Settings

- **Start with Windows** - Launch automatically on system startup
- **Check for Updates** - Enable automatic update checking
- **Minimize to Tray** - Hide to system tray when minimized
- **Close to Tray** - Keep running in background when closed

## 🎨 Color Customization

You can customize colors in two ways:

1. **Default Colors** - Applied to all languages without specific settings
   - Select colors in the color pickers
   - Click "Set as Default Colors"

2. **Language-Specific Colors** - Override defaults for specific languages
   - Select colors in the color pickers
   - Choose a language from the dropdown
   - Click "Add Language with Current Colors"
   - The language will be added to your configured list

### Supported Languages

The app includes presets for 35+ common languages including:
- English, Spanish, French, German, Italian, Portuguese
- Russian, Ukrainian, Polish, Czech, Hungarian
- Arabic, Persian, Hebrew, Turkish
- Chinese, Japanese, Korean, Thai, Vietnamese
- And many more...

You can also add custom language codes manually.

## 🔧 Keyboard Shortcuts

- **Ctrl+Q** - Dismiss persistent notifications
- **Double-click tray icon** - Open settings window

## 📋 System Requirements

- **Operating System**: Windows 10 (1809 or later) / Windows 11
- **Framework**: .NET 10.0 Runtime (included)
- **Memory**: 50 MB RAM
- **Storage**: 20 MB available space
- **Display**: Any resolution (optimized for multi-monitor setups)

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

### Colors not changing per language

- Make sure you've added the language in the Appearance tab
- Verify the language code matches what Windows uses
- Click "Save" after adding custom language colors

### App not starting with Windows

- Re-enable "Start with Windows" in Behavior settings
- Check Windows Task Manager > Startup tab
- Ensure you have proper user permissions

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

## 🗺️ Roadmap

- [ ] Custom notification sounds
- [ ] Animation effects
- [ ] Mouse badge indicator


---

**Made with ❤️ by [tech-tonic-coder](https://github.com/tech-tonic-coder)**

⭐ If you find this project useful, please consider giving it a star!
