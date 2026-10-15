using System;
using System.Collections.Generic;
using System.Linq;
using OpenDBDiff.Abstractions.Schema;

namespace OpenDBDiff.Ui.Model
{
    public sealed class DiffCounts
    {
        public int Create { get; set; }

        public int Alter { get; set; }

        public int Drop { get; set; }

        public int Identical { get; set; }

        public int Differences
        {
            get { return Create + Alter + Drop; }
        }
    }

    /// <summary>Result of asking whether Apply may run, with the reason when it may not (section 6.7).</summary>
    public sealed class ApplyDecision
    {
        public ApplyDecision(bool canApply, string disabledReason = null)
        {
            CanApply = canApply;
            DisabledReason = disabledReason;
        }

        public bool CanApply { get; }

        public string DisabledReason { get; }
    }

    public enum GroupCheckState
    {
        Unchecked,
        Indeterminate,
        Checked
    }

    /// <summary>One object-type group in the review list (section 6.4.2).</summary>
    public sealed class TypeGroup
    {
        public TypeGroup(DiffItemKey key, IReadOnlyList<DiffItem> items)
        {
            ObjectType = key.ObjectType;
            Items = items;

            // Groups with up to twelve differences start expanded; larger ones start collapsed.
            IsExpanded = items.Count <= 12;
        }

        public ObjectType ObjectType { get; }

        public IReadOnlyList<DiffItem> Items { get; }

        /// <summary>Differences after filters, which is what the group header reports.</summary>
        public int VisibleDifferenceCount
        {
            get { return Items.Count(item => item.Status != DiffStatus.Identical); }
        }

        public bool IsExpanded { get; set; }

        public string Title
        {
            get { return TypeNames.Plural(ObjectType); }
        }

        public string CountText
        {
            get
            {
                var count = VisibleDifferenceCount;

                return count == 1 ? "1 difference" : count + " differences";
            }
        }
    }

    /// <summary>
    /// The results of a comparison and every rule that decides what the user sees and what an apply
    /// would run: filtering and search (6.3, 9.3), inclusion (7.1), grouping (6.4.2) and enablement
    /// (6.7).
    /// </summary>
    public sealed class DiffSet
    {
        private readonly List<DiffItem> allItems;
        private readonly HashSet<DiffItemKey> included = new HashSet<DiffItemKey>();
        private readonly List<DiffItem> flat = new List<DiffItem>();

        public DiffSet(IEnumerable<DiffItem> items)
        {
            allItems = items.ToList();
            StatusFilter = new HashSet<DiffStatus> { DiffStatus.Create, DiffStatus.Alter, DiffStatus.Drop };
            SearchText = string.Empty;
            DetailMode = DetailMode.SideBySide;
            ViewMode = ListViewMode.Grouped;
            ApplyDefaults();
        }

        public IReadOnlyList<DiffItem> AllItems
        {
            get { return allItems; }
        }

        /// <summary>Statuses the list shows. Create, Alter and Drop are on by default.</summary>
        public HashSet<DiffStatus> StatusFilter { get; }

        /// <summary>The Identical chip: hidden until the user asks for them.</summary>
        public bool ShowIdentical { get; set; }

        public string SearchText { get; set; }

        /// <summary>Also match inside object definitions; off by default because it is more expensive.</summary>
        public bool SearchDefinitions { get; set; }

        public ListViewMode ViewMode { get; set; }

        public DetailMode DetailMode { get; set; }

        public DiffItemKey Focused { get; private set; }

        /// <summary>True when a comparison produced any difference at all.</summary>
        public bool HasDifferences
        {
            get { return Counts.Differences > 0; }
        }

        public DiffCounts Counts
        {
            get
            {
                var counts = new DiffCounts();

                foreach (var item in allItems)
                {
                    switch (item.Status)
                    {
                        case DiffStatus.Create:
                            counts.Create++;
                            break;
                        case DiffStatus.Alter:
                            counts.Alter++;
                            break;
                        case DiffStatus.Drop:
                            counts.Drop++;
                            break;
                        default:
                            counts.Identical++;
                            break;
                    }
                }

                return counts;
            }
        }

        /// <summary>
        /// Section 9.3: status filter, then the identical toggle, then search.
        /// </summary>
        public IReadOnlyList<DiffItem> VisibleItems
        {
            get
            {
                return allItems.Where(IsVisible).ToList();
            }
        }

