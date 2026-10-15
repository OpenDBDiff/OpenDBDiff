using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Driver;
using OpenDBDiff.SqlServer.Schema.Options;
using OpenDBDiff.Wpf.Infrastructure;
using OpenDBDiff.Wpf.Services;
using PropertyChanged;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>
    /// The main window.
    /// </summary>
    /// <remarks>
    /// Behaviour is carried over from MainForm: comparing loads the right-hand database first and the
    /// left-hand one second, the comparer marks the left-hand snapshot up so that the generated script
    /// migrates the left database to match the right one, the ticked objects survive a re-compare, and
    /// "Update selected"/"Update all" run their script against the right-hand connection. The
    /// inversion is the one the WinForms front end had; it is kept rather than "fixed" so that both
    /// produce the same script for the same inputs.
    /// </remarks>
    [AddINotifyPropertyChangedInterface]
    public sealed class MainViewModel
    {
        private readonly IComparisonService comparisons;
        private readonly ISettingsStore settings;
        private readonly IDialogService dialogs;
        private readonly IUpdateConfirmation updateConfirmation;
        private readonly SqlOption options;

        private IDatabase? comparedDatabase;
        private IDatabase? originDatabase;

        public MainViewModel(IComparisonService comparisons, ISettingsStore settings, IDialogService dialogs)
        {
            this.comparisons = comparisons;
            this.settings = settings;
            this.dialogs = dialogs;

            options = new SqlOption(true);
            updateConfirmation = new WpfUpdateConfirmation(dialogs);

            Source = new ConnectionViewModel("Source database:");
            Destination = new ConnectionViewModel("Destination database:");
            Tree = new SchemaTreeViewModel();
            Diff = new DiffViewModel();

            Tree.SelectedObjectChanged += (_, fullName) => ShowObject(fullName);

            CompareCommand = new AsyncRelayCommand(CompareAsync);
            SwapCommand = new RelayCommand(Swap);
            OptionsCommand = new RelayCommand(ShowOptions);
            SaveScriptAsCommand = new RelayCommand(SaveScriptAs, () => HasResult);
            CopyScriptCommand = new RelayCommand(CopyScript, () => HasResult);
            RefreshSyncScriptCommand = new RelayCommand(BuildSyncScript, () => HasResult);
            UpdateSelectedCommand = new AsyncRelayCommand(UpdateSelectedAsync, () => HasResult);
            UpdateAllCommand = new AsyncRelayCommand(UpdateAllAsync, () => HasResult);
            CompareDataCommand = new RelayCommand(CompareData, () => HasResult && Diff.CanCompareData);
            SaveProjectCommand = new RelayCommand(SaveProject);
            OpenProjectCommand = new RelayCommand(() => InformNotPorted("Opening a saved project"));
            NewProjectCommand = new RelayCommand(ResetProject);

            var state = settings.Load();
            SplitterDistance = state.SchemaSplitterDistance;

            if (!string.IsNullOrEmpty(state.SourceConnectionString))
            {
                Source.ConnectionString = state.SourceConnectionString!;
            }

            if (!string.IsNullOrEmpty(state.DestinationConnectionString))
            {
                Destination.ConnectionString = state.DestinationConnectionString!;
            }

            StatusMessage = "Select a source and a destination database, then compare.";
        }

        public ConnectionViewModel Source { get; }

        public ConnectionViewModel Destination { get; }

        public SchemaTreeViewModel Tree { get; }

        public DiffViewModel Diff { get; }

        public string Title
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "OpenDBDiff v{0}",
                    typeof(MainViewModel).Assembly.GetName().Version);
            }
        }

        /// <summary>The script that brings the source database in line with the destination.</summary>
        public string SyncScript { get; private set; } = string.Empty;

        /// <summary>What the comparison did, as the schema library reported it.</summary>
        public string ActionReport { get; private set; } = string.Empty;

        public string StatusMessage { get; private set; }

        public bool IsBusy { get; private set; }

        public bool HasResult { get; private set; }

        /// <summary>Width of the schema tree pane, persisted with the rest of the window state.</summary>
        public int SplitterDistance { get; set; }

        public AsyncRelayCommand CompareCommand { get; }

        public RelayCommand SwapCommand { get; }

        public RelayCommand OptionsCommand { get; }

        public RelayCommand SaveScriptAsCommand { get; }

        public RelayCommand CopyScriptCommand { get; }

        public RelayCommand RefreshSyncScriptCommand { get; }

        public AsyncRelayCommand UpdateSelectedCommand { get; }

        public AsyncRelayCommand UpdateAllCommand { get; }

        public RelayCommand CompareDataCommand { get; }

        public RelayCommand SaveProjectCommand { get; }

        public RelayCommand OpenProjectCommand { get; }

        public RelayCommand NewProjectCommand { get; }

        public IOption Options
        {
            get { return options; }
        }

        /// <summary>Called by the window when it closes, so the splitter position survives.</summary>
        public void SaveWindowState()
        {
            var state = settings.Load();
            state.SchemaSplitterDistance = SplitterDistance;
            state.SourceConnectionString = Source.ConnectionString;
            state.DestinationConnectionString = Destination.ConnectionString;
            settings.Save(state);
        }

        private async Task CompareAsync()
        {
            // The pipeline's origin is the right-hand panel and its destination the left-hand one.
            var request = new ComparisonRequest(Destination.ConnectionString, Source.ConnectionString, options);
            var progress = new ProgressViewModel(Destination.Describe(), Source.Describe());

            dialogs.ShowModeless(progress);

            // Ticks made before the comparison are re-applied afterwards, as MainForm did.
            var previouslyChecked = Tree.GetCheckedSchemas();

            IsBusy = true;
            StatusMessage = "Comparing " + Source.Describe() + " with " + Destination.Describe() + "...";

            try
            {
                originDatabase = null;
                comparedDatabase = null;

                var result = await comparisons
                    .CompareAsync(
                        request,
                        new Progress<DatabaseProgress>(report => progress.Origin.Report(report)),
                        new Progress<DatabaseProgress>(report => progress.Destination.Report(report)),
                        CancellationToken.None)
                    .ConfigureAwait(true);

                originDatabase = result.Origin;
                comparedDatabase = result.Destination;

                Tree.Database = comparedDatabase;
                Tree.SetCheckedSchemas(previouslyChecked);

                Diff.Clear();
                ActionReport = originDatabase.ActionMessage.Message;
                HasResult = true;

                BuildSyncScript();

                progress.Complete();
                StatusMessage = string.Format(CultureInfo.CurrentCulture, "Compared. {0} objects loaded.",
                    CountNodes(Tree.Nodes));

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
            }
        }

        /// <summary>
        /// Regenerates the synchronized script for the objects currently ticked. Called when the
        /// script tab is shown, as the WinForms front end did.
        /// </summary>
        private void BuildSyncScript()
        {
            if (comparedDatabase is null)
            {
                return;
            }

            SyncScript = comparedDatabase.ToSqlDiff(Tree.GetCheckedSchemas()).ToSQL();
        }

        private void ShowObject(string? fullName)
        {
            if (comparedDatabase is null || originDatabase is null)
            {
                Diff.Clear();
                return;
            }

            Diff.Show(comparedDatabase, originDatabase, fullName);
            CompareDataCommand.RaiseCanExecuteChanged();
        }

        private void Swap()
        {
            var source = Source.ConnectionString;
            Source.ConnectionString = Destination.ConnectionString;
            Destination.ConnectionString = source;
            StatusMessage = "Swapped source and destination.";
        }

        private void ShowOptions()
        {
            dialogs.ShowDialog(new OptionsViewModel(options));
            StatusMessage = "Options changed. Compare again to apply them.";
        }

        private void SaveScriptAs()
        {
            var path = dialogs.AskSaveScriptPath(SuggestedScriptFileName());

            if (path is null)
            {
                return;
            }

            try
            {
                BuildSyncScript();
                System.IO.File.WriteAllText(path, SyncScript);
                StatusMessage = "Script written to " + path;
            }
            catch (Exception ex)
            {
                dialogs.ShowError("Save Script As", ex);
            }
        }

        private void CopyScript()
        {
            try
            {
                BuildSyncScript();
                Clipboard.SetText(SyncScript);
                StatusMessage = "Script copied to the clipboard.";
            }
            catch (Exception ex)
            {
                dialogs.ShowError("Copying the script to the clipboard", ex);
            }
        }

        /// <summary>
        /// Runs the change script for the ticked objects against the destination connection, and
        /// reports what failed rather than stopping at the first failure.
        /// </summary>
        private async Task UpdateSelectedAsync()
        {
            var connectionString = Destination.ConnectionString;
            var report = new System.Text.StringBuilder();
            var updated = 0;

            foreach (var item in Tree.GetCheckedSchemas())
            {
                var target = comparedDatabase?.Find(item.FullName) as ISchemaBase;

                if (target is null)
                {
                    report.AppendLine("Nothing could be found to do for '" + item.Name + "'");
                    continue;
                }

                switch (target.ObjectType)
                {
                    case ObjectType.Table:
                    case ObjectType.StoredProcedure:
                        report.Append(target.Status == ObjectStatus.Create
                            ? Updater.createNew(target, connectionString, updateConfirmation)
                            : Updater.alter(target, connectionString, updateConfirmation));
                        updated++;
                        break;

                    case ObjectType.Function:
                    case ObjectType.View:
                        report.Append(target.Status == ObjectStatus.Create
                            ? Updater.createNew(target, connectionString, updateConfirmation)
                            : Updater.alter(target, connectionString, updateConfirmation));
                        updated++;
                        break;

                    default:
                        if (target.Status == ObjectStatus.Create)
                        {
                            report.Append(Updater.addNew(target, connectionString, updateConfirmation));
                            updated++;
                        }
                        else
                        {
                            report.AppendLine("Nothing could be found to do for '" + target.Name + "'");
                        }

                        break;
                }
            }

            ReportUpdate(report, updated, "All successful.");

            if (options.Comparison.ReloadComparisonOnUpdate)
            {
                await CompareAsync().ConfigureAwait(true);
            }
        }

        private async Task UpdateAllAsync()
        {
            if (!dialogs.Confirm("Are you sure you want to update all?", "Confirm update"))
            {
                return;
            }

            var connectionString = Destination.ConnectionString;
            var report = new System.Text.StringBuilder();
            var updated = 0;

            foreach (var node in Tree.Nodes.SelectMany(Walk))
            {
                var item = node.Item;

                if (item is null)
                {
                    continue;
                }

                switch (item.Status)
                {
                    case ObjectStatus.Create:
                        report.Append(Updater.createNew(item, connectionString, updateConfirmation));
                        updated++;
                        break;

                    case ObjectStatus.Alter:
                        report.Append(Updater.alter(item, connectionString, updateConfirmation));
                        updated++;
                        break;
                }
            }

            ReportUpdate(report, updated, "Update successful.");

            await CompareAsync().ConfigureAwait(true);
        }

        private void ReportUpdate(System.Text.StringBuilder report, int updated, string successMessage)
        {
            if (report.Length == 0)
            {
                dialogs.Inform("Success", successMessage);
                StatusMessage = string.Format(CultureInfo.CurrentCulture, "{0} object(s) updated.", updated);
            }
            else
            {
                dialogs.Inform("Error", report.ToString());
                StatusMessage = "Some objects could not be updated.";
            }
        }

        private void CompareData()
        {
            var item = Diff.ObjectFullName is null ? null : comparedDatabase?.Find(Diff.ObjectFullName);

            if (item is null)
            {
                dialogs.Inform("Compare data", "Select a table in the schema tree first.");
                return;
            }

            var viewModel = new DataCompareViewModel(
                item,
                Source.ConnectionString,
                Destination.ConnectionString,
                dialogs,
                updateConfirmation);

            dialogs.ShowDialog(viewModel);
        }

        private void SaveProject()
        {
            SaveWindowState();
            dialogs.Inform("Projects",
                "The connection strings and window layout were saved. Named project files, which the " +
                "WinForms front end keeps in its LiteDB store, are not ported yet.");
        }

        private void ResetProject()
        {
            Source.ConnectionString = string.Empty;
            Destination.ConnectionString = string.Empty;
            StatusMessage = "Cleared the connections.";
        }

        private void InformNotPorted(string what)
        {
            dialogs.Inform(what, "Not ported to this front end yet; the WinForms front end still does it.");
        }

        private string SuggestedScriptFileName()
        {
            var database = string.IsNullOrEmpty(Source.DatabaseName) ? "script" : Source.DatabaseName;
            return database + "_sync.sql";
        }

        private static IEnumerable<SchemaNodeViewModel> Walk(SchemaNodeViewModel node)
        {
            yield return node;

            foreach (var child in node.Children)
            {
                foreach (var descendant in Walk(child))
                {
                    yield return descendant;
                }
            }
        }

        private static int CountNodes(IEnumerable<SchemaNodeViewModel> nodes)
        {
            return nodes.SelectMany(Walk).Count(node => node.Item != null);
        }
    }
}
