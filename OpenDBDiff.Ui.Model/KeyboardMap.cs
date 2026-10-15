using System;
using System.Collections.Generic;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>
    /// How a shortcut is actually carried out, so the overlay cannot claim a key that was never wired.
    /// </summary>
    public enum ShortcutWiring
    {
        /// <summary>A Window.InputBindings entry in the main window.</summary>
        KeyBinding,

        /// <summary>Handled in the window's code-behind, because it moves focus or depends on the screen.</summary>
        CodeBehind,

        /// <summary>The toolkit's own behaviour, needing no code of ours.</summary>
        Native
    }

    /// <summary>One row of the keyboard map (section 7.2).</summary>
    public sealed class ShortcutRow
    {
        public ShortcutRow(string chord, string key, string modifiers, string action, ShortcutWiring wiring)
        {
            Chord = chord;
            Key = key;
            Modifiers = modifiers;
            Action = action;
            Wiring = wiring;
        }

        /// <summary>How the key is written, e.g. "Ctrl+Shift+C".</summary>
        public string Chord { get; }

        /// <summary>The WPF Key name, e.g. "C".</summary>
        public string Key { get; }

        /// <summary>The modifiers in the order the window binds them, e.g. "Control,Shift".</summary>
        public string Modifiers { get; }

        public string Action { get; }

        public ShortcutWiring Wiring { get; }
    }

    /// <summary>
    /// The keyboard map of section 7.2, as data.
    /// </summary>
    /// <remarks>
    /// One list rather than a paragraph in a specification, because the help overlay and the tests both
    /// read it: a shortcut that is advertised but not bound, or bound but not advertised, is a bug the
    /// tests can find. Only keys that are actually implemented appear here.
    /// </remarks>
    public static class KeyboardMap
    {
        public static IReadOnlyList<ShortcutRow> Rows { get; } = new[]
        {
            new ShortcutRow("F5", "F5", string.Empty, "Compare / Compare again", ShortcutWiring.KeyBinding),
            new ShortcutRow("Esc", "Escape", string.Empty, "Cancel the running operation, close the panel, clear search", ShortcutWiring.CodeBehind),
            new ShortcutRow("Ctrl+F", "F", "Control", "Focus search", ShortcutWiring.CodeBehind),
            new ShortcutRow("Ctrl+Enter", "Enter", "Control", "Open the Script Preview", ShortcutWiring.KeyBinding),
            new ShortcutRow("Ctrl+Shift+Enter", "Enter", "Control,Shift", "Start the apply flow (opens the confirmation; never applies directly)", ShortcutWiring.KeyBinding),
            new ShortcutRow("Ctrl+Shift+C", "C", "Control,Shift", "Copy script", ShortcutWiring.KeyBinding),
            new ShortcutRow("Ctrl+S", "S", "Control", "Save script as\u2026", ShortcutWiring.KeyBinding),
            new ShortcutRow("Ctrl+O", "O", "Control", "Open project", ShortcutWiring.KeyBinding),
            new ShortcutRow("Ctrl+Shift+S", "S", "Control,Shift", "Save project", ShortcutWiring.KeyBinding),
            new ShortcutRow("Ctrl+E", "E", "Control", "Edit connections", ShortcutWiring.KeyBinding),
            new ShortcutRow("Ctrl+Shift+X", "X", "Control,Shift", "Swap source and destination", ShortcutWiring.KeyBinding),
            new ShortcutRow("Ctrl+1", "D1", "Control", "Detail mode: Side by side", ShortcutWiring.KeyBinding),
            new ShortcutRow("Ctrl+2", "D2", "Control", "Detail mode: Inline", ShortcutWiring.KeyBinding),
            new ShortcutRow("Ctrl+3", "D3", "Control", "Detail mode: Script", ShortcutWiring.KeyBinding),
            new ShortcutRow("F7", "F7", string.Empty, "Next change", ShortcutWiring.KeyBinding),
            new ShortcutRow("Shift+F7", "F7", "Shift", "Previous change", ShortcutWiring.KeyBinding),
            new ShortcutRow("Space", "Space", string.Empty, "Toggle inclusion for the focused row", ShortcutWiring.CodeBehind),
            new ShortcutRow("Left", "Left", string.Empty, "Collapse the focused group", ShortcutWiring.CodeBehind),
            new ShortcutRow("Right", "Right", string.Empty, "Expand the focused group", ShortcutWiring.CodeBehind),
            new ShortcutRow("Ctrl+A", "A", "Control", "Select all visible differences", ShortcutWiring.KeyBinding),
            new ShortcutRow("Ctrl+Z", "Z", "Control", "Undo a selection change", ShortcutWiring.KeyBinding),
            new ShortcutRow("Ctrl+Y", "Y", "Control", "Redo a selection change", ShortcutWiring.KeyBinding),
            new ShortcutRow("F1", "F1", string.Empty, "Show this list", ShortcutWiring.CodeBehind),
            new ShortcutRow("Up / Down", "Up", string.Empty, "Move the focus in the review list", ShortcutWiring.Native),
            new ShortcutRow("Tab", "Tab", string.Empty, "Move through the regions in order", ShortcutWiring.Native)
        };
    }
}
