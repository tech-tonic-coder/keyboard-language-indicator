using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace KeyboardLanguageIndicator;

public class InputLanguageMonitor
{
    private const int CHECK_INTERVAL_MS = 100;
    private DispatcherTimer? _timer;
    private string _currentLanguage = string.Empty;

    public event EventHandler<string>? LanguageChanged;

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);

    [DllImport("user32.dll")]
    private static extern IntPtr GetKeyboardLayout(uint idThread);

    public void Start()
    {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(CHECK_INTERVAL_MS) };
        _timer.Tick += (s, e) => CheckLanguage();
        _timer.Start();
        CheckLanguage();
    }

    public void Stop() => _timer?.Stop();

    private void CheckLanguage()
    {
        try
        {
            var layout = GetCurrentKeyboardLayout();
            var language = GetLanguageCode(layout);

            if (language != _currentLanguage && !string.IsNullOrEmpty(language))
            {
                _currentLanguage = language;
                LanguageChanged?.Invoke(this, language);
            }
        }
        catch { }
    }

    private static IntPtr GetCurrentKeyboardLayout()
    {
        var window = GetForegroundWindow();
        var threadId = GetWindowThreadProcessId(window, IntPtr.Zero);
        return GetKeyboardLayout(threadId);
    }

    private static string GetLanguageCode(IntPtr layout)
    {
        var layoutId = (int)layout & 0xFFFF;

        // Try CultureInfo first
        try
        {
            var culture = new CultureInfo(layoutId);
            return culture.TwoLetterISOLanguageName.ToUpperInvariant();
        }
        catch
        {
            // Fallback to manual mapping for known layouts
            var manualMapping = layoutId switch
            {
                // English variants
                0x0409 => "EN", // English (United States)
                0x0809 => "EN", // English (United Kingdom)
                0x0C09 => "EN", // English (Australia)
                0x1009 => "EN", // English (Canada)
                0x1409 => "EN", // English (New Zealand)
                0x1809 => "EN", // English (Ireland)
                0x1C09 => "EN", // English (South Africa)
                0x2009 => "EN", // English (Jamaica)
                0x2409 => "EN", // English (Caribbean)
                0x2809 => "EN", // English (Belize)
                0x2C09 => "EN", // English (Trinidad)
                0x3009 => "EN", // English (Zimbabwe)
                0x3409 => "EN", // English (Philippines)

                // Arabic variants
                0x0401 => "AR", // Arabic (Saudi Arabia)
                0x0801 => "AR", // Arabic (Iraq)
                0x0C01 => "AR", // Arabic (Egypt)
                0x1001 => "AR", // Arabic (Libya)
                0x1401 => "AR", // Arabic (Algeria)
                0x1801 => "AR", // Arabic (Morocco)
                0x1C01 => "AR", // Arabic (Tunisia)
                0x2001 => "AR", // Arabic (Oman)
                0x2401 => "AR", // Arabic (Yemen)
                0x2801 => "AR", // Arabic (Syria)
                0x2C01 => "AR", // Arabic (Jordan)
                0x3001 => "AR", // Arabic (Lebanon)
                0x3401 => "AR", // Arabic (Kuwait)
                0x3801 => "AR", // Arabic (UAE)
                0x3C01 => "AR", // Arabic (Bahrain)
                0x4001 => "AR", // Arabic (Qatar)

                // Spanish variants
                0x040A => "ES", // Spanish (Traditional Sort)
                0x080A => "ES", // Spanish (Mexico)
                0x0C0A => "ES", // Spanish (Modern Sort)
                0x100A => "ES", // Spanish (Guatemala)
                0x140A => "ES", // Spanish (Costa Rica)
                0x180A => "ES", // Spanish (Panama)
                0x1C0A => "ES", // Spanish (Dominican Republic)
                0x200A => "ES", // Spanish (Venezuela)
                0x240A => "ES", // Spanish (Colombia)
                0x280A => "ES", // Spanish (Peru)
                0x2C0A => "ES", // Spanish (Argentina)
                0x300A => "ES", // Spanish (Ecuador)
                0x340A => "ES", // Spanish (Chile)
                0x380A => "ES", // Spanish (Uruguay)
                0x3C0A => "ES", // Spanish (Paraguay)
                0x400A => "ES", // Spanish (Bolivia)
                0x440A => "ES", // Spanish (El Salvador)
                0x480A => "ES", // Spanish (Honduras)
                0x4C0A => "ES", // Spanish (Nicaragua)
                0x500A => "ES", // Spanish (Puerto Rico)

                // French variants
                0x040C => "FR", // French (France)
                0x080C => "FR", // French (Belgium)
                0x0C0C => "FR", // French (Canada)
                0x100C => "FR", // French (Switzerland)
                0x140C => "FR", // French (Luxembourg)
                0x180C => "FR", // French (Monaco)

                // German variants
                0x0407 => "DE", // German (Germany)
                0x0807 => "DE", // German (Switzerland)
                0x0C07 => "DE", // German (Austria)
                0x1007 => "DE", // German (Luxembourg)
                0x1407 => "DE", // German (Liechtenstein)

                // Portuguese variants
                0x0416 => "PT", // Portuguese (Brazil)
                0x0816 => "PT", // Portuguese (Portugal)

                // Italian variants
                0x0410 => "IT", // Italian (Italy)
                0x0810 => "IT", // Italian (Switzerland)

                // Chinese variants
                0x0404 => "ZH", // Chinese (Taiwan)
                0x0804 => "ZH", // Chinese (PRC)
                0x0C04 => "ZH", // Chinese (Hong Kong SAR)
                0x1004 => "ZH", // Chinese (Singapore)
                0x1404 => "ZH", // Chinese (Macau SAR)

                // Russian variants
                0x0419 => "RU", // Russian (Russia)
                0x0819 => "RU", // Russian (Moldova)

                // Japanese
                0x0411 => "JA", // Japanese

                // Korean
                0x0412 => "KO", // Korean

                // Turkish
                0x041F => "TR", // Turkish

                // Persian/Farsi
                0x0429 => "FA", // Persian

                // Dutch variants
                0x0413 => "NL", // Dutch (Netherlands)
                0x0813 => "NL", // Dutch (Belgium)

                // Polish
                0x0415 => "PL", // Polish

                // Swedish variants
                0x041D => "SV", // Swedish (Sweden)
                0x081D => "SV", // Swedish (Finland)

                // Danish
                0x0406 => "DA", // Danish

                // Norwegian variants
                0x0414 => "NO", // Norwegian (Bokmal)
                0x0814 => "NO", // Norwegian (Nynorsk)

                // Finnish
                0x040B => "FI", // Finnish

                // Czech
                0x0405 => "CS", // Czech

                // Hungarian
                0x040E => "HU", // Hungarian

                // Romanian variants
                0x0418 => "RO", // Romanian (Romania)
                0x0818 => "RO", // Romanian (Moldova)

                // Greek
                0x0408 => "EL", // Greek

                // Hebrew
                0x040D => "HE", // Hebrew

                // Thai
                0x041E => "TH", // Thai

                // Vietnamese
                0x042A => "VI", // Vietnamese

                // Indonesian
                0x0421 => "ID", // Indonesian

                // Ukrainian
                0x0422 => "UK", // Ukrainian

                // Bulgarian
                0x0402 => "BG", // Bulgarian

                // Croatian
                0x041A => "HR", // Croatian
                0x101A => "HR", // Croatian (Bosnia and Herzegovina)

                // Slovak
                0x041B => "SK", // Slovak

                // Slovenian
                0x0424 => "SL", // Slovenian

                // Estonian
                0x0425 => "ET", // Estonian

                // Latvian
                0x0426 => "LV", // Latvian

                // Lithuanian
                0x0427 => "LT", // Lithuanian

                // Serbian variants
                0x0C1A => "SR", // Serbian (Cyrillic)
                0x081A => "SR", // Serbian (Latin)
                0x181A => "SR", // Serbian (Bosnia and Herzegovina - Cyrillic)
                0x241A => "SR", // Serbian (Bosnia and Herzegovina - Latin)

                // Bosnian
                0x141A => "BS", // Bosnian (Latin)
                0x201A => "BS", // Bosnian (Cyrillic)

                // Macedonian
                0x042F => "MK", // Macedonian

                // Albanian
                0x041C => "SQ", // Albanian

                // Belarusian
                0x0423 => "BE", // Belarusian

                // Kazakh
                0x043F => "KK", // Kazakh

                // Uzbek variants
                0x0443 => "UZ", // Uzbek (Latin)
                0x0843 => "UZ", // Uzbek (Cyrillic)

                // Azerbaijani variants
                0x042C => "AZ", // Azerbaijani (Latin)
                0x082C => "AZ", // Azerbaijani (Cyrillic)

                // Georgian
                0x0437 => "KA", // Georgian

                // Armenian
                0x042B => "HY", // Armenian

                // Catalan
                0x0403 => "CA", // Catalan

                // Basque
                0x042D => "EU", // Basque

                // Galician
                0x0456 => "GL", // Galician

                // Icelandic
                0x040F => "IS", // Icelandic

                // Maltese
                0x043A => "MT", // Maltese

                // Hindi
                0x0439 => "HI", // Hindi

                // Bengali variants
                0x0445 => "BN", // Bengali (India)
                0x0845 => "BN", // Bengali (Bangladesh)

                // Punjabi
                0x0446 => "PA", // Punjabi (India)

                // Tamil
                0x0449 => "TA", // Tamil

                // Telugu
                0x044A => "TE", // Telugu

                // Kannada
                0x044B => "KN", // Kannada

                // Malayalam
                0x044C => "ML", // Malayalam

                // Marathi
                0x044E => "MR", // Marathi

                // Sanskrit
                0x044F => "SA", // Sanskrit

                // Urdu
                0x0420 => "UR", // Urdu

                // Malay variants
                0x043E => "MS", // Malay (Malaysia)
                0x083E => "MS", // Malay (Brunei Darussalam)

                // Swahili
                0x0441 => "SW", // Swahili

                // Afrikaans
                0x0436 => "AF", // Afrikaans

                _ => null,
            };

            if (manualMapping != null)
                return manualMapping;

            // For completely unknown layouts, try to extract language from upper word
            // Windows sometimes stores the language in the upper 16 bits
            var primaryLangId = layoutId & 0x3FF; // Extract primary language ID
            if (primaryLangId > 0)
            {
                try
                {
                    var culture = new CultureInfo(primaryLangId);
                    return culture.TwoLetterISOLanguageName.ToUpperInvariant();
                }
                catch { }
            }

            // Last resort: return "??" instead of hex code to indicate unknown layout
            return "??";
        }
    }
}
