using System;
using System.Windows.Forms;
using OpenDBDiff.Schema.Events;
using System.Collections.Generic;

namespace OpenDBDiff.Front
{
    public partial class ProgressForm : Form
    {
        private IGenerator OriginGenerator;
        private IGenerator DestinationGenerator;
        private bool IsProcessing = false;
        private Schema.Model.IDatabase originClone = null;
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

        public Schema.Model.IDatabase Origin { get; private set; }

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
                    OriginGenerator.OnProgress += new ProgressEventHandler.ProgressHandler(genData1_OnProgress);
                    DestinationGenerator.OnProgress += handler;

                    this.ErrorLocation = "Loading " + destinationProgressControl.DatabaseName;
                    Origin = OriginGenerator.Process();
                    originProgressControl.Message = "Complete";
                    originProgressControl.Value = OriginGenerator.GetMaxValue();

                    this.ErrorLocation = "Loading " + originProgressControl.DatabaseName;
                    Destination = DestinationGenerator.Process();

                    originClone = (Schema.Model.IDatabase)Origin.Clone(null);

                    this.ErrorLocation = "Comparing Databases";
                    Destination = Comparer.Compare(Origin, Destination);
                    Origin = originClone;

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
                OriginGenerator.OnProgress -= handler;
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
            if (e.Progress > -1 && originProgressControl.Value != e.Progress)
            {
                originProgressControl.Value = e.Progress;
            }

            if (String.Compare(originProgressControl.Message, e.Message) != 0)
            {
                originProgressControl.Message = e.Message;
            }

            this.ErrorMostRecentProgress = e.Message;
        }
    }
}
