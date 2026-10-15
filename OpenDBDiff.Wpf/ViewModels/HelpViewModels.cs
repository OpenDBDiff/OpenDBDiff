using System.Collections.Generic;
using System.Reflection;
using OpenDBDiff.Wpf.Infrastructure;
using PropertyChanged;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>One row of the keyboard map (section 7.2).</summary>
    public sealed class ShortcutRow
    {
        public ShortcutRow(string key, string action)
        {
            Key = key;
            Action = action;
        }

        public string Key { get; }

        public string Action { get; }
    }

    /// <summary>
    /// Help → Keyboard shortcuts: the overlay of section 7.2, opened with F1.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed class ShortcutsViewModel : DialogViewModelBase
    {
        public ShortcutsViewModel()
        {
            Rows = new List<ShortcutRow>
            {
                new ShortcutRow("F5", "Compare / Compare again"),
                new ShortcutRow("Esc", "Cancel the running operation, close the panel, clear search"),
                new ShortcutRow("Ctrl+F", "Focus search"),
                new ShortcutRow("Ctrl+Enter", "Open the Script Preview"),
                new ShortcutRow("Ctrl+Shift+C", "Copy script"),
                new ShortcutRow("Ctrl+S", "Save script as\u2026"),
                new ShortcutRow("Ctrl+O", "Open project"),
                new ShortcutRow("Ctrl+Shift+S", "Save project"),
                new ShortcutRow("Ctrl+E", "Edit connections"),
                new ShortcutRow("Ctrl+Shift+X", "Swap source and destination"),
                new ShortcutRow("Ctrl+1 / Ctrl+2 / Ctrl+3", "Detail mode: Side by side / Inline / Script"),
                new ShortcutRow("F1", "This list"),
                new ShortcutRow("Space", "Toggle inclusion for the focused row"),
                new ShortcutRow("Ctrl+A", "Select all visible differences")
            };

            CloseCommand = new RelayCommand(Close);
        }

        public string Title
        {
            get { return "Keyboard shortcuts"; }
        }

        public IReadOnlyList<ShortcutRow> Rows { get; }

        public RelayCommand CloseCommand { get; }

        public void Close()
        {
            RequestClose();
        }
    }

    /// <summary>Help → About.</summary>
    [AddINotifyPropertyChangedInterface]
    public sealed class AboutViewModel : DialogViewModelBase
    {
        public AboutViewModel()
        {
            CloseCommand = new RelayCommand(Close);
        }

        public string Title
        {
            get { return "About OpenDBDiff"; }
        }

        public string ProductName
        {
            get { return "OpenDBDiff"; }
        }

        public string Version
        {
            get
            {
                var assembly = Assembly.GetEntryAssembly() ?? typeof(AboutViewModel).Assembly;

                return assembly.GetName().Version?.ToString() ?? "0.14.0";
            }
        }

        public string Blurb
        {
            get
            {
                return "Compares two SQL Server databases, shows what differs and applies the changes you choose.";
            }
        }

        public RelayCommand CloseCommand { get; }

        public void Close()
        {
            RequestClose();
        }
    }
}
