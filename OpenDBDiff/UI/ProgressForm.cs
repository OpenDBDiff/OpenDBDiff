using OpenDBDiff.Abstractions.Schema.Events;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.Abstractions.Ui;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDBDiff.UI
{
    public partial class ProgressForm : Form
    {
        private IGenerator OriginGenerator;
        private IGenerator DestinationGenerator;
        private bool IsProcessing = false;
        private IDatabase originClone = null;
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

                try
                {
                    this.Refresh();
                    IsProcessing = true;
                    OriginGenerator.OnProgress += originHandler;
                    DestinationGenerator.OnProgress += destinationHandler;

                    this.ErrorLocation = "Loading " + destinationProgressControl.DatabaseName;

                    //Destination = DestinationGenerator.Process();
                    Destination = await Task.Run(() => DestinationGenerator.Process());

                    // Update destinationProgressControl on the UI thread
                    destinationProgressControl.Invoke(new Action(() =>
                    {
                        destinationProgressControl.Message = "Complete";
                        destinationProgressControl.Value = DestinationGenerator.GetMaxValue();
                    }));

                    //destinationProgressControl.Message = "Complete";
                    //destinationProgressControl.Value = DestinationGenerator.GetMaxValue();

                    this.ErrorLocation = "Loading " + originProgressControl.DatabaseName;

                    //Origin = OriginGenerator.Process();
                    Origin = await Task.Run(() => OriginGenerator.Process());

                    // Update originProgressControl on the UI thread
                    originProgressControl.Invoke(new Action(() =>
                    {
                        originProgressControl.Message = "Complete";
                        originProgressControl.Value = OriginGenerator.GetMaxValue();
                    }));

                    //originProgressControl.Message = "Complete";
                    //originProgressControl.Value = OriginGenerator.GetMaxValue();

                    originClone = await Task.Run(() => (IDatabase)Origin.Clone(null));

                    this.ErrorLocation = "Comparing Databases";
                    Destination = await Task.Run(() => Comparer.Compare(Origin, Destination));
                    Origin = originClone;

                }
                catch (Exception err)
                {
                    this.Error = err;
                }
                finally
                {
                    OriginGenerator.OnProgress -= originHandler;
                    DestinationGenerator.OnProgress -= destinationHandler;
                    this.Close();
                }
            }
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

        /*private void genData1_OnProgress(ProgressEventArgs e)
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
        }*/

        /*private void genData2_OnProgress(ProgressEventArgs e)
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
        }*/
    }
}
