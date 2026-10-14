namespace OpenDBDiff.Wpf.Services
{
    /// <summary>
    /// Window state that outlives a session.
    /// </summary>
    public sealed class UiState
    {
        /// <summary>
        /// Width of the schema tree pane, carried over from the WinForms front end's default.
        /// </summary>
        public int SchemaSplitterDistance { get; set; } = 340;

        public string? SourceConnectionString { get; set; }

        public string? DestinationConnectionString { get; set; }
    }

    public interface ISettingsStore
    {
        UiState Load();

        void Save(UiState state);
    }
}
