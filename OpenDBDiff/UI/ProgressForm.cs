using OpenDBDiff.Abstractions.Schema.Events;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.Abstractions.Ui;
using OpenDBDiff.SqlServer.Schema.Generates;
using OpenDBDiff.SqlServer.Schema.Compare;
using OpenDBDiff.SqlServer.Schema.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;

namespace OpenDBDiff.UI
{
    public partial class ProgressForm : Form
    {
        private IGenerator OriginGenerator;
        private IGenerator DestinationGenerator;
        private bool IsProcessing = false;
        //private IDatabase originClone = null;
        private readonly IDatabaseComparer Comparer;

        // TODO: thread-safe error reporting

        public ProgressForm(KeyValuePair<string, IGenerator> originDatabase, KeyValuePair<string, IGenerator> destinationDatabase, IDatabaseComparer comparer)
        {
            InitializeComponent();

            Origin = null;
            Destination = null;
            originProgressControl.Maximum = originDatabase.Value.GetMaxValue();
            originProgressControl.DatabaseName = originDatabase.Key;
            this.OriginGenerator = originDatabase.Value;

            destinationProgressControl.Maximum = destinationDatabase.Value.GetMaxValue();
            destinationProgressControl.DatabaseName = destinationDatabase.Key;
            this.DestinationGenerator = destinationDatabase.Value;

            this.Comparer = comparer;
        }

        public Abstractions.Schema.Model.IDatabase Origin { get; private set; }

        public Abstractions.Schema.Model.IDatabase Destination { get; private set; }

        public string ErrorLocation { get; private set; }

        public string ErrorMostRecentProgress { get; private set; }

        public Exception Error { get; private set; }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            this.Close();
            this.Cursor = Cursors.Default;
        }

        private async void ProgressForm_Activated(object sender, EventArgs e)
        {
            if (!IsProcessing)
            {
                var originHandler = new ProgressEventHandler.ProgressHandler(genData1_OnProgress);
                var destinationHandler = new ProgressEventHandler.ProgressHandler(genData2_OnProgress);
                Generate.OnCompareProgress += Generator_OnCompareProgress;

                try
                {
                    this.Refresh();
                    IsProcessing = true;
                    OriginGenerator.OnProgress += originHandler;
                    DestinationGenerator.OnProgress += destinationHandler;

                    this.ErrorLocation = "Loading " + destinationProgressControl.DatabaseName;

                    Destination = await Task.Run(() => DestinationGenerator.Process());

                    // Update destinationProgressControl on the UI thread
                    destinationProgressControl.Invoke(new Action(() =>
                    {
                        destinationProgressControl.Message = "Complete";
                        destinationProgressControl.Value = DestinationGenerator.GetMaxValue();
                    }));

                    this.ErrorLocation = "Loading " + originProgressControl.DatabaseName;

                    Origin = await Task.Run(() => OriginGenerator.Process());

                    // Update originProgressControl on the UI thread
                    originProgressControl.Invoke(new Action(() =>
                    {
                        originProgressControl.Message = "Complete";
                        originProgressControl.Value = OriginGenerator.GetMaxValue();
                    }));

                    //originClone = await Task.Run(() => (IDatabase)Origin.Clone(null));

                    this.ErrorLocation = "Comparing Databases";

                    compareProgressControl.Maximum = GetCount(Origin as Database, Destination as Database);

                    Destination = await Task.Run(() => Comparer.Compare(Origin, Destination));

                    //Origin = originClone;

                }
                catch (Exception err)
                {
                    this.Error = err;
                }
                finally
                {
                    OriginGenerator.OnProgress -= originHandler;
                    DestinationGenerator.OnProgress -= destinationHandler;
                    Generate.OnCompareProgress -= Generator_OnCompareProgress;
                    this.Close();
                }
            }
        }

