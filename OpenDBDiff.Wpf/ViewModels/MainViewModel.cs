using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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
    /// The shell of the redesigned window: the four regions of section 5.1 - summary bar, review list,
    /// detail pane and action bar - over the rules that live in OpenDBDiff.Ui.Model.
    /// </summary>
    /// <remarks>
    /// The comparison pipeline is unchanged from the previous front end: the right-hand panel is loaded
    /// first and the left-hand second, and the comparer marks the left-hand snapshot up as the desired
    /// state. In the specification's words (section 3) the left-hand panel is the Source, whose
    /// definitions are the desired state, and the right-hand panel is the Destination that an apply
    /// modifies - which is what the detail pane's column headers and the apply button say.
    ///
    /// Phases 2 to 4 of the specification are not here: the connection strip, streaming progress and
    /// per-side status, stale results, dependency checks, typed confirmation, the apply report and the
    /// keyboard map beyond F5 and Ctrl+F. The connection panels are therefore still the ones from the
    /// previous layout, which is what Phase 1 allows.
    /// </remarks>
    [AddINotifyPropertyChangedInterface]
    public sealed class MainViewModel : Infrastructure.ObservableObject
    {
        private readonly IComparisonService comparisons;
        private readonly ISettingsStore settings;
        private readonly IDialogService dialogs;
        private readonly IUpdateConfirmation updateConfirmation;
        private readonly SqlOption options;

        private IDatabase? sourceDatabase;
        private IDatabase? destinationDatabase;

        public MainViewModel(IComparisonService comparisons, ISettingsStore settings, IDialogService dialogs)
        {
            this.comparisons = comparisons;
            this.settings = settings;
            this.dialogs = dialogs;

            options = new SqlOption(true);
            updateConfirmation = new WpfUpdateConfirmation(dialogs);

            Source = new ConnectionViewModel("Source database:");
            Destination = new ConnectionViewModel("Destination database:");

            CompareCommand = new AsyncRelayCommand(CompareAsync);
            SwapCommand = new RelayCommand(Swap);
            OptionsCommand = new RelayCommand(ShowOptions);
            ApplyCommand = new AsyncRelayCommand(ApplyAsync, () => CanApply);
            PreviewScriptCommand = new RelayCommand(PreviewScript, () => HasScript);
            CopyScriptCommand = new RelayCommand(CopyScript, () => HasScript);
            SaveScriptAsCommand = new RelayCommand(SaveScriptAs, () => HasScript);
            ShowSideBySideCommand = new RelayCommand(() => SetDetailMode(DetailMode.SideBySide));
            ShowInlineCommand = new RelayCommand(() => SetDetailMode(DetailMode.Inline));
            ShowScriptCommand = new RelayCommand(() => SetDetailMode(DetailMode.Script));
            OpenProjectCommand = new RelayCommand(() => dialogs.Inform("Open project", "Named project files are not ported to this front end yet."));
            NewProjectCommand = new RelayCommand(ResetProject);
            SaveProjectCommand = new RelayCommand(SaveProject);

            var state = settings.Load();

            if (!string.IsNullOrEmpty(state.SourceConnectionString))
            {
                Source.ConnectionString = state.SourceConnectionString!;
            }

            if (!string.IsNullOrEmpty(state.DestinationConnectionString))
            {
                Destination.ConnectionString = state.DestinationConnectionString!;
            }

            StatusMessage = "Choose a source and destination, then compare.";
        }

        public ConnectionViewModel Source { get; }

        public ConnectionViewModel Destination { get; }

        /// <summary>What the summary bar, review list, detail pane and action bar bind to.</summary>
        public ResultsViewModel? Results { get; private set; }

        public string StatusMessage { get; private set; }

        public bool IsBusy { get; private set; }

        public bool HasResults
        {
            get { return Results != null; }
        }

        public AsyncRelayCommand CompareCommand { get; }

        public RelayCommand SwapCommand { get; }

        public RelayCommand OptionsCommand { get; }

        public AsyncRelayCommand ApplyCommand { get; }

        public RelayCommand PreviewScriptCommand { get; }

        public RelayCommand CopyScriptCommand { get; }

        public RelayCommand SaveScriptAsCommand { get; }

        public RelayCommand ShowSideBySideCommand { get; }

        public RelayCommand ShowInlineCommand { get; }

        public RelayCommand ShowScriptCommand { get; }

        public RelayCommand OpenProjectCommand { get; }

        public RelayCommand NewProjectCommand { get; }

        public RelayCommand SaveProjectCommand { get; }

        public IOption Options
        {
            get { return options; }
        }

        /// <summary>Width of the review list, remembered per user (section 11).</summary>
        public double ReviewListWidth { get; set; } = 400;

        /// <summary>"SOURCE · {server}", as section 6.5.2 labels the left column.</summary>
        public string SourceColumnHeader
        {
            get { return string.Format("SOURCE \u00b7 {0}", ServerOf(Source)); }
        }

        /// <summary>"DESTINATION · {server} (will be changed)".</summary>
        public string DestinationColumnHeader
        {
            get { return string.Format("DESTINATION \u00b7 {0} (will be changed)", ServerOf(Destination)); }
        }

        public string PreSelectionHeading
        {
            get
            {
                var results = Results;

                return results == null
                    ? "No comparison yet"
                    : string.Format("{0} differences across {1} object types", results.DifferenceCount, results.DiffSet.Groups.Count);
            }
        }

        public string ApplyLabel
        {
            get
            {
                var set = Results?.DiffSet;

                return set == null
                    ? "Review and apply changes"
                    : set.ApplyLabel(
                        string.IsNullOrEmpty(Destination.DatabaseName) ? "destination" : Destination.DatabaseName,
                        ServerOf(Destination));
            }
        }

        public bool CanApply
        {
            get
            {
                var set = Results?.DiffSet;

                return set != null && set.CanApply(AppState.Results, false, IsDestinationUsable(), false).CanApply;
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

                var decision = set.CanApply(AppState.Results, false, IsDestinationUsable(), false);

                return decision.CanApply ? ApplyLabel : decision.DisabledReason;
            }
        }

        private bool HasScript
        {
            get { return Results != null && Results.DiffSet.SelectedCount > 0; }
        }

        public void SaveWindowState()
        {
            var state = settings.Load();
            state.SourceConnectionString = Source.ConnectionString;
            state.DestinationConnectionString = Destination.ConnectionString;
            settings.Save(state);
        }

        private async Task CompareAsync()
        {
            var request = new ComparisonRequest(Destination.ConnectionString, Source.ConnectionString, options);
            var progress = new ProgressViewModel(Destination.Describe(), Source.Describe());

            dialogs.ShowModeless(progress);

            var previousIncluded = Results?.DiffSet.IncludedKeys;
            var previousExcluded = Results?.DiffSet.ExcludedKeys;

            IsBusy = true;
            StatusMessage = "Comparing " + Source.Describe() + " with " + Destination.Describe() + "...";

            try
            {
                sourceDatabase = null;
                destinationDatabase = null;

                var result = await comparisons
                    .CompareAsync(
                        request,
                        new Progress<DatabaseProgress>(report => progress.Origin.Report(report)),
                        new Progress<DatabaseProgress>(report => progress.Destination.Report(report)),
                        CancellationToken.None)
                    .ConfigureAwait(true);

                sourceDatabase = result.Destination;
                destinationDatabase = result.Origin;

                var diffSet = new DiffSet(DiffItemBuilder.Build(sourceDatabase, destinationDatabase));

                if (previousIncluded != null)
                {
                    diffSet.CarryInclusionFrom(previousIncluded, previousExcluded);
                }

                Results = new ResultsViewModel(diffSet);
                FocusFirstDifference();

                progress.Complete();
                StatusMessage = BuildCompletionStatus(diffSet);

                SaveWindowState();
            }
            catch (ComparisonException ex)
            {
                progress.Fail(ex.Message);
                StatusMessage = ex.Message;
                dialogs.ShowError(ex.Location, ex);
            }
            catch (Exception ex)
            {
                progress.Fail(ex.Message);
                StatusMessage = ex.Message;
                dialogs.ShowError("Comparing Databases", ex);
            }
            finally
            {
                IsBusy = false;
                RaiseResultState();
            }
        }

        /// <summary>Section 9.1 step 5: focus the first Drop if there is one, otherwise the first difference.</summary>
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
            }
        }

        private async Task ApplyAsync()
        {
            var set = Results?.DiffSet;

            if (set == null || !CanApply)
            {
                dialogs.Inform("Apply", ApplyTooltip);
                return;
            }

            var confirmation = new ConfirmationViewModel(
                set,
                string.IsNullOrEmpty(Destination.DatabaseName) ? "(unknown)" : Destination.DatabaseName,
                ServerOf(Destination),
                Source.Describe(),
                string.Empty);

            dialogs.ShowDialog(confirmation);

            if (!confirmation.Confirmed)
            {
                StatusMessage = "Apply cancelled.";
                return;
            }

            IsBusy = true;
            StatusMessage = "Applying " + set.SelectedCount + " changes...";

            var total = set.SelectedCount;

            try
            {
                var failures = await Task.Run(() => Apply(set)).ConfigureAwait(true);

                StatusMessage = failures == 0
                    ? string.Format("Applied {0} of {0} changes.", total)
                    : string.Format("Applied {0} of {1} changes.", total - failures, total);

                dialogs.Inform("Apply", StatusMessage);
            }
            catch (Exception ex)
            {
                dialogs.ShowError("Applying changes", ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>Runs each included item's script against the destination and counts the failures.</summary>
        private int Apply(DiffSet set)
        {
            var connectionString = Destination.ConnectionString;
            var failures = 0;

            foreach (var item in set.IncludedItems)
            {
                var target = ItemFor(item);

                var report = item.Status == DiffStatus.Create
                    ? Updater.createNew(target, connectionString, updateConfirmation)
                    : Updater.alter(target, connectionString, updateConfirmation);

                if (!string.IsNullOrWhiteSpace(report))
                {
                    failures++;
                }
            }

            return failures;
        }

        private ISchemaBase ItemFor(DiffItem item)
        {
            var found = sourceDatabase?.Find(item.Key.FullName);

            if (found == null)
            {
                throw new InvalidOperationException("The object " + item.Key.FullName + " is no longer part of the comparison.");
            }

            return found;
        }

        private void PreviewScript()
        {
            var set = Results?.DiffSet;

            if (set != null)
            {
                dialogs.ShowDialog(new ScriptPreviewViewModel(BuildScript(set), set.SelectedCount));
            }
        }

        private void CopyScript()
        {
            try
            {
                Clipboard.SetText(BuildScript(Results!.DiffSet));
                StatusMessage = string.Format("Script copied \u00b7 {0} objects", Results.DiffSet.SelectedCount);
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

            var path = dialogs.AskSaveScriptPath(string.Format("{0}-{1}.sql",
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

        private static string BuildScript(DiffSet set)
        {
            var builder = new StringBuilder();

            foreach (var item in set.IncludedItems)
            {
                builder.AppendLine(string.Format("-- {0} {1}", StatusVocabulary.Badge(item.Status), item.Key.FullName));
                builder.AppendLine(item.Script);
                builder.AppendLine();
            }

            return builder.ToString();
        }

        private void SetDetailMode(DetailMode mode)
        {
            if (Results != null)
            {
                Results.DetailMode = mode;
            }
        }

        private void Swap()
        {
            var source = Source.ConnectionString;
            Source.ConnectionString = Destination.ConnectionString;
            Destination.ConnectionString = source;
            StatusMessage = "Swapped source and destination. Compare again to refresh the results.";
            RaiseResultState();
        }

        private void ShowOptions()
        {
            dialogs.ShowDialog(new OptionsViewModel(options));
            StatusMessage = "Options changed. Compare again to apply them.";
        }

        private void SaveProject()
        {
            SaveWindowState();
            dialogs.Inform("Projects", "The connection strings were saved. Named project files are not ported to this front end yet.");
        }

        private void ResetProject()
        {
            Source.ConnectionString = string.Empty;
            Destination.ConnectionString = string.Empty;
            Results = null;
            StatusMessage = "Cleared the connections.";
            RaiseResultState();
        }

        private bool IsDestinationUsable()
        {
            return !string.IsNullOrEmpty(Destination.ServerName) && !string.IsNullOrEmpty(Destination.DatabaseName);
        }

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
            OnPropertyChanged(nameof(Results));
            OnPropertyChanged(nameof(HasResults));
            OnPropertyChanged(nameof(CanApply));
            OnPropertyChanged(nameof(ApplyLabel));
            OnPropertyChanged(nameof(ApplyTooltip));
            OnPropertyChanged(nameof(PreSelectionHeading));
            ApplyCommand.RaiseCanExecuteChanged();
            PreviewScriptCommand.RaiseCanExecuteChanged();
            CopyScriptCommand.RaiseCanExecuteChanged();
            SaveScriptAsCommand.RaiseCanExecuteChanged();
        }
    }
}