        /// <summary>Flat list, sorted by name ascending, as section 6.4.1 specifies.</summary>
        public IReadOnlyList<DiffItem> FlatItems
        {
            get
            {
                flat.Clear();
                flat.AddRange(VisibleItems.Where(item => ShowIdentical || item.Status != DiffStatus.Identical));
                flat.Sort((left, right) => string.Compare(left.Key.FullName, right.Key.FullName, StringComparison.OrdinalIgnoreCase));
                return flat;
            }
        }

        /// <summary>
        /// Groups in the fixed order of section 6.4.2. Groups with nothing to show are not returned;
        /// the footer says how many were hidden.
        /// </summary>
        public IReadOnlyList<TypeGroup> Groups
        {
            get
            {
                var visible = VisibleItems
                    .Where(item => ShowIdentical || item.Status != DiffStatus.Identical)
                    .ToList();

                return visible
                    .GroupBy(item => item.Key.ObjectType)
                    .OrderBy(group => TypeOrder.Of(group.Key))
                    .ThenBy(group => TypeNames.Plural(group.Key), StringComparer.OrdinalIgnoreCase)
                    .Select(group => new TypeGroup(group.First().Key, group.ToList()))
                    .ToList();
            }
        }

        /// <summary>Object types with differences that no group is showing.</summary>
        public int HiddenTypeCount
        {
            get
            {
                var withDifferences = allItems
                    .Where(item => item.Status != DiffStatus.Identical)
                    .Select(item => item.Key.ObjectType)
                    .Distinct()
                    .Count();

                return withDifferences - Groups.Count;
            }
        }

        public int VisibleDifferenceCount
        {
            get { return Groups.Sum(group => group.VisibleDifferenceCount); }
        }

        public bool HasSearch
        {
            get { return !string.IsNullOrWhiteSpace(SearchText); }
        }

        public bool IsIncluded(DiffItemKey key)
        {
            return included.Contains(key);
        }

        public void SetIncluded(DiffItemKey key, bool isIncluded)
        {
            var item = allItems.FirstOrDefault(candidate => candidate.Key.Equals(key));

            // Unsupported items are never included (section 9.5).
            if (item != null && !item.IsSelectable)
            {
                included.Remove(key);
                return;
            }

            if (isIncluded)
            {
                included.Add(key);
            }
            else
            {
                included.Remove(key);
            }
        }

        public void IncludeAllVisible()
        {
            foreach (var item in VisibleItems.Where(item => item.IsSelectable))
            {
                included.Add(item.Key);
            }
        }

        public void ExcludeAllVisible()
        {
            foreach (var item in VisibleItems)
            {
                included.Remove(item.Key);
            }
        }

        /// <summary>
        /// Default inclusion: Create and Alter in, Drop out (section 6.4.3 and 7.1 rule 4).
        /// </summary>
        public void ApplyDefaults()
        {
            included.Clear();

            foreach (var item in allItems)
            {
                if (item.IsSelectable && item.Status != DiffStatus.Drop)
                {
                    included.Add(item.Key);
                }
            }
        }

        /// <summary>
        /// Carries inclusion over to a new comparison, keeping the choices for objects that are still
        /// there (section 7.1 rule 5) and applying the defaults to the ones that are new.
        /// </summary>
        public void CarryInclusionFrom(IEnumerable<DiffItemKey> previousIncluded, IEnumerable<DiffItemKey> previousExcluded)
        {
            var previous = new HashSet<DiffItemKey>(previousIncluded ?? Enumerable.Empty<DiffItemKey>());
            var excluded = new HashSet<DiffItemKey>(previousExcluded ?? Enumerable.Empty<DiffItemKey>());
            var carried = new HashSet<DiffItemKey>();

            foreach (var item in allItems.Where(item => item.IsSelectable))
            {
                if (previous.Contains(item.Key))
                {
                    carried.Add(item.Key);
                }
                else if (!excluded.Contains(item.Key) && item.Status != DiffStatus.Drop)
                {
                    carried.Add(item.Key);
                }
            }

            included.Clear();

            foreach (var key in carried)
            {
                included.Add(key);
            }
        }

        public IReadOnlyList<DiffItemKey> IncludedKeys
        {
            get { return included.ToList(); }
        }

        public IReadOnlyList<DiffItemKey> ExcludedKeys
        {
            get
            {
                return allItems
                    .Where(item => item.IsSelectable && !included.Contains(item.Key))
                    .Select(item => item.Key)
                    .ToList();
            }
        }

        /// <summary>What an apply would run, in list order.</summary>
        public IReadOnlyList<DiffItem> IncludedItems
        {
            get { return allItems.Where(item => included.Contains(item.Key)).ToList(); }
        }

        public int SelectedCount
        {
            get { return allItems.Count(item => included.Contains(item.Key)); }
        }

        public int TotalDifferenceCount
        {
            get { return Counts.Differences; }
        }

