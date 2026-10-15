using System;
using System.Collections.Generic;
using System.Linq;
using OpenDBDiff.Abstractions.Schema;

namespace OpenDBDiff.Ui.Model
{
    /// <summary>One line of the pre-selection overview's type table (section 6.5.4).</summary>
    public sealed class OverviewTypeRow
    {
        public OverviewTypeRow(ObjectType type, string title, int create, int alter, int drop)
        {
            Type = type;
            Title = title;
            Create = create;
            Alter = alter;
            Drop = drop;
        }

        public ObjectType Type { get; }

        public string Title { get; }

        public int Create { get; }

        public int Alter { get; }

        public int Drop { get; }

        public int DifferenceCount
        {
            get { return Create + Alter + Drop; }
        }
    }

    /// <summary>Why an object appears under "Needs attention".</summary>
    public enum AttentionReason
    {
        /// <summary>A Drop, which is destructive and is not included by default.</summary>
        Drop,

        /// <summary>A dependency warning (section 9.4).</summary>
        Dependency,

        /// <summary>The engine reports it but cannot script it (section 9.5).</summary>
        Unsupported
    }

    /// <summary>One entry of "Needs attention", which is a link that focuses the object (section 6.5.4).</summary>
    public sealed class OverviewAttention
    {
        public OverviewAttention(DiffItemKey key, AttentionReason reason, string text, bool isBlocking)
        {
            Key = key;
            Reason = reason;
            Text = text;
            IsBlocking = isBlocking;
        }

        public DiffItemKey Key { get; }

        public AttentionReason Reason { get; }

        /// <summary>The line the overview shows, e.g. "Will be dropped".</summary>
        public string Text { get; }

        public bool IsBlocking { get; }
    }

    /// <summary>The detail pane before anything is focused (section 6.5.4).</summary>
    public sealed class PreselectionOverview
    {
        public PreselectionOverview(string heading, IReadOnlyList<OverviewTypeRow> rows, IReadOnlyList<OverviewAttention> attention)
        {
            Heading = heading;
            Rows = rows;
            Attention = attention;
        }

        /// <summary>"{n} differences across {k} object types".</summary>
        public string Heading { get; }

        public IReadOnlyList<OverviewTypeRow> Rows { get; }

        public IReadOnlyList<OverviewAttention> Attention { get; }

        public bool HasAttention
        {
            get { return Attention.Count > 0; }
        }
    }

    /// <summary>
    /// The pre-selection overview of section 6.5.4: what the detail pane shows when nothing is focused,
    /// and the "Needs attention" list that decides which drops and dependency problems get looked at
    /// first.
    /// </summary>
    public static class OverviewRules
    {
        /// <summary>
        /// The section asks for the drops, the items with dependency warnings and the items that cannot
        /// be scripted, in that order, with duplicates removed: an object that is both a drop and
        /// blocked appears once, with the reason that matters more.
        /// </summary>
        public static PreselectionOverview Build(IReadOnlyList<DiffItem> items, IReadOnlyList<DependencyWarning> warnings)
        {
            var all = items ?? new List<DiffItem>();
            var rows = TypeRows(all);
            var attention = Attention(all, warnings);
            var heading = string.Format("{0} differences across {1} object types",
                all.Count(item => item.Status != DiffStatus.Identical), rows.Count);

            return new PreselectionOverview(heading, rows, attention);
        }

        /// <summary>The type table, in the group order the review list uses.</summary>
        public static IReadOnlyList<OverviewTypeRow> TypeRows(IReadOnlyList<DiffItem> items)
        {
            var rows = new List<OverviewTypeRow>();

            if (items == null)
            {
                return rows;
            }

            var groups = items
                .Where(item => item.Status != DiffStatus.Identical)
                .GroupBy(item => item.Key.ObjectType)
                .OrderBy(group => TypeOrder.Of(group.Key))
                .ThenBy(group => TypeNames.Plural(group.Key), StringComparer.OrdinalIgnoreCase);

            foreach (var group in groups)
            {
                rows.Add(new OverviewTypeRow(
                    group.Key,
                    TypeNames.Plural(group.Key),
                    group.Count(item => item.Status == DiffStatus.Create),
                    group.Count(item => item.Status == DiffStatus.Alter),
                    group.Count(item => item.Status == DiffStatus.Drop)));
            }

            return rows;
        }

        public static IReadOnlyList<OverviewAttention> Attention(IReadOnlyList<DiffItem> items, IReadOnlyList<DependencyWarning> warnings)
        {
            var attention = new List<OverviewAttention>();
            var seen = new HashSet<DiffItemKey>();

            if (warnings != null)
            {
                foreach (var warning in warnings.Where(warning => warning.IsBlocking))
                {
                    if (seen.Add(warning.Item))
                    {
                        attention.Add(new OverviewAttention(warning.Item, AttentionReason.Dependency, warning.Message, true));
                    }
                }
            }

            if (items != null)
            {
                foreach (var item in items.Where(item => item.Status == DiffStatus.Drop))
                {
                    if (seen.Add(item.Key))
                    {
                        attention.Add(new OverviewAttention(item.Key, AttentionReason.Drop, "Will be dropped; not included by default.", false));
                    }
                }

                foreach (var item in items.Where(item => !item.IsSelectable))
                {
                    if (seen.Add(item.Key))
                    {
                        var reason = string.IsNullOrEmpty(item.UnsupportedReason)
                            ? "The engine cannot script this object."
                            : item.UnsupportedReason;

                        attention.Add(new OverviewAttention(item.Key, AttentionReason.Unsupported, reason, false));
                    }
                }
            }

            return attention;
        }
    }
}
