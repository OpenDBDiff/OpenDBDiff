using System.Collections.Generic;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Wpf.Services
{
    /// <summary>A saved connection as it is written to disk (section 6.1.6).</summary>
    /// <remarks>
    /// There is no password field and there must never be one: section 11.3 allows a SQL authentication
    /// password only through the Windows Credential Manager or DPAPI, never in the settings file.
    /// </remarks>
    public sealed class SavedConnectionState
    {
        public string? Name { get; set; }

        public string? Host { get; set; }

        public string? Database { get; set; }

        public string? UserName { get; set; }

        public bool IntegratedSecurity { get; set; } = true;

        public EnvironmentKind Environment { get; set; } = EnvironmentKind.None;

        /// <summary>Section 6.7.2's "Never write to this database from OpenDBDiff".</summary>
        public bool NeverWrite { get; set; }
    }

    /// <summary>One recent source/destination pair (section 6.1.6).</summary>
    public sealed class RecentPairState
    {
        public string? SourceHost { get; set; }

        public string? SourceDatabase { get; set; }

        public string? DestinationHost { get; set; }

        public string? DestinationDatabase { get; set; }

        public System.DateTime LastRunUtc { get; set; }
    }

    /// <summary>
    /// Window state that outlives a session, extended in Phase 2 with the items section 11 lists and
    /// the connection strip needs (saved connections, recent pairs, window and panel state).
    /// </summary>
    public sealed class UiState
    {
        /// <summary>
        /// Width of the schema tree pane, carried over from the WinForms front end's default.
        /// </summary>
        public int SchemaSplitterDistance { get; set; } = 340;

        public string? SourceConnectionString { get; set; }

        public string? DestinationConnectionString { get; set; }

        /// <summary>Bumped when a field changes meaning, so an old file can be migrated rather than guessed at.</summary>
        public int SchemaVersion { get; set; } = 1;

        // ---- Section 11: the settings the redesigned window keeps --------------------------------

        /// <summary>The review list width, 400 by default (section 11).</summary>
        public int ReviewListWidth { get; set; } = 400;

        /// <summary>"SideBySide", "Inline" or "Script"; Side by side by default (section 11).</summary>
        public string? DetailMode { get; set; }

        /// <summary>"Grouped" or "Flat"; Grouped by default (section 11).</summary>
        public string? ListViewMode { get; set; }

        /// <summary>
        /// Section 11's "Select Drop items by default", off unless the user turns it on. Section 7.1
        /// rule 4 is the default it overrides, so this is a stored preference rather than a code default.
        /// </summary>
        public bool IncludeDropByDefault { get; set; }

        /// <summary>
        /// Section 11's "Ignore whitespace in diff", held here because the engine's comparison option is
        /// what implements it and this is how it survives a restart.
        /// </summary>
        public bool IgnoreWhitespace { get; set; }

        /// <summary>Section 11's "Last used folder for scripts/reports", Documents until one is chosen.</summary>
        public string? LastSaveFolder { get; set; }

        /// <summary>
        /// The one-time tips that have been shown (section 19, R4). Ids rather than a flag each, because the
        /// second tip should not need a settings schema change to exist.
        /// </summary>
        public List<string>? SeenTips { get; set; }

        public bool ShowIdentical { get; set; }

        /// <summary>Environment tag per side, as the tag text (section 6.1.4).</summary>
        public string? SourceEnvironment { get; set; }

        public string? DestinationEnvironment { get; set; }

        /// <summary>Window bounds and state; validated against the current monitors on restore (section 11).</summary>
        public double? WindowLeft { get; set; }

        public double? WindowTop { get; set; }

        public double? WindowWidth { get; set; }

        public double? WindowHeight { get; set; }

        public string? WindowState { get; set; }

        /// <summary>Whether the connection panel was left open (section 6.1.1).</summary>
        public bool ConnectionPanelOpen { get; set; }

        /// <summary>"Light", "Dark" or "System" (section 11). Dark rendering itself is Phase 4.</summary>
        public string? Theme { get; set; }

        public List<SavedConnectionState> SavedConnections { get; set; } = new List<SavedConnectionState>();

        public List<RecentPairState> RecentPairs { get; set; } = new List<RecentPairState>();

        public List<string> RecentProjects { get; set; } = new List<string>();
    }

    public interface ISettingsStore
    {
        UiState Load();

        void Save(UiState state);
    }
}
