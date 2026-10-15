using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>One report folder on disk (section 11.2).</summary>
    public sealed class ReportFolderEntry
    {
        public ReportFolderEntry(string name, string path, DateTime createdUtc)
        {
            Name = name;
            Path = path;
            CreatedUtc = createdUtc;
        }

        public string Name { get; }

        public string Path { get; }

        public DateTime CreatedUtc { get; }
    }

    /// <summary>
    /// Where a report lives and what it is called (section 11.2):
    /// <c>%APPDATA%\OpenDBDiff\Reports\{yyyyMMdd-HHmmss}-{db}\script.sql</c> and <c>report.html</c>.
    /// </summary>
    public static class ReportPaths
    {
        public const string Folder = "Reports";
        public const string ScriptFileName = "script.sql";
        public const string ReportFileName = "report.html";

        /// <summary>
        /// The folder for one run. A database name can contain characters a path cannot, so it is
        /// reduced to something every file system accepts before it is used.
        /// </summary>
        public static string FolderName(DateTime when, string database)
        {
            var stamp = when.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);
            var name = SafeName(database);

            return name.Length == 0 ? stamp : stamp + "-" + name;
        }

        public static string SafeName(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            // A fixed set rather than the running platform's: a report is written by a front end that
            // only runs on Windows, and the same run on any build machine must produce the same folder
            // name. Every character Windows refuses is refused here.
            var invalid = new HashSet<char>("<>:\"/\\|?*");

            foreach (var character in System.IO.Path.GetInvalidFileNameChars())
            {
                invalid.Add(character);
            }

            var builder = new StringBuilder();

            foreach (var character in text.Trim())
            {
                builder.Append(invalid.Contains(character) ? '_' : character);
            }

            var name = builder.ToString();

            // A path segment also has to survive the file system's length limit.
            return name.Length > 60 ? name.Substring(0, 60) : name;
        }

        public static string Root(string applicationDataFolder)
        {
            return System.IO.Path.Combine(applicationDataFolder, "OpenDBDiff", Folder);
        }

        public static string FolderPath(string applicationDataFolder, DateTime when, string database)
        {
            return System.IO.Path.Combine(Root(applicationDataFolder), FolderName(when, database));
        }
    }

    /// <summary>
    /// Section 11.2's retention rule: a report is kept while it is younger than ninety days, or while it
    /// is one of the newest two hundred - whichever keeps more. Only a folder that fails both is
    /// expired, which is what "or ... whichever is greater" means.
    /// </summary>
    public static class ReportRetentionRules
    {
        public const int Days = 90;
        public const int Entries = 200;

        public static IReadOnlyList<ReportFolderEntry> Expired(IReadOnlyList<ReportFolderEntry> folders, DateTime nowUtc)
        {
            return Select(folders, nowUtc, expired: true);
        }

        public static IReadOnlyList<ReportFolderEntry> Kept(IReadOnlyList<ReportFolderEntry> folders, DateTime nowUtc)
        {
            return Select(folders, nowUtc, expired: false);
        }

        private static IReadOnlyList<ReportFolderEntry> Select(IReadOnlyList<ReportFolderEntry> folders, DateTime nowUtc, bool expired)
        {
            if (folders == null || folders.Count == 0)
            {
                return new List<ReportFolderEntry>();
            }

            var oldest = nowUtc.AddDays(-Days);

            var ranked = folders
                .OrderByDescending(folder => folder.CreatedUtc)
                .ThenBy(folder => folder.Name, StringComparer.Ordinal)
                .ToList();

            var selected = new List<ReportFolderEntry>();

            for (var i = 0; i < ranked.Count; i++)
            {
                var oldEnough = ranked[i].CreatedUtc < oldest;
                var beyondTheCount = i >= Entries;

                if (expired == (oldEnough && beyondTheCount))
                {
                    selected.Add(ranked[i]);
                }
            }

            return selected;
        }
    }

    /// <summary>
    /// The report file's two formats (section 6.9's "Save report… (HTML or CSV)"). No UI toolkit is
    /// involved, so both are testable.
    /// </summary>
    public static class ReportFormat
    {
        private static readonly string[] Columns = { "Object", "Action", "Result", "Duration", "Message" };

        public static string ToCsv(ApplyReport report)
        {
            var builder = new StringBuilder();

            builder.AppendLine(string.Join(",", Columns.Select(Escape)));
            builder.AppendLine(string.Join(",", new[]
            {
                Escape("Target"),
                Escape(report.Target.Database),
                Escape(report.Target.Server),
                Escape(report.DurationText),
                Escape(report.OutcomeText)
            }));

            foreach (var item in report.Items)
            {
                builder.AppendLine(string.Join(",", new[]
                {
                    Escape(item.Key.FullName),
                    Escape(item.Action),
                    Escape(item.ResultText),
                    Escape(item.DurationText),
                    Escape(item.Message)
                }));
            }

            return builder.ToString();
        }

        public static string ToHtml(ApplyReport report)
        {
            var builder = new StringBuilder();

            builder.AppendLine("<!DOCTYPE html>");
            builder.AppendLine("<html><head><meta charset=\"utf-8\">");
            builder.AppendLine("<title>" + Html(report.Header) + "</title>");
            builder.AppendLine("<style>body{font-family:Segoe UI,sans-serif;margin:24px;color:#1B2230}");
            builder.AppendLine("table{border-collapse:collapse;width:100%}th,td{border:1px solid #CBD2DE;padding:6px 8px;text-align:left;font-size:13px}");
            builder.AppendLine("th{background:#EEF0F4}.failed{color:#A5210F}.ok{color:#0F6B38}</style>");
            builder.AppendLine("</head><body>");

            builder.AppendLine("<h1>" + Html(report.Header) + "</h1>");
            builder.AppendLine("<p class=\"" + CssClass(report) + "\">" + Html(report.OutcomeText) + "</p>");

            builder.AppendLine("<table>");
            builder.AppendLine("<tr><th>Server</th><td>" + Html(report.Target.Server) + "</td></tr>");
            builder.AppendLine("<tr><th>Database</th><td>" + Html(report.Target.Database) + "</td></tr>");

            if (report.Target.Environment.HasValue)
            {
                builder.AppendLine("<tr><th>Environment</th><td>" + Html(report.Target.Environment.Text) + "</td></tr>");
            }

            builder.AppendLine("<tr><th>Started</th><td>" + Html(report.StartedAt.ToString("u", CultureInfo.InvariantCulture)) + "</td></tr>");
            builder.AppendLine("<tr><th>Duration</th><td>" + Html(report.DurationText) + "</td></tr>");
            builder.AppendLine("<tr><th>Transaction</th><td>" + (report.UsedTransaction ? "yes" : "no") + "</td></tr>");
            builder.AppendLine("</table>");

            builder.AppendLine("<h2>Results</h2>");
            builder.AppendLine("<table><tr>" + string.Concat(Columns.Select(column => "<th>" + Html(column) + "</th>")) + "</tr>");

            foreach (var item in report.Items)
            {
                var css = item.Outcome == ApplyItemOutcome.Failed ? "failed" : "ok";

                builder.AppendLine("<tr><td>" + Html(item.Key.FullName) + "</td>");
                builder.AppendLine("<td>" + Html(item.Action) + "</td>");
                builder.AppendLine("<td class=\"" + css + "\">" + Html(item.ResultText) + "</td>");
                builder.AppendLine("<td>" + Html(item.DurationText) + "</td>");
                builder.AppendLine("<td>" + Html(item.Message) + "</td></tr>");
            }

            builder.AppendLine("</table>");
            builder.AppendLine("<p>The script this report describes is in <code>" + Html(ReportPaths.ScriptFileName) + "</code>.</p>");
            builder.AppendLine("</body></html>");

            return builder.ToString();
        }

        /// <summary>
        /// HTML's own escaping. An object can be named anything at all, and a report is opened in a
        /// browser, so nothing that came from the database goes in unescaped.
        /// </summary>
        public static string Html(string text)
        {
            var value = text ?? string.Empty;

            return value
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;");
        }

        /// <summary>CSV's own escaping: quotes doubled, and the field quoted only when it needs to be.</summary>
        public static string Escape(string text)
        {
            var value = text ?? string.Empty;

            if (value.IndexOfAny(new[] { ',', '"', '\r', '\n' }) < 0)
            {
                return value;
            }

            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        private static string CssClass(ApplyReport report)
        {
            switch (report.Outcome)
            {
                case ApplyOutcome.Succeeded: return "ok";
                default: return "failed";
            }
        }
    }
}
