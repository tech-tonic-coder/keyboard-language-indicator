using System.Text;

namespace KeyboardLanguageIndicator.Services
{
    public static class TextConversionService
    {
        private static readonly object _lock = new object();
        private static Lazy<Dictionary<string, IReadOnlyDictionary<char, char>>> _lazyMaps =
            new Lazy<Dictionary<string, IReadOnlyDictionary<char, char>>>(InitializeMaps, true);

        private static Dictionary<string, IReadOnlyDictionary<char, char>> Maps => _lazyMaps.Value;

        // =========================
        // UNICODE NORMALIZATION
        // =========================
        private static char NormalizeChar(char ch) =>
            ch switch
            {
                '\u064A' => '\u06CC', // Arabic Yeh → Persian Yeh
                '\u0643' => '\u06A9', // Arabic Kaf → Persian Kaf
                _ => ch,
            };

        // =========================
        // INITIALIZE MAPS
        // =========================
        private static Dictionary<string, IReadOnlyDictionary<char, char>> InitializeMaps()
        {
            var maps = new Dictionary<string, IReadOnlyDictionary<char, char>>(
                StringComparer.OrdinalIgnoreCase
            );

            // -------------------------
            // PERSIAN STANDARD MAP (EN ↔ FA) - Updated from Excel (Persian Standard Keyboard Layout)
            // -------------------------
            var enToFaStd = new Dictionary<char, char>
            {
                // Numbers row - Normal level
                ['`'] = '\u200D', // ‍ Zero Width Joiner (ZWJ)
                ['1'] = '\u06F1', // ۱ Persian digit one
                ['2'] = '\u06F2', // ۲ Persian digit two
                ['3'] = '\u06F3', // ۳ Persian digit three
                ['4'] = '\u06F4', // ۴ Persian digit four
                ['5'] = '\u06F5', // ۵ Persian digit five
                ['6'] = '\u06F6', // ۶ Persian digit six
                ['7'] = '\u06F7', // ۷ Persian digit seven
                ['8'] = '\u06F8', // ۸ Persian digit eight
                ['9'] = '\u06F9', // ۹ Persian digit nine
                ['0'] = '\u06F0', // ۰ Persian digit zero
                ['-'] = '\u002D', // - Hyphen-minus
                ['='] = '\u003D', // = Equals sign

                // Numbers row - Shift level
                ['~'] = '\u00F7', // ÷ Division sign
                ['!'] = '\u0021', // ! Exclamation mark
                ['@'] = '\u066C', // ٬ Arabic thousands separator
                ['#'] = '\u066B', // ٫ Arabic decimal separator
                ['$'] = '\uFDFC', // ﷼ Rial sign
                ['%'] = '\u066A', // ٪ Arabic percent sign
                ['^'] = '\u00D7', // × Multiplication sign
                ['&'] = '\u060C', // ، Arabic comma
                ['*'] = '\u002A', // * Asterisk
                ['('] = '\u0029', // ) Right parenthesis (swapped in Persian)
                [')'] = '\u0028', // ( Left parenthesis (swapped in Persian)
                ['_'] = '\u0640', // ـ Arabic tatweel (kashida)
                ['+'] = '\u002B', // + Plus sign

                // QWERTY row - lowercase (normal level)
                ['q'] = '\u0636', // ض Dad
                ['w'] = '\u0635', // ص Sad
                ['e'] = '\u062B', // ث Theh
                ['r'] = '\u0642', // ق Qaf
                ['t'] = '\u0641', // ف Feh
                ['y'] = '\u063A', // غ Ghain
                ['u'] = '\u0639', // ع Ain
                ['i'] = '\u0647', // ه Heh
                ['o'] = '\u062E', // خ Khah
                ['p'] = '\u062D', // ح Hah
                ['['] = '\u062C', // ج Jeem
                [']'] = '\u0686', // چ Tcheh (Persian letter)

                // QWERTY row - uppercase (shift level)
                ['Q'] = '\u0652', // ْ Sukun (diacritic - no vowel)
                ['W'] = '\u064C', // ٌ Tanween damm
                ['E'] = '\u064D', // ٍ Tanween kasr
                ['R'] = '\u064B', // ً Tanween fath
                ['T'] = '\u064F', // ُ Dammah (short u vowel)
                ['Y'] = '\u0650', // ِ Kasrah (short i vowel)
                ['U'] = '\u064E', // َ Fathah (short a vowel)
                ['I'] = '\u0651', // ّ Shaddah (consonant doubling)
                ['O'] = '\u005D', // ] Right square bracket
                ['P'] = '\u005B', // [ Left square bracket
                ['{'] = '\u007D', // } Right curly brace
                ['}'] = '\u007B', // { Left curly brace

                // ASDF row - lowercase (normal level)
                ['a'] = '\u0634', // ش Sheen
                ['s'] = '\u0633', // س Seen
                ['d'] = '\u06CC', // ی Persian Yeh (Farsi Yeh)
                ['f'] = '\u0628', // ب Beh
                ['g'] = '\u0644', // ل Lam
                ['h'] = '\u0627', // ا Alef
                ['j'] = '\u062A', // ت Teh
                ['k'] = '\u0646', // ن Noon
                ['l'] = '\u0645', // م Meem
                [';'] = '\u06A9', // ک Persian Kaf (Keheh)
                ['\''] = '\u06AF', // گ Gaf (Persian letter)

                // ASDF row - uppercase (shift level)
                ['A'] = '\u0624', // ؤ Waw with hamza above
                ['S'] = '\u0626', // ئ Yeh with hamza above
                ['D'] = '\u064A', // ي Arabic Yeh
                ['F'] = '\u0625', // إ Alef with hamza below
                ['G'] = '\u0623', // أ Alef with hamza above
                ['H'] = '\u0622', // آ Alef with madda above
                ['J'] = '\u0629', // ة Teh marbuta
                ['K'] = '\u00BB', // » Right-pointing double angle quotation mark
                ['L'] = '\u00AB', // « Left-pointing double angle quotation mark
                [':'] = '\u003A', // : Colon
                ['"'] = '\u061B', // ؛ Arabic semicolon

                // ZXCV row - lowercase (normal level)
                ['z'] = '\u0638', // ظ Zah
                ['x'] = '\u0637', // ط Tah
                ['c'] = '\u0632', // ز Zain
                ['v'] = '\u0631', // ر Reh
                ['b'] = '\u0630', // ذ Thal
                ['n'] = '\u062F', // د Dal
                ['m'] = '\u067E', // پ Peh (Persian letter)
                [','] = '\u0648', // و Waw
                ['.'] = '\u002E', // . Period/full stop
                ['/'] = '\u002F', // / Slash

                // ZXCV row - uppercase (shift level)
                ['Z'] = '\u0643', // ك Arabic Kaf
                ['X'] = '\u0653', // ٓ Maddah above
                ['C'] = '\u0698', // ژ Jeh (Persian letter)
                ['V'] = '\u0670', // ٰ Superscript alef
                ['B'] = '\u200C', // ‌ Zero Width Non-Joiner (ZWNJ)
                ['N'] = '\u0654', // ٔ Hamza above
                ['M'] = '\u0621', // ء Hamza
                ['<'] = '\u003E', // > Greater-than sign
                ['>'] = '\u003C', // < Less-than sign
                ['?'] = '\u061F', // ؟ Arabic question mark

                // Backslash key
                ['\\'] = '\u005C', // \ Backslash
                ['|'] = '\u007C', // | Vertical bar

                // Space
                [' '] = '\u0020', // Space (normal space)
            };
            maps["EN_FA_STD"] = enToFaStd;
            maps["FA_EN_STD"] = ReverseDictionary(enToFaStd);

            // FA Legacy / non-standard (alternative layout where backslash maps to ژ)
            var enToFaLeg = new Dictionary<char, char>(enToFaStd)
            {
                ['\\'] = '\u0698', // ژ Jeh (in legacy layout, backslash gives ژ directly)
            };
            maps["EN_FA_LEG"] = enToFaLeg;
            maps["FA_EN_LEG"] = ReverseDictionary(enToFaLeg);

            // -------------------------
            // ARABIC
            // -------------------------
            var enToAr = new Dictionary<char, char>();
            MapChars(
                enToAr,
                "1234567890-=",
                "\u0661\u0662\u0663\u0664\u0665\u0666\u0667\u0668\u0669\u0660-="
            );
            MapChars(enToAr, "qwertyuiop[]\\", "ضصثقفغعهخحجد\\");
            MapChars(enToAr, "asdfghjkl;'", "شسيبلاتنمكط");
            MapChars(enToAr, "zxcvbnm,./", "ئءؤرىةوز,.");
            enToAr[' '] = ' ';
            enToAr['`'] = 'ذ';
            enToAr['~'] = 'ّ';
            maps["EN_AR"] = enToAr;
            maps["AR_EN"] = ReverseDictionary(enToAr);

            // -------------------------
            // TURKISH
            // -------------------------
            var enToTr = new Dictionary<char, char>();
            MapChars(enToTr, "abcdefghijklmnopqrstuvwxyz", "abcçdefgğhıijklmnoöprsştuüvyz");
            MapChars(enToTr, "ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ABCÇDEFGĞHIİJKLMNOÖPRSŞTUÜVYZ");
            MapChars(enToTr, "1234567890-=", "1234567890-=");
            enToTr[' '] = ' ';
            maps["EN_TR"] = enToTr;
            maps["TR_EN"] = ReverseDictionary(enToTr);

            // -------------------------
            // HEBREW
            // -------------------------
            var enToHe = new Dictionary<char, char>();
            MapChars(enToHe, "qwertyuiop", "'קראטוןםפ");
            MapChars(enToHe, "asdfghjkl", "שדגכעיחל");
            MapChars(enToHe, "zxcvbnm", "זסבנהצ");
            MapChars(enToHe, "QWERTYUIOPASDFGHJKLZXCVBNM", "'קראטוןםפשדגכעיחלזסבנהצ");
            MapChars(enToHe, "1234567890-=", "1234567890-=");
            enToHe[' '] = ' ';
            maps["EN_HE"] = enToHe;
            maps["HE_EN"] = ReverseDictionary(enToHe);

            // -------------------------
            // RUSSIAN
            // -------------------------
            var enToRu = new Dictionary<char, char>();
            MapChars(enToRu, "qwertyuiop[]\\", "йцукенгшщзхъ");
            MapChars(enToRu, "asdfghjkl;'", "фывапролджэ");
            MapChars(enToRu, "zxcvbnm,./", "ячсмитьбю.");
            MapChars(enToRu, "QWERTYUIOPASDFGHJKLZXCVBNM", "ЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮ");
            MapChars(enToRu, "1234567890-=", "1234567890-=");
            enToRu[' '] = ' ';
            maps["EN_RU"] = enToRu;
            maps["RU_EN"] = ReverseDictionary(enToRu);

            return maps;
        }

