using System;
using System.Collections.Generic;
using System.Linq;
using OpenDBDiff.Abstractions.Schema;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>
    /// One statement of a generated script, as the confirmation dialog's statement list and the
    /// transaction decision need it (sections 6.8 and 14).
    /// </summary>
    /// <remarks>
    /// The engine supplies all four flags. The UI never reads <see cref="Sql"/> to work out whether a
    /// statement drops something or can be wrapped in a transaction - section 9.7 forbids it, and the
    /// engine is the side that knows.
    /// </remarks>
    public sealed class ScriptStatement
    {
        public ScriptStatement(string objectFullName, ScriptAction action, string sql, bool isTransactional, bool isDestructive)
        {
            ObjectFullName = objectFullName ?? string.Empty;
            Action = action;
            Sql = sql ?? string.Empty;
            IsTransactional = isTransactional;
            IsDestructive = isDestructive;
        }

        /// <summary>The object the statement belongs to, which is how the list groups and jumps.</summary>
        public string ObjectFullName { get; }

        public ScriptAction Action { get; }

        public string Sql { get; }

        /// <summary>False for the statements SQL Server refuses inside a transaction (section 6.8).</summary>
        public bool IsTransactional { get; }

        public bool IsDestructive { get; }

        /// <summary>How the statement list names the action, e.g. "DROP TABLE".</summary>
        public string ActionName
        {
            get { return ScriptTransactionRules.Description(Action); }
        }

        /// <summary>The statement's first line, which is what the list shows beside the object.</summary>
        public string FirstLine
        {
            get
            {
                var text = Sql.TrimStart();
                var end = text.IndexOfAny(new[] { '\r', '\n' });
                var line = end < 0 ? text : text.Substring(0, end);

                return line.Length > 120 ? line.Substring(0, 120) + "\u2026" : line;
            }
        }
    }

    /// <summary>
    /// Which statements can run inside one transaction, and the words used when some cannot
    /// (section 6.8).
    /// </summary>
    public static class ScriptTransactionRules
    {
        /// <summary>
        /// Section 6.8's transaction option defaults on, and is disabled with an explanation when any
        /// statement cannot be transactional. These are the statements SQL Server refuses: the
        /// ALTER DATABASE family (filegroups and files) and the full-text family, which the section
        /// names as its example.
        /// </summary>
        public static bool IsTransactional(ScriptAction action)
        {
            switch (action)
            {
                case ScriptAction.None:
                case ScriptAction.UseDatabase:
                case ScriptAction.AddFileGroup:
                case ScriptAction.AddFile:
                case ScriptAction.AlterFile:
                case ScriptAction.AlterFileGroup:
                case ScriptAction.DropFile:
                case ScriptAction.DropFileGroup:
                case ScriptAction.AddFullText:
                case ScriptAction.DropFullText:
                case ScriptAction.AddFullTextIndex:
                case ScriptAction.AlterFullTextIndex:
                case ScriptAction.DropFullTextIndex:
                    return false;
                default:
                    return true;
            }
        }

        public static bool AllTransactional(IEnumerable<ScriptStatement> statements)
        {
            return statements != null && statements.All(statement => statement.IsTransactional);
        }

        /// <summary>The reason shown on the disabled transaction checkbox.</summary>
        public static string DisabledReason(IEnumerable<ScriptStatement> statements)
        {
            var blockers = BlockingActions(statements);

            if (blockers.Count == 0)
            {
                return string.Empty;
            }

            return string.Format("{0} cannot run inside a transaction: {1}",
                blockers.Count == 1 ? "One statement" : "Some statements",
                string.Join(", ", blockers));
        }

        /// <summary>Section 6.8: "disabled with explanation if some are not, such as full-text index changes".</summary>
        public static IReadOnlyList<string> BlockingActions(IEnumerable<ScriptStatement> statements)
        {
            if (statements == null)
            {
                return new List<string>();
            }

            return statements
                .Where(statement => !statement.IsTransactional)
                .Select(statement => Description(statement.Action))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToList();
        }

        /// <summary>The plain words for one action, for the statement list and the explanations.</summary>
        public static string Description(ScriptAction action)
        {
            var name = action.ToString();

            // ScriptAction names are already verb-first (DropTable, AddIndex); the split makes them
            // readable and the upper case matches how the tool names actions elsewhere.
            var words = new List<string>();
            var current = string.Empty;

            foreach (var character in name)
            {
                if (char.IsUpper(character) && current.Length > 0)
                {
                    words.Add(current);
                    current = string.Empty;
                }

                current += character;
            }

            if (current.Length > 0)
            {
                words.Add(current);
            }

            return string.Join(" ", words).ToUpperInvariant();
        }
    }

    /// <summary>
    /// The statement list and the script the confirmation dialog shows (sections 6.8 and 6.7.1).
    /// </summary>
    public sealed class ScriptBundle
    {
        public ScriptBundle(IEnumerable<ScriptStatement> statements)
        {
            Statements = statements == null ? new List<ScriptStatement>() : statements.ToList();
        }

        public static ScriptBundle Empty { get; } = new ScriptBundle(null);

        public IReadOnlyList<ScriptStatement> Statements { get; }

        public int Count
        {
            get { return Statements.Count; }
        }

        public int DestructiveCount
        {
            get { return Statements.Count(statement => statement.IsDestructive); }
        }

        public IReadOnlyList<ScriptStatement> DestructiveStatements
        {
            get { return Statements.Where(statement => statement.IsDestructive).ToList(); }
        }

        public string Text
        {
            get { return string.Concat(Statements.Select(statement => statement.Sql)); }
        }

        /// <summary>Section 6.8's transaction checkbox: on when every statement allows it.</summary>
        public bool CanRunInTransaction
        {
            get { return ScriptTransactionRules.AllTransactional(Statements); }
        }

        public string TransactionDisabledReason
        {
            get { return ScriptTransactionRules.DisabledReason(Statements); }
        }
    }
}
