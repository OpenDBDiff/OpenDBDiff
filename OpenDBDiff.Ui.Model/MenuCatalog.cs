using System.Collections.Generic;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>One item of the menu bar (section 12).</summary>
    public sealed class MenuItem
    {
        public MenuItem(string menu, string caption, string shortcut = null, string legacyControl = null)
        {
            Menu = menu;
            Caption = caption;
            Shortcut = shortcut;
            LegacyControl = legacyControl;
        }

        public string Menu { get; }

        public string Caption { get; }

        /// <summary>The key chord the item advertises, empty when it has none.</summary>
        public string Shortcut { get; }

        /// <summary>The old control this item gives a home to, from section 12's legacy mapping.</summary>
        public string LegacyControl { get; }
    }

    /// <summary>One row of section 12's legacy mapping table.</summary>
    public sealed class LegacyMapping
    {
        public LegacyMapping(string oldControl, string newLocation)
        {
            OldControl = oldControl;
            NewLocation = newLocation;
        }

        public string OldControl { get; }

        public string NewLocation { get; }
    }

    /// <summary>
    /// The menu bar's structure and the legacy mapping of section 12, as data.
    /// </summary>
    /// <remarks>
    /// AC-12 requires that no legacy capability is lost, and a claim like that is only checkable if the
    /// mapping is a list rather than a paragraph in a specification: the tests assert that every old
    /// control in section 12 has a new location and that every menu item is reachable from a menu.
    /// </remarks>
    public static class MenuCatalog
    {
        public const string File = "File";
        public const string Compare = "Compare";
        public const string View = "View";
        public const string Script = "Script";
        public const string Tools = "Tools";
        public const string Help = "Help";

        /// <summary>The menu bar, left to right (section 12).</summary>
        public static IReadOnlyList<string> Menus { get; } = new[] { File, Compare, View, Script, Tools, Help };

        public static IReadOnlyList<MenuItem> Items { get; } = new[]
        {
            new MenuItem(File, "New project", null, "New project"),
            new MenuItem(File, "Open project\u2026", "Ctrl+O", "Open project"),
            new MenuItem(File, "Save project", "Ctrl+Shift+S", "Save project"),
            new MenuItem(File, "Save project as\u2026", null),
            new MenuItem(File, "Recent projects", null),
            new MenuItem(File, "Exit", null),

            new MenuItem(Compare, "Compare", "F5", "Compare"),
            new MenuItem(Compare, "Cancel", "Esc"),
            new MenuItem(Compare, "Swap source and destination", "Ctrl+Shift+X"),
            new MenuItem(Compare, "Schema", null),
            new MenuItem(Compare, "Data", null, "Compare data"),
            new MenuItem(Compare, "Recent comparisons", null),

            new MenuItem(View, "Show identical objects", null, "Show unchanged/changed/new/missing items checkboxes"),
            new MenuItem(View, "Group by type", null),
            new MenuItem(View, "Flat list", null),
            new MenuItem(View, "Side by side", "Ctrl+1", "Tabs: Schema"),
            new MenuItem(View, "Inline", "Ctrl+2"),
            new MenuItem(View, "Script", "Ctrl+3", "Tabs: Synchronized script"),
            new MenuItem(View, "Reset layout"),
            new MenuItem(View, "Theme"),

            new MenuItem(Script, "Preview", "Ctrl+Enter"),
            new MenuItem(Script, "Copy", "Ctrl+Shift+C", "Copy script"),
            new MenuItem(Script, "Save as\u2026", "Ctrl+S", "Save as"),

            new MenuItem(Tools, "Options\u2026", null, "Options"),
            new MenuItem(Tools, "Open reports folder", null),

            new MenuItem(Help, "Keyboard shortcuts", "F1"),
            new MenuItem(Help, "Documentation", null),
            new MenuItem(Help, "About", null)
        };

        /// <summary>Section 12's legacy mapping table, verbatim on the left.</summary>
        public static IReadOnlyList<LegacyMapping> LegacyMappings { get; } = new[]
        {
            new LegacyMapping("Open project", "File menu \u2192 Open project"),
            new LegacyMapping("New project", "File menu \u2192 New project"),
            new LegacyMapping("Save project", "File menu \u2192 Save project"),
            new LegacyMapping("Compare", "Primary button, connection strip"),
            new LegacyMapping("Options", "Gear button on the strip and Tools menu"),
            new LegacyMapping("Save as", "Action Bar \u2192 Save as\u2026"),
            new LegacyMapping("Copy script", "Action Bar \u2192 Copy script and Script menu"),
            new LegacyMapping("Update selected", "Apply, selection-based"),
            new LegacyMapping("Update all", "Select all + Apply; the button states the count that will run"),
            new LegacyMapping("Compare data", "Mode switch \u2192 Data"),
            new LegacyMapping("Tabs: Schema", "Detail mode Side by side"),
            new LegacyMapping("Tabs: Synchronized script", "Detail mode Script and the Script Preview"),
            new LegacyMapping("Tabs: Action report", "REPORT state"),
            new LegacyMapping("Legend (Create / Alter / Drop)", "Summary chips and list badges"),
            new LegacyMapping("Show unchanged/changed/new/missing items checkboxes", "Summary chips plus the Identical chip")
        };

        /// <summary>Every legacy control section 12 lists, so AC-12 can be asserted rather than claimed.</summary>
        public static IReadOnlyList<string> LegacyControls { get; } = new[]
        {
            "Open project", "New project", "Save project", "Compare", "Options", "Save as", "Copy script",
            "Update selected", "Update all", "Compare data", "Tabs: Schema", "Tabs: Synchronized script",
            "Tabs: Action report", "Legend (Create / Alter / Drop)",
            "Show unchanged/changed/new/missing items checkboxes"
        };

        /// <summary>True when the legacy mapping gives an old control a new location.</summary>
        public static bool HasHomeFor(string legacyControl)
        {
            foreach (var mapping in LegacyMappings)
            {
                if (string.Equals(mapping.OldControl, legacyControl, System.StringComparison.Ordinal))
                {
                    return !string.IsNullOrEmpty(mapping.NewLocation);
                }
            }

            return false;
        }

        /// <summary>The items under one menu, in order.</summary>
        public static IReadOnlyList<MenuItem> ItemsUnder(string menu)
        {
            var found = new List<MenuItem>();

            foreach (var item in Items)
            {
                if (string.Equals(item.Menu, menu, System.StringComparison.Ordinal))
                {
                    found.Add(item);
                }
            }

            return found;
        }
    }
}
