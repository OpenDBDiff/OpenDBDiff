using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Tests.Ui.Model
{
    /// <summary>
    /// Section 13.2 requires every foreground/background pair to be confirmed with a contrast tool, and
    /// section 6.1.4 puts the same requirement on the environment tag colours. This computes the WCAG
    /// 2.x relative-luminance ratio so the palette is checked rather than asserted.
    /// </summary>
    [TestClass]
    public class ColourContrastTests
    {
        private const double Minimum = 4.5;

        [TestMethod]
        public void TheEnvironmentTagsMeetTheContrastThreshold()
        {
            AssertAtLeast("Dev", EnvironmentPalette.DevForeground, EnvironmentPalette.DevBackground);
            AssertAtLeast("Test", EnvironmentPalette.TestForeground, EnvironmentPalette.TestBackground);
            AssertAtLeast("UAT", EnvironmentPalette.UatForeground, EnvironmentPalette.UatBackground);
            AssertAtLeast("Prod", EnvironmentPalette.ProdForeground, EnvironmentPalette.ProdBackground);
            AssertAtLeast("Custom", EnvironmentPalette.CustomForeground, EnvironmentPalette.CustomBackground);
        }

        [TestMethod]
        public void TheBodyAndStatusTokensMeetTheContrastThreshold()
        {
            AssertAtLeast("text on surface", StatusPalette.Text, StatusPalette.Surface);
            AssertAtLeast("text-muted on surface", StatusPalette.TextMuted, StatusPalette.Surface);
            AssertAtLeast("accent-fg on accent-bg", StatusPalette.AccentForeground, StatusPalette.AccentBackground);
            AssertAtLeast("Create", StatusPalette.StatusForeground(DiffStatus.Create), StatusPalette.StatusBackground(DiffStatus.Create));
            AssertAtLeast("Alter", StatusPalette.StatusForeground(DiffStatus.Alter), StatusPalette.StatusBackground(DiffStatus.Alter));
            AssertAtLeast("Drop", StatusPalette.StatusForeground(DiffStatus.Drop), StatusPalette.StatusBackground(DiffStatus.Drop));
            AssertAtLeast("warning", StatusPalette.WarningForeground, StatusPalette.WarningBackground);
            AssertAtLeast("white on danger", "#FFFFFF", StatusPalette.Danger);
        }

        private static void AssertAtLeast(string name, string foreground, string background)
        {
            var ratio = Contrast(foreground, background);

            Assert.IsTrue(ratio >= Minimum,
                name + " is " + ratio.ToString("0.00") + ":1, below the " + Minimum.ToString("0.0") + ":1 threshold");
        }

        /// <summary>WCAG 2.x contrast ratio for two #RRGGBB colours.</summary>
        private static double Contrast(string foreground, string background)
        {
            var high = Math.Max(Luminance(foreground), Luminance(background));
            var low = Math.Min(Luminance(foreground), Luminance(background));

            return (high + 0.05) / (low + 0.05);
        }

        private static double Luminance(string hex)
        {
            var text = hex.TrimStart('#');
            var r = Channel(text, 0);
            var g = Channel(text, 2);
            var b = Channel(text, 4);

            return (0.2126 * r) + (0.7152 * g) + (0.0722 * b);
        }

        private static double Channel(string hex, int offset)
        {
            var value = Convert.ToInt32(hex.Substring(offset, 2), 16) / 255.0;

            return value <= 0.03928 ? value / 12.92 : Math.Pow((value + 0.055) / 1.055, 2.4);
        }
    }
}
