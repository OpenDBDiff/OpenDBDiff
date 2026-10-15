using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Ui.Model
{
    /// <summary>
    /// Section 13.4's dark theme: the same token set, the same contrast thresholds, and diff tints between
    /// 22% and 30% alpha over the dark surface.
    /// </summary>
    /// <remarks>
    /// The dark dictionary is generated from the light one, which is what keeps the styles identical, but
    /// generation is exactly the kind of step that drops a key the day someone adds a colour. The first
    /// test is therefore key parity between the two dictionaries, read as files because the test project
    /// does not reference the WPF assembly the way a themed window would.
    /// </remarks>
    [TestClass]
    public class DarkThemeTests
    {
        private const double Minimum = 4.5;

        private const double MinimumTintAlpha = 0.22;

        private const double MaximumTintAlpha = 0.30;

        /// <summary>The colours a person reads in the dark theme, checked the way the light ones are.</summary>
        [TestMethod]
        public void TheDarkPaletteMeetsTheContrastThreshold()
        {
            var dark = Theme.Dark;

            AssertAtLeast("text on surface", dark.Text, dark.Surface);
            AssertAtLeast("muted text on surface", dark.TextMuted, dark.Surface);
            AssertAtLeast("faint text on surface", dark.TextFaint, dark.Surface);
            AssertAtLeast("accent-fg on accent-bg", dark.AccentForeground, dark.AccentBackground);
            AssertAtLeast("Create badge", dark.CreateForeground, dark.CreateBackground);
            AssertAtLeast("Alter badge", dark.AlterForeground, dark.AlterBackground);
            AssertAtLeast("Drop badge", dark.DropForeground, dark.DropBackground);
            AssertAtLeast("warning", dark.WarningForeground, dark.WarningBackground);
            AssertAtLeast("white on danger", "#FFFFFF", dark.Danger);

            // Section 6.3's highlighted search match: the text inside the highlight has to stay readable,
            // and the highlight has to be distinguishable from the row it sits in.
            AssertAtLeast("text on the search highlight", dark.Text, dark.Highlight);
            Assert.IsTrue(Contrast(dark.Highlight, dark.Surface) >= 2.0,
                "the search highlight is "
                    + Contrast(dark.Highlight, dark.Surface).ToString("0.00")
                    + ":1 against the row, too close to see");
            AssertAtLeast("Dev tag", dark.EnvironmentDevForeground, dark.EnvironmentDevBackground);
            AssertAtLeast("Test tag", dark.EnvironmentTestForeground, dark.EnvironmentTestBackground);
            AssertAtLeast("UAT tag", dark.EnvironmentUatForeground, dark.EnvironmentUatBackground);
            AssertAtLeast("Prod tag", dark.EnvironmentProductionForeground, dark.EnvironmentProductionBackground);
            AssertAtLeast("custom tag", dark.EnvironmentCustomForeground, dark.EnvironmentCustomBackground);
        }

        /// <summary>
        /// Section 13.4 is specific about the diff tints: 22-30% alpha over the dark surface, not opaque
        /// pastels. The body text has to stay readable on whatever those tints composite to, which is the
        /// part that a glance at the hex values cannot tell you.
        /// </summary>
        [TestMethod]
        public void TheDarkDiffTintsAreTranslucentAndStillReadable()
        {
            var dark = Theme.Dark;

            foreach (var pair in new[]
            {
                new KeyValuePair<string, string>("added", dark.DiffCreate),
                new KeyValuePair<string, string>("removed", dark.DiffDrop),
                new KeyValuePair<string, string>("changed", dark.DiffAlter)
            })
            {
                var alpha = Alpha(pair.Value);

                Assert.IsTrue(alpha >= MinimumTintAlpha && alpha <= MaximumTintAlpha,
                    "the " + pair.Key + " diff tint is " + (alpha * 100).ToString("0") + "% opaque, outside the 22-30% band");

                var over = Composite(pair.Value, dark.Surface);

                Assert.IsTrue(Contrast(dark.Text, over) >= Minimum,
                    "the body text on the " + pair.Key + " tint is "
                        + Contrast(dark.Text, over).ToString("0.00") + ":1, below the threshold");
            }
        }

        /// <summary>The changed-word tints sit on top of the line's tint, so both washes are composed.</summary>
        [TestMethod]
        public void TheDarkWordTintsStayReadableOverTheirLineTint()
        {
            var dark = Theme.Dark;

            var overLine = Composite(dark.DiffCreate, dark.Surface);
            var overWord = Composite(dark.WordCreate, overLine);

            Assert.IsTrue(Contrast(dark.Text, overWord) >= Minimum,
                "a changed word is " + Contrast(dark.Text, overWord).ToString("0.00") + ":1");
        }

        /// <summary>
        /// Both token dictionaries must define the same keys, or a view that reads one of them throws
        /// XamlParseException the moment the theme is switched - the failure mode that only appears on the
        /// machine of the user who happens to pick Dark.
        /// </summary>
        [TestMethod]
        public void BothTokenDictionariesDefineTheSameKeys()
        {
            var light = KeysOf("Tokens.xaml");
            var dark = KeysOf("Tokens.Dark.xaml");

            Assert.IsTrue(light.Count > 40, "the light dictionary should define the full token set");

            var missing = light.Except(dark).OrderBy(key => key, StringComparer.Ordinal).ToList();
            var extra = dark.Except(light).OrderBy(key => key, StringComparer.Ordinal).ToList();

            Assert.AreEqual(0, missing.Count,
                "these keys are in Tokens.xaml but not Tokens.Dark.xaml, so they would be unresolved in dark: "
                    + string.Join(", ", missing));

            Assert.AreEqual(0, extra.Count,
                "these keys are in Tokens.Dark.xaml but not Tokens.xaml: " + string.Join(", ", extra));
        }

        [TestMethod]
        public void SystemFollowsTheOperatingSystemAndTheOtherTwoDoNot()
        {
            Assert.AreSame(Theme.Dark, Theme.Resolve(AppTheme.System, true));
            Assert.AreSame(Theme.Light, Theme.Resolve(AppTheme.System, false));

            Assert.AreSame(Theme.Dark, Theme.Resolve(AppTheme.Dark, false));
            Assert.AreSame(Theme.Light, Theme.Resolve(AppTheme.Light, true));
        }

        /// <summary>
        /// The palette the converters read follows the theme, and the light values are unchanged when it
        /// goes back - which is what the light contrast tests depend on.
        /// </summary>
        [TestMethod]
        public void ApplyingAThemeChangesThePaletteTheConvertersRead()
        {
            var lightText = StatusPalette.Text;
            var lightDiff = StatusPalette.DiffBackground(DiffLineKind.Added);

            try
            {
                Theme.Apply(AppTheme.Dark, false);

                Assert.AreNotEqual(lightText, StatusPalette.Text);
                Assert.AreEqual(Theme.Dark.CreateForeground, StatusPalette.StatusForeground(DiffStatus.Create));
                Assert.AreEqual(Theme.Dark.DiffCreate, StatusPalette.DiffBackground(DiffLineKind.Added));
                Assert.AreEqual(Theme.Dark.EnvironmentProductionForeground, EnvironmentPalette.ProdForeground);

                // An unchanged line is drawn on nothing, whatever the theme is.
                Assert.IsNull(StatusPalette.DiffBackground(DiffLineKind.Unchanged));
            }
            finally
            {
                Theme.Apply(AppTheme.Light, false);
            }

            Assert.AreEqual(lightText, StatusPalette.Text);
            Assert.AreEqual(lightDiff, StatusPalette.DiffBackground(DiffLineKind.Added));
        }

        private static IReadOnlyList<string> KeysOf(string fileName)
        {
            var path = Path.Combine(ThemesDirectory(), fileName);
            var text = File.ReadAllText(path);

            return Regex.Matches(text, "x:Key=\"([^\"]+)\"")
                .Cast<Match>()
                .Select(match => match.Groups[1].Value)
                .ToList();
        }

        private static string ThemesDirectory()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory != null)
            {
                var candidate = Path.Combine(directory.FullName, "OpenDBDiff.Wpf", "Themes");

                if (Directory.Exists(candidate))
                {
                    return candidate;
                }

                directory = directory.Parent;
            }

            throw new DirectoryNotFoundException("Could not find OpenDBDiff.Wpf\\Themes above " + AppContext.BaseDirectory);
        }

        /// <summary>The alpha of #AARRGGBB, or 1 when the value is fully opaque.</summary>
        private static double Alpha(string hex)
        {
            var digits = hex.TrimStart('#');

            return digits.Length == 8
                ? int.Parse(digits.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture) / 255.0
                : 1.0;
        }

        /// <summary>
        /// What WPF draws when a translucent brush is painted over an opaque one. The shell does not do
        /// this arithmetic - the compositor does - but the contrast of the result cannot be checked without
        /// it.
        /// </summary>
        private static string Composite(string over, string under)
        {
            var alpha = Alpha(over);
            var top = Channels(over);
            var bottom = Channels(under);

            var mixed = new int[3];

            for (var i = 0; i < 3; i++)
            {
                mixed[i] = (int)Math.Round((alpha * top[i]) + ((1 - alpha) * bottom[i]));
            }

            return string.Format(CultureInfo.InvariantCulture, "#{0:X2}{1:X2}{2:X2}", mixed[0], mixed[1], mixed[2]);
        }

        private static int[] Channels(string hex)
        {
            var digits = hex.TrimStart('#');
            var start = digits.Length == 8 ? 2 : 0;

            return new[]
            {
                int.Parse(digits.Substring(start, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture),
                int.Parse(digits.Substring(start + 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture),
                int.Parse(digits.Substring(start + 4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture)
            };
        }

        private static void AssertAtLeast(string name, string foreground, string background)
        {
            var ratio = Contrast(foreground, background);

            Assert.IsTrue(ratio >= Minimum,
                name + " is " + ratio.ToString("0.00") + ":1, below the " + Minimum.ToString("0.0") + ":1 threshold");
        }

        /// <summary>WCAG 2.x relative luminance and contrast ratio.</summary>
        private static double Contrast(string foreground, string background)
        {
            var lighter = Luminance(Channels(foreground));
            var darker = Luminance(Channels(background));

            if (lighter < darker)
            {
                var swap = lighter;
                lighter = darker;
                darker = swap;
            }

            return (lighter + 0.05) / (darker + 0.05);
        }

        private static double Luminance(int[] channels)
        {
            var parts = new double[3];

            for (var i = 0; i < 3; i++)
            {
                var value = channels[i] / 255.0;

                parts[i] = value <= 0.03928 ? value / 12.92 : Math.Pow((value + 0.055) / 1.055, 2.4);
            }

            return (0.2126 * parts[0]) + (0.7152 * parts[1]) + (0.0722 * parts[2]);
        }
    }
}
