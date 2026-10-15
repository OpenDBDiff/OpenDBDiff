using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Ui.Model;
using OpenDBDiff.Wpf.Infrastructure;
using PropertyChanged;

namespace OpenDBDiff.Wpf.ViewModels
{
    public enum ResultsRowKind
    {
        Group,
        Item
    }

    /// <summary>
    /// One row of the review list. Group rows and item rows are the same list, which is what keeps the
    /// list virtualised and the group headers in place (section 6.4).
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed class ResultsRowViewModel
    {
        private readonly ResultsViewModel owner;

        public ResultsRowViewModel(ResultsViewModel owner, TypeGroup group)
        {
            this.owner = owner;
            Kind = ResultsRowKind.Group;
            Group = group;
            Title = group.Title;
            CountText = group.CountText;
            IsExpanded = group.IsExpanded;
        }

        public ResultsRowViewModel(ResultsViewModel owner, DiffItem item)
        {
            this.owner = owner;
            Kind = ResultsRowKind.Item;
            Item = item;
            Name = item.Key.FullName;
            Status = item.Status;
            IsUnsupported = item.Applicability == Applicability.Unsupported;
        }

        public ResultsRowKind Kind { get; }

        public TypeGroup Group { get; }

        public DiffItem Item { get; }

        public string Title { get; }

        public string CountText { get; }

        public string Name { get; }

        public DiffStatus Status { get; }

        public bool IsUnsupported { get; }

        public string UnsupportedReason
        {
            get { return Item == null ? null : Item.UnsupportedReason; }
        }

        /// <summary>Set when the row should carry the dependency flag of section 9.4.</summary>
        public bool IsWarning { get; set; }

        /// <summary>
        /// Section 6.9: a completed object's row shows its result instead of its checkbox. Empty means
        /// the row has no result yet, which is what the empty-to-visible converter keys off.
        /// </summary>
        public string ResultGlyph { get; private set; } = string.Empty;

        public string ResultTooltip { get; private set; } = string.Empty;

        public bool ResultIsFailure { get; private set; }

        /// <summary>Applies an item's apply outcome, or clears it when the run has been left behind.</summary>
        public void SetResult(ApplyItemOutcome? outcome)
        {
            if (outcome == null)
            {
                ResultGlyph = string.Empty;
                ResultTooltip = string.Empty;
                ResultIsFailure = false;
                return;
            }

            switch (outcome.Value)
            {
                case ApplyItemOutcome.Succeeded:
                    ResultGlyph = "\u2713";
                    ResultTooltip = "Succeeded";
                    break;
                case ApplyItemOutcome.Failed:
                    ResultGlyph = "\u2715";
                    ResultTooltip = "Failed";
                    break;
                default:
                    ResultGlyph = "\u2013";
                    ResultTooltip = "Skipped";
                    break;
            }

            ResultIsFailure = outcome.Value == ApplyItemOutcome.Failed;
        }

        /// <summary>Bound to the row's check box: null renders the indeterminate state on a group.</summary>
        public bool? CheckBoxState
        {
            get
            {
                switch (CheckState)
                {
                    case GroupCheckState.Checked: return true;
                    case GroupCheckState.Unchecked: return false;
                    default: return null;
                }
            }
            set
            {
                CheckState = value.HasValue
                    ? (value.Value ? GroupCheckState.Checked : GroupCheckState.Unchecked)
                    : GroupCheckState.Indeterminate;
            }
        }

        public bool CanToggle
        {
            get { return !IsUnsupported; }
        }

        public bool IsSelected { get; set; }

        public bool IsExpanded
        {
            get { return Group != null && Group.IsExpanded; }
            set
            {
                if (Group != null && Group.IsExpanded != value)
                {
                    Group.IsExpanded = value;
                    owner.RefreshRows();
                }
            }
        }

        public GroupCheckState CheckState
        {
            get
            {
                if (Kind == ResultsRowKind.Group)
                {
                    return owner.DiffSet.GroupCheckStateOf(Group);
                }

                return owner.DiffSet.IsIncluded(Item.Key) ? GroupCheckState.Checked : GroupCheckState.Unchecked;
            }
            set
            {
                if (Kind == ResultsRowKind.Group)
                {
                    owner.SetGroupChecked(Group, value);
                }
                else
                {
                    owner.SetIncluded(
                        Item.Key,
                        value != GroupCheckState.Unchecked,
                        (value == GroupCheckState.Unchecked ? "Exclude " : "Include ") + Name);
                }
            }
        }
    }

