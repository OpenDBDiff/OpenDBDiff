using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace OpenDBDiff.Wpf.Services
{
    /// <summary>One side of a saved project file.</summary>
    public sealed class ProjectConnection
    {
        public string? Host { get; set; }

        public string? Database { get; set; }

        public bool IntegratedSecurity { get; set; } = true;

        public string? UserName { get; set; }

        /// <summary>The environment tag's text, empty for none (section 6.1.4).</summary>
        public string? Environment { get; set; }

        /// <summary>
        /// Section 6.7.2's "Never write to this database from OpenDBDiff". An additive field in the sense
        /// of section 11.1: a reader that does not know it ignores it, and a file without it means false.
        /// </summary>
        public bool NeverWrite { get; set; }
    }

    /// <summary>
    /// The project file this front end writes for File → Save project as… and reads back through
    /// Open project (sections 6.1.6 and 12).
    /// </summary>
    /// <remarks>
    /// The fields are exactly the additive ones section 11.1 allows - the two connections and their
    /// environment tags - and no credential of any kind, because section 11.3 forbids storing one. The
    /// legacy .odbd file is a LiteDB database owned by the WinForms project, so this reader recognises
    /// its own files and reports anything else as unreadable rather than guessing at a format it cannot
    /// parse.
    ///
    /// This type is a plain data holder; turning it into and out of the connection view models is the
    /// shell's job, which keeps the persistence layer free of any dependency on the UI.
    /// </remarks>
    public sealed class ProjectFile
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions { WriteIndented = true };

        public int Version { get; set; } = 1;

        public ProjectConnection? Source { get; set; }

        public ProjectConnection? Destination { get; set; }

        /// <summary>
        /// Section 11.1's additive field: the objects that were ticked when the project was saved, by
        /// full name.
        /// </summary>
        /// <remarks>
        /// Kept as names rather than as serialised keys, because a name is what a reader can check by eye
        /// when a project comes back different, and because section 11.1 only asks for the field to be
        /// additive. A file without it means nothing was recorded, which is not the same as nothing being
        /// included: the next comparison applies the section 7.1 defaults, exactly as it would for a
        /// project saved before this field existed.
        /// </remarks>
        public List<string> InclusionKeys { get; set; } = new List<string>();

        /// <summary>Reads a project file, or returns null when the file is not one this front end wrote.</summary>
        public static ProjectFile? Load(string path)
        {
            try
            {
                var json = File.ReadAllText(path);
                var project = JsonSerializer.Deserialize<ProjectFile>(json, Options);

                return project != null && (project.Source != null || project.Destination != null) ? project : null;
            }
            catch (Exception ex) when (ex is IOException || ex is JsonException || ex is UnauthorizedAccessException)
            {
                return null;
            }
        }

        public void Save(string path)
        {
            File.WriteAllText(path, JsonSerializer.Serialize(this, Options));
        }
    }
}
