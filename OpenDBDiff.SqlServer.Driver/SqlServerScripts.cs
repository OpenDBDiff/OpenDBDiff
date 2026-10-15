using System;
using System.Collections.Generic;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.SqlServer.Driver
{
    /// <summary>
    /// A generated script: the statements in the order they must run, each tied to the item that
    /// produced it (section 14's "dependency-safe order and a per-item mapping").
    /// </summary>
    public sealed class GeneratedScript
    {
        private readonly Dictionary<DiffItemKey, List<ScriptStatement>> byItem = new Dictionary<DiffItemKey, List<ScriptStatement>>();

        public GeneratedScript(IEnumerable<ScriptStatement> statements, Dictionary<DiffItemKey, List<ScriptStatement>> byItem)
        {
            Bundle = new ScriptBundle(statements);
            this.byItem = byItem ?? new Dictionary<DiffItemKey, List<ScriptStatement>>();
        }

        public ScriptBundle Bundle { get; }

        public IReadOnlyList<ScriptStatement> Statements
        {
            get { return Bundle.Statements; }
        }

        public string Text
        {
            get { return Bundle.Text; }
        }

        public bool CanRunInTransaction
        {
            get { return Bundle.CanRunInTransaction; }
        }

        public string TransactionDisabledReason
        {
            get { return Bundle.TransactionDisabledReason; }
        }

        /// <summary>The statements one item contributed, which is what the statement list shows.</summary>
        public IReadOnlyList<ScriptStatement> For(DiffItemKey key)
        {
            List<ScriptStatement> found;

            return key != null && byItem.TryGetValue(key, out found) ? found : new List<ScriptStatement>();
        }
    }

    /// <summary>
    /// Turns items into statements, using each object's own generator.
    /// </summary>
    /// <remarks>
    /// The statements come from one <c>ToSqlDiff</c> call per item and are then put in the engine's own
    /// order with <see cref="SQLScript.CompareTo"/>, which is what <c>SQLScriptList.Sort</c> uses. Doing
    /// it this way rather than calling <c>Database.ToSqlDiff</c> once is what makes the per-item mapping
    /// possible, which section 14 requires and the confirmation dialog's statement list needs; the cost
    /// is that a statement two items both emit has to be de-duplicated here, which is done by the
    /// statement's own text.
    ///
    /// None of this is verified against a live server from this machine: it is a faithful reading of
    /// what the generators return, exercised only by the tests that pin the ordering rules.
    /// </remarks>
    public static class SqlServerScriptBuilder
    {
        public static GeneratedScript ForSelection(IReadOnlyList<DiffItem> items, Func<DiffItemKey, ISchemaBase> resolve)
        {
            return ForSelection(items, resolve, null);
        }

        public static GeneratedScript ForSelection(IReadOnlyList<DiffItem> items, Func<DiffItemKey, ISchemaBase> resolve, ICollection<DiffItemKey> selected)
        {
            var pairs = new List<KeyValuePair<DiffItemKey, SQLScript>>();
            var byItem = new Dictionary<DiffItemKey, List<ScriptStatement>>();

            if (items == null || resolve == null)
            {
                return new GeneratedScript(new List<ScriptStatement>(), byItem);
            }

            foreach (var item in items)
            {
                if (selected != null && !selected.Contains(item.Key))
                {
                    continue;
                }

                foreach (var script in ScriptsFor(item, resolve))
                {
                    pairs.Add(new KeyValuePair<DiffItemKey, SQLScript>(item.Key, script));
                }
            }

            // The engine's own ordering: deep first, then by action, then by dependency count.
            pairs.Sort((left, right) => left.Value.CompareTo(right.Value));

            var statements = new List<ScriptStatement>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var owners = new Dictionary<ScriptStatement, DiffItemKey>();

            foreach (var pair in pairs)
            {
                var sql = pair.Value.SQL ?? string.Empty;

                if (sql.Length == 0 || !seen.Add(sql))
                {
                    continue;
                }

                var item = Find(items, pair.Key);
                var statement = new ScriptStatement(
                    pair.Key.FullName,
                    pair.Value.Status,
                    sql,
                    ScriptTransactionRules.IsTransactional(pair.Value.Status),
                    IsDestructive(pair.Value.Status, item == null ? DestructiveKinds.None : item.DestructiveKinds));

                statements.Add(statement);

                var list = byItem.ContainsKey(pair.Key) ? byItem[pair.Key] : byItem[pair.Key] = new List<ScriptStatement>();

                list.Add(statement);
                owners[statement] = pair.Key;
            }

            // Section 6.7.1's per-object section comments: the first statement an item contributed, in the
            // order the script runs, is labelled with the item's name and what is happening to it. The
            // label is held beside the SQL rather than inside it, so the script a reader copies carries
            // the comments that make it readable and the script that runs stays the engine's.
            var headed = new HashSet<DiffItemKey>();

            foreach (var statement in statements)
            {
                DiffItemKey owner;

                if (!owners.TryGetValue(statement, out owner) || !headed.Add(owner))
                {
                    continue;
                }

                var ownerItem = Find(items, owner);

                if (ownerItem != null)
                {
                    statement.SectionHeader = StatusVocabulary.Badge(ownerItem.Status) + " " + ownerItem.Key.FullName;
                }
            }

            return new GeneratedScript(statements, byItem);
        }

        /// <summary>One item's own statements, which is what its Script view and its flags are built from.</summary>
        public static IReadOnlyList<ScriptStatement> ForItem(DiffItem item, ISchemaBase schema)
        {
            var statements = new List<ScriptStatement>();

            if (item == null || schema == null)
            {
                return statements;
            }

            foreach (var script in ScriptsFor(item, null, schema))
            {
                statements.Add(new ScriptStatement(
                    item.Key.FullName,
                    script.Status,
                    script.SQL ?? string.Empty,
                    ScriptTransactionRules.IsTransactional(script.Status),
                    IsDestructive(script.Status, item.DestructiveKinds)));
            }

            return statements;
        }

        /// <summary>
        /// Section 9.7's list, as far as this engine has an action for it: the object-level drops. A
        /// column drop has no action of its own - it comes out inside an ALTER TABLE - so that one comes
        /// from the model instead, and a TRUNCATE is never generated by this tool at all.
        /// </summary>
        public static bool IsDropObjectAction(ScriptAction action)
        {
            switch (action)
            {
                case ScriptAction.DropTable:
                // DropView and DropFunction share the value 17 in the enum, so one case covers both.
                case ScriptAction.DropView:
                case ScriptAction.DropStoredProcedure:
                case ScriptAction.DropTrigger:
                case ScriptAction.DropUser:
                case ScriptAction.DropRole:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Whether one statement is destructive, from what the engine emitted and what it already knows
        /// about the item. The column and data-loss cases ride along on the table's ALTER, because that
        /// is the statement they appear in.
        /// </summary>
        public static bool IsDestructive(ScriptAction action, DestructiveKinds kinds)
        {
            if (IsDropObjectAction(action))
            {
                return true;
            }

            var risky = (kinds & (DestructiveKinds.DropColumn | DestructiveKinds.DataLossRisk)) != DestructiveKinds.None;

            if (!risky)
            {
                return false;
            }

            return action == ScriptAction.AlterTable
                || action == ScriptAction.RebuildTable
                || action == ScriptAction.AlterColumn
                || action == ScriptAction.AlterColumnFormula;
        }

        private static IReadOnlyList<SQLScript> ScriptsFor(DiffItem item, Func<DiffItemKey, ISchemaBase> resolve, ISchemaBase schema = null)
        {
            var target = schema ?? (resolve == null ? null : SafeResolve(resolve, item.Key));

            if (target == null)
            {
                return new List<SQLScript>();
            }

            try
            {
                var scripts = target.ToSqlDiff(new List<ISchemaBase> { target });
                var result = new List<SQLScript>();

                for (var i = 0; i < scripts.Count; i++)
                {
                    if (scripts[i] != null && !string.IsNullOrEmpty(scripts[i].SQL))
                    {
                        result.Add(scripts[i]);
                    }
                }

                return result;
            }
            catch (Exception)
            {
                // An object whose script cannot be generated contributes nothing; the item is still
                // listed, and the item's own Script view says what went wrong.
                return new List<SQLScript>();
            }
        }

        private static ISchemaBase SafeResolve(Func<DiffItemKey, ISchemaBase> resolve, DiffItemKey key)
        {
            try
            {
                return resolve(key);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static DiffItem Find(IReadOnlyList<DiffItem> items, DiffItemKey key)
        {
            foreach (var item in items)
            {
                if (item.Key.Equals(key))
                {
                    return item;
                }
            }

            return null;
        }
    }
}