    /// <summary>One status chip in the summary bar (section 6.3).</summary>
    [AddINotifyPropertyChangedInterface]
    public sealed class StatusChipViewModel : Infrastructure.ObservableObject
    {
        private readonly ResultsViewModel owner;

        public StatusChipViewModel(ResultsViewModel owner, DiffStatus status)
        {
            this.owner = owner;
            Status = status;
            Label = StatusVocabulary.PlainLanguage(status);
            Tooltip = StatusVocabulary.ChipTooltip(status);
        }

        public DiffStatus Status { get; }

        public string Label { get; }

        public string Tooltip { get; }

        public int Count
        {
            get
            {
                var counts = owner.DiffSet.Counts;

                switch (Status)
                {
                    case DiffStatus.Create: return counts.Create;
                    case DiffStatus.Alter: return counts.Alter;
                    case DiffStatus.Drop: return counts.Drop;
                    default: return counts.Identical;
                }
            }
        }

        public string Text
        {
            get
            {
                if (Status == DiffStatus.Identical)
                {
                    return string.Format("{0} Identical ({1})", Count, IsOn ? "shown" : "hidden");
                }

                return string.Format("{0} {1}", Count, Label);
            }
        }

        /// <summary>Raised by the results when the counts changed, since the count is computed.</summary>
        public void RefreshCount()
        {
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(Text));
        }

