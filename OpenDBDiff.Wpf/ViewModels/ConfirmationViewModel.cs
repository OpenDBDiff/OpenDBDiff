using System;
using System.Collections.Generic;
using OpenDBDiff.SqlServer.Driver;
using OpenDBDiff.Ui.Model;
using OpenDBDiff.Wpf.Infrastructure;
using PropertyChanged;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>One row of the confirmation dialog's statement list (section 6.8).</summary>
    [AddINotifyPropertyChangedInterface]
    public sealed class ConfirmationStatementViewModel
    {
        public ConfirmationStatementViewModel(ScriptStatement statement, int offset)
        {
            Statement = statement;
            Offset = offset;
            Header = statement.ActionName + "  " + statement.ObjectFullName;
        }

        public ScriptStatement Statement { get; }

        /// <summary>Where this statement starts in the script, so clicking the row can scroll to it.</summary>
        public int Offset { get; }

        public string Header { get; }

        public string Text
        {
            get { return Statement.FirstLine; }
        }

        public bool IsDestructive
        {
            get { return Statement.IsDestructive; }
        }
    }

    /// <summary>
    /// The confirmation dialog of section 6.8 in full: the target, the counts, the destructive callout,
    /// the statement list beside the script, the three options, and the typed confirmation once the
    /// destination is production or anything included is destructive.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed class ConfirmationViewModel : DialogViewModelBase
    {
        private readonly GeneratedScript script;

        public ConfirmationViewModel(DiffSet diffSet, ApplyTarget target, GeneratedScript script, string sourceDescription)
        {
            this.script = script ?? new GeneratedScript(new List<ScriptStatement>(), new Dictionary<DiffItemKey, List<ScriptStatement>>());

            SelectionCount = diffSet.SelectedCount;
            Target = target;
            Database = target.Database;
            Server = target.Server;
            Environment = target.Environment;
            SourceDescription = sourceDescription;

            var counts = diffSet.Counts;

            CreateCount = counts.Create;
            AlterCount = counts.Alter;
            DropCount = counts.Drop;

            DestructiveCount = diffSet.DestructiveSelected.Count;
            DestructiveCallout = ConfirmationRules.DestructiveCallout(DestructiveCount);

            ScriptText = this.script.Text;
            Statements = BuildStatements(this.script);

            RequiresTypedConfirmation = ConfirmationRules.RequiresTypedConfirmation(Environment, DestructiveCount);
            TypedConfirmation = string.Empty;

            // Section 6.8's defaults: a transaction when every statement allows it, stop on the first
            // error, and keep a copy of the script.
            RunInSingleTransaction = this.script.CanRunInTransaction;
            TransactionDisabledReason = this.script.TransactionDisabledReason;
            StopOnFirstError = true;
            SaveScriptCopy = true;

            ApplyCommand = new RelayCommand(Apply, () => CanApply);
            CancelCommand = new RelayCommand(Cancel);
        }

        /// <summary>Raised when a statement row is clicked, so the view can scroll its script box.</summary>
        public event EventHandler<StatementSelectedEventArgs>? StatementSelected;

        public string Title
        {
            get { return ConfirmationRules.Title(SelectionCount, Database); }
        }

        public string Database { get; }

        public string Server { get; }

        public ApplyTarget Target { get; }

        public string SourceDescription { get; }

        public EnvironmentTag Environment { get; }

        public bool HasEnvironmentTag
        {
            get { return Environment.HasValue; }
        }

        public string EnvironmentText
        {
            get { return Environment.Text; }
        }

        public string EnvironmentBackground
        {
            get { return EnvironmentPalette.Background(Environment.Kind); }
        }

        public string EnvironmentForeground
        {
            get { return EnvironmentPalette.Foreground(Environment.Kind); }
        }

        public int SelectionCount { get; }

        public int CreateCount { get; }

        public int AlterCount { get; }

        public int DropCount { get; }

        public int DestructiveCount { get; }

        public bool HasDestructive
        {
            get { return DestructiveCount > 0; }
        }

        public string DestructiveCallout { get; }

        /// <summary>The reviewed script, as the dialog's read-only box shows it.</summary>
        public string ScriptText { get; }

        public IReadOnlyList<ConfirmationStatementViewModel> Statements { get; }

        public int StatementCount
        {
            get { return Statements.Count; }
        }

        public int DestructiveStatementCount
        {
            get { return script.Bundle.DestructiveCount; }
        }

        public bool HasDestructiveStatements
        {
            get { return DestructiveStatementCount > 0; }
        }

        public string StatementSummary
        {
            get { return script.Bundle.Summary; }
        }

        // ---- Options (section 6.8) -----------------------------------------------------------------

        public bool RunInSingleTransaction { get; set; }

        public bool CanRunInSingleTransaction
        {
            get { return script.CanRunInTransaction; }
        }

        public string TransactionDisabledReason { get; }

        public bool StopOnFirstError { get; set; }

        public bool SaveScriptCopy { get; set; }

        // ---- Typed confirmation (section 6.8) ------------------------------------------------------

        public bool RequiresTypedConfirmation { get; }

        public string TypeToConfirmText
        {
            get { return ConfirmationRules.TypeToConfirmText; }
        }

        public string TypedConfirmation { get; set; }

        public bool IsTypedConfirmationAccepted
        {
            get { return !RequiresTypedConfirmation || ConfirmationRules.AcceptsTypedConfirmation(Database, TypedConfirmation); }
        }

        public bool CanApply
        {
            get { return ConfirmationRules.CanApply(RequiresTypedConfirmation, IsTypedConfirmationAccepted, SelectionCount); }
        }

        public string ApplyLabel
        {
            get { return ConfirmationRules.ApplyLabel(SelectionCount); }
        }

        public RelayCommand ApplyCommand { get; }

        public RelayCommand CancelCommand { get; }

        /// <summary>True once the user asked for the changes to run.</summary>
        public bool Confirmed { get; private set; }

        public void Select(ConfirmationStatementViewModel statement)
        {
            if (statement == null)
            {
                return;
            }

            var handler = StatementSelected;

            if (handler != null)
            {
                handler(this, new StatementSelectedEventArgs(statement.Offset, statement.Statement.Sql.Length));
            }
        }

        public void Cancel()
        {
            Confirmed = false;
            RequestClose();
        }

        public void Apply()
        {
            if (!CanApply)
            {
                return;
            }

            Confirmed = true;
            RequestClose();
        }

        private void OnTypedConfirmationChanged()
        {
            OnPropertyChanged(nameof(IsTypedConfirmationAccepted));
            OnPropertyChanged(nameof(CanApply));
            ApplyCommand.RaiseCanExecuteChanged();
        }

        private static IReadOnlyList<ConfirmationStatementViewModel> BuildStatements(GeneratedScript script)
        {
            var rows = new List<ConfirmationStatementViewModel>();
            var offset = 0;

            foreach (var statement in script.Statements)
            {
                rows.Add(new ConfirmationStatementViewModel(statement, offset));
                offset += statement.Sql.Length;
            }

            return rows;
        }
    }

    /// <summary>Which statement the user clicked, and where it is in the script.</summary>
    public sealed class StatementSelectedEventArgs : EventArgs
    {
        public StatementSelectedEventArgs(int offset, int length)
        {
            Offset = offset;
            Length = length;
        }

        public int Offset { get; }

        public int Length { get; }
    }
}
