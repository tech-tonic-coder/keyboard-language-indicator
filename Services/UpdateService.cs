using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KeyboardLanguageIndicator.Services;

public class UpdateService
{
    private const string GITHUB_API_URL =
        "https://api.github.com/repos/tech-tonic-coder/keyboard-language-indicator/releases/latest";
    private const string RELEASES_PAGE =
        "https://github.com/tech-tonic-coder/keyboard-language-indicator/releases";

    private static readonly HttpClient _httpClient = new()
    {
        DefaultRequestHeaders = { { "User-Agent", "KeyboardLanguageIndicator" } },
    };

    public record UpdateInfo(
        string Version,
        string DownloadUrl,
        string ReleaseNotesUrl,
        string Body,
        bool IsNewerVersion
    );

    private class GitHubRelease
    {
        [JsonPropertyName("tag_name")]
        public string TagName { get; set; } = "";

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = "";

        [JsonPropertyName("body")]
        public string Body { get; set; } = "";

        [JsonPropertyName("assets")]
        public List<GitHubAsset> Assets { get; set; } = new();
    }

    private class GitHubAsset
    {
        [JsonPropertyName("browser_download_url")]
        public string BrowserDownloadUrl { get; set; } = "";

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
    }

    public static string GetCurrentVersion()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;

        return version != null ? $"{version.Major}.{version.Minor}.{version.Build}" : "1.0.0";
    }

    public async Task<UpdateInfo?> CheckForUpdatesAsync()
    {
        try
        {
            var response = await _httpClient.GetStringAsync(GITHUB_API_URL);
            var release = JsonSerializer.Deserialize<GitHubRelease>(response);

            if (release == null)
                return null;

            var latestVersion = release.TagName.TrimStart('v');
            var currentVersion = GetCurrentVersion();
            var isNewer = IsNewerVersion(currentVersion, latestVersion);

            // Find .exe asset
            var exeAsset = release.Assets.FirstOrDefault(a =>
                a.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
            );

            return new UpdateInfo(
                latestVersion,
                exeAsset?.BrowserDownloadUrl ?? release.HtmlUrl,
                release.HtmlUrl,
                release.Body,
                isNewer
            );
        }
        catch
        {
            return null; // Network error or API issue
        }
    }

    private static bool IsNewerVersion(string current, string latest)
    {
        try
        {
            var currentParts = current.Split('.').Select(int.Parse).ToArray();
            var latestParts = latest.Split('.').Select(int.Parse).ToArray();

            for (int i = 0; i < Math.Min(currentParts.Length, latestParts.Length); i++)
            {
                if (latestParts[i] > currentParts[i])
                    return true;
                if (latestParts[i] < currentParts[i])
                    return false;
            }

            return latestParts.Length > currentParts.Length;
        }
        catch
        {
            return false;
        }
    }

    public static void OpenReleasesPage()
    {
        try
        {
            Process.Start(
                new ProcessStartInfo { FileName = RELEASES_PAGE, UseShellExecute = true }
            );
        }
        catch { }
    }

    public static void OpenDownloadUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }
        catch { }
    }
}
