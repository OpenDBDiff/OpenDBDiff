using System;
using System.Collections.Generic;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model;
using OpenDBDiff.Ui.Model;

namespace OpenDBDiff.SqlServer.Driver
{
    /// <summary>
    /// Decides which items are destructive, and why (section 9.7).
    /// </summary>
    /// <remarks>
    /// Section 9.7 says the engine supplies these flags and that the UI must never read SQL text to
    /// work them out. Two questions are asked here instead, both of them of things the engine already
    /// knows:
    ///
    /// 1. What did the item's own script emit? An object-level DROP is in the list the section gives.
    /// 2. What does the schema model say the change removes? A dropped column has no action of its own -
    ///    it comes out inside an ALTER TABLE - and a narrowed type is a change the model can compare
    ///    directly, because the compared database keeps the table as it was in OriginalTable.
    ///
    /// A TRUNCATE is in the section's list but this tool never generates one, so nothing here looks for
    /// it; saying so is better than pretending the flag is covered.
    /// </remarks>
    public static class SqlServerDestructiveAnalysis
    {
        /// <summary>Sets every item's destructive flags from the schema model and its own script.</summary>
        public static void Apply(IReadOnlyList<DiffItem> items, Func<DiffItemKey, ISchemaBase> resolve)
        {
            if (items == null || resolve == null)
            {
                return;
            }

            foreach (var item in items)
            {
                if (item.Status == DiffStatus.Identical)
                {
                    continue;
                }

                var schema = SafeResolve(resolve, item.Key);
                var kinds = DestructiveKinds.None;

                if (item.Status == DiffStatus.Drop)
                {
                    kinds |= DestructiveKinds.DropObject;
                }

                foreach (var statement in SqlServerScriptBuilder.ForItem(item, schema))
                {
                    if (SqlServerScriptBuilder.IsDropObjectAction(statement.Action))
                    {
                        kinds |= DestructiveKinds.DropObject;
                    }
                }

                kinds |= RemovalsOf(schema);

                item.DestructiveKinds = kinds;
            }
        }

        /// <summary>What a table's own change removes: columns, and room in the columns that stay.</summary>
        public static DestructiveKinds RemovalsOf(ISchemaBase schema)
        {
            var table = schema as Table;

            if (table == null)
            {
                return DestructiveKinds.None;
            }

            var kinds = DestructiveKinds.None;

            foreach (Column column in table.Columns)
            {
                if (column.Status == ObjectStatus.Drop)
                {
                    kinds |= DestructiveKinds.DropColumn;
                }
            }

            if (Narrows(table))
            {
                kinds |= DestructiveKinds.DataLossRisk;
            }

            return kinds;
        }

        /// <summary>
        /// Whether the table's change can lose data in a column that is staying: a shorter or less
        /// precise type, or a NOT NULL column losing the default that filled it.
        /// </summary>
        public static bool Narrows(Table table)
        {
            if (table == null || table.OriginalTable == null)
            {
                return false;
            }

            foreach (Column column in table.Columns)
            {
                if (column.Status == ObjectStatus.Original || column.Status == ObjectStatus.Create || column.Status == ObjectStatus.Drop)
                {
                    continue;
                }

                var before = table.OriginalTable.Columns[column.FullName];

                if (before == null)
                {
                    continue;
                }

                if (SizesDiffer(before, column))
                {
                    return true;
                }

                if (DefaultWasRemoved(before, column))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool SizesDiffer(Column before, Column after)
        {
            // A different type is a rebuild rather than a narrowing, and this cannot tell whether the
            // new type is wider, so it is left alone.
            if (!string.Equals(before.Type, after.Type, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // A negative size means max, which is never a narrowing.
            if (before.Size > 0 && after.Size > 0 && after.Size < before.Size)
            {
                return true;
            }

            if (before.Precision > 0 && after.Precision > 0 && after.Precision < before.Precision)
            {
                return true;
            }

            return before.Scale > 0 && after.Scale > 0 && after.Scale < before.Scale;
        }

        /// <summary>Section 9.7's last clause: "dropping NOT NULL defaults on populated columns".</summary>
        private static bool DefaultWasRemoved(Column before, Column after)
        {
            return before.DefaultConstraint != null && after.DefaultConstraint == null && !after.IsNullable;
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
    }
}
