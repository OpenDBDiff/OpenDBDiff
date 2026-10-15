using System;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>The environments a connection can be tagged with (section 6.1.4).</summary>
    public enum EnvironmentKind
    {
        None,
        Dev,
        Test,
        Uat,
        Prod,
        Custom
    }

    /// <summary>
    /// The optional per-connection environment tag (section 6.1.4): None, Dev, Test, UAT, Prod or a
    /// custom label of at most eight characters.
    /// </summary>
    /// <remarks>
    /// A tag is never inferred silently. <see cref="EnvironmentPalette.ShouldSuggest"/> only says the
    /// panel may offer the suggestion with an Accept link; the tag is applied when the user accepts it.
    /// </remarks>
    public sealed class EnvironmentTag : IEquatable<EnvironmentTag>
    {
        /// <summary>Section 6.1.4: "custom (max 8 chars)".</summary>
        public const int MaxCustomChars = 8;

        private EnvironmentTag(EnvironmentKind kind, string text)
        {
            Kind = kind;
            Text = text;
        }

        public static EnvironmentTag None { get; } = new EnvironmentTag(EnvironmentKind.None, string.Empty);

        public EnvironmentKind Kind { get; }

        /// <summary>The tag as rendered: upper case, empty for None. Section 6.1.4 renders tags upper case.</summary>
        public string Text { get; }

        public bool IsNone
        {
            get { return Kind == EnvironmentKind.None; }
        }

        public bool HasValue
        {
            get { return Kind != EnvironmentKind.None; }
        }

        /// <summary>Section 6.1.4: a Prod tag is what makes the confirmation dialog demand the typed name.</summary>
        public bool IsProduction
        {
            get { return Kind == EnvironmentKind.Prod; }
        }

        public static EnvironmentTag For(EnvironmentKind kind)
        {
            switch (kind)
            {
                case EnvironmentKind.Dev: return new EnvironmentTag(kind, "DEV");
                case EnvironmentKind.Test: return new EnvironmentTag(kind, "TEST");
                case EnvironmentKind.Uat: return new EnvironmentTag(kind, "UAT");
                case EnvironmentKind.Prod: return new EnvironmentTag(kind, "PROD");
                case EnvironmentKind.Custom: return new EnvironmentTag(kind, string.Empty);
                default: return None;
            }
        }

        /// <summary>
        /// A custom tag. Longer input is truncated to the maximum rather than rejected, so a paste
        /// cannot silently produce a tag the chip refuses to draw.
        /// </summary>
        public static EnvironmentTag Custom(string text)
        {
            var trimmed = (text ?? string.Empty).Trim();

            if (trimmed.Length == 0)
            {
                return None;
            }

            if (trimmed.Length > MaxCustomChars)
            {
                trimmed = trimmed.Substring(0, MaxCustomChars);
            }

            return new EnvironmentTag(EnvironmentKind.Custom, trimmed.ToUpperInvariant());
        }

        public bool Equals(EnvironmentTag other)
        {
            return other != null
                && Kind == other.Kind
                && string.Equals(Text, other.Text, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EnvironmentTag);
        }

        public override int GetHashCode()
        {
            return ((int)Kind * 397) ^ Text.GetHashCode();
        }

        public override string ToString()
        {
            return Text;
        }
    }

    /// <summary>
    /// The environment tag colours of section 6.1.4, as hex strings so the rules layer needs no
    /// drawing types. A test asserts every pair against the WCAG 4.5:1 threshold the section requires.
    /// </summary>
    public static class EnvironmentPalette
    {
        // As with StatusPalette, the values live in Theme so section 13.4's dark equivalents are one
        // definition rather than two. The light values are unchanged.
        public static string DevBackground
        {
            get { return Theme.Current.EnvironmentDevBackground; }
        }

        public static string DevForeground
        {
            get { return Theme.Current.EnvironmentDevForeground; }
        }

        public static string TestBackground
        {
            get { return Theme.Current.EnvironmentTestBackground; }
        }

        public static string TestForeground
        {
            get { return Theme.Current.EnvironmentTestForeground; }
        }

        public static string UatBackground
        {
            get { return Theme.Current.EnvironmentUatBackground; }
        }

        public static string UatForeground
        {
            get { return Theme.Current.EnvironmentUatForeground; }
        }

        public static string ProdBackground
        {
            get { return Theme.Current.EnvironmentProductionBackground; }
        }

        public static string ProdForeground
        {
            get { return Theme.Current.EnvironmentProductionForeground; }
        }

        public static string CustomBackground
        {
            get { return Theme.Current.EnvironmentCustomBackground; }
        }

        public static string CustomForeground
        {
            get { return Theme.Current.EnvironmentCustomForeground; }
        }

        public static string Background(EnvironmentKind kind)
        {
            switch (kind)
            {
                case EnvironmentKind.Dev: return DevBackground;
                case EnvironmentKind.Test: return TestBackground;
                case EnvironmentKind.Uat: return UatBackground;
                case EnvironmentKind.Prod: return ProdBackground;
                case EnvironmentKind.Custom: return CustomBackground;
                default: return CustomBackground;
            }
        }

        public static string Foreground(EnvironmentKind kind)
        {
            switch (kind)
            {
                case EnvironmentKind.Dev: return DevForeground;
                case EnvironmentKind.Test: return TestForeground;
                case EnvironmentKind.Uat: return UatForeground;
                case EnvironmentKind.Prod: return ProdForeground;
                default: return CustomForeground;
            }
        }

        /// <summary>
        /// Section 6.1.4: "if a server host or database name contains prod (case-insensitive), the
        /// Connection Panel proposes tag Prod with an Accept link. Never apply silently."
        /// </summary>
        public static bool ShouldSuggest(string host, string database)
        {
            return Contains(host, "prod") || Contains(database, "prod");
        }

        public static EnvironmentKind Suggest(string host, string database)
        {
            return ShouldSuggest(host, database) ? EnvironmentKind.Prod : EnvironmentKind.None;
        }

        private static bool Contains(string text, string needle)
        {
            return !string.IsNullOrEmpty(text) && text.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
