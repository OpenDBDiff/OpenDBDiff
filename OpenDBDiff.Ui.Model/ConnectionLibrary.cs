using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>
    /// One entry in the Saved connections list (section 6.1.6).
    /// </summary>
    /// <remarks>
    /// There is deliberately no password field: section 11.3 allows SQL authentication passwords only
    /// through the Windows Credential Manager or DPAPI, never in the settings file. Keeping the secret
    /// out of this type is what makes "never persisted in plain text" a property of the model rather
    /// than of every writer.
    /// </remarks>
    public sealed class SavedConnection
    {
        public SavedConnection(string name, string host, string database, bool integratedSecurity, string userName, EnvironmentKind environment)
        {
            Name = name ?? string.Empty;
            Host = host ?? string.Empty;
            Database = database ?? string.Empty;
            IntegratedSecurity = integratedSecurity;
            UserName = userName ?? string.Empty;
            Environment = environment;
        }

        public string Name { get; }

        public string Host { get; }

        public string Database { get; }

        public bool IntegratedSecurity { get; }

        public string UserName { get; }

        public EnvironmentKind Environment { get; }

        /// <summary>What the picker lists next to the name.</summary>
        public string Description
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Database))
                {
                    return Host;
                }

                return string.IsNullOrWhiteSpace(Host) ? Database : Host + " / " + Database;
            }
        }
    }

    /// <summary>
    /// One source/destination pair from the Recent comparisons menu (section 6.1.6).
    /// </summary>
    public sealed class RecentPair
    {
        public RecentPair(string sourceHost, string sourceDatabase, string destinationHost, string destinationDatabase, DateTime lastRunUtc)
        {
            SourceHost = sourceHost ?? string.Empty;
            SourceDatabase = sourceDatabase ?? string.Empty;
            DestinationHost = destinationHost ?? string.Empty;
            DestinationDatabase = destinationDatabase ?? string.Empty;
            LastRunUtc = lastRunUtc;
        }

        public string SourceHost { get; }

        public string SourceDatabase { get; }

        public string DestinationHost { get; }

        public string DestinationDatabase { get; }

        public DateTime LastRunUtc { get; }

        /// <summary>Section 6.1.6: the menu lists the pair "with their direction".</summary>
        public string Direction
        {
            get
            {
                return ConnectionRules.ServerDisplay(SourceHost) + " / " + SourceDatabase
                    + " \u2192 "
                    + ConnectionRules.ServerDisplay(DestinationHost) + " / " + DestinationDatabase;
            }
        }

        /// <summary>The last-run time as the menu shows it, relative to <paramref name="nowUtc"/>.</summary>
        public string LastRunText(DateTime nowUtc)
        {
            var elapsed = nowUtc - LastRunUtc;

            if (elapsed.TotalSeconds < 60)
            {
                return "just now";
            }

            if (elapsed.TotalMinutes < 60)
            {
                var minutes = (int)elapsed.TotalMinutes;
                return minutes == 1 ? "1 minute ago" : minutes + " minutes ago";
            }

            if (elapsed.TotalHours < 24)
            {
                var hours = (int)elapsed.TotalHours;
                return hours == 1 ? "1 hour ago" : hours + " hours ago";
            }

            var days = (int)elapsed.TotalDays;
            return days == 1 ? "1 day ago" : days + " days ago";
        }
    }

    /// <summary>
    /// The ordering rules behind Saved connections and Recent comparisons (section 6.1.6): at most 50
    /// saved connections and the 10 most recent pairs, most recent first.
    /// </summary>
    public static class ConnectionLibraryRules
    {
        public const int MaxSavedConnections = 50;
        public const int MaxRecentPairs = 10;

        /// <summary>
        /// Adds a saved connection, or replaces the one with the same name, and keeps the list at its
        /// cap by dropping the oldest entry.
        /// </summary>
        public static IReadOnlyList<SavedConnection> Save(IReadOnlyList<SavedConnection> saved, SavedConnection entry)
        {
            var list = new List<SavedConnection>();

            if (entry != null)
            {
                list.Add(entry);
            }

            if (saved != null)
            {
                foreach (var candidate in saved)
                {
                    if (candidate != null && !string.Equals(candidate.Name, entry == null ? null : entry.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        list.Add(candidate);
                    }
                }
            }

            return list.Take(MaxSavedConnections).ToList();
        }

        /// <summary>
        /// Records a comparison: an already-known pair moves to the front with the new timestamp
        /// rather than appearing twice, and the list stays at ten.
        /// </summary>
        public static IReadOnlyList<RecentPair> Record(IReadOnlyList<RecentPair> recent, RecentPair pair)
        {
            var list = new List<RecentPair>();

            if (pair != null)
            {
                list.Add(pair);
            }

            if (recent != null)
            {
                foreach (var candidate in recent)
                {
                    if (candidate != null && (pair == null || !IsSamePair(candidate, pair)))
                    {
                        list.Add(candidate);
                    }
                }
            }

            return list.Take(MaxRecentPairs).ToList();
        }

        /// <summary>
        /// A pair is the same comparison when both sides point at the same server and database, in the
        /// same direction. The opposite direction is a different pair.
        /// </summary>
        public static bool IsSamePair(RecentPair left, RecentPair right)
        {
            if (left == null || right == null)
            {
                return false;
            }

            return Same(left.SourceHost, right.SourceHost)
                && Same(left.SourceDatabase, right.SourceDatabase)
                && Same(left.DestinationHost, right.DestinationHost)
                && Same(left.DestinationDatabase, right.DestinationDatabase);
        }

        private static bool Same(string left, string right)
        {
            return string.Equals(left ?? string.Empty, right ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }
    }
}
