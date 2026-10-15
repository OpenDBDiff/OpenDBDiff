using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Wpf.Services
{
    /// <summary>
    /// Section 13.4's theme switch, as the view model sees it: the mode is chosen, the palette follows,
    /// and the operating system's own preference is the default.
    /// </summary>
    public interface IThemeService
    {
        /// <summary>What the operating system is doing, which is what "System" means.</summary>
        bool OperatingSystemIsDark { get; }

        /// <summary>Applies a mode and makes the window show it.</summary>
        void Apply(AppTheme mode);
    }
}
