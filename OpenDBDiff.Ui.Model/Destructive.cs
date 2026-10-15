using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>
    /// The words the destructive count and the confirmation dialog use (sections 6.7 and 6.8).
    /// </summary>
    /// <remarks>
    /// Section 9.7 is explicit that the engine decides what is destructive and the UI never reads SQL
    /// text to find out; this type only puts names to the flags the engine set.
    /// </remarks>
    public static class DestructiveRules
    {
        /// <summary>Section 6.8's typed confirmation is required once anything destructive is included.</summary>
        public static bool RequiresConfirmation(DestructiveKinds kinds)
        {
            return kinds != DestructiveKinds.None;
        }

        public static DestructiveKinds Merge(IEnumerable<DiffItem> items)
        {
            var kinds = DestructiveKinds.None;

            if (items == null)
            {
                return kinds;
            }

            foreach (var item in items)
            {
                kinds |= item.DestructiveKinds;
            }

            return kinds;
        }

        /// <summary>
        /// The short list in the Action Bar's destructive count: "DROP", "DROP COLUMN",
        /// "data loss risk" (section 6.7).
        /// </summary>
        public static IReadOnlyList<string> Describe(DestructiveKinds kinds)
        {
            var descriptions = new List<string>();

            if ((kinds & DestructiveKinds.DropObject) == DestructiveKinds.DropObject)
            {
                descriptions.Add("DROP");
            }

            if ((kinds & DestructiveKinds.DropColumn) == DestructiveKinds.DropColumn)
            {
                descriptions.Add("DROP COLUMN");
            }

            if ((kinds & DestructiveKinds.DataLossRisk) == DestructiveKinds.DataLossRisk)
            {
                descriptions.Add("data loss risk");
            }

            return descriptions;
        }

        /// <summary>
        /// The one-line summary the statement list and the report use, in the same order every time.
        /// </summary>
        public static string Summary(DestructiveKinds kinds)
        {
            var descriptions = Describe(kinds);

            return descriptions.Count == 0 ? string.Empty : string.Join(", ", descriptions);
        }
    }
}
