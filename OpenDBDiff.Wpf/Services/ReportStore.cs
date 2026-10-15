using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.Wpf.Services
{
    /// <summary>
    /// The reports folder of section 11.2: one folder per apply, holding the script that ran and the
    /// report, with the retention rule applied on the way past.
    /// </summary>
    /// <remarks>
    /// AC-11 asks for a full history for every apply, so the folder is written whether or not the user
    /// asked for a copy of the script; the confirmation dialog's checkbox decides whether the script is
    /// written before the run as well, which is what protects a reader from a run that never returned.
    ///
    /// The folder name carries a local timestamp because a person browses it; the retention rule compares
    /// UTC, because a clock change must not resurrect or expire anything.
    /// </remarks>
    public sealed class ReportStore
    {
        private readonly string applicationDataFolder;

        public ReportStore()
            : this(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))
        {
        }

        public ReportStore(string applicationDataFolder)
        {
            this.applicationDataFolder = applicationDataFolder;
        }

        public string Root
        {
            get { return ReportPaths.Root(applicationDataFolder); }
        }

        /// <summary>
        /// Creates this run's folder and writes the report into it. The script is passed separately so a
        /// caller that wrote it before the run does not write it twice.
        /// </summary>
        public string Write(ApplyReport report, string script)
        {
            var folder = ReportPaths.FolderPath(applicationDataFolder, DateTime.Now, report.Target.Database);

            Directory.CreateDirectory(folder);

            File.WriteAllText(Path.Combine(folder, ReportPaths.ReportFileName), ReportFormat.ToHtml(report));
            File.WriteAllText(Path.Combine(folder, ReportPaths.ScriptFileName), script ?? report.Script ?? string.Empty);

            return folder;
        }

        /// <summary>Writes the script alone, before the run, into this run's folder (section 6.8's third option).</summary>
        public string WriteScriptBeforeRun(string script, DateTime when, string database)
        {
            var folder = ReportPaths.FolderPath(applicationDataFolder, when, database);

            Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder, ReportPaths.ScriptFileName), script ?? string.Empty);

            return folder;
        }

        /// <summary>The folders currently on disk, newest first, with their creation times.</summary>
        public IReadOnlyList<ReportFolderEntry> List()
        {
            var entries = new List<ReportFolderEntry>();

            if (!Directory.Exists(Root))
            {
                return entries;
            }

            foreach (var directory in Directory.GetDirectories(Root))
            {
                entries.Add(new ReportFolderEntry(
                    Path.GetFileName(directory),
                    directory,
                    Directory.GetCreationTimeUtc(directory)));
            }

            return entries;
        }

        /// <summary>
        /// Section 11.2's retention, as a cleanup the Options menu can offer: everything the rule calls
        /// expired is removed, and the count is returned so the caller can say what happened.
        /// </summary>
        public int Cleanup()
        {
            var removed = 0;

            foreach (var folder in ReportRetentionRules.Expired(List(), DateTime.UtcNow))
            {
                try
                {
                    Directory.Delete(folder.Path, true);
                    removed++;
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    // A folder that cannot be removed is left alone rather than failing the cleanup.
                }
            }

            return removed;
        }

        /// <summary>How many folders the retention rule would remove, without removing anything.</summary>
        public int ExpiredCount()
        {
            return ReportRetentionRules.Expired(List(), DateTime.UtcNow).Count;
        }

        /// <summary>The newest report, which is what "Open reports folder" and the report view link to.</summary>
        public string NewestFolder()
        {
            var newest = List()
                .OrderByDescending(folder => folder.CreatedUtc)
                .FirstOrDefault();

            return newest == null ? Root : newest.Path;
        }
    }
}
