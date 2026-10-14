using System;
using System.IO;
using System.Text.Json;

namespace OpenDBDiff.Wpf.Services
{
    /// <summary>
    /// Stores the window state as JSON under %APPDATA%\OpenDBDiff.
    /// </summary>
    /// <remarks>
    /// The WinForms front end keeps its settings in LiteDB (Project.GetDatabase) together with saved
    /// projects. That store lives in the net48 project, so this front end has its own file; when the
    /// project file format is ported, the two stores should become one.
    /// </remarks>
    public sealed class JsonSettingsStore : ISettingsStore
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions { WriteIndented = true };

        private readonly string path;

        public JsonSettingsStore()
        {
            var directory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "OpenDBDiff");

            path = Path.Combine(directory, "wpf-ui.json");
        }

        public UiState Load()
        {
            try
            {
                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path);
                    var state = JsonSerializer.Deserialize<UiState>(json, Options);

                    if (state != null)
                    {
                        return state;
                    }
                }
            }
            catch (Exception ex) when (ex is IOException || ex is JsonException || ex is UnauthorizedAccessException)
            {
                // A missing or unreadable settings file is not worth failing a launch over.
            }

            return new UiState();
        }

        public void Save(UiState state)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                File.WriteAllText(path, JsonSerializer.Serialize(state, Options));
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                // Losing window geometry is not worth interrupting the user for.
            }
        }
    }
}
