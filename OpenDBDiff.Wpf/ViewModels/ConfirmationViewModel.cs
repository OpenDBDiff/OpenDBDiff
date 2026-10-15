using System;
using System.Linq;
using System.Text;
using OpenDBDiff.Ui.Model;
using OpenDBDiff.Wpf.Infrastructure;
using PropertyChanged;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>
    /// The confirmation dialog of section 6.8, in its Phase 2 form: target, counts, the destructive
    /// callout, the script and the destination's environment tag in the title bar. The typed
    /// confirmation for production and destructive changes arrives with Phase 3.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed class ConfirmationViewModel : DialogViewModelBase
    {
        private readonly DiffSet diffSet;

        public ConfirmationViewModel(DiffSet diffSet, string database, string server, string sourceDescription, EnvironmentTag? environment)
        {
            this.diffSet = diffSet;

            Database = database;
            Server = server;
            SourceDescription = sourceDescription;
            Environment = environment ?? EnvironmentTag.None;

            var counts = diffSet.Counts;

            CreateCount = counts.Create;
            AlterCount = counts.Alter;
            DropCount = counts.Drop;

            var destructive = diffSet.DestructiveSelected;

            DestructiveCount = destructive.Count;
            DestructiveCallout = string.Format(
                "{0} destructive statements. Dropped objects and columns cannot be recovered by this tool.",
                destructive.Count);

            ScriptText = BuildScript();
        }

        public string Title
        {
            get { return string.Format("Apply {0} changes to {1}", diffSet.SelectedCount, Database); }
        }

        public string Database { get; }

        public string Server { get; }

        public string SourceDescription { get; }

        /// <summary>Section 6.8: the tag of the destination, which is the database an apply would change.</summary>
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

        public int CreateCount { get; }

        public int AlterCount { get; }

        public int DropCount { get; }

        public int DestructiveCount { get; }

        public bool HasDestructive
        {
            get { return DestructiveCount > 0; }
        }

        public string DestructiveCallout { get; }

        public string ScriptText { get; }

        public string ApplyLabel
        {
            get { return string.Format("Apply {0} changes", diffSet.SelectedCount); }
        }

        /// <summary>True once the user asked for the changes to run.</summary>
        public bool Confirmed { get; private set; }

        public void Cancel()
        {
            RequestClose();
        }

        public void Apply()
        {
            Confirmed = true;
            RequestClose();
        }

        /// <summary>The script, with a section comment per object so a reviewer can find each change.</summary>
        private string BuildScript()
        {
            var builder = new StringBuilder();

            foreach (var item in diffSet.IncludedItems)
            {
                builder.AppendLine(string.Format("-- {0} {1}", StatusVocabulary.Badge(item.Status), item.Key.FullName));

                if (!string.IsNullOrWhiteSpace(item.Script))
                {
                    builder.AppendLine(item.Script);
                }

                builder.AppendLine();
            }

            return builder.ToString().TrimEnd() + System.Environment.NewLine;
        }
    }
}
