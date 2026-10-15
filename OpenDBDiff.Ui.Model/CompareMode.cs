using System.Collections.Generic;
using System.Linq;
using OpenDBDiff.Abstractions.Schema;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>What a comparison looks at (section 6.1.5).</summary>
    public enum CompareMode
    {
        Schema,
        Data
    }

    /// <summary>
    /// The Schema / Data mode switch (section 6.1.5): Data replaces the old always-present "Compare
    /// data" button and is only offered once a schema compare has found tables that exist on both sides.
    /// </summary>
    public static class CompareModeRules
    {
        /// <summary>The Data segment's tooltip while it is disabled (section 6.1.5).</summary>
        public const string DataDisabledTooltip = "Compare schema first, then compare data for tables that exist on both sides.";

        /// <summary>
        /// Data mode is enabled only after a Schema compare has completed and at least one table is
        /// Identical or Alter - i.e. the table exists on both sides, so there is something to compare
        /// row by row.
        /// </summary>
        public static bool IsDataEnabled(bool schemaCompared, int tablesOnBothSides)
        {
            return schemaCompared && tablesOnBothSides > 0;
        }

        /// <summary>Tables that exist on both sides: Identical or Alter (section 6.1.5).</summary>
        public static int TablesOnBothSides(IEnumerable<DiffItem> items)
        {
            if (items == null)
            {
                return 0;
            }

            return items.Count(item =>
                item.Key.ObjectType == ObjectType.Table
                && (item.Status == DiffStatus.Identical || item.Status == DiffStatus.Alter));
        }
    }
}
