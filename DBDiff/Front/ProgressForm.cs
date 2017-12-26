using System;
using System.Windows.Forms;
using DBDiff.Schema.Events;

namespace DBDiff.Front
{
    public partial class ProgressForm : Form
    {
        private IGenerator SourceGenerator;
        private IGenerator DestinationGenerator;
        private bool IsProcessing = false;
        private Schema.Model.IDatabase origenClone = null;
        private readonly IDatabaseComparer Comparer;

        // TODO: thread-safe error reporting

        public ProgressForm(string sourceDatabaseName, string destinationDatabaseName, IGenerator destinationGenerator, IGenerator sourceGenerator, IDatabaseComparer comparer)
        {
            Destination = null;
            Source = null;
            InitializeComponent();
            sourceProgressControl.Maximum = sourceGenerator.GetMaxValue();
            sourceProgressControl.DatabaseName = sourceDatabaseName;
            this.SourceGenerator = sourceGenerator;

            destinationProgressControl.Maximum = destinationGenerator.GetMaxValue();
            destinationProgressControl.DatabaseName = destinationDatabaseName;
            this.DestinationGenerator = destinationGenerator;

            this.Comparer = comparer;
        }

        public Schema.Model.IDatabase Source { get; private set; }

        public Schema.Model.IDatabase Destination { get; private set; }

        public string ErrorLocation { get; private set; }

        public string ErrorMostRecentProgress { get; private set; }

        public Exception Error { get; private set; }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            this.Close();
            this.Cursor = Cursors.Default;
        }

        private void ProgressForm_Activated(object sender, EventArgs e)
        {
            var handler = new ProgressEventHandler.ProgressHandler(genData2_OnProgress);
            try
            {
                if (!IsProcessing)
                {
                    this.Refresh();
                    IsProcessing = false;
                    SourceGenerator.OnProgress += new ProgressEventHandler.ProgressHandler(genData1_OnProgress);
                    DestinationGenerator.OnProgress += handler;

                    this.ErrorLocation = "Loading " + destinationProgressControl.DatabaseName;
                    Source = SourceGenerator.Process();
                    sourceProgressControl.Message = "Complete";
                    sourceProgressControl.Value = SourceGenerator.GetMaxValue();

                    this.ErrorLocation = "Loading " + sourceProgressControl.DatabaseName;
                    Destination = DestinationGenerator.Process();

                    origenClone = (Schema.Model.IDatabase)Source.Clone(null);

                    this.ErrorLocation = "Comparing Databases";
                    Destination = Comparer.Compare(Source, Destination);
                    Source = origenClone;

                    destinationProgressControl.Message = "Complete";
                    destinationProgressControl.Value = DestinationGenerator.GetMaxValue();
                }
            }
            catch (Exception err)
            {
                this.Error = err;
            }
            finally
            {
                SourceGenerator.OnProgress -= handler;
                DestinationGenerator.OnProgress -= handler;
                this.Dispose();
            }
        }

        void genData2_OnProgress(ProgressEventArgs e)
        {
            if (e.Progress > -1 && destinationProgressControl.Value != e.Progress)
            {
                destinationProgressControl.Value = e.Progress;
            }

            if (String.Compare(destinationProgressControl.Message, e.Message) != 0)
            {
                destinationProgressControl.Message = e.Message;
            }

            this.ErrorMostRecentProgress = e.Message;
        }

        void genData1_OnProgress(ProgressEventArgs e)
        {
            if (e.Progress > -1 && sourceProgressControl.Value != e.Progress)
            {
                sourceProgressControl.Value = e.Progress;
            }

            if (String.Compare(sourceProgressControl.Message, e.Message) != 0)
            {
                sourceProgressControl.Message = e.Message;
            }

            this.ErrorMostRecentProgress = e.Message;
        }
    }
}