        public int HiddenSelectedCount
        {
            get { return allItems.Count(item => included.Contains(item.Key) && !IsVisible(item)); }
        }

        public IReadOnlyList<DiffItem> DestructiveSelected
        {
            get { return IncludedItems.Where(item => item.IsDestructive).ToList(); }
        }

        public void Focus(DiffItemKey key)
        {
            Focused = key;
        }

        public DiffItem FocusedItem
        {
            get { return Focused == null ? null : allItems.FirstOrDefault(item => item.Key.Equals(Focused)); }
        }

        public GroupCheckState GroupCheckStateOf(TypeGroup group)
        {
            var selectable = group.Items.Where(item => item.IsSelectable).ToList();

            if (selectable.Count == 0)
            {
                return GroupCheckState.Unchecked;
            }

            var checkedCount = selectable.Count(item => included.Contains(item.Key));

            if (checkedCount == 0)
            {
                return GroupCheckState.Unchecked;
            }

            return checkedCount == selectable.Count ? GroupCheckState.Checked : GroupCheckState.Indeterminate;
        }

        /// <summary>Section 6.4.1: acts on the visible selectable items.</summary>
        public GroupCheckState SelectAllCheckState
        {
            get
            {
                var selectable = VisibleItems.Where(item => item.IsSelectable).ToList();

                if (selectable.Count == 0)
                {
                    return GroupCheckState.Unchecked;
                }

                var checkedCount = selectable.Count(item => included.Contains(item.Key));

                if (checkedCount == 0)
                {
                    return GroupCheckState.Unchecked;
                }

                return checkedCount == selectable.Count ? GroupCheckState.Checked : GroupCheckState.Indeterminate;
            }
        }

        /// <summary>
        /// Section 6.7: everything that has to hold before Apply is offered, with the reason shown in
        /// its tooltip when it is not.
        /// </summary>
        public ApplyDecision CanApply(AppState state, bool isStale, bool destinationValid, bool destinationIsReadOnly)
        {
            if (state != AppState.Results)
            {
                return new ApplyDecision(false, "There are no results to apply.");
            }

            if (isStale)
            {
                return new ApplyDecision(false, "Results are out of date. Compare again.");
            }

            if (SelectedCount == 0)
            {
                return new ApplyDecision(false, "Nothing is selected. Choose at least one difference to apply.");
            }

            if (!destinationValid)
            {
                return new ApplyDecision(false, "The destination connection is not valid.");
            }

            if (destinationIsReadOnly)
            {
                return new ApplyDecision(false, "This connection is set to never be written to.");
            }

            return new ApplyDecision(true);
        }

        /// <summary>The Apply button's label: count, database and server (section 6.7).</summary>
        public string ApplyLabel(string database, string server)
        {
            return string.Format("Review and apply {0} changes to {1} on {2} \u2192", SelectedCount, database, server);
        }

        public string SelectionText
        {
            get
            {
                if (TotalDifferenceCount == 0)
                {
                    return "No differences";
                }

                return SelectedCount == 0
                    ? "Nothing selected"
                    : string.Format("{0} of {1} selected", SelectedCount, TotalDifferenceCount);
            }
        }

        public string DestructiveText
        {
            get
            {
                var destructive = DestructiveSelected;

                if (destructive.Count == 0)
                {
                    return string.Empty;
                }

                var kinds = destructive
                    .Select(item => item.DestructiveDescription)
                    .Distinct()
                    .OrderBy(kind => kind, StringComparer.OrdinalIgnoreCase);

                return string.Format("\u00b7 {0} destructive ({1})", destructive.Count, string.Join(", ", kinds));
            }
        }

        private bool IsVisible(DiffItem item)
        {
            if (!StatusFilter.Contains(item.Status) && item.Status != DiffStatus.Identical)
            {
                return false;
            }

            if (item.Status == DiffStatus.Identical && !ShowIdentical)
            {
                return false;
            }

            return Matches(item);
        }

        /// <summary>
        /// Search matches the object name, the schema-qualified name and the object type name, case
        /// insensitively (section 6.3).
        /// </summary>
        private bool Matches(DiffItem item)
        {
            if (!HasSearch)
            {
                return true;
            }

            var query = SearchText.Trim();

            if (item.Key.FullName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            if (TypeNames.Singular(item.Key.ObjectType).IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            if (TypeNames.Plural(item.Key.ObjectType).IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            if (SearchDefinitions)
            {
                if (Contains(item.SourceDefinition, query) || Contains(item.DestinationDefinition, query))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool Contains(string text, string query)
        {
            return text != null && text.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
