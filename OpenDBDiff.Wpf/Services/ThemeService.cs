using System;
using System.Windows;
using Microsoft.Win32;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Wpf.Services
{
    /// <summary>
    /// Swaps the theme's token dictionary and asks the shell to show it.
    /// </summary>
    /// <remarks>
    /// Section 13.4 needs a full token set per theme. The swap replaces one merged dictionary rather than
    /// re-resolving resources in place, which is why the styles live in both token dictionaries: a
    /// StaticResource is resolved once, when the view that uses it is loaded, so a reference made under
    /// the light theme keeps pointing at the light brush however the dictionaries are rearranged
    /// afterwards. Re-creating the window is what makes every view resolve against the new dictionary,
    /// and it costs nothing that matters here because the state all lives in the view model.
    ///
    /// Reading the operating system's preference is the one thing here that cannot be tested without
    /// Windows, so it is isolated in one small method that treats anything unexpected as light.
    /// </remarks>
    public sealed class ThemeService : IThemeService
    {
        private const string PersonalizeKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        private readonly Action rebuild;

        public ThemeService(Action rebuild)
        {
            this.rebuild = rebuild;
        }

        public bool OperatingSystemIsDark
        {
            get { return OperatingSystemPrefersDark(); }
        }

        public void Apply(AppTheme mode)
        {
            var isDark = Theme.Resolve(mode, OperatingSystemIsDark) == Theme.Dark;

            Theme.Apply(mode, OperatingSystemIsDark);
            SwapDictionary(isDark ? "Tokens.Dark.xaml" : "Tokens.xaml");

            if (rebuild != null)
            {
                rebuild();
            }
        }

        /// <summary>
        /// Replaces the token dictionary in place, keeping its position so the dictionaries after it -
        /// which build on the tokens - still find what they expect.
        /// </summary>
        private static void SwapDictionary(string fileName)
        {
            var application = Application.Current;

            if (application == null)
            {
                return;
            }

            var dictionaries = application.Resources.MergedDictionaries;
            var wanted = new Uri("Themes/" + fileName, UriKind.Relative);

            for (var i = 0; i < dictionaries.Count; i++)
            {
                var source = dictionaries[i].Source;

                if (source == null || source.OriginalString.IndexOf("Tokens", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                dictionaries[i] = new ResourceDictionary { Source = wanted };
                return;
            }
        }

        private static bool OperatingSystemPrefersDark()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(PersonalizeKey))
                {
                    // AppsUseLightTheme is 0 when the user asked for dark, and absent on older systems.
                    var value = key == null ? null : key.GetValue("AppsUseLightTheme");

                    return value is int light && light == 0;
                }
            }
            catch (Exception ex) when (ex is System.Security.SecurityException || ex is System.IO.IOException || ex is UnauthorizedAccessException)
            {
                // A locked-down or non-Windows machine has no opinion, and light is the safe answer.
                return false;
            }
        }
    }
}