        private int GetCount(Database Origin, Database Destination)
        {
            int count = 0;
            count += (Origin.Tables.Count + Destination.Tables.Count +
                Origin.Assemblies.Count + Destination.Assemblies.Count +
                Origin.UserTypes.Count + Destination.UserTypes.Count +
                Origin.XmlSchemas.Count + Destination.XmlSchemas.Count +
                Origin.Schemas.Count + Destination.Schemas.Count +
                Origin.FileGroups.Count + Destination.FileGroups.Count +
                Origin.Rules.Count + Destination.Rules.Count +
                Origin.DDLTriggers.Count + Destination.DDLTriggers.Count +
                Origin.Synonyms.Count + Destination.Synonyms.Count +
                Origin.Users.Count + Destination.Users.Count +
                Origin.Procedures.Count + Destination.Procedures.Count +
                Origin.CLRProcedures.Count + Destination.CLRProcedures.Count +
                Origin.CLRFunctions.Count + Destination.CLRFunctions.Count +
                Origin.Views.Count + Destination.Views.Count +
                Origin.Functions.Count + Destination.Functions.Count +
                Origin.Roles.Count + Destination.Roles.Count +
                Origin.PartitionFunctions.Count + Destination.PartitionFunctions.Count +
                Origin.PartitionSchemes.Count + Destination.PartitionSchemes.Count +
                Origin.TablesTypes.Count + Destination.TablesTypes.Count +
                Origin.FullText.Count + Destination.FullText.Count);

            var oTables = Origin.Tables;
            foreach (var item in oTables)
            {
                //count += item.Columns.Count;
                count += item.Constraints.Count;
                count += item.Indexes.Count;
                count += item.Options.Count;
                count += item.Triggers.Count;
                count += item.CLRTriggers.Count;
                count += item.FullTextIndex.Count;
            }

            var oViews = Origin.Views;
            foreach (var item in oViews)
            {
                count += item.Indexes.Count;
                count += item.Triggers.Count;
            }

            var oTableTypes = Origin.TablesTypes;
            foreach (var item in oTableTypes)
            {
                //count += item.Columns.Count;
                count += item.Constraints.Count;
                count += item.Indexes.Count;
            }

            var dTables = Destination.Tables;
            foreach (var item in dTables)
            {
                //count += item.Columns.Count;
                count += item.Constraints.Count;
                count += item.Indexes.Count;
                count += item.Options.Count;
                count += item.Triggers.Count;
                count += item.CLRTriggers.Count;
                count += item.FullTextIndex.Count;
            }

            var dViews = Destination.Views;
            foreach (var item in dViews)
            {
                count += item.Indexes.Count;
                count += item.Triggers.Count;
            }

            var dTableTypes = Destination.TablesTypes;
            foreach (var item in dTableTypes)
            {
                //count += item.Columns.Count;
                count += item.Constraints.Count;
                count += item.Indexes.Count;
            }

            return count;
        }

        private void genData1_OnProgress(ProgressEventArgs e)
        {
            if (originProgressControl.IsHandleCreated)
            {
                // Marshal UI updates to the UI thread
                Invoke((MethodInvoker)delegate
                {
                    if (e.Progress > -1 && originProgressControl.Value != e.Progress)
                    {
                        originProgressControl.Value = e.Progress;
                    }

                    if (string.Compare(originProgressControl.Message, e.Message) != 0)
                    {
                        originProgressControl.Message = e.Message;
                    }

                    this.ErrorMostRecentProgress = e.Message;
                });
            }
        }

        private void genData2_OnProgress(ProgressEventArgs e)
        {
            // Marshal UI updates to the UI thread
            if (destinationProgressControl.IsHandleCreated)
            {
                destinationProgressControl.Invoke((MethodInvoker)delegate
                {
                    if (e.Progress > -1 && destinationProgressControl.Value != e.Progress)
                    {
                        destinationProgressControl.Value = e.Progress;
                    }

                    if (string.Compare(destinationProgressControl.Message, e.Message) != 0)
                    {
                        destinationProgressControl.Message = e.Message;
                    }

                    this.ErrorMostRecentProgress = e.Message;
                });
            }
        }

        private void Generator_OnCompareProgress(ProgressEventArgs e)
        {
            if (compareProgressControl.IsHandleCreated)
            {
                // Update the label on the UI thread
                compareProgressControl.Invoke(new Action(() =>
                {
                    compareProgressControl.Message = e.Message;

                    if (e.Progress < compareProgressControl.Maximum && compareProgressControl.Value != e.Progress)
                    {
                        compareProgressControl.Value = e.Progress;
                    }
                }));
            }
        }
    }
}
