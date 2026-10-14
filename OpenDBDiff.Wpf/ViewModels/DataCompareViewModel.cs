using System.Data;
using System.Threading.Tasks;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Driver;
using OpenDBDiff.Wpf.Infrastructure;
using OpenDBDiff.Wpf.Services;
using PropertyChanged;

namespace OpenDBDiff.Wpf.ViewModels
{
    /// <summary>
    /// Table data comparison, replacing DataCompareForm: the source rows read-only on top, the
    /// destination rows editable underneath, and a commit that pushes the edits back.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed class DataCompareViewModel : DialogViewModelBase
    {
        private readonly ISchemaBase table;
        private readonly string destinationConnectionString;
        private readonly IDialogService dialogs;
        private readonly IUpdateConfirmation confirmation;

        public DataCompareViewModel(
            ISchemaBase table,
            string sourceConnectionString,
            string destinationConnectionString,
            IDialogService dialogs,
            IUpdateConfirmation confirmation)
        {
            this.table = table;
            this.destinationConnectionString = destinationConnectionString;
            this.dialogs = dialogs;
            this.confirmation = confirmation;

            Caption = "Data: " + table.FullName;
            SourceData = Updater.getData(table, sourceConnectionString, confirmation);
            DestinationData = Updater.getData(table, destinationConnectionString, confirmation);

            CommitCommand = new RelayCommand(Commit);
            CloseCommand = new RelayCommand(Close);
        }

        public string Caption { get; }

        public DataTable SourceData { get; }

        public DataTable DestinationData { get; }

        public string StatusMessage { get; private set; } = "Edit the destination rows, then commit.";

        public RelayCommand CommitCommand { get; }

        public RelayCommand CloseCommand { get; }

        public void Close()
        {
            RequestClose();
        }

        private void Commit()
        {
            var committed = Updater.CommitTable(DestinationData, table.FullName, destinationConnectionString, confirmation);

            StatusMessage = committed
                ? "Committed."
                : "The destination did not accept the edits; see the message that was shown.";

            if (committed)
            {
                dialogs.Inform("Compare data", "Changes committed to " + table.FullName + ".");
            }
        }
    }
}
