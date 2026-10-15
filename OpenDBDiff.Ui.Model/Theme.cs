namespace OpenDBDiff.Ui.Model
{
    /// <summary>
    /// Which palette the shell is using (sections 13.2 and 13.4). It is called AppTheme rather than
    /// ThemeMode because modern WPF has a type of that name, and every file here has both namespaces open.
    /// </summary>
    public enum AppTheme
    {
        /// <summary>Follow the operating system.</summary>
        System,

        Light,

        Dark
    }

    /// <summary>
    /// One theme's colours.
    /// </summary>
    /// <remarks>
    /// Section 13.4 asks for a full token set mapping each light token to a dark equivalent, with the same
    /// contrast thresholds and the diff tints expressed as alpha over the dark surface rather than as
    /// opaque pastels. Both palettes live here rather than one here and one in XAML, because the shell's
    /// converters and the XAML tokens have to agree: a badge drawn from the view model and a badge drawn
    /// from a resource dictionary that drifted apart would be two themes, not one.
    ///
    /// The properties are settable only because there is no initialiser that reaches a private setter
    /// across types; the two instances below are readonly and nothing else constructs one. The contrast
    /// thresholds are checked by ColourContrastTests rather than asserted in a comment.
    /// </remarks>
    public sealed class ThemePalette
    {
        public string Background { get; set; } = string.Empty;

        public string Surface { get; set; } = string.Empty;

        public string SurfaceAlt { get; set; } = string.Empty;

        public string BorderSubtle { get; set; } = string.Empty;

        public string BorderStrong { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public string TextMuted { get; set; } = string.Empty;

        public string TextFaint { get; set; } = string.Empty;

        public string Accent { get; set; } = string.Empty;

        public string AccentBackground { get; set; } = string.Empty;

        public string AccentForeground { get; set; } = string.Empty;

        public string SelectionBackground { get; set; } = string.Empty;

        public string WarningBackground { get; set; } = string.Empty;

        public string WarningForeground { get; set; } = string.Empty;

        public string Danger { get; set; } = string.Empty;

        public string Focus { get; set; } = string.Empty;

        public string Highlight { get; set; } = string.Empty;

        // The badge and chip tints. Opaque, because a status word sits on them.
        public string CreateBackground { get; set; } = string.Empty;

        public string CreateForeground { get; set; } = string.Empty;

        public string CreateBorder { get; set; } = string.Empty;

        public string AlterBackground { get; set; } = string.Empty;

        public string AlterForeground { get; set; } = string.Empty;

        public string AlterBorder { get; set; } = string.Empty;

        public string DropBackground { get; set; } = string.Empty;

        public string DropForeground { get; set; } = string.Empty;

        public string DropBorder { get; set; } = string.Empty;

        // The diff line tints. In the light theme these are the badge tints; in the dark theme section
        // 13.4 asks for 22-30% alpha over the surface, which is what makes four rows of code stand out
        // without four blocks of colour in a dark window.
        public string DiffCreate { get; set; } = string.Empty;

        public string DiffAlter { get; set; } = string.Empty;

        public string DiffDrop { get; set; } = string.Empty;

        // The changed-word tints of section 6.5.3. They are drawn inside the line's own tint rather than
        // on the surface, so they are stronger than the 22-30% band section 13.4 sets for the line tints:
        // at that strength the second wash would be invisible against the first. What they must satisfy is
        // the contrast of the text inside them, which DarkThemeTests checks by compositing both washes.

        public string WordCreate { get; set; } = string.Empty;

        public string WordAlter { get; set; } = string.Empty;

        public string WordDrop { get; set; } = string.Empty;

        public string EnvironmentDevForeground { get; set; } = string.Empty;

        public string EnvironmentDevBackground { get; set; } = string.Empty;

        public string EnvironmentTestForeground { get; set; } = string.Empty;

        public string EnvironmentTestBackground { get; set; } = string.Empty;

        public string EnvironmentUatForeground { get; set; } = string.Empty;

        public string EnvironmentUatBackground { get; set; } = string.Empty;

        public string EnvironmentProductionForeground { get; set; } = string.Empty;

        public string EnvironmentProductionBackground { get; set; } = string.Empty;

        public string EnvironmentCustomForeground { get; set; } = string.Empty;

        public string EnvironmentCustomBackground { get; set; } = string.Empty;

        /// <summary>Any translucent tint the caller wants, or null for a clear one.</summary>
        public string DiffBackground(DiffLineKind kind)
        {
            switch (kind)
            {
                case DiffLineKind.Added:
                    return DiffCreate;
                case DiffLineKind.Removed:
                    return DiffDrop;
                case DiffLineKind.Changed:
                    return DiffAlter;
                case DiffLineKind.Filler:
                    return SurfaceAlt;
                default:
                    return null;
            }
        }

        public string WordTint(DiffLineKind kind)
        {
            switch (kind)
            {
                case DiffLineKind.Added:
                    return WordCreate;
                case DiffLineKind.Removed:
                    return WordDrop;
                default:
                    return WordAlter;
            }
        }
    }

    /// <summary>
    /// The theme in force, and the two palettes.
    /// </summary>
    /// <remarks>
    /// Section 13.4: the theme follows the operating system by default and the View menu overrides it.
    /// Resolving "System" needs a fact the model cannot see, so the shell passes it in; that keeps the
    /// decision testable without a window.
    /// </remarks>
    public static class Theme
    {
        /// <summary>The light palette of section 13.2, unchanged.</summary>
        public static readonly ThemePalette Light = new ThemePalette
        {
            Background = "#F4F5F7",
            Surface = "#FFFFFF",
            SurfaceAlt = "#EEF0F4",
            BorderSubtle = "#DDE2EA",
            BorderStrong = "#CBD2DE",
            Text = "#1B2230",
            TextMuted = "#5B6577",
            TextFaint = "#8A93A5",
            Accent = "#2F5BEA",
            AccentBackground = "#E5ECFF",
            AccentForeground = "#1F4FD1",
            SelectionBackground = "#EEF3FF",
            WarningBackground = "#FFF8E6",
            WarningForeground = "#7A4A00",
            Danger = "#B42318",
            Focus = "#1F4FD1",
            Highlight = "#FFE58F",
            CreateBackground = "#E3F4EA",
            CreateForeground = "#0F6B38",
            CreateBorder = "#9BD3B4",
            AlterBackground = "#E5ECFF",
            AlterForeground = "#1F4FD1",
            AlterBorder = "#A9BDF5",
            DropBackground = "#FDE8E6",
            DropForeground = "#A5210F",
            DropBorder = "#F2A9A0",
            DiffCreate = "#E3F4EA",
            DiffAlter = "#E5ECFF",
            DiffDrop = "#FDE8E6",
            WordCreate = "#A8DFC0",
            WordAlter = "#FFD98A",
            WordDrop = "#F5BDB4",
            EnvironmentDevForeground = "#0F6B38",
            EnvironmentDevBackground = "#E3F4EA",
            EnvironmentTestForeground = "#1F4FD1",
            EnvironmentTestBackground = "#E5ECFF",
            EnvironmentUatForeground = "#5B21B6",
            EnvironmentUatBackground = "#F0E9FB",
            EnvironmentProductionForeground = "#93370D",
            EnvironmentProductionBackground = "#FEF0C7",
            EnvironmentCustomForeground = "#5B6577",
            EnvironmentCustomBackground = "#EEF0F4"
        };

        /// <summary>
        /// The dark palette of section 13.4. The diff tints are translucent, at 26% alpha, which is inside
        /// the 22-30% the section asks for; the tints the status words sit on are opaque, because a badge
        /// is read, not scanned.
        /// </summary>
        public static readonly ThemePalette Dark = new ThemePalette
        {
            Background = "#12161C",
            Surface = "#1B2028",
            SurfaceAlt = "#232A34",
            BorderSubtle = "#2E3742",
            BorderStrong = "#3B4654",
            Text = "#E7EAF0",
            TextMuted = "#A2ACBA",
            TextFaint = "#8792A2",
            Accent = "#6E9BFF",
            AccentBackground = "#1E2A45",
            AccentForeground = "#A8C2FF",
            SelectionBackground = "#22314B",
            WarningBackground = "#3A2E12",
            WarningForeground = "#F5CE86",
            // Dark enough for white text on it (5.2:1) and light enough to read as a block against the
            // surface (3.1:1), which is the narrow band a filled danger button has to sit in.
            Danger = "#C7363B",
            Focus = "#8AB0FF",
            Highlight = "#6B5A16",
            CreateBackground = "#1D3A2C",
            CreateForeground = "#7FE3AE",
            CreateBorder = "#2F6B4E",
            AlterBackground = "#1E2A4A",
            AlterForeground = "#A8C2FF",
            AlterBorder = "#3A5390",
            DropBackground = "#3A1F1D",
            DropForeground = "#FFA79C",
            DropBorder = "#7A3A33",
            DiffCreate = "#423FBF7F",
            DiffAlter = "#426E9BFF",
            DiffDrop = "#42F97066",
            WordCreate = "#5941A86A",
            WordAlter = "#59B98A2E",
            WordDrop = "#59E86A5F",
            EnvironmentDevForeground = "#7FE3AE",
            EnvironmentDevBackground = "#1D3A2C",
            EnvironmentTestForeground = "#A8C2FF",
            EnvironmentTestBackground = "#1E2A45",
            EnvironmentUatForeground = "#D0B4FF",
            EnvironmentUatBackground = "#2C2340",
            EnvironmentProductionForeground = "#F5CE86",
            EnvironmentProductionBackground = "#3A2E12",
            EnvironmentCustomForeground = "#A2ACBA",
            EnvironmentCustomBackground = "#232A34"
        };

        private static AppTheme mode = AppTheme.System;

        /// <summary>The palette in force. Light until the shell says otherwise.</summary>
        public static ThemePalette Current { get; private set; } = Light;

        public static AppTheme Mode
        {
            get { return mode; }
        }

        public static bool IsDark
        {
            get { return Current == Dark; }
        }

        /// <summary>
        /// Section 13.4: "System" follows the operating system, and the shell is the side that knows what
        /// the operating system says. Applying the same mode twice is allowed and changes nothing.
        /// </summary>
        public static void Apply(AppTheme requested, bool operatingSystemIsDark)
        {
            mode = requested;
            Current = Resolve(requested, operatingSystemIsDark);
        }

        /// <summary>What a mode resolves to, given what the operating system is doing.</summary>
        public static ThemePalette Resolve(AppTheme requested, bool operatingSystemIsDark)
        {
            switch (requested)
            {
                case AppTheme.Dark:
                    return Dark;
                case AppTheme.Light:
                    return Light;
                default:
                    return operatingSystemIsDark ? Dark : Light;
            }
        }

        /// <summary>Reads back the words the View menu uses, without needing a view model.</summary>
        public static AppTheme Parse(string value)
        {
            return string.Equals(value, "Dark", System.StringComparison.OrdinalIgnoreCase)
                ? AppTheme.Dark
                : string.Equals(value, "Light", System.StringComparison.OrdinalIgnoreCase)
                    ? AppTheme.Light
                    : AppTheme.System;
        }
    }
}
