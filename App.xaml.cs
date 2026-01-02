using System.Windows;
using HandyControl.Data;
using HandyControl.Themes;
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
            var skin = isDark ? SkinType.Dark : SkinType.Default;

            SharedResourceDictionary.SharedDictionaries.Clear();

            Resources.MergedDictionaries.Clear();

            Resources.MergedDictionaries.Add(ResourceHelper.GetSkin(skin));

            Resources.MergedDictionaries.Add(
                new ResourceDictionary
                {
                    Source = new Uri(
                        "pack://application:,,,/HandyControl;component/Themes/Theme.xaml"
                    ),
                }
            );
        }
    }
}
