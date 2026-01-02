namespace KeyboardLanguageIndicator.Services;

public static class LanguageList
{
    public record Language(string Code, string Name);

    public static readonly List<Language> Common = new()
    {
        new("FA", "Persian"),
        new("EN", "English"),
        new("ES", "Spanish"),
        new("FR", "French"),
        new("DE", "German"),
        new("IT", "Italian"),
        new("PT", "Portuguese"),
        new("RU", "Russian"),
        new("ZH", "Chinese"),
        new("JA", "Japanese"),
        new("KO", "Korean"),
        new("AR", "Arabic"),
        new("TR", "Turkish"),
        new("NL", "Dutch"),
        new("PL", "Polish"),
        new("SV", "Swedish"),
        new("DA", "Danish"),
        new("NO", "Norwegian"),
        new("FI", "Finnish"),
        new("CS", "Czech"),
        new("HU", "Hungarian"),
        new("RO", "Romanian"),
        new("EL", "Greek"),
        new("HE", "Hebrew"),
        new("TH", "Thai"),
        new("VI", "Vietnamese"),
        new("ID", "Indonesian"),
        new("UK", "Ukrainian"),
        new("BG", "Bulgarian"),
        new("HR", "Croatian"),
        new("SK", "Slovak"),
        new("SL", "Slovenian"),
        new("ET", "Estonian"),
        new("LV", "Latvian"),
        new("LT", "Lithuanian"),
    };

    public static string GetName(string code)
    {
        var lang = Common.FirstOrDefault(l =>
            l.Code.Equals(code, StringComparison.OrdinalIgnoreCase)
        );
        return lang?.Name ?? code;
    }

    public static string GetCode(string nameOrCode)
    {
        // Try to find by name first
        var byName = Common.FirstOrDefault(l =>
            l.Name.Equals(nameOrCode, StringComparison.OrdinalIgnoreCase)
        );
        if (byName != null)
            return byName.Code;

        // Try to find by code
        var byCode = Common.FirstOrDefault(l =>
            l.Code.Equals(nameOrCode, StringComparison.OrdinalIgnoreCase)
        );
        if (byCode != null)
            return byCode.Code;

        // Return as-is (uppercase)
        return nameOrCode.ToUpperInvariant();
    }
}
