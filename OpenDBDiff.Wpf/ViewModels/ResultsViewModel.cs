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

        public bool IsWarning { get; private set; }

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
                    owner.DiffSet.SetIncluded(Item.Key, value != GroupCheckState.Unchecked);
                    owner.OnInclusionChanged();
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
                }
                else if (value == false)
                {
                    DiffSet.ExcludeAllVisible();
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
                    : DiffText.Inline(focused.SourceDefinition, focused.DestinationDefinition);
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
                    rows.Add(new ResultsRowViewModel(this, item));
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
                            rows.Add(new ResultsRowViewModel(this, item));
                        }
                    }
                }
            }

            RefreshFocusedSelection();
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

            OnInclusionChanged();
        }

        public void OnInclusionChanged()
        {
            OnPropertyChanged(nameof(SelectionText));
            OnPropertyChanged(nameof(DestructiveText));
            OnPropertyChanged(nameof(HasDestructive));
            OnPropertyChanged(nameof(DestructiveTooltip));
            RefreshRows();
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
            OnInclusionChanged();
        }

        private void ClearSelection()
        {
            DiffSet.ExcludeAllVisible();
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