        /// <summary>Pressed state: the status is part of the filter.</summary>
        public bool IsOn
        {
            get
            {
                return Status == DiffStatus.Identical
                    ? owner.DiffSet.ShowIdentical
                    : owner.DiffSet.StatusFilter.Contains(Status);
            }
            set
            {
                if (Status == DiffStatus.Identical)
                {
                    owner.DiffSet.ShowIdentical = value;
                }
                else if (value)
                {
                    owner.DiffSet.StatusFilter.Add(Status);
                }
                else
                {
                    owner.DiffSet.StatusFilter.Remove(Status);
                }

                owner.OnFilterChanged();
            }
        }
    }

    /// <summary>
    /// The results regions of the redesign - summary bar (6.3), review list (6.4), detail pane (6.5)
    /// and the counts the action bar shows (6.7) - over the rules that live in OpenDBDiff.Ui.Model.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed class ResultsViewModel : Infrastructure.ObservableObject
    {
        private readonly ObservableCollection<ResultsRowViewModel> rows = new ObservableCollection<ResultsRowViewModel>();
        private readonly List<DependencyWarning> warnings = new List<DependencyWarning>();
        private readonly Dictionary<DiffItemKey, ApplyItemOutcome> results = new Dictionary<DiffItemKey, ApplyItemOutcome>();
        private ResultsRowViewModel selectedRow;
        private DiffItem focused;

        public ResultsViewModel(DiffSet diffSet)
        {
            DiffSet = diffSet;
            Chips = new List<StatusChipViewModel>
            {
                new StatusChipViewModel(this, DiffStatus.Create),
                new StatusChipViewModel(this, DiffStatus.Alter),
                new StatusChipViewModel(this, DiffStatus.Drop),
                new StatusChipViewModel(this, DiffStatus.Identical)
            };

            SelectAllCommand = new RelayCommand(SelectAllVisible);
            ClearSelectionCommand = new RelayCommand(ClearSelection);
            RefreshRows();
        }

        public DiffSet DiffSet { get; }

        /// <summary>
        /// Raised whenever the included set changed. The shell listens, because the undo stack of
        /// section 9.8 and the dependency check of section 9.4 both have to run on every change.
        /// </summary>
        public event EventHandler? InclusionChanged;

        public IReadOnlyList<StatusChipViewModel> Chips { get; }

        public ObservableCollection<ResultsRowViewModel> Rows
        {
            get { return rows; }
        }

        public string SearchText
        {
            get { return DiffSet.SearchText; }
            set
            {
                DiffSet.SearchText = value;
                OnFilterChanged();
            }
        }

        public bool SearchDefinitions
        {
            get { return DiffSet.SearchDefinitions; }
            set
            {
                DiffSet.SearchDefinitions = value;
                OnFilterChanged();
            }
        }

        public bool IsGrouped
        {
            get { return DiffSet.ViewMode == ListViewMode.Grouped; }
            set
            {
                DiffSet.ViewMode = value ? ListViewMode.Grouped : ListViewMode.Flat;
                RefreshRows();
            }
        }

        public DetailMode DetailMode
        {
            get { return DiffSet.DetailMode; }
            set
            {
                DetailSet(value);
            }
        }

        public bool IsSideBySide
        {
            get { return DiffSet.DetailMode == DetailMode.SideBySide; }
        }

        public bool IsInline
        {
            get { return DiffSet.DetailMode == DetailMode.Inline; }
        }

        public bool IsScript
        {
            get { return DiffSet.DetailMode == DetailMode.Script; }
        }

        public RelayCommand SelectAllCommand { get; }

        public RelayCommand ClearSelectionCommand { get; }

        /// <summary>Section 6.4.1: acts on the visible items only.</summary>
        public bool? SelectAllState
        {
            get
            {
                switch (DiffSet.SelectAllCheckState)
                {
                    case GroupCheckState.Checked: return true;
                    case GroupCheckState.Unchecked: return false;
                    default: return null;
                }
            }
            set
            {
                if (value == true)
                {
                    DiffSet.IncludeAllVisible();
                    LastChange = "Select all visible";
                }
                else if (value == false)
                {
                    DiffSet.ExcludeAllVisible();
                    LastChange = "Clear the selection";
                }
                else
                {
                    // A tri-state box reporting the indeterminate state is not a request to change
                    // anything, so nothing is recorded for it.
                    return;
                }

                OnInclusionChanged();
            }
        }

        public int DifferenceCount
        {
            get { return DiffSet.TotalDifferenceCount; }
        }

        public bool HasDifferences
        {
            get { return DiffSet.HasDifferences; }
        }

        public bool IsEmpty
        {
            get { return !DiffSet.HasDifferences; }
        }

        public string SelectionText
        {
            get { return DiffSet.SelectionText; }
        }

        public string DestructiveText
        {
            get { return DiffSet.DestructiveText; }
        }

        public bool HasDestructive
        {
            get { return DiffSet.DestructiveSelected.Count > 0; }
        }

        public string DestructiveTooltip
        {
            get
            {
                var items = DiffSet.DestructiveSelected.Take(10).Select(item => item.Key.FullName);
                return string.Join(Environment.NewLine, items);
            }
        }

        public string FooterText
        {
            get
            {
                if (DiffSet.HasSearch || DiffSet.StatusFilter.Count < 3)
                {
                    return string.Format("{0} of {1} differences shown", DiffSet.VisibleDifferenceCount, DiffSet.TotalDifferenceCount);
                }

                var hidden = DiffSet.HiddenTypeCount;

                return hidden == 0
                    ? string.Empty
                    : string.Format("{0} object types with no differences are hidden", hidden);
            }
        }

        public bool HasFooter
        {
            get { return !string.IsNullOrEmpty(FooterText); }
        }

        /// <summary>The focused object, which is what the detail pane shows.</summary>
        public DiffItem FocusedItem
        {
            get { return focused; }
        }

        public ResultsRowViewModel SelectedRow
        {
            get { return selectedRow; }
            set
            {
                selectedRow = value;
                Focus(value?.Item?.Key);
            }
        }

        public string DetailTitle
        {
            get { return focused == null ? "Select an object to see its differences" : focused.Key.FullName; }
        }

        public string DetailSummary
        {
            get
            {
                if (focused == null)
                {
                    return string.Format("{0} differences across {1} object types", DiffSet.TotalDifferenceCount, DiffSet.Groups.Count);
                }

                return string.Format("{0} \u00b7 {1}", TypeNames.Singular(focused.Key.ObjectType), focused.DiffSummary);
            }
        }

        public DiffStatus DetailStatus
        {
            get { return focused == null ? DiffStatus.Identical : focused.Status; }
        }

        public bool HasFocusedItem
        {
            get { return focused != null; }
        }

        public IReadOnlyList<SideBySideRow> SideBySideRows
        {
            get
            {
                return focused == null
                    ? new List<SideBySideRow>()
                    : DiffText.SideBySide(focused.SourceDefinition, focused.DestinationDefinition);
            }
        }

        public IReadOnlyList<InlineRow> InlineRows
        {
            get
            {
                return focused == null
                    ? new List<InlineRow>()
                    : DiffText.Inline(focused.SourceDefinition, focused.DestinationDefinition, DiffSet.IgnoreWhitespace);
            }
        }

        public string ScriptText
        {
            get { return focused == null ? string.Empty : focused.Script; }
        }

        public bool ShowDestinationPlaceholder
        {
            get { return focused != null && focused.Status == DiffStatus.Create; }
        }

        public bool ShowSourcePlaceholder
        {
            get { return focused != null && focused.Status == DiffStatus.Drop; }
        }

        public void RefreshRows()
        {
            rows.Clear();

            if (DiffSet.ViewMode == ListViewMode.Flat)
            {
                foreach (var item in DiffSet.FlatItems)
                {
                    Add(new ResultsRowViewModel(this, item));
                }
            }
            else
            {
                foreach (var group in DiffSet.Groups)
                {
                    rows.Add(new ResultsRowViewModel(this, group));

                    if (group.IsExpanded)
                    {
                        foreach (var item in group.Items.Where(item => DiffSet.ShowIdentical || item.Status != DiffStatus.Identical))
                        {
                            Add(new ResultsRowViewModel(this, item));
                        }
                    }
                }
            }

            RefreshFocusedSelection();
        }

        /// <summary>
        /// Adds an item row with its warning flag and its apply result already applied: a row is rebuilt
        /// on every filter, grouping and inclusion change, so its state cannot be set once and forgotten.
        /// </summary>
        private void Add(ResultsRowViewModel row)
        {
            ApplyRowState(row);
            rows.Add(row);
        }

        public void Focus(DiffItemKey key)
        {
            DiffSet.Focus(key);
            focused = DiffSet.FocusedItem;

            RefreshFocusedSelection();
        }

        public void SetGroupChecked(TypeGroup group, GroupCheckState state)
        {
            var include = state == GroupCheckState.Checked;

            foreach (var item in group.Items.Where(item => item.IsSelectable))
            {
                DiffSet.SetIncluded(item.Key, include);
            }

            LastChange = (include ? "Include all of " : "Exclude all of ") + TypeNames.Plural(group.ObjectType);
            OnInclusionChanged();
        }

        /// <summary>
        /// Includes or excludes one object, naming the change so the undo stack of section 9.8 can say
        /// what Ctrl+Z would take back.
        /// </summary>
        public void SetIncluded(DiffItemKey key, bool included, string label)
        {
            DiffSet.SetIncluded(key, included);
            LastChange = label;
            OnInclusionChanged();
        }

        /// <summary>
        /// Replaces the whole included set in one step, which is what section 9.4's "Fix all" needs: the
        /// result is one undoable change rather than one per object it selected.
        /// </summary>
        public void ApplyInclusionChange(IReadOnlyList<DiffItemKey> included, string label)
        {
            SetInclusion(included);
            LastChange = label;
            OnInclusionChanged();
        }

        /// <summary>
        /// Restores a snapshot without treating it as a change. Undo and redo use it, because recording
        /// the restoration would make Ctrl+Z alternate between two states instead of walking back.
        /// </summary>
        public void RestoreInclusion(IReadOnlyList<DiffItemKey> included)
        {
            SetInclusion(included);

            OnPropertyChanged(nameof(SelectionText));
            OnPropertyChanged(nameof(DestructiveText));
            OnPropertyChanged(nameof(HasDestructive));
            OnPropertyChanged(nameof(DestructiveTooltip));
            RefreshRows();
        }

        /// <summary>What the last inclusion change was, for the undo label and the toast.</summary>
        public string LastChange { get; private set; }

        /// <summary>The change the detail pane is showing, as an index into the change starts.</summary>
        public int FocusedHunk { get; private set; } = -1;

        /// <summary>
        /// Raised when F7 or Shift+F7 moved to another change, so the detail pane can scroll to it. The
        /// scrolling itself belongs to the view; the position belongs to the model.
        /// </summary>
        public event EventHandler<int>? HunkFocused;

        public void FocusHunk(int index)
        {
            FocusedHunk = index;

            var handler = HunkFocused;

            if (handler != null)
            {
                handler(this, index);
            }
        }

        private void SetInclusion(IReadOnlyList<DiffItemKey> included)
        {
            var wanted = new HashSet<DiffItemKey>(included ?? new List<DiffItemKey>());

            foreach (var item in DiffSet.AllItems)
            {
                DiffSet.SetIncluded(item.Key, wanted.Contains(item.Key));
            }
        }

        public void OnInclusionChanged()
        {
            OnPropertyChanged(nameof(SelectionText));
            OnPropertyChanged(nameof(DestructiveText));
            OnPropertyChanged(nameof(HasDestructive));
            OnPropertyChanged(nameof(DestructiveTooltip));
            RefreshRows();

            var handler = InclusionChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        // ---- Dependency warnings (section 9.4) ------------------------------------------------------

        /// <summary>Everything the rules found, in the order they found it.</summary>
        public IReadOnlyList<DependencyWarning> Warnings
        {
            get { return warnings; }
        }

        public int WarningCount
        {
            get { return warnings.Count; }
        }

        public int BlockingWarningCount
        {
            get { return DependencyRules.BlockingCount(warnings); }
        }

        public bool HasBlockingWarnings
        {
            get { return BlockingWarningCount > 0; }
        }

        /// <summary>Section 9.4's "Fix {n} dependency problems first", for the Apply tooltip.</summary>
        public string BlockingText
        {
            get { return HasBlockingWarnings ? DependencyRules.FixFirstText(BlockingWarningCount) : string.Empty; }
        }

        /// <summary>Section 9.4's "Fix all", offered only when every blocking warning has a fix.</summary>
        public bool CanFixAll
        {
            get
            {
                var blocking = warnings.Where(warning => warning.IsBlocking).ToList();

                return blocking.Count > 0 && blocking.All(warning => warning.Fix.IsInclude);
            }
        }

        public int FixableCount
        {
            get { return warnings.Count(warning => warning.IsBlocking && warning.Fix.IsInclude); }
        }

        public void SetWarnings(IEnumerable<DependencyWarning> newWarnings)
        {
            warnings.Clear();

            if (newWarnings != null)
            {
                warnings.AddRange(newWarnings);
            }

            RefreshWarningState();
        }

        public IReadOnlyList<DependencyWarning> WarningsFor(DiffItemKey key)
        {
            return key == null
                ? new List<DependencyWarning>()
                : warnings.Where(warning => warning.Item.Equals(key)).ToList();
        }

        // ---- Apply results (sections 6.9 and 7.2's result icons) ------------------------------------

        public void SetResults(IEnumerable<ApplyItemResult> applyResults)
        {
            results.Clear();

            if (applyResults != null)
            {
                foreach (var result in applyResults)
                {
                    results[result.Key] = result.Outcome;
                }
            }

            RefreshResultState();
        }

        public void ClearResults()
        {
            results.Clear();
            RefreshResultState();
        }

        public bool HasResults()
        {
            return results.Count > 0;
        }

        private void RefreshWarningState()
        {
            OnPropertyChanged(nameof(Warnings));
            OnPropertyChanged(nameof(WarningCount));
            OnPropertyChanged(nameof(BlockingWarningCount));
            OnPropertyChanged(nameof(HasBlockingWarnings));
            OnPropertyChanged(nameof(BlockingText));
            OnPropertyChanged(nameof(CanFixAll));
            OnPropertyChanged(nameof(FixableCount));
            RefreshRows();
        }

        private void RefreshResultState()
        {
            foreach (var row in rows)
            {
                ApplyRowState(row);
            }
        }

        /// <summary>
        /// A row is rebuilt whenever the filter, the grouping or the inclusions change, so both the
        /// warning flag and the apply result are applied here rather than at the point they were set.
        /// </summary>
        private void ApplyRowState(ResultsRowViewModel row)
        {
            if (row == null || row.Kind != ResultsRowKind.Item || row.Item == null)
            {
                return;
            }

            row.IsWarning = warnings.Any(warning => warning.Item.Equals(row.Item.Key));

            ApplyItemOutcome outcome;

            row.SetResult(results.TryGetValue(row.Item.Key, out outcome) ? (ApplyItemOutcome?)outcome : null);
        }

        public void OnFilterChanged()
        {
            OnPropertyChanged(nameof(FooterText));
            OnPropertyChanged(nameof(HasFooter));
            CountsChanged();
        }

        /// <summary>
        /// The chips show totals that a filter does not change, so they are refreshed when the data
        /// changes rather than when a filter does.
        /// </summary>
        public void CountsChanged()
        {
            foreach (var chip in Chips)
            {
                chip.RefreshCount();
            }

            OnPropertyChanged(nameof(DifferenceCount));
            OnPropertyChanged(nameof(HasDifferences));
            OnPropertyChanged(nameof(IsEmpty));
            RefreshRows();
        }

        private void DetailSet(DetailMode mode)
        {
            DiffSet.DetailMode = mode;
            OnPropertyChanged(nameof(DetailMode));
            OnPropertyChanged(nameof(IsSideBySide));
            OnPropertyChanged(nameof(IsInline));
            OnPropertyChanged(nameof(IsScript));
            RefreshDetail();
        }

        private void SelectAllVisible()
        {
            DiffSet.IncludeAllVisible();
            LastChange = "Select all visible";
            OnInclusionChanged();
        }

        private void ClearSelection()
        {
            DiffSet.ExcludeAllVisible();
            LastChange = "Clear the selection";
            OnInclusionChanged();
        }

        private void RefreshFocusedSelection()
        {
            selectedRow = rows.FirstOrDefault(row => row.Kind == ResultsRowKind.Item && focused != null && row.Item.Key.Equals(focused.Key));

            foreach (var row in rows)
            {
                row.IsSelected = ReferenceEquals(row, selectedRow);
            }

            RefreshDetail();
        }

        private void RefreshDetail()
        {
            OnPropertyChanged(nameof(FocusedItem));
            OnPropertyChanged(nameof(DetailTitle));
            OnPropertyChanged(nameof(DetailSummary));
            OnPropertyChanged(nameof(DetailStatus));
            OnPropertyChanged(nameof(HasFocusedItem));
            OnPropertyChanged(nameof(SideBySideRows));
            OnPropertyChanged(nameof(InlineRows));
            OnPropertyChanged(nameof(ScriptText));
            OnPropertyChanged(nameof(ShowDestinationPlaceholder));
            OnPropertyChanged(nameof(ShowSourcePlaceholder));
        }
    }
}
