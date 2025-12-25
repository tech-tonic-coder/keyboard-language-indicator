using System.Windows;
using HandyControl.Data;
using HandyControl.Tools;

namespace KeyboardLanguageIndicator
{
    public partial class App : Application
    {
        public static bool StartMinimized { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args != null && e.Args.Length > 0)
            {
                StartMinimized = e.Args.Contains("--minimized");
            }

            ConfigHelper.Instance.SetLang("en");

            System.Threading.Thread.CurrentThread.CurrentCulture =
                new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentUICulture =
                new System.Globalization.CultureInfo("en-US");
        }

        public void SetTheme(bool isDark)
        {
            var skinType = isDark ? SkinType.Dark : SkinType.Default;
            var skinDict = ResourceHelper.GetSkin(skinType);

            var oldSkin = Resources.MergedDictionaries.FirstOrDefault(d =>
                d.Source != null && d.Source.OriginalString.Contains("Skin")
            );

            if (oldSkin != null)
            {
                int index = Resources.MergedDictionaries.IndexOf(oldSkin);
                Resources.MergedDictionaries[index] = skinDict;
            }
            else
            {
                Resources.MergedDictionaries.Add(skinDict);
            }
        }
    }
}
