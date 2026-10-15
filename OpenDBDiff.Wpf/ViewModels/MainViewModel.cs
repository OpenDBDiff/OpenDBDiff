using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Driver;
using OpenDBDiff.SqlServer.Schema.Options;
using OpenDBDiff.Ui.Model;
using OpenDBDiff.Wpf.Infrastructure;
using OpenDBDiff.Wpf.Services;
using PropertyChanged;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>
    /// The shell of the redesigned window: the connection strip and its panel (section 6.1), the
    /// progress bar (6.2), the summary bar, review list, detail pane and action bar of section 5.1,
    /// the stale-results rules of 9.6, and the menus of section 12.
    /// </summary>
    /// <remarks>
    /// The comparison pipeline is unchanged from the previous front end: the right-hand panel is loaded
    /// first and the left-hand second, and the comparer marks the left-hand snapshot up as the desired
    /// state. In the specification's words (section 3) the left-hand panel is the Source, whose
    /// definitions are the desired state, and the right-hand panel is the Destination that an apply
    /// modifies - which is what the strip, the detail pane's column headers and the apply button say.
    ///
    /// Later phases are still missing and deliberately so: dependency checks and destructive detection
    /// (Phase 3), the apply report and history (Phase 3), streaming results, the flat list minimap and
    /// the dark theme (Phase 4).
    /// </remarks>
    [AddINotifyPropertyChangedInterface]
    public sealed class MainViewModel : Infrastructure.ObservableObject
    {
        private const int MaxRecentProjects = 10;

        private readonly IComparisonService comparisons;
        private readonly ISettingsStore settings;
        private readonly IDialogService dialogs;
        private readonly IUpdateConfirmation updateConfirmation;
        private readonly SqlOption options;
        private readonly IThemeService themes;
        private readonly ReportStore reports = new ReportStore();
        private readonly InclusionHistory history = new InclusionHistory();

        private IDatabase? sourceDatabase;
        private IDatabase? destinationDatabase;
        private IDependencyGraph? dependencies;
        private ResultsViewModel? subscribedResults;

        private CancellationTokenSource? cancellation;
        private Task<ComparisonResult>? runningCompare;
        private CancellationTokenSource? applyCancellation;
        private int compareGeneration;
        private int applyGeneration;
        private int hunkIndex = -1;

        private List<SavedConnection> savedConnections = new List<SavedConnection>();
        private List<RecentPair> recentPairs = new List<RecentPair>();
        private List<string> recentProjects = new List<string>();
        private bool includeDropByDefault;

        /// <summary>Where File → Save project writes, once a project has been opened or saved as (section 12).</summary>
        private string? currentProjectPath;

        /// <summary>
        /// Section 11.1's inclusionKeys, waiting for the comparison that can receive them. A project's
        /// included objects only mean something against the items a comparison produces, so they are held
        /// until one does rather than being applied to nothing.
        /// </summary>
        private List<string>? pendingInclusion;

        public MainViewModel(
            IComparisonService comparisons,
            ISettingsStore settings,
            IDialogService dialogs,
            IThemeService themes)
        {
            this.comparisons = comparisons;
            this.settings = settings;
            this.dialogs = dialogs;
            this.themes = themes;

            options = new SqlOption(true);
            updateConfirmation = new WpfUpdateConfirmation(dialogs);

            Source = new ConnectionViewModel("Source database:", dialogs);
            Destination = new ConnectionViewModel("Destination database:", dialogs);
            Progress = new ProgressViewModel(string.Empty, string.Empty);

            Destination.PropertyChanged += OnDestinationPropertyChanged;
            Source.Edited += OnConnectionEdited;
            Destination.Edited += OnConnectionEdited;
            Source.Saved += OnConnectionSaved;
            Destination.Saved += OnConnectionSaved;

            CompareCommand = new AsyncRelayCommand(CompareAsync, () => CanInvokeCompare && !IsRunning);
            CompareAgainCommand = new AsyncRelayCommand(CompareAsync, () => CanInvokeCompare && !IsRunning);
            CancelCommand = new RelayCommand(CancelCompare, () => State == AppState.Comparing || State == AppState.Applying);
            SwapCommand = new RelayCommand(Swap, () => !IsRunning);
            OptionsCommand = new RelayCommand(ShowOptions);
            ApplyCommand = new AsyncRelayCommand(ApplyAsync, () => CanApply);
            PreviewScriptCommand = new RelayCommand(PreviewScript, () => HasScript);
            CloseScriptPreviewCommand = new RelayCommand(CloseScriptPreview, () => IsScriptPreviewOpen);
            ClearSearchCommand = new RelayCommand(ClearSearch, () => Results != null && !string.IsNullOrEmpty(Results.SearchText));
            CopyScriptCommand = new RelayCommand(CopyScript, () => HasScript);
            SaveScriptAsCommand = new RelayCommand(SaveScriptAs, () => HasScript);
            ShowSideBySideCommand = new RelayCommand(() => SetDetailMode(DetailMode.SideBySide));
            ShowInlineCommand = new RelayCommand(() => SetDetailMode(DetailMode.Inline));
            ShowScriptCommand = new RelayCommand(() => SetDetailMode(DetailMode.Script));
            ShowGroupedCommand = new RelayCommand(() => SetListViewMode(true));
            ShowFlatCommand = new RelayCommand(() => SetListViewMode(false));
            ToggleShowIdenticalCommand = new RelayCommand(ToggleShowIdentical);
            ResetLayoutCommand = new RelayCommand(ResetLayout);
            NewProjectCommand = new RelayCommand(ResetProject);
            OpenProjectCommand = new RelayCommand(OpenProject);
            SaveProjectCommand = new RelayCommand(SaveProject);
            SaveProjectAsCommand = new RelayCommand(SaveProjectAs);
            EditConnectionsCommand = new RelayCommand(OpenPanel, () => !IsRunning);
            OpenPanelCommand = new RelayCommand<object>(OpenPanelFor);
            ClosePanelCommand = new RelayCommand(ClosePanel);
            DonePanelCommand = new AsyncRelayCommand(DonePanelAsync);
            ModeSchemaCommand = new RelayCommand(() => SetMode(CompareMode.Schema));
            ModeDataCommand = new RelayCommand(OpenDataCompare, () => IsDataModeEnabled);
            ShowShortcutsCommand = new RelayCommand(ShowShortcuts);
            OpenReportsFolderCommand = new RelayCommand(OpenReportsFolder);
            OpenDocumentationCommand = new RelayCommand(OpenDocumentation);
            AboutCommand = new RelayCommand(ShowAbout);
            ThemeCommand = new RelayCommand<string>(SetTheme);
            ExitCommand = new RelayCommand(() => WindowClosingRequested?.Invoke(this, EventArgs.Empty));

            UndoCommand = new RelayCommand(UndoInclusion, () => CanUndo);
            RedoCommand = new RelayCommand(RedoInclusion, () => CanRedo);
            SelectAllVisibleCommand = new RelayCommand(SelectAllVisible, () => Results != null);
            NextChangeCommand = new RelayCommand(() => MoveChange(1), () => HasChanges);
            PreviousChangeCommand = new RelayCommand(() => MoveChange(-1), () => HasChanges);
            FocusAttentionCommand = new RelayCommand<DiffItemKey>(FocusItem);
            FixWarningCommand = new RelayCommand<DependencyWarning>(FixWarning);
            FixAllCommand = new RelayCommand(FixAll, () => CanFixAllBlocking);
            ToggleDestructiveFilterCommand = new RelayCommand(ToggleDestructiveFilter, () => Results != null && Results.HasDestructive);
            IncludeOnlyCommand = new RelayCommand<DiffItemKey>(IncludeOnly, IsKnownKey);
            IncludeTypeCommand = new RelayCommand<DiffItemKey>(key => SetTypeIncluded(key, true), IsKnownKey);
            ExcludeTypeCommand = new RelayCommand<DiffItemKey>(key => SetTypeIncluded(key, false), IsKnownKey);
            CopyNameCommand = new RelayCommand<DiffItemKey>(CopyName, IsKnownKey);
            CopyItemScriptCommand = new RelayCommand<DiffItemKey>(CopyItemScript, IsKnownKey);
            ShowDependenciesCommand = new RelayCommand<DiffItemKey>(ShowDependencies, IsKnownKey);
            ShowFocusedDependenciesCommand = new RelayCommand(() => ShowDependencies(Results?.FocusedItem?.Key), () => Results?.FocusedItem != null);
            CloseDependenciesCommand = new RelayCommand(CloseDependencies, () => IsDependenciesOpen);
            CopyReportCommand = new RelayCommand(CopyReport, () => IsReport);
            SaveReportCommand = new RelayCommand(SaveReport, () => IsReport);
            RetryFailedCommand = new AsyncRelayCommand(RetryFailedAsync, () => IsReport && (Report?.CanRetryFailed ?? false));
            BackToResultsCommand = new RelayCommand(BackToResults, () => IsReport);
            OpenReportFolderCommand = new RelayCommand(OpenReportFolder);

            var state = settings.Load();

            Restore(state);

            StatusMessage = SetupGuidance;
        }

        /// <summary>Raised by the Exit menu item, which only the window can carry out.</summary>
        public event EventHandler? WindowClosingRequested;

        public ConnectionViewModel Source { get; }

        public ConnectionViewModel Destination { get; }

        /// <summary>Section 4: the window is a state machine and only one state is active.</summary>
        public AppState State { get; private set; } = AppState.Setup;

        public bool IsSetup
        {
            get { return State == AppState.Setup; }
        }

        public bool IsComparing
        {
            get { return State == AppState.Comparing; }
        }

        public bool IsResults
        {
            get { return State == AppState.Results; }
        }

        /// <summary>A compare or an apply is in flight; the controls that must not run twice read this.</summary>
        public bool IsRunning
        {
            get { return State == AppState.Comparing || State == AppState.Applying; }
        }

        /// <summary>
        /// Section 6.2: while COMPARING the review list is read-only. The previous results stay visible
        /// (section 9.1) but nothing in the list can be selected or included until the run finishes.
        /// </summary>
        public bool IsNotComparing
        {
            get { return State != AppState.Comparing; }
        }

        /// <summary>What the summary bar, review list, detail pane and action bar bind to.</summary>
        public ResultsViewModel? Results { get; private set; }

        public ProgressViewModel Progress { get; }

        public string StatusMessage { get; private set; }

        public ObservableCollection<ToastViewModel> Toasts { get; } = new ObservableCollection<ToastViewModel>();

        public ObservableCollection<RecentPairViewModel> RecentPairs { get; } = new ObservableCollection<RecentPairViewModel>();

        public ObservableCollection<SavedConnection> SavedConnections { get; private set; } = new ObservableCollection<SavedConnection>();

        public ObservableCollection<RecentProjectViewModel> RecentProjects { get; } = new ObservableCollection<RecentProjectViewModel>();

        public bool HasRecentPairs
        {
            get { return RecentPairs.Count > 0; }
        }

        public bool HasRecentProjects
        {
            get { return RecentProjects.Count > 0; }
        }

        // ---- Region A: the connection strip (section 6.1) -------------------------------------------

        public AsyncRelayCommand CompareCommand { get; }

        public AsyncRelayCommand CompareAgainCommand { get; }

        public RelayCommand CancelCommand { get; }

        public RelayCommand SwapCommand { get; }

        public RelayCommand OptionsCommand { get; }

        public RelayCommand EditConnectionsCommand { get; }

        public RelayCommand<object> OpenPanelCommand { get; }

        public RelayCommand ClosePanelCommand { get; }

        public AsyncRelayCommand DonePanelCommand { get; }

        public AsyncRelayCommand ApplyCommand { get; }

        public RelayCommand PreviewScriptCommand { get; }

        public RelayCommand CopyScriptCommand { get; }

        public RelayCommand SaveScriptAsCommand { get; }

        public RelayCommand ShowSideBySideCommand { get; }

        public RelayCommand ShowInlineCommand { get; }

        public RelayCommand ShowScriptCommand { get; }

        public RelayCommand ModeSchemaCommand { get; }

        public RelayCommand ModeDataCommand { get; }

        public RelayCommand ShowGroupedCommand { get; }

        public RelayCommand ShowFlatCommand { get; }

        public RelayCommand ToggleShowIdenticalCommand { get; }

        public RelayCommand ResetLayoutCommand { get; }

        public RelayCommand NewProjectCommand { get; }

        public RelayCommand OpenProjectCommand { get; }

        public RelayCommand SaveProjectCommand { get; }

        public RelayCommand SaveProjectAsCommand { get; }

        public RelayCommand ShowShortcutsCommand { get; }

        public RelayCommand OpenReportsFolderCommand { get; }

        public RelayCommand OpenDocumentationCommand { get; }

        public RelayCommand AboutCommand { get; }

        public RelayCommand<string> ThemeCommand { get; }

        public RelayCommand ExitCommand { get; }

        // ---- Section 9.8: undo, and the rest of the review keys ---------------------------------

        public RelayCommand UndoCommand { get; }

        public RelayCommand RedoCommand { get; }

        public RelayCommand SelectAllVisibleCommand { get; }

        public RelayCommand NextChangeCommand { get; }

        public RelayCommand PreviousChangeCommand { get; }

        public RelayCommand<DiffItemKey> FocusAttentionCommand { get; }

        public RelayCommand<DependencyWarning> FixWarningCommand { get; }

        public RelayCommand FixAllCommand { get; }

        public RelayCommand ToggleDestructiveFilterCommand { get; }

        // ---- Section 7.3: the row context menu -------------------------------------------------------

        public RelayCommand<DiffItemKey> IncludeOnlyCommand { get; }

        public RelayCommand<DiffItemKey> IncludeTypeCommand { get; }

        public RelayCommand<DiffItemKey> ExcludeTypeCommand { get; }

        public RelayCommand<DiffItemKey> CopyNameCommand { get; }

        public RelayCommand<DiffItemKey> CopyItemScriptCommand { get; }

        public RelayCommand<DiffItemKey> ShowDependenciesCommand { get; }

        public RelayCommand ShowFocusedDependenciesCommand { get; }

        public RelayCommand CloseDependenciesCommand { get; }

        /// <summary>Section 7.3's dependency listing, which is a panel rather than a dialog (section 6.10).</summary>
        public DependencyPanelViewModel? DependencyPanel { get; private set; }

        public bool IsDependenciesOpen { get; private set; }

        // ---- Section 6.9: the report state --------------------------------------------------------

        public RelayCommand CopyReportCommand { get; }

        public RelayCommand SaveReportCommand { get; }

        public AsyncRelayCommand RetryFailedCommand { get; }

        public RelayCommand BackToResultsCommand { get; }

        public RelayCommand OpenReportFolderCommand { get; }

        /// <summary>
        /// Section 6.7.2: this destination is marked "Never write to this database from OpenDBDiff", so
        /// the primary action saves the script instead of applying it and no confirmation is asked for,
        /// because nothing is going to happen to the database.
        /// </summary>
        public bool IsScriptOnlyMode
        {
            get { return Destination.NeverWriteToDatabase; }
        }

        /// <summary>The report of the last apply, which is what the REPORT state shows.</summary>
        public ApplyReportViewModel? Report { get; private set; }

        /// <summary>Section 6.7.1's panel, which stays null until it has been opened once.</summary>
        public ScriptPreviewViewModel? ScriptPreview { get; private set; }

        public bool IsScriptPreviewOpen { get; private set; }

        public RelayCommand CloseScriptPreviewCommand { get; }

        /// <summary>Section 6.3's clear button, disabled whenever there is nothing to clear.</summary>
        public RelayCommand ClearSearchCommand { get; }

        /// <summary>
        /// Raised after the search is cleared, so the window can put the focus back in the review list,
        /// which section 6.3 asks for and a view model cannot do for itself.
        /// </summary>
        public event EventHandler? SearchCleared;

        public bool IsReport
        {
            get { return State == AppState.Report && Report != null; }
        }

        public bool CanUndo
        {
            get { return history.CanUndo; }
        }

        public bool CanRedo
        {
            get { return history.CanRedo; }
        }

        public string UndoLabel
        {
            get { return CanUndo ? "Undo " + history.UndoLabel : "Nothing to undo"; }
        }

        public string RedoLabel
        {
            get { return CanRedo ? "Redo " + history.RedoLabel : "Nothing to redo"; }
        }

        // ---- Section 9.4: dependencies ---------------------------------------------------------------

        public bool HasBlockingWarnings
        {
            get { return Results != null && Results.HasBlockingWarnings; }
        }

        public string DependencySummary
        {
            get
            {
                if (Results == null || Results.WarningCount == 0)
                {
                    return string.Empty;
                }

                return Results.BlockingWarningCount > 0
                    ? Results.BlockingText
                    : "1 dependency warning";
            }
        }

        public bool CanFixAllBlocking
        {
            get { return Results != null && Results.CanFixAll; }
        }

        public string FixAllLabel
        {
            get
            {
                return Results == null
                    ? "Fix all"
                    : string.Format("Fix all ({0})", Results.FixableCount);
            }
        }

        /// <summary>Section 6.5.5: the banner under the focused object's diff.</summary>
        public bool HasDependencyBanner
        {
            get { return FocusedWarning != null; }
        }

        public DependencyWarning? FocusedWarning
        {
            get
            {
                var results = Results;

                if (results == null || results.FocusedItem == null)
                {
                    return null;
                }

                var warnings = results.WarningsFor(results.FocusedItem.Key);

                return warnings.Count > 0 ? warnings[0] : null;
            }
        }

        public string DependencyMessage
        {
            get { return FocusedWarning == null ? string.Empty : FocusedWarning.Message; }
        }

        public string DependencyFixLabel
        {
            get
            {
                var warning = FocusedWarning;

                if (warning == null || !warning.Fix.Exists)
                {
                    return string.Empty;
                }

                return (warning.Fix.IsInclude ? "Select " : "Deselect ") + DependencyRules.Name(warning.Fix.Key);
            }
        }

        public bool HasDependencyFix
        {
            get { return FocusedWarning != null && FocusedWarning.Fix.Exists; }
        }

        // ---- Section 6.5.4: the pre-selection overview ------------------------------------------------

        public PreselectionOverview? Overview { get; private set; }

        public IReadOnlyList<OverviewTypeRow> OverviewRows
        {
            get { return Overview == null ? new List<OverviewTypeRow>() : Overview.Rows; }
        }

        /// <summary>The overview's heading while nothing is focused: "{n} differences across {k} types".</summary>
        public string OverviewHeading
        {
            get { return Overview == null ? PreSelectionHeading : Overview.Heading; }
        }

        public IReadOnlyList<OverviewAttention> AttentionPreview
        {
            get
            {
                if (Overview == null)
                {
                    return new List<OverviewAttention>();
                }

                return Overview.Attention.Count > AttentionPreviewSize
                    ? Overview.Attention.Take(AttentionPreviewSize).ToList()
                    : Overview.Attention;
            }
        }

        public int AttentionPreviewSize
        {
            get { return 10; }
        }

        public bool HasAttention
        {
            get { return Overview != null && Overview.HasAttention; }
        }

        /// <summary>How many attention entries the preview leaves out, which the list says in words.</summary>
        public int AttentionRemainder
        {
            get { return Overview == null ? 0 : Math.Max(0, Overview.Attention.Count - AttentionPreviewSize); }
        }

        public bool HasAttentionRemainder
        {
            get { return AttentionRemainder > 0; }
        }

        public string AttentionRemainderText
        {
            get { return AttentionRemainder == 1 ? "1 more needs attention" : AttentionRemainder + " more need attention"; }
        }

        // ---- Section 6.5.3: hunk navigation ----------------------------------------------------------

        public bool HasChanges
        {
            get { return Results != null && HunkStarts.Count > 0; }
        }

        public string ChangePosition
        {
            get
            {
                var starts = HunkStarts;

                if (starts.Count == 0)
                {
                    return string.Empty;
                }

                var position = hunkIndex < 0 ? 1 : IndexOfHunk(starts, hunkIndex) + 1;

                return position <= 0 ? "1 of " + starts.Count : position + " of " + starts.Count;
            }
        }

        private static int IndexOfHunk(IReadOnlyList<int> starts, int index)
        {
            for (var i = 0; i < starts.Count; i++)
            {
                if (starts[i] == index)
                {
                    return i;
                }
            }

            return -1;
        }

        private IReadOnlyList<int> HunkStarts
        {
            get
            {
                var results = Results;

                if (results == null)
                {
                    return new List<int>();
                }

                return results.IsInline
                    ? HunkRules.HunkStarts(results.InlineRows)
                    : HunkRules.HunkStarts(results.SideBySideRows);
            }
        }

        /// <summary>Section 6.1 item 9: "Compare" in SETUP, "Compare again  F5" in RESULTS, "Cancel" while COMPARING.</summary>
        public string CompareLabel
        {
            get
            {
                if (State == AppState.Comparing)
                {
                    return "Cancel";
                }

                return Results == null ? "Compare" : "Compare again  F5";
            }
        }

        public string CompareTooltip
        {
            get
            {
                if (State == AppState.Comparing)
                {
                    return "Stop the comparison (Esc)";
                }

                if (!CanInvokeCompare)
                {
                    return "Set up both connections first. Open the panel to fix them.";
                }

                if (ConnectionRules.ShouldTestBothBeforeCompare(Source.Validity, Destination.Validity))
                {
                    return "Test both connections, then compare (F5)";
                }

                return "Compare the two databases (F5)";
            }
        }

        /// <summary>Section 6.1 item 3: the tooltip states the new direction.</summary>
        public string SwapTooltip
        {
            get
            {
                return string.Format(
                    "Swap source and destination. Source will become {0}",
                    ConnectionRules.ChipTooltip(Destination.ServerName, Destination.DatabaseName));
            }
        }

        public bool CanInvokeCompare
        {
            get { return ConnectionRules.CanInvokeCompare(Source.Validity, Destination.Validity); }
        }

        // ---- Region A: the connection panel (section 6.1.1) ----------------------------------------

        public bool IsConnectionPanelOpen { get; private set; } = true;

        /// <summary>Section 6.1.1: expanded inline in SETUP, a popover below the strip in every other state.</summary>
        public bool IsConnectionPanelInline
        {
            get { return IsConnectionPanelOpen && State == AppState.Setup; }
        }

        public bool IsConnectionPanelPopover
        {
            get { return IsConnectionPanelOpen && State != AppState.Setup; }
        }

        public ConnectionSide FocusedSide { get; private set; } = ConnectionSide.Source;

        public bool IsSourceFocused
        {
            get { return FocusedSide == ConnectionSide.Source; }
        }

        public bool IsDestinationFocused
        {
            get { return FocusedSide == ConnectionSide.Destination; }
        }

        public string PanelHeading
        {
            get { return IsConnectionPanelInline ? "Connect" : "Connections"; }
        }

        // ---- Region C: the mode switch (section 6.1.5) ---------------------------------------------

        public CompareMode Mode { get; private set; } = CompareMode.Schema;

        public bool IsSchemaMode
        {
            get { return Mode == CompareMode.Schema; }
        }

        public bool IsDataMode
        {
            get { return Mode == CompareMode.Data; }
        }

        /// <summary>Section 6.1.5: enabled only after a schema compare found a table on both sides.</summary>
        public bool IsDataModeEnabled { get; private set; }

        public string DataModeTooltip
        {
            get { return IsDataModeEnabled ? "Compare the rows of tables that exist on both sides" : CompareModeRules.DataDisabledTooltip; }
        }

        private void SetMode(CompareMode mode)
        {
            Mode = mode;
            OnPropertyChanged(nameof(Mode));
            OnPropertyChanged(nameof(IsSchemaMode));
            OnPropertyChanged(nameof(IsDataMode));
        }

        /// <summary>
        /// Section 6.1.5: Data mode compares the rows of a table that exists on both sides. It reuses the
        /// existing data-comparison component rather than reimplementing it, so the component is opened
        /// for the table the detail pane is showing.
        /// </summary>
        private void OpenDataCompare()
        {
            SetMode(CompareMode.Data);

            var focused = Results?.DiffSet.FocusedItem;

            if (focused == null || focused.Key.ObjectType != ObjectType.Table)
            {
                StatusMessage = "Select a table in the list to compare its rows.";
                return;
            }

            var table = sourceDatabase?.Find(focused.Key.FullName);

            if (table == null)
            {
                StatusMessage = "That table is no longer part of the comparison.";
                return;
            }

            try
            {
                dialogs.ShowDialog(new DataCompareViewModel(
                    table,
                    Source.ConnectionString,
                    Destination.ConnectionString,
                    dialogs,
                    updateConfirmation));
            }
            catch (Exception ex)
            {
                dialogs.ShowError("Compare data", ex);
            }
        }

        // ---- Region B: the progress bar (section 6.2) -----------------------------------------------

        public bool IsProgressActive
        {
            get { return Progress.IsActive; }
        }

        // ---- Section 9.6: stale results -------------------------------------------------------------

        public bool IsStale { get; private set; }

        public CompareStaleness Staleness { get; private set; } = CompareStaleness.Fresh;

        public bool IsStaleBannerVisible
        {
            get { return IsStale && Results != null; }
        }

        public string StaleBannerText
        {
            get { return StaleRules.BannerText; }
        }

        public string StaleReasonText
        {
            get
            {
                switch (Staleness)
                {
                    case CompareStaleness.ConnectionChanged:
                        return "A connection changed after the comparison.";
                    case CompareStaleness.DirectionSwapped:
                        return "Source and destination were swapped.";
                    case CompareStaleness.OptionsChanged:
                        return "Comparison options changed.";
                    case CompareStaleness.ProjectLoaded:
                        return "A different project was loaded.";
                    case CompareStaleness.Applied:
                        return "The destination was modified by an apply.";
                    default:
                        return string.Empty;
                }
            }
        }

        // ---- Region F: the action bar (section 6.7) -------------------------------------------------

        public bool HasResults
        {
            get { return Results != null; }
        }

        public string ApplyLabel
        {
            get
            {
                var set = Results?.DiffSet;

                if (IsScriptOnlyMode)
                {
                    return ScriptOnlyLabel;
                }

                return set == null
                    ? "Review and apply changes"
                    : set.ApplyLabel(
                        string.IsNullOrEmpty(Destination.DatabaseName) ? "destination" : Destination.DatabaseName,
                        ServerOf(Destination));
            }
        }

        /// <summary>Section 6.7.2's replacement label for the primary action.</summary>
        public string ScriptOnlyLabel
        {
            get { return "Save script\u2026 \u2192"; }
        }

        public string ScriptOnlyTooltip
        {
            get { return "This destination is marked never-write, so the script is saved instead of applied."; }
        }

        public bool CanApply
        {
            get
            {
                var set = Results?.DiffSet;

                // Section 9.4: a blocking dependency problem stops Apply, whatever else says. The user
                // gets the count and the way out in the tooltip rather than a greyed-out button with no
                // explanation.
                return set != null
                    && !HasBlockingWarnings
                    && set.CanApply(State, IsStale, IsDestinationValid, false).CanApply;
            }
        }

        public string ApplyTooltip
        {
            get
            {
                var set = Results?.DiffSet;

                if (set == null)
                {
                    return "Compare two databases first.";
                }

                if (HasBlockingWarnings)
                {
                    return DependencySummary;
                }

                var decision = set.CanApply(State, IsStale, IsDestinationValid, false);

                if (!decision.CanApply)
                {
                    return decision.DisabledReason;
                }

                if (IsScriptOnlyMode)
                {
                    return ScriptOnlyTooltip;
                }

                // Section 6.7: the tooltip repeats the label and the environment tag.
                return Destination.Environment.HasValue
                    ? ApplyLabel + "  \u00b7  " + Destination.Environment.Text
                    : ApplyLabel;
            }
        }

        public bool IsDestinationValid
        {
            get { return Destination.Validity == ConnectionValidity.Valid; }
        }

        public string SourceColumnHeader
        {
            get { return string.Format("SOURCE \u00b7 {0}", ServerOf(Source)); }
        }

        /// <summary>Section 6.5.2: the destination column header repeats the environment tag.</summary>
        public string DestinationColumnHeader
        {
            get
            {
                var header = string.Format("DESTINATION \u00b7 {0} (will be changed)", ServerOf(Destination));

                return Destination.Environment.HasValue ? header + "  \u00b7  " + Destination.Environment.Text : header;
            }
        }

        /// <summary>
        /// Section 6.5.4: the hint under the pre-selection heading. In SETUP it is the setup guidance
        /// rather than "select an object", because there is nothing to select yet.
        /// </summary>
        public string PreSelectionHint
        {
            get
            {
                return IsSetup && Results == null
                    ? SetupGuidance
                    : "Select an object on the left to see its differences.";
            }
        }

        public string PreSelectionHeading
        {
            get
            {
                var results = Results;

                return results == null
                    ? "No comparison yet"
                    : string.Format(CultureInfo.CurrentCulture, "{0} differences across {1} object types",
                        results.DifferenceCount, results.DiffSet.Groups.Count);
            }
        }

        /// <summary>Width of the review list, remembered per user (section 11).</summary>
        public double ReviewListWidth { get; set; } = 400;

        /// <summary>
        /// Section 12: the menu bar collapses to a ≡ button at narrow widths. Section 5.3 puts the
        /// boundary at 1180; the window itself never goes below 1024.
        /// </summary>
        public bool IsNarrow { get; private set; }

        public void UpdateWidth(double width)
        {
            var narrow = width < 1180;

            if (narrow == IsNarrow)
            {
                return;
            }

            IsNarrow = narrow;
            OnPropertyChanged(nameof(IsNarrow));
        }

        // The View menu's check marks. They are computed from the results, so they are refreshed
        // whenever a view setting changes rather than bound through a nullable Results.
        public bool IsIdenticalShown
        {
            get { return Results != null && Results.DiffSet.ShowIdentical; }
        }

        public bool IsGroupedView
        {
            get { return Results == null || Results.DiffSet.ViewMode == ListViewMode.Grouped; }
        }

        public bool IsFlatView
        {
            get { return Results != null && Results.DiffSet.ViewMode == ListViewMode.Flat; }
        }

        public bool IsSideBySideDetail
        {
            get { return Results == null || Results.DiffSet.DetailMode == DetailMode.SideBySide; }
        }

        public bool IsInlineDetail
        {
            get { return Results != null && Results.DiffSet.DetailMode == DetailMode.Inline; }
        }

        public bool IsScriptDetail
        {
            get { return Results != null && Results.DiffSet.DetailMode == DetailMode.Script; }
        }

        private void RaiseViewState()
        {
            OnPropertyChanged(nameof(IsIdenticalShown));
            OnPropertyChanged(nameof(IsGroupedView));
            OnPropertyChanged(nameof(IsFlatView));
            OnPropertyChanged(nameof(IsSideBySideDetail));
            OnPropertyChanged(nameof(IsInlineDetail));
            OnPropertyChanged(nameof(IsScriptDetail));
        }

        /// <summary>
        /// Raised when the focused object or the focused change moved: the detail pane, the dependency
        /// banner of section 6.5.5 and the "n of m" position all read from the focused item.
        /// </summary>
        private void RaiseSelectionState()
        {
            OnPropertyChanged(nameof(FocusedWarning));
            OnPropertyChanged(nameof(HasDependencyBanner));
            OnPropertyChanged(nameof(DependencyMessage));
            OnPropertyChanged(nameof(DependencyFixLabel));
            OnPropertyChanged(nameof(HasDependencyFix));
            OnPropertyChanged(nameof(ChangePosition));
            OnPropertyChanged(nameof(HasChanges));
            ShowFocusedDependenciesCommand.RaiseCanExecuteChanged();
        }

        public string SetupGuidance
        {
            get { return "Choose a source and destination, then compare."; }
        }

        // ---- Window bounds, filled in by the window before it saves (section 11) --------------------

        public double? WindowLeft { get; set; }

        public double? WindowTop { get; set; }

        public double? WindowWidth { get; set; }

        public double? WindowHeight { get; set; }

        public string? WindowStateText { get; set; }

        /// <summary>"Light", "Dark" or "System" (section 11). Remembered; dark rendering is Phase 4.</summary>
        public string Theme { get; private set; } = "System";

        public IOption Options
        {
            get { return options; }
        }

        private bool HasScript
        {
            get { return Results != null && Results.DiffSet.SelectedCount > 0; }
        }

        // ---- Persistence (section 11) ---------------------------------------------------------------

        public void SaveWindowState()
        {
            var state = settings.Load();

            state.SourceConnectionString = Source.ConnectionString;
            state.DestinationConnectionString = Destination.ConnectionString;
            state.ReviewListWidth = (int)Math.Round(ReviewListWidth);
            state.DetailMode = (Results?.DiffSet.DetailMode ?? DetailMode.SideBySide).ToString();
            state.ListViewMode = (Results?.DiffSet.ViewMode ?? ListViewMode.Grouped).ToString();
            state.ShowIdentical = Results != null && Results.DiffSet.ShowIdentical;
            state.SourceEnvironment = Source.Environment.Text;
            state.DestinationEnvironment = Destination.Environment.Text;
            state.ConnectionPanelOpen = IsConnectionPanelOpen;
            state.Theme = Theme;
            state.IncludeDropByDefault = includeDropByDefault;
            state.IgnoreWhitespace = options.Comparison.IgnoreWhiteSpacesInCode;
            state.LastSaveFolder = dialogs.LastSaveFolder;

            state.WindowLeft = WindowLeft;
            state.WindowTop = WindowTop;
            state.WindowWidth = WindowWidth;
            state.WindowHeight = WindowHeight;
            state.WindowState = WindowStateText;

            state.SavedConnections = savedConnections
                .Select(connection => new SavedConnectionState
                {
                    Name = connection.Name,
                    Host = connection.Host,
                    Database = connection.Database,
                    IntegratedSecurity = connection.IntegratedSecurity,
                    UserName = connection.UserName,
                    Environment = connection.Environment,
                    NeverWrite = connection.NeverWrite
                })
                .ToList();

            state.RecentPairs = recentPairs
                .Select(pair => new RecentPairState
                {
                    SourceHost = pair.SourceHost,
                    SourceDatabase = pair.SourceDatabase,
                    DestinationHost = pair.DestinationHost,
                    DestinationDatabase = pair.DestinationDatabase,
                    LastRunUtc = pair.LastRunUtc
                })
                .ToList();

            state.RecentProjects = recentProjects.ToList();

            settings.Save(state);
        }

        private void Restore(UiState state)
        {
            if (!string.IsNullOrEmpty(state.SourceConnectionString))
            {
                Source.ConnectionString = state.SourceConnectionString!;
            }

            if (!string.IsNullOrEmpty(state.DestinationConnectionString))
            {
                Destination.ConnectionString = state.DestinationConnectionString!;
            }

            Source.Environment = ParseEnvironment(state.SourceEnvironment);
            Destination.Environment = ParseEnvironment(state.DestinationEnvironment);

            if (state.ReviewListWidth > 0)
            {
                ReviewListWidth = state.ReviewListWidth;
            }

            includeDropByDefault = state.IncludeDropByDefault;
            options.Comparison.IgnoreWhiteSpacesInCode = state.IgnoreWhitespace;
            dialogs.LastSaveFolder = state.LastSaveFolder;

            savedConnections = (state.SavedConnections ?? new List<SavedConnectionState>())
                .Where(entry => !string.IsNullOrWhiteSpace(entry.Name))
                .Select(entry => new SavedConnection(
                    entry.Name!, entry.Host ?? string.Empty, entry.Database ?? string.Empty,
                    entry.IntegratedSecurity, entry.UserName ?? string.Empty, entry.Environment, entry.NeverWrite))
                .ToList();

            recentPairs = (state.RecentPairs ?? new List<RecentPairState>())
                .Select(entry => new RecentPair(
                    entry.SourceHost ?? string.Empty, entry.SourceDatabase ?? string.Empty,
                    entry.DestinationHost ?? string.Empty, entry.DestinationDatabase ?? string.Empty,
                    entry.LastRunUtc))
                .ToList();

            recentProjects = (state.RecentProjects ?? new List<string>()).ToList();

            ShareSavedConnections();
            RefreshRecentPairs();
            RefreshRecentProjects();

            // Section 6.1.1: the panel is what has to be open to make the app usable, so a remembered
            // "closed" only stands when the two connections can actually start a comparison.
            IsConnectionPanelOpen = state.ConnectionPanelOpen
                || !ConnectionRules.CanInvokeCompare(Source.Validity, Destination.Validity);

            // Restored so the window can put itself back where it was, subject to the current monitors
            // (section 11).
            WindowLeft = state.WindowLeft;
            WindowTop = state.WindowTop;
            WindowWidth = state.WindowWidth;
            WindowHeight = state.WindowHeight;
            WindowStateText = state.WindowState;
            Theme = string.IsNullOrEmpty(state.Theme) ? "System" : state.Theme!;

            Staleness = CompareStaleness.Fresh;
        }

        private static EnvironmentTag ParseEnvironment(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return EnvironmentTag.None;
            }

            foreach (EnvironmentKind kind in Enum.GetValues(typeof(EnvironmentKind)))
            {
                if (string.Equals(EnvironmentTag.For(kind).Text, text, StringComparison.OrdinalIgnoreCase))
                {
                    return EnvironmentTag.For(kind);
                }
            }

            return EnvironmentTag.Custom(text!);
        }

        private void ShareSavedConnections()
        {
            SavedConnections = new ObservableCollection<SavedConnection>(savedConnections);
            Source.SavedConnections = SavedConnections;
            Destination.SavedConnections = SavedConnections;

            OnPropertyChanged(nameof(SavedConnections));
        }

        // ---- Compare (sections 6.1.3, 9.1 and 9.2) --------------------------------------------------

        private async Task CompareAsync()
        {
            if (State == AppState.Comparing)
            {
                return;
            }

            if (!await ConnectionsAreReadyAsync().ConfigureAwait(true))
            {
                return;
            }

            var previousResults = Results;
            var generation = ++compareGeneration;
            var source = new CancellationTokenSource();
            cancellation = source;

            CancelToasts();
            State = AppState.Comparing;
            Progress.StartReading();

            // Section 6.1.5: Data mode is only offered after a schema compare has completed, so it goes
            // back to disabled while this one runs.
            IsDataModeEnabled = false;
            ModeDataCommand.RaiseCanExecuteChanged();
            StatusMessage = "Comparing " + Source.Describe() + " with " + Destination.Describe() + "...";
            ClearStale();
            RaiseResultState();

            var request = new ComparisonRequest(Destination.ConnectionString, Source.ConnectionString, options);

            runningCompare = comparisons.CompareAsync(
                request,
                new Progress<DatabaseProgress>(report => Progress.ReportSource(report)),
                new Progress<DatabaseProgress>(report => Progress.ReportDestination(report)),
                source.Token);

            try
            {
                var result = await runningCompare.ConfigureAwait(true);

                // A cancel puts the window back immediately, so a result that arrives afterwards
                // belongs to a run the user has already abandoned (section 6.2).
                if (generation != compareGeneration)
                {
                    return;
                }

                sourceDatabase = result.Destination;
                destinationDatabase = result.Origin;

                // Section 11's two diff settings are settled before the results are built: the whitespace
                // option is the engine's comparison option, so it decides what counts as a difference at
                // all, and the drop default decides what is ticked when the list appears.
                var ignoreWhitespace = options.Comparison.IgnoreWhiteSpacesInCode;
                var diffSet = new DiffSet(
                    DiffItemBuilder.Build(sourceDatabase, destinationDatabase, ignoreWhitespace),
                    includeDropByDefault)
                {
                    IgnoreWhitespace = ignoreWhitespace
                };

                if (previousResults != null)
                {
                    diffSet.CarryInclusionFrom(previousResults.DiffSet.IncludedKeys, previousResults.DiffSet.ExcludedKeys);
                }

                Results = new ResultsViewModel(diffSet);
                ApplyStoredViewSettings(diffSet);

                // The engine's dependency graph, its destructive flags, the overview of section 6.5.4
                // and the first undo step all belong to this comparison, so they are built together.
                AttachAnalysis(diffSet);
                ApplyPendingInclusion();
                FocusFirstDifference();

                Progress.Complete();
                State = AppState.Results;

                // Section 6.1.1: the panel is inline in SETUP and a popover otherwise, so it closes
                // when results arrive and the chips take over (AC-01).
                IsConnectionPanelOpen = false;
                RefreshPanelState();

                IsDataModeEnabled = CompareModeRules.IsDataEnabled(true, CompareModeRules.TablesOnBothSides(diffSet.AllItems));
                ModeDataCommand.RaiseCanExecuteChanged();

                StatusMessage = BuildCompletionStatus(diffSet);
                RecordRecentPair();
                SaveWindowState();
            }
            catch (OperationCanceledException)
            {
                // CancelCompare already restored the previous state; nothing left to do.
            }
            catch (ComparisonException ex)
            {
                if (generation == compareGeneration)
                {
                    FailCompare(previousResults, ex.Message);
                    dialogs.ShowError(ex.Location, ex);
                }
            }
            catch (Exception ex)
            {
                if (generation == compareGeneration)
                {
                    FailCompare(previousResults, ex.Message);
                    dialogs.ShowError("Comparing Databases", ex);
                }
            }
            finally
            {
                if (generation == compareGeneration)
                {
                    Progress.Reset();
                    cancellation = null;
                    runningCompare = null;
                    RaiseResultState();
                }
            }
        }

        /// <summary>
        /// Section 6.1.3: two Valid sides compare straight away; two Untested sides are tested in
        /// parallel first and the compare then proceeds automatically if both succeed, otherwise the
        /// panel opens on the failing side with its error shown.
        /// </summary>
        private async Task<bool> ConnectionsAreReadyAsync()
        {
            if (ConnectionRules.CanCompare(Source.Validity, Destination.Validity))
            {
                return true;
            }

            if (!ConnectionRules.ShouldTestBothBeforeCompare(Source.Validity, Destination.Validity))
            {
                FocusedSide = Source.Validity == ConnectionValidity.Valid ? ConnectionSide.Destination : ConnectionSide.Source;
                OpenPanel();
                RefreshPanelState();
                StatusMessage = "Set up both connections before comparing.";
                return false;
            }

            StatusMessage = "Testing both connections...";
            Progress.StartReading();

            var sourceTest = Source.TestAsync();
            var destinationTest = Destination.TestAsync();

            await Task.WhenAll(sourceTest, destinationTest).ConfigureAwait(true);

            Progress.Reset();

            if (sourceTest.Result && destinationTest.Result)
            {
                return true;
            }

            FocusedSide = sourceTest.Result ? ConnectionSide.Destination : ConnectionSide.Source;
            OpenPanel();
            RefreshPanelState();
            StatusMessage = "The " + (FocusedSide == ConnectionSide.Source ? "source" : "destination") + " connection could not be tested.";
            RaiseResultState();
            return false;
        }

        private void FailCompare(ResultsViewModel? previousResults, string message)
        {
            Results = previousResults;
            State = previousResults == null ? AppState.Setup : AppState.Results;
            Progress.Reset();
            StatusMessage = message;
            OpenPanel();
            RefreshPanelState();
            RaiseResultState();
        }

        /// <summary>
        /// Section 6.2 and AC-05: the previous stable state comes back immediately rather than when the
        /// engine notices, because the engine may not be able to abort a query that is already running.
        /// The bar keeps saying "Cancelling…" until the engine really stops; if that takes longer than
        /// two seconds the user gets a non-blocking toast rather than a frozen window.
        /// </summary>
        private async void CancelCompare()
        {
            if (State == AppState.Applying)
            {
                // A cancelled apply is not abandoned: the statements that already ran have changed the
                // destination, so the session is allowed to stop and report what it did (section 6.9).
                applyCancellation?.Cancel();
                Progress.Cancel();
                StatusMessage = "Cancelling the apply\u2026";
                return;
            }

            if (State != AppState.Comparing)
            {
                return;
            }

            var generation = ++compareGeneration;
            var task = runningCompare;

            Progress.Cancel();
            cancellation?.Cancel();

            // Put the window back now: the previous results stay, and the partial results of the run
            // that was abandoned were never applied to the view.
            State = Results == null ? AppState.Setup : AppState.Results;
            StatusMessage = "Comparison cancelled.";
            RaiseResultState();

            if (task == null)
            {
                Progress.Reset();
                return;
            }

            var stopped = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(2))).ConfigureAwait(true);

            if (generation != compareGeneration)
            {
                return;
            }

            if (stopped != task)
            {
                ShowToast("Still stopping\u2026");
            }

            try
            {
                await task.ConfigureAwait(true);
            }
            catch (Exception)
            {
                // The run was abandoned; whatever it reported is not the user's problem.
            }

            if (generation == compareGeneration)
            {
                Progress.Reset();
            }
        }

        private void FocusFirstDifference()
        {
            var set = Results?.DiffSet;

            if (set == null || set.TotalDifferenceCount == 0)
            {
                return;
            }

            var first = set.AllItems.FirstOrDefault(item => item.Status == DiffStatus.Drop)
                        ?? set.AllItems.FirstOrDefault(item => item.Status != DiffStatus.Identical);

            if (first != null)
            {
                Results!.Focus(first.Key);
                RaiseSelectionState();
            }
        }

        private void ApplyStoredViewSettings(DiffSet diffSet)
        {
            var state = settings.Load();

            if (string.Equals(state.DetailMode, DetailMode.Inline.ToString(), StringComparison.Ordinal))
            {
                diffSet.DetailMode = DetailMode.Inline;
            }
            else if (string.Equals(state.DetailMode, DetailMode.Script.ToString(), StringComparison.Ordinal))
            {
                diffSet.DetailMode = DetailMode.Script;
            }

            if (string.Equals(state.ListViewMode, ListViewMode.Flat.ToString(), StringComparison.Ordinal))
            {
                diffSet.ViewMode = ListViewMode.Flat;
            }

            diffSet.ShowIdentical = state.ShowIdentical;

            if (state.ShowIdentical)
            {
                diffSet.StatusFilter.Add(DiffStatus.Identical);
            }
        }

        // ---- Dependencies, the overview and the undo stack (sections 9.4, 6.5.4 and 9.8) ---------

        /// <summary>
        /// Finds an item's schema object. It answers null for anything the comparison does not hold, which
        /// is why every caller that can be reached with a stale key checks first; the script builder's
        /// contract takes the nullable away because it is only ever handed keys that came from the results.
        /// </summary>
        private ISchemaBase ResolveItem(DiffItemKey key)
        {
            return key == null || sourceDatabase == null ? null! : sourceDatabase.Find(key.FullName);
        }

        /// <summary>
        /// Builds what depends on a finished comparison: the engine's dependency graph, the engine's
        /// destructive flags, the overview of section 6.5.4 and the first entry of the undo stack.
        /// </summary>
        /// <remarks>
        /// Sections 9.7 and 14 put both the graph and the destructive flags in the engine's hands, so
        /// nothing here reads SQL text to decide whether a change is destructive.
        /// </remarks>
        private void AttachAnalysis(DiffSet diffSet)
        {
            var results = Results;

            if (results == null)
            {
                return;
            }

            dependencies = SqlServerDependencyGraph.Build(sourceDatabase, diffSet.AllItems, ResolveItem);
            SqlServerDestructiveAnalysis.Apply(diffSet.AllItems, ResolveItem);

            hunkIndex = -1;
            Report = null;

            if (subscribedResults != null)
            {
                subscribedResults.InclusionChanged -= OnInclusionChanged;
            }

            subscribedResults = results;
            results.InclusionChanged += OnInclusionChanged;

            history.Reset("Compare", diffSet.IncludedKeys);
            results.ClearResults();
            results.SetWarnings(RecomputeWarnings());
            RefreshOverview();
        }

        private IReadOnlyList<DependencyWarning> RecomputeWarnings()
        {
            var results = Results;

            if (results == null || dependencies == null)
            {
                return new List<DependencyWarning>();
            }

            var included = new HashSet<DiffItemKey>(results.DiffSet.IncludedKeys);

            return DependencyRules.Analyze(dependencies, results.DiffSet.AllItems, key => included.Contains(key));
        }

        private void RefreshOverview()
        {
            var results = Results;

            Overview = results == null ? null : OverviewRules.Build(results.DiffSet.AllItems, results.Warnings);
            RaiseOverviewState();
        }

        private void RaiseOverviewState()
        {
            OnPropertyChanged(nameof(Overview));
            OnPropertyChanged(nameof(OverviewRows));
            OnPropertyChanged(nameof(OverviewHeading));
            OnPropertyChanged(nameof(AttentionPreview));
            OnPropertyChanged(nameof(HasAttention));
            OnPropertyChanged(nameof(AttentionRemainder));
            OnPropertyChanged(nameof(HasAttentionRemainder));
            OnPropertyChanged(nameof(AttentionRemainderText));
        }

        /// <summary>
        /// Every inclusion change arrives here: it is recorded as one undoable step, the dependency rules
        /// are asked again, and the overview is rebuilt. Doing it in one place is what keeps Ctrl+Z and
        /// the warnings from disagreeing with the list.
        /// </summary>
        private void OnInclusionChanged(object? sender, EventArgs e)
        {
            var results = Results;

            if (results == null)
            {
                return;
            }

            var label = string.IsNullOrEmpty(results.LastChange) ? "Change the selection" : results.LastChange;

            history.Record(label, results.DiffSet.IncludedKeys);

            results.SetWarnings(RecomputeWarnings());
            RefreshOverview();

            // Section 6.7.1: the preview re-generates while it is open, so the script on screen is
            // always the script for the current selection.
            RegenerateScriptPreview();

            RaiseDependencyState();
            RaiseHistoryState();
        }

        private void RaiseDependencyState()
        {
            OnPropertyChanged(nameof(HasBlockingWarnings));
            OnPropertyChanged(nameof(DependencySummary));
            OnPropertyChanged(nameof(CanFixAllBlocking));
            OnPropertyChanged(nameof(FixAllLabel));
            OnPropertyChanged(nameof(HasDependencyBanner));
            OnPropertyChanged(nameof(FocusedWarning));
            OnPropertyChanged(nameof(DependencyMessage));
            OnPropertyChanged(nameof(DependencyFixLabel));
            OnPropertyChanged(nameof(HasDependencyFix));
            OnPropertyChanged(nameof(CanApply));
            OnPropertyChanged(nameof(ApplyTooltip));
            OnPropertyChanged(nameof(HasChanges));
            OnPropertyChanged(nameof(ChangePosition));

            FixAllCommand.RaiseCanExecuteChanged();
            FixWarningCommand.RaiseCanExecuteChanged();
            ApplyCommand.RaiseCanExecuteChanged();
        }

        private void RaiseHistoryState()
        {
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
            OnPropertyChanged(nameof(UndoLabel));
            OnPropertyChanged(nameof(RedoLabel));

            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
        }

        private void RaiseReportState()
        {
            OnPropertyChanged(nameof(Report));
            OnPropertyChanged(nameof(IsReport));

            CopyReportCommand.RaiseCanExecuteChanged();
            SaveReportCommand.RaiseCanExecuteChanged();
            RetryFailedCommand.RaiseCanExecuteChanged();
            BackToResultsCommand.RaiseCanExecuteChanged();
        }

        private void UndoInclusion()
        {
            var results = Results;
            var step = history.Undo();

            if (results == null || step == null)
            {
                return;
            }

            results.RestoreInclusion(step.Included);
            results.SetWarnings(RecomputeWarnings());
            RefreshOverview();
            RaiseDependencyState();
            RaiseHistoryState();

            StatusMessage = "Undid " + step.Label + ".";
            ShowToast(StatusMessage);
        }

        private void RedoInclusion()
        {
            var results = Results;
            var step = history.Redo();

            if (results == null || step == null)
            {
                return;
            }

            results.RestoreInclusion(step.Included);
            results.SetWarnings(RecomputeWarnings());
            RefreshOverview();
            RaiseDependencyState();
            RaiseHistoryState();

            StatusMessage = "Redid " + step.Label + ".";
            ShowToast(StatusMessage);
        }

        private void SelectAllVisible()
        {
            var results = Results;

            if (results == null)
            {
                return;
            }

            if (results.SelectAllCommand.CanExecute(null))
            {
                results.SelectAllCommand.Execute(null);
            }
        }

        private void FocusItem(DiffItemKey key)
        {
            var results = Results;

            if (results == null || key == null)
            {
                return;
            }

            results.Focus(key);
            RaiseSelectionState();
        }

        /// <summary>
        /// Section 9.4's one-click fix. It goes through the included set so it is undoable, because the
        /// user is expected to look at what it did and sometimes take it back.
        /// </summary>
        private void FixWarning(DependencyWarning warning)
        {
            var results = Results;

            if (results == null || warning == null || !warning.Fix.Exists)
            {
                return;
            }

            var label = DependencyRules.Name(warning.Fix.Key);

            results.SetIncluded(warning.Fix.Key, warning.Fix.IsInclude, "Fix: " + label);

            StatusMessage = "Fixed the dependency on " + label + ".";
            ShowToast(StatusMessage);
        }

        /// <summary>Section 9.4's "Fix all": one step, so one Ctrl+Z takes all of it back.</summary>
        private void FixAll()
        {
            var results = Results;

            if (results == null || !results.CanFixAll)
            {
                return;
            }

            var wanted = new HashSet<DiffItemKey>(results.DiffSet.IncludedKeys);
            var fixedCount = 0;

            foreach (var warning in results.Warnings.Where(warning => warning.IsBlocking && warning.Fix.Exists))
            {
                if (warning.Fix.IsInclude)
                {
                    if (wanted.Add(warning.Fix.Key))
                    {
                        fixedCount++;
                    }
                }
                else if (wanted.Remove(warning.Fix.Key))
                {
                    fixedCount++;
                }
            }

            if (fixedCount == 0)
            {
                return;
            }

            results.ApplyInclusionChange(wanted.ToList(), "Fix all dependencies");

            StatusMessage = string.Format(CultureInfo.CurrentCulture, "Fixed {0} dependency problems.", fixedCount);
            ShowToast(StatusMessage);
        }

        /// <summary>Section 6.7: the destructive count is a control that narrows the list to those objects.</summary>
        private void ToggleDestructiveFilter()
        {
            var results = Results;

            if (results == null)
            {
                return;
            }

            results.DiffSet.OnlyDestructiveSelected = !results.DiffSet.OnlyDestructiveSelected;
            results.RefreshRows();

            StatusMessage = results.DiffSet.OnlyDestructiveSelected
                ? "Showing the selected destructive changes."
                : "Showing everything.";
        }

        private DiffItem? ItemOf(DiffItemKey key)
        {
            var results = Results;

            if (key == null || results == null)
            {
                return null;
            }

            return results.DiffSet.AllItems.FirstOrDefault(item => item.Key.Equals(key));
        }

        private bool IsKnownKey(DiffItemKey key)
        {
            return ItemOf(key) != null;
        }

        /// <summary>Section 7.3: "Include only this", as one undoable step like every other inclusion change.</summary>
        private void IncludeOnly(DiffItemKey key)
        {
            var results = Results;
            var item = ItemOf(key);

            if (results == null || item == null)
            {
                return;
            }

            results.ApplyInclusionChange(new List<DiffItemKey> { item.Key }, "Include only " + item.Key.FullName);
        }

        /// <summary>
        /// Section 7.3's include or exclude a whole type. It is one step rather than one per object, so a
        /// single Ctrl+Z takes the whole decision back.
        /// </summary>
        private void SetTypeIncluded(DiffItemKey key, bool included)
        {
            var results = Results;
            var item = ItemOf(key);

            if (results == null || item == null)
            {
                return;
            }

            var type = item.Key.ObjectType;
            var wanted = new HashSet<DiffItemKey>(results.DiffSet.IncludedKeys);
            var changed = 0;

            foreach (var candidate in results.DiffSet.AllItems.Where(candidate => candidate.Key.ObjectType == type && candidate.IsSelectable))
            {
                changed += included ? (wanted.Add(candidate.Key) ? 1 : 0) : (wanted.Remove(candidate.Key) ? 1 : 0);
            }

            if (changed == 0)
            {
                return;
            }

            results.ApplyInclusionChange(wanted.ToList(), (included ? "Include all " : "Exclude all ") + TypeNames.Plural(type));
        }

        private void CopyName(DiffItemKey key)
        {
            var item = ItemOf(key);

            if (item == null)
            {
                return;
            }

            try
            {
                Clipboard.SetText(item.Key.FullName);
                StatusMessage = "Copied " + item.Key.FullName + " to the clipboard.";
                ShowToast(StatusMessage);
            }
            catch (Exception ex)
            {
                dialogs.ShowError("Copying the name", ex);
            }
        }

        /// <summary>Section 7.3's "Copy script": this object's statements, not the whole selection's.</summary>
        private void CopyItemScript(DiffItemKey key)
        {
            var item = ItemOf(key);

            if (item == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(item.Script))
            {
                StatusMessage = item.Key.FullName + " has no script of its own.";
                ShowToast(StatusMessage);
                return;
            }

            try
            {
                Clipboard.SetText(item.Script);
                StatusMessage = "Copied the script for " + item.Key.FullName + ".";
                ShowToast(StatusMessage);
            }
            catch (Exception ex)
            {
                dialogs.ShowError("Copying the object's script", ex);
            }
        }

        /// <summary>
        /// Section 7.3's "Show dependencies…": what this object needs and what needs it, from the engine's
        /// graph. Shown as a panel because section 6.10 allows no extra message-box modal.
        /// </summary>
        private void ShowDependencies(DiffItemKey? key)
        {
            var item = ItemOf(key!);

            if (item == null)
            {
                return;
            }

            var graph = dependencies;
            var known = item.Key;

            FocusItem(known);

            DependencyPanel = new DependencyPanelViewModel(
                known.FullName,
                graph == null ? new List<DiffItemKey>() : graph.DependenciesOf(known),
                graph == null ? new List<DiffItemKey>() : graph.DependentsOf(known));

            IsDependenciesOpen = true;
            RaiseDependencyPanelState();
        }

        private void CloseDependencies()
        {
            IsDependenciesOpen = false;
            RaiseDependencyPanelState();
        }

        private void RaiseDependencyPanelState()
        {
            OnPropertyChanged(nameof(DependencyPanel));
            OnPropertyChanged(nameof(IsDependenciesOpen));
            ShowFocusedDependenciesCommand.RaiseCanExecuteChanged();
            CloseDependenciesCommand.RaiseCanExecuteChanged();
        }

        /// <summary>Section 6.5.3: F7 and Shift+F7 walk the changes of the focused object.</summary>
        private void MoveChange(int direction)
        {
            var results = Results;
            var starts = HunkStarts;

            if (results == null || starts.Count == 0)
            {
                return;
            }

            hunkIndex = direction >= 0
                ? HunkRules.Next(starts, hunkIndex)
                : HunkRules.Previous(starts, hunkIndex);

            results.FocusHunk(hunkIndex);

            RaiseSelectionState();
        }

        // ---- Apply (sections 6.7 and 6.8) -----------------------------------------------------------

        private async Task ApplyAsync()
        {
            // Section 6.7.2: a destination marked never-write gets the script, not the statements, and no
            // confirmation dialog, because there is nothing to confirm.
            if (IsScriptOnlyMode)
            {
                SaveScriptAs();
                return;
            }

            var set = Results?.DiffSet;

            if (set == null || !CanApply)
            {
                dialogs.Inform("Apply", ApplyTooltip);
                return;
            }

            // The script is the engine's (section 14), built from the same statement list the preview
            // and the copy use, so what is confirmed is what runs.
            var script = SqlServerScriptBuilder.ForSelection(set.AllItems, ResolveItem, new HashSet<DiffItemKey>(set.IncludedKeys));
            var target = new ApplyTarget(
                ServerOf(Destination),
                string.IsNullOrEmpty(Destination.DatabaseName) ? "(unknown)" : Destination.DatabaseName,
                Destination.Environment);

            var confirmation = new ConfirmationViewModel(set, target, script, Source.Describe());

            dialogs.ShowDialog(confirmation);

            if (!confirmation.Confirmed)
            {
                StatusMessage = "Apply cancelled.";
                return;
            }

            await RunApplyAsync(
                script,
                target,
                confirmation.RunInSingleTransaction,
                confirmation.StopOnFirstError,
                confirmation.SaveScriptCopy,
                set.IncludedKeys).ConfigureAwait(true);
        }

        /// <summary>
        /// Runs an apply and turns it into the report of section 6.9. Shared with "Retry failed", so a
        /// retry is the same operation as the run that produced the failures.
        /// </summary>
        private async Task RunApplyAsync(
            GeneratedScript script,
            ApplyTarget target,
            bool useTransaction,
            bool stopOnFirstError,
            bool saveScriptCopy,
            IReadOnlyList<DiffItemKey> included)
        {
            var results = Results;
            var generation = ++applyGeneration;

            using (var source = new CancellationTokenSource())
            {
                applyCancellation = source;

                State = AppState.Applying;
                StatusMessage = string.Format(CultureInfo.CurrentCulture, "Applying {0} changes\u2026", included.Count);
                Progress.ReportApplying(string.Empty, 0, included.Count);

                if (results != null)
                {
                    results.ClearResults();
                }

                if (saveScriptCopy)
                {
                    // Section 6.8's third option exists for the run that never comes back, so the copy is
                    // written before the first statement rather than with the report afterwards.
                    try
                    {
                        reports.WriteScriptBeforeRun(script.Text, DateTime.Now, target.Database);
                    }
                    catch (Exception ex)
                    {
                        dialogs.ShowError("Saving a copy of the script", ex);
                    }
                }

                var request = new ApplyRequest(included, useTransaction, stopOnFirstError);
                var progress = new Progress<ApplyProgress>(update =>
                    Progress.ReportApplying(update.ObjectFullName, update.Done, update.Total));

                ApplyReport report;

                try
                {
                    var items = results == null ? new List<DiffItem>() : results.DiffSet.AllItems;
                    var connectionString = Destination.ConnectionString;

                    report = await Task.Run(
                        () => SqlServerApplySession.Apply(script, items, request, target, connectionString, progress, source.Token))
                        .ConfigureAwait(true);
                }
                catch (Exception ex)
                {
                    StatusMessage = "Applying the changes failed: " + ex.Message;
                    dialogs.ShowError("Applying changes", ex);

                    State = results == null ? AppState.Setup : AppState.Results;
                    Progress.Reset();
                    RaiseResultState();
                    return;
                }
                finally
                {
                    applyCancellation = null;
                }

                if (generation != applyGeneration)
                {
                    return;
                }

                var folder = string.Empty;

                try
                {
                    folder = reports.Write(report, script.Text);
                }
                catch (Exception ex)
                {
                    dialogs.ShowError("Writing the report", ex);
                }

                Report = new ApplyReportViewModel(report, folder);

                if (results != null)
                {
                    results.SetResults(report.Items);
                }

                Progress.Reset();
                State = AppState.Report;

                // Section 4: after an apply the destination has moved on, so the results behind the
                // report are marked stale rather than pretending they still describe it.
                MarkStale(CompareStaleness.Applied);

                StatusMessage = report.Header + " \u00b7 " + report.OutcomeText;
                ShowToast(StatusMessage);

                RaiseResultState();
                RaiseReportState();
            }
        }

        /// <summary>Section 6.9's "Retry failed": the failures alone, with the options that produced them.</summary>
        private async Task RetryFailedAsync()
        {
            var report = Report?.Report;
            var results = Results;

            if (report == null || results == null || !report.CanRetryFailed)
            {
                return;
            }

            var keys = report.FailedKeys;

            if (keys.Count == 0)
            {
                return;
            }

            var script = SqlServerScriptBuilder.ForSelection(results.DiffSet.AllItems, ResolveItem, new HashSet<DiffItemKey>(keys));

            await RunApplyAsync(script, report.Target, report.UsedTransaction, true, false, keys).ConfigureAwait(true);
        }

        private void CopyReport()
        {
            var report = Report;

            if (report == null)
            {
                return;
            }

            try
            {
                Clipboard.SetText(ReportFormat.ToCsv(report.Report));
                StatusMessage = "Report copied \u00b7 " + report.Report.TotalCount + " objects";
                ShowToast(StatusMessage);
            }
            catch (Exception ex)
            {
                dialogs.ShowError("Copying the report", ex);
            }
        }

        private void SaveReport()
        {
            var report = Report;

            if (report == null)
            {
                return;
            }

            var suggested = string.Format(
                CultureInfo.InvariantCulture,
                "{0}-{1}.html",
                string.IsNullOrEmpty(report.Report.Target.Database) ? "report" : report.Report.Target.Database,
                report.Report.StartedAt.ToLocalTime().ToString("yyyyMMdd-HHmm", CultureInfo.InvariantCulture));

            var path = dialogs.AskSaveReportPath(suggested);

            if (path == null)
            {
                return;
            }

            try
            {
                var html = path.EndsWith(".csv", StringComparison.OrdinalIgnoreCase)
                    ? ReportFormat.ToCsv(report.Report)
                    : ReportFormat.ToHtml(report.Report);

                System.IO.File.WriteAllText(path, html);
                StatusMessage = "Report written to " + path;
            }
            catch (Exception ex)
            {
                dialogs.ShowError("Save report", ex);
            }
        }

        /// <summary>Section 6.9's "Back to results": the list, with a result icon per object.</summary>
        private void BackToResults()
        {
            if (Results == null)
            {
                return;
            }

            State = AppState.Results;
            RaiseResultState();
            RaiseReportState();
        }

        private ISchemaBase ItemFor(DiffItem item)
        {
            var found = ResolveItem(item.Key);

            if (found == null)
            {
                throw new InvalidOperationException("The object " + item.Key.FullName + " is no longer part of the comparison.");
            }

            return found;
        }

        /// <summary>
        /// Section 6.3's clear button and the Esc key that does the same thing. Clearing is the view
        /// model's; sending the focus back to the list is the window's, which is why it is announced
        /// rather than done here.
        /// </summary>
        private void ClearSearch()
        {
            var results = Results;

            if (results == null)
            {
                return;
            }

            results.SearchText = string.Empty;
            ClearSearchCommand.RaiseCanExecuteChanged();

            var handler = SearchCleared;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void PreviewScript()
        {
            OpenScriptPreview();
        }

        /// <summary>
        /// Section 6.7.1: the preview is a panel over the detail pane rather than a window, so the review
        /// list stays visible and the selection can change under it.
        /// </summary>
        private void OpenScriptPreview()
        {
            var set = Results?.DiffSet;

            if (set == null)
            {
                return;
            }

            IsScriptPreviewOpen = true;
            RegenerateScriptPreview();
            RaiseScriptPreviewState();
        }

        private void CloseScriptPreview()
        {
            IsScriptPreviewOpen = false;
            RaiseScriptPreviewState();
        }

        /// <summary>
        /// Re-generates the open panel, which section 6.7.1 asks for whenever the selection changes. It
        /// updates the view model in place so the panel does not flicker or lose its scroll position.
        /// </summary>
        private void RegenerateScriptPreview()
        {
            var set = Results?.DiffSet;

            if (set == null || !IsScriptPreviewOpen)
            {
                return;
            }

            var script = ScriptFor(set);

            if (ScriptPreview == null)
            {
                ScriptPreview = new ScriptPreviewViewModel(script.Text, set.SelectedCount, script.Bundle, IsStale);
            }
            else
            {
                ScriptPreview.Update(script.Text, set.SelectedCount, script.Bundle, IsStale);
            }

            OnPropertyChanged(nameof(ScriptPreview));
            CloseScriptPreviewCommand.RaiseCanExecuteChanged();
        }

        private void RaiseScriptPreviewState()
        {
            OnPropertyChanged(nameof(IsScriptPreviewOpen));
            OnPropertyChanged(nameof(ScriptPreview));
            OnPropertyChanged(nameof(IsDependenciesOpen));
            OnPropertyChanged(nameof(DependencyPanel));
            CloseScriptPreviewCommand.RaiseCanExecuteChanged();
        }

        private void CopyScript()
        {
            try
            {
                Clipboard.SetText(BuildScript(Results!.DiffSet));
                StatusMessage = string.Format(CultureInfo.CurrentCulture, "Script copied \u00b7 {0} objects", Results.DiffSet.SelectedCount);
                ShowToast(StatusMessage);
            }
            catch (Exception ex)
            {
                dialogs.ShowError("Copying the script", ex);
            }
        }

        private void SaveScriptAs()
        {
            var set = Results?.DiffSet;

            if (set == null)
            {
                return;
            }

            var path = dialogs.AskSaveScriptPath(string.Format(CultureInfo.InvariantCulture, "{0}-{1}.sql",
                string.IsNullOrEmpty(Source.DatabaseName) ? "script" : Source.DatabaseName,
                DateTime.Now.ToString("yyyyMMdd-HHmm", CultureInfo.InvariantCulture)));

            if (path == null)
            {
                return;
            }

            try
            {
                System.IO.File.WriteAllText(path, BuildScript(set));
                StatusMessage = "Script written to " + path;
            }
            catch (Exception ex)
            {
                dialogs.ShowError("Save script as", ex);
            }
        }

        /// <summary>
        /// The script the preview, the copy, the save and the apply all use. One builder, so a script a
        /// reader copies is the script a reader runs (section 14).
        /// </summary>
        private GeneratedScript ScriptFor(DiffSet set)
        {
            return SqlServerScriptBuilder.ForSelection(set.AllItems, ResolveItem, new HashSet<DiffItemKey>(set.IncludedKeys));
        }

        private string BuildScript(DiffSet set)
        {
            return ScriptFor(set).Text;
        }

        // ---- Strip actions --------------------------------------------------------------------------

        private void SetDetailMode(DetailMode mode)
        {
            if (Results != null)
            {
                Results.DetailMode = mode;
            }

            RaiseViewState();
        }

        private void SetListViewMode(bool grouped)
        {
            if (Results != null)
            {
                Results.IsGrouped = grouped;
            }

            RaiseViewState();
        }

        private void ToggleShowIdentical()
        {
            if (Results == null)
            {
                return;
            }

            Results.DiffSet.ShowIdentical = !Results.DiffSet.ShowIdentical;

            if (Results.DiffSet.ShowIdentical)
            {
                Results.DiffSet.StatusFilter.Add(DiffStatus.Identical);
            }
            else
            {
                Results.DiffSet.StatusFilter.Remove(DiffStatus.Identical);
            }

            Results.CountsChanged();
            RaiseViewState();
        }

        /// <summary>Section 6.1 item 3: swaps the two connections and marks the results stale.</summary>
        private void Swap()
        {
            Source.SwapWith(Destination);
            StatusMessage = "Swapped source and destination. Compare again to refresh the results.";
            MarkStale(CompareStaleness.DirectionSwapped);
            RaiseResultState();
        }

        private void ResetLayout()
        {
            ReviewListWidth = 400;

            if (Results != null)
            {
                Results.DiffSet.DetailMode = DetailMode.SideBySide;
                Results.DiffSet.ViewMode = ListViewMode.Grouped;
                Results.IsGrouped = true;
                Results.DetailMode = DetailMode.SideBySide;
            }

            StatusMessage = "Layout reset.";
            RaiseViewState();
        }

        // ---- The connection panel (section 6.1.1) ---------------------------------------------------

        private void OpenPanel()
        {
            IsConnectionPanelOpen = true;
            RefreshPanelState();
        }

        private void OpenPanelFor(object? side)
        {
            var name = side as string;

            if (string.Equals(name, "Destination", StringComparison.OrdinalIgnoreCase))
            {
                FocusedSide = ConnectionSide.Destination;
            }
            else if (side is ConnectionViewModel connection)
            {
                FocusedSide = ReferenceEquals(connection, Destination) ? ConnectionSide.Destination : ConnectionSide.Source;
            }
            else
            {
                FocusedSide = ConnectionSide.Source;
            }

            OpenPanel();
        }

        private void ClosePanel()
        {
            // The panel is the only way to set a connection up, so it cannot be closed while there is
            // nothing else to do: SETUP without the panel would be a dead end.
            if (State == AppState.Setup && !ConnectionRules.CanInvokeCompare(Source.Validity, Destination.Validity))
            {
                StatusMessage = "Both connections are needed before the panel can be closed.";
                return;
            }

            IsConnectionPanelOpen = false;
            RefreshPanelState();
        }

        /// <summary>Section 6.1.1: "Done validates and closes".</summary>
        private async Task DonePanelAsync()
        {
            if (!ConnectionRules.IsComplete(Source.ServerName, Source.DatabaseName))
            {
                FocusedSide = ConnectionSide.Source;
                StatusMessage = "The source connection needs a server and a database.";
                RefreshPanelState();
                return;
            }

            if (!ConnectionRules.IsComplete(Destination.ServerName, Destination.DatabaseName))
            {
                FocusedSide = ConnectionSide.Destination;
                StatusMessage = "The destination connection needs a server and a database.";
                RefreshPanelState();
                return;
            }

            if (Source.Validity == ConnectionValidity.Untested || Destination.Validity == ConnectionValidity.Untested)
            {
                var sourceTest = Source.TestAsync();
                var destinationTest = Destination.TestAsync();

                await Task.WhenAll(sourceTest, destinationTest).ConfigureAwait(true);

                if (!sourceTest.Result || !destinationTest.Result)
                {
                    FocusedSide = sourceTest.Result ? ConnectionSide.Destination : ConnectionSide.Source;
                    StatusMessage = "A connection could not be tested; the panel stays open.";
                    RefreshPanelState();
                    return;
                }
            }

            IsConnectionPanelOpen = false;
            RefreshPanelState();
            StatusMessage = "Connections ready.";
        }

        private void RefreshPanelState()
        {
            OnPropertyChanged(nameof(IsConnectionPanelOpen));
            OnPropertyChanged(nameof(IsConnectionPanelInline));
            OnPropertyChanged(nameof(IsConnectionPanelPopover));
            OnPropertyChanged(nameof(FocusedSide));
            OnPropertyChanged(nameof(IsSourceFocused));
            OnPropertyChanged(nameof(IsDestinationFocused));
            OnPropertyChanged(nameof(PanelHeading));
        }

        /// <summary>
        /// Section 6.7.2's checkbox changes what the primary action does and nothing else, so it refreshes
        /// the action bar directly rather than through the connection-changed path, which would mark the
        /// results stale and disable the button the user just asked for.
        /// </summary>
        private void OnDestinationPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ConnectionViewModel.NeverWriteToDatabase))
            {
                OnPropertyChanged(nameof(IsScriptOnlyMode));
                OnPropertyChanged(nameof(ApplyLabel));
                OnPropertyChanged(nameof(ApplyTooltip));
            }
        }

        private void OnConnectionEdited(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(CanInvokeCompare));
            OnPropertyChanged(nameof(CompareTooltip));
            OnPropertyChanged(nameof(IsDestinationValid));
            OnPropertyChanged(nameof(ApplyTooltip));
            OnPropertyChanged(nameof(CopyScriptCommand));
            CompareCommand.RaiseCanExecuteChanged();
            CompareAgainCommand.RaiseCanExecuteChanged();
            ApplyCommand.RaiseCanExecuteChanged();

            MarkStale(CompareStaleness.ConnectionChanged);
        }

        private void OnConnectionSaved(object? sender, SavedConnectionEventArgs e)
        {
            savedConnections = ConnectionLibraryRules.Save(savedConnections, e.Connection).ToList();
            ShareSavedConnections();
            StatusMessage = "Saved the connection " + e.Connection.Name + ".";
            SaveWindowState();
        }

        // ---- Recent pairs and projects (sections 6.1.6 and 12) --------------------------------------

        private void RecordRecentPair()
        {
            var pair = new RecentPair(
                Source.ServerName,
                Source.DatabaseName,
                Destination.ServerName,
                Destination.DatabaseName,
                DateTime.UtcNow);

            recentPairs = ConnectionLibraryRules.Record(recentPairs, pair).ToList();
            RefreshRecentPairs();
        }

        private void RefreshRecentPairs()
        {
            RecentPairs.Clear();

            var now = DateTime.UtcNow;

            foreach (var pair in recentPairs)
            {
                RecentPairs.Add(new RecentPairViewModel(pair, now, OpenRecentPair));
            }

            OnPropertyChanged(nameof(HasRecentPairs));
        }

        private void RefreshRecentProjects()
        {
            RecentProjects.Clear();

            foreach (var path in recentProjects)
            {
                RecentProjects.Add(new RecentProjectViewModel(path, OpenRecentProject));
            }

            OnPropertyChanged(nameof(HasRecentProjects));
        }

        private void OpenRecentPair(RecentPair pair)
        {
            if (pair == null)
            {
                return;
            }

            Source.ApplySavedConnection(new SavedConnection("recent", pair.SourceHost, pair.SourceDatabase, true, string.Empty, Source.Environment.Kind));
            Destination.ApplySavedConnection(new SavedConnection("recent", pair.DestinationHost, pair.DestinationDatabase, true, string.Empty, Destination.Environment.Kind));

            // Section 6.1.6: choosing a recent pair loads both sides and leaves Compare as the next step.
            StatusMessage = "Loaded the recent comparison. Compare to refresh the results.";
            MarkStale(CompareStaleness.ConnectionChanged);
            RaiseResultState();
        }

        // ---- Menus (section 12) ---------------------------------------------------------------------

        private void ShowOptions()
        {
            var model = new OptionsViewModel(options)
            {
                IncludeDropByDefault = includeDropByDefault
            };

            var changed = false;

            EventHandler onChanged = (_, _) => changed = true;
            model.Changed += onChanged;

            try
            {
                dialogs.ShowDialog(model);
            }
            finally
            {
                model.Changed -= onChanged;
            }

            // Section 11's drop default is a presentation setting: it decides what is ticked when a list
            // appears, so on its own it changes nothing that is already on screen.
            includeDropByDefault = model.IncludeDropByDefault;

            if (changed)
            {
                // Section 9.6: an option that affects the comparison invalidates the visible results.
                MarkStale(CompareStaleness.OptionsChanged);
                StatusMessage = "Options changed. Compare again to apply them.";
            }
        }

        private void ResetProject()
        {
            Source.ConnectionString = string.Empty;
            Destination.ConnectionString = string.Empty;
            Source.Environment = EnvironmentTag.None;
            Destination.Environment = EnvironmentTag.None;
            Results = null;
            State = AppState.Setup;
            IsDataModeEnabled = false;
            Progress.Reset();
            IsConnectionPanelOpen = true;
            ClearStale();
            StatusMessage = "New project. Set up both connections, then compare.";
            RefreshPanelState();
            RaiseResultState();
        }

        private void OpenProject()
        {
            var path = dialogs.AskOpenProjectPath();

            if (path == null)
            {
                return;
            }

            OpenProject(path);
        }

        private void OpenRecentProject(string? path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                OpenProject(path!);
            }
        }

        private void OpenProject(string path)
        {
            var project = ProjectFile.Load(path);

            if (project == null)
            {
                dialogs.Inform("Open project",
                    "This file is not a project this front end can read. The WinForms .odbd format is not ported yet.");
                return;
            }

            ApplyProject(project);
            RememberRecentProject(path);

            currentProjectPath = path;
            pendingInclusion = project.InclusionKeys == null || project.InclusionKeys.Count == 0
                ? null
                : new List<string>(project.InclusionKeys);

            // Section 9.6: a project loaded with different connections invalidates the results.
            MarkStale(CompareStaleness.ProjectLoaded);
            StatusMessage = "Loaded the project " + path + ". Compare to refresh the results.";
            RaiseResultState();
        }

        /// <summary>
        /// File → Save project. Without a project path there is nothing to write over, so it asks for one
        /// the way section 12's File menu implies; with one it writes the project, which is what the item
        /// has always claimed to do.
        /// </summary>
        private void SaveProject()
        {
            if (string.IsNullOrEmpty(currentProjectPath))
            {
                SaveProjectAs();
                return;
            }

            WriteProject(currentProjectPath!);
        }

        private void SaveProjectAs()
        {
            var path = dialogs.AskSaveProjectPath(SuggestedProjectName());

            if (path == null)
            {
                return;
            }

            if (WriteProject(path))
            {
                currentProjectPath = path;
                RememberRecentProject(path);
            }
        }

        private bool WriteProject(string path)
        {
            try
            {
                var project = new ProjectFile
                {
                    Source = Capture(Source),
                    Destination = Capture(Destination),
                    InclusionKeys = CapturedInclusion()
                };

                project.Save(path);
                SaveWindowState();
                StatusMessage = "Project written to " + path;
                ShowToast("Project saved.");
                return true;
            }
            catch (Exception ex)
            {
                dialogs.ShowError("Save project", ex);
                return false;
            }
        }

        /// <summary>Section 11.1's inclusionKeys, taken from what is ticked now.</summary>
        private List<string> CapturedInclusion()
        {
            var results = Results;

            return results == null
                ? new List<string>()
                : results.DiffSet.IncludedKeys.Select(key => key.FullName).ToList();
        }

        /// <summary>
        /// Section 11.1: a project's included objects are re-applied to the comparison it asked for. The
        /// comparison keeps the section 7.1 defaults until then, and an object the stored list does not
        /// name is left as the defaults made it rather than being silently excluded.
        /// </summary>
        private void ApplyPendingInclusion()
        {
            var pending = pendingInclusion;
            var results = Results;

            if (pending == null || results == null)
            {
                return;
            }

            pendingInclusion = null;

            var wanted = new HashSet<string>(pending, StringComparer.Ordinal);
            var keys = results.DiffSet.AllItems
                .Where(item => item.IsSelectable && wanted.Contains(item.Key.FullName))
                .Select(item => item.Key)
                .ToList();

            if (keys.Count == 0)
            {
                return;
            }

            results.ApplyInclusionChange(keys, "Include what the project had ticked");
        }

        private string SuggestedProjectName()
        {
            var database = string.IsNullOrEmpty(Destination.DatabaseName) ? "project" : Destination.DatabaseName;

            return database + ".odbdproj";
        }

        /// <summary>One side of the project file, taken from the card (section 11.1). No credential is copied.</summary>
        private static ProjectConnection Capture(ConnectionViewModel connection)
        {
            return new ProjectConnection
            {
                Host = connection.ServerName,
                Database = connection.DatabaseName,
                IntegratedSecurity = connection.UseWindowsAuthentication,
                UserName = connection.UserName,
                Environment = connection.Environment.Text,
                NeverWrite = connection.NeverWriteToDatabase
            };
        }

        /// <summary>Section 11.1: opening a project places both connections in the strip.</summary>
        private void ApplyProject(ProjectFile project)
        {
            if (project.Source != null)
            {
                ApplyProjectSide(project.Source, Source);
            }

            if (project.Destination != null)
            {
                ApplyProjectSide(project.Destination, Destination);
            }
        }

        private static void ApplyProjectSide(ProjectConnection stored, ConnectionViewModel connection)
        {
            var environment = ParseProjectEnvironment(stored.Environment);

            connection.ApplySavedConnection(new SavedConnection(
                "project",
                stored.Host ?? string.Empty,
                stored.Database ?? string.Empty,
                stored.IntegratedSecurity,
                stored.UserName ?? string.Empty,
                environment,
                stored.NeverWrite));

            if (environment == EnvironmentKind.Custom)
            {
                connection.CustomEnvironment = stored.Environment ?? string.Empty;
            }
        }

        /// <summary>A tag that is not one of the fixed five is the custom tag, keeping its own text.</summary>
        private static EnvironmentKind ParseProjectEnvironment(string? text)
        {
            var value = text ?? string.Empty;

            foreach (EnvironmentKind kind in Enum.GetValues(typeof(EnvironmentKind)))
            {
                if (string.Equals(EnvironmentTag.For(kind).Text, value, StringComparison.OrdinalIgnoreCase))
                {
                    return kind;
                }
            }

            return value.Length > 0 ? EnvironmentKind.Custom : EnvironmentKind.None;
        }

        private void RememberRecentProject(string path)
        {
            recentProjects.RemoveAll(existing => string.Equals(existing, path, StringComparison.OrdinalIgnoreCase));
            recentProjects.Insert(0, path);

            if (recentProjects.Count > MaxRecentProjects)
            {
                recentProjects.RemoveRange(MaxRecentProjects, recentProjects.Count - MaxRecentProjects);
            }

            RefreshRecentProjects();
        }

        private void ShowShortcuts()
        {
            dialogs.ShowDialog(new ShortcutsViewModel());
        }

        private void ShowAbout()
        {
            dialogs.ShowDialog(new AboutViewModel());
        }

        private void OpenDocumentation()
        {
            OpenUrl("https://github.com/OpenDBDiff/OpenDBDiff");
        }

        private void OpenReportsFolder()
        {
            OpenFolder(reports.Root);
        }

        /// <summary>Section 6.9's link to the folder this run's report and script were written to.</summary>
        private void OpenReportFolder()
        {
            var folder = Report == null || string.IsNullOrEmpty(Report.ReportFolder)
                ? reports.Root
                : Report.ReportFolder;

            OpenFolder(folder);
        }

        private void OpenFolder(string folder)
        {
            try
            {
                if (string.IsNullOrEmpty(folder))
                {
                    return;
                }

                System.IO.Directory.CreateDirectory(folder);
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(folder) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                dialogs.ShowError("Open reports folder", ex);
            }
        }

        private void OpenUrl(string url)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                dialogs.ShowError("Open documentation", ex);
            }
        }

        /// <summary>
        /// Section 12's View → Theme. The dark theme itself is Phase 4, so the choice is remembered and
        /// the user is told rather than being left with a switch that appears to do nothing.
        /// </summary>
        /// <summary>
        /// Section 13.4: Light, Dark or System. The palette and the window follow immediately, and the
        /// choice is written with the rest of the settings, so it survives a restart.
        /// </summary>
        private void SetTheme(string? theme)
        {
            // Fully qualified: this class has a Theme property, which shadows the palette type's name.
            var mode = OpenDBDiff.Ui.Model.Theme.Parse(theme);

            Theme = mode == AppTheme.Dark ? "Dark" : mode == AppTheme.Light ? "Light" : "System";

            themes.Apply(mode);

            StatusMessage = string.Format(CultureInfo.CurrentCulture, "Theme set to {0}.", Theme);
            ShowToast(StatusMessage);
            SaveWindowState();
        }

        // ---- Stale results (section 9.6) ------------------------------------------------------------

        private void MarkStale(CompareStaleness reason)
        {
            if (!StaleRules.MakesResultsStale(reason) || Results == null)
            {
                return;
            }

            IsStale = true;
            Staleness = reason;

            OnPropertyChanged(nameof(IsStale));
            OnPropertyChanged(nameof(IsStaleBannerVisible));
            OnPropertyChanged(nameof(StaleReasonText));
            OnPropertyChanged(nameof(CanApply));
            OnPropertyChanged(nameof(ApplyTooltip));
            OnPropertyChanged(nameof(ApplyLabel));
            ApplyCommand.RaiseCanExecuteChanged();

            // Section 9.6's script shelf: the open preview says which comparison the script came from.
            RegenerateScriptPreview();
        }

        private void ClearStale()
        {
            IsStale = false;
            Staleness = CompareStaleness.Fresh;

            OnPropertyChanged(nameof(IsStale));
            OnPropertyChanged(nameof(IsStaleBannerVisible));
            OnPropertyChanged(nameof(StaleReasonText));
            OnPropertyChanged(nameof(CanApply));
            OnPropertyChanged(nameof(ApplyTooltip));
            ApplyCommand.RaiseCanExecuteChanged();
        }

        // ---- Toasts (section 6.10) ------------------------------------------------------------------

        public void ShowToast(string message, bool isError = false)
        {
            var toast = new ToastViewModel(message, isError);

            Toasts.Add(toast);

            // Section 6.10: at most three stacked.
            while (Toasts.Count > 3)
            {
                Toasts.RemoveAt(0);
            }

            var timer = new DispatcherTimer { Interval = toast.Lifetime };

            timer.Tick += (_, _) =>
            {
                timer.Stop();
                Toasts.Remove(toast);
            };

            timer.Start();
        }

        private void CancelToasts()
        {
            Toasts.Clear();
        }

        // ---- Plumbing -------------------------------------------------------------------------------

        private static string ServerOf(ConnectionViewModel connection)
        {
            return string.IsNullOrEmpty(connection.ServerName) ? "(not set)" : connection.ServerName;
        }

        private static string BuildCompletionStatus(DiffSet set)
        {
            var counts = set.Counts;

            return string.Format(CultureInfo.CurrentCulture,
                "Comparison complete: {0} new, {1} changed, {2} missing",
                counts.Create, counts.Alter, counts.Drop);
        }

        private void RaiseResultState()
        {
            OnPropertyChanged(nameof(State));
            OnPropertyChanged(nameof(IsSetup));
            OnPropertyChanged(nameof(IsComparing));
            OnPropertyChanged(nameof(IsResults));
            OnPropertyChanged(nameof(IsRunning));
            OnPropertyChanged(nameof(IsNotComparing));
            OnPropertyChanged(nameof(Results));
            OnPropertyChanged(nameof(HasResults));
            OnPropertyChanged(nameof(CanApply));
            OnPropertyChanged(nameof(ApplyLabel));
            OnPropertyChanged(nameof(ApplyTooltip));
            OnPropertyChanged(nameof(CompareLabel));
            OnPropertyChanged(nameof(CompareTooltip));
            OnPropertyChanged(nameof(CanInvokeCompare));
            OnPropertyChanged(nameof(SwapTooltip));
            OnPropertyChanged(nameof(PreSelectionHeading));
            OnPropertyChanged(nameof(SourceColumnHeader));
            OnPropertyChanged(nameof(DestinationColumnHeader));
            OnPropertyChanged(nameof(PreSelectionHint));
            OnPropertyChanged(nameof(IsDestinationValid));
            OnPropertyChanged(nameof(IsStaleBannerVisible));
            OnPropertyChanged(nameof(IsProgressActive));
            OnPropertyChanged(nameof(IsDataModeEnabled));
            OnPropertyChanged(nameof(IsConnectionPanelInline));
            OnPropertyChanged(nameof(IsConnectionPanelPopover));
            OnPropertyChanged(nameof(StatusMessage));
            OnPropertyChanged(nameof(ReviewListWidth));
            OnPropertyChanged(nameof(IsScriptPreviewOpen));
            OnPropertyChanged(nameof(ScriptPreview));
            OnPropertyChanged(nameof(Report));
            OnPropertyChanged(nameof(IsReport));
            OnPropertyChanged(nameof(Overview));
            OnPropertyChanged(nameof(OverviewRows));
            OnPropertyChanged(nameof(OverviewHeading));
            OnPropertyChanged(nameof(AttentionPreview));
            OnPropertyChanged(nameof(HasAttention));
            OnPropertyChanged(nameof(AttentionRemainder));
            OnPropertyChanged(nameof(HasAttentionRemainder));
            OnPropertyChanged(nameof(AttentionRemainderText));
            OnPropertyChanged(nameof(HasChanges));
            OnPropertyChanged(nameof(ChangePosition));

            CompareCommand.RaiseCanExecuteChanged();
            CompareAgainCommand.RaiseCanExecuteChanged();
            CancelCommand.RaiseCanExecuteChanged();
            SwapCommand.RaiseCanExecuteChanged();
            ApplyCommand.RaiseCanExecuteChanged();
            PreviewScriptCommand.RaiseCanExecuteChanged();
            CopyScriptCommand.RaiseCanExecuteChanged();
            SaveScriptAsCommand.RaiseCanExecuteChanged();
            EditConnectionsCommand.RaiseCanExecuteChanged();
            ModeDataCommand.RaiseCanExecuteChanged();
            SelectAllVisibleCommand.RaiseCanExecuteChanged();
            NextChangeCommand.RaiseCanExecuteChanged();
            PreviousChangeCommand.RaiseCanExecuteChanged();
            ToggleDestructiveFilterCommand.RaiseCanExecuteChanged();
            CopyReportCommand.RaiseCanExecuteChanged();
            SaveReportCommand.RaiseCanExecuteChanged();
            RetryFailedCommand.RaiseCanExecuteChanged();
            BackToResultsCommand.RaiseCanExecuteChanged();
            RaiseDependencyState();
            RaiseHistoryState();
            RaiseViewState();
        }
    }
}