        private static void MapChars(Dictionary<char, char> dict, string source, string target)
        {
            int length = Math.Min(source.Length, target.Length);
            for (int i = 0; i < length; i++)
                dict[source[i]] = target[i];
        }

        private static IReadOnlyDictionary<char, char> ReverseDictionary(
            Dictionary<char, char> source
        )
        {
            var reversed = new Dictionary<char, char>(source.Count);
            foreach (var kvp in source)
            {
                if (!reversed.ContainsKey(kvp.Value))
                    reversed[kvp.Value] = kvp.Key;
            }
            return reversed;
        }

        // =========================
        // TEXT CONVERSION
        // =========================
        public static string ConvertText(string text, string fromLang, string toLang)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            string from = fromLang.ToUpperInvariant();
            string to = toLang.ToUpperInvariant();

            string suffix = string.Empty;
            if (from == "FA" || to == "FA")
                suffix = DetectPersianLayout(text) ? "_STD" : "_LEG";

            string key = $"{from}_{to}{suffix}";

            if (!Maps.TryGetValue(key, out var map))
                return text;

            // Only normalize when converting FROM English TO Persian
            // Don't normalize when converting FROM Persian, as it breaks Shift key mappings
            bool shouldNormalize = from == "EN" && to == "FA";

            var sb = new StringBuilder(text.Length);
            foreach (char raw in text)
            {
                char ch = shouldNormalize ? NormalizeChar(raw) : raw;
                sb.Append(map.TryGetValue(ch, out char mapped) ? mapped : ch);
            }
            return sb.ToString();
        }

        public static bool DetectPersianLayout(string text)
        {
            if (string.IsNullOrEmpty(text))
                return true;
            // Check for diacritics (tanween, vowels, etc.) - these indicate standard layout
            if (text.Any(c => c >= '\u064B' && c <= '\u0652'))
                return true;
            // Check for ژ (Jeh) - if present without shift context, likely legacy layout
            if (text.Contains('\u0698'))
                return false;
            return true;
        }

        public static bool HasMapping(string from, string to)
        {
            from = from.ToUpperInvariant();
            to = to.ToUpperInvariant();
            return Maps.ContainsKey($"{from}_{to}")
                || Maps.ContainsKey($"{from}_{to}_STD")
                || Maps.ContainsKey($"{from}_{to}_LEG");
        }

        public static IReadOnlyList<string> GetSupportedLanguages() =>
            new[] { "EN", "FA", "AR", "TR", "HE", "RU" };

        public static void ClearCache()
        {
            lock (_lock)
            {
                if (_lazyMaps.IsValueCreated)
                    Maps.Clear();
                _lazyMaps = new Lazy<Dictionary<string, IReadOnlyDictionary<char, char>>>(
                    InitializeMaps,
                    true
                );
            }
        }
    }
}
