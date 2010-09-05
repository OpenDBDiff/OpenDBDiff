using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using DBDiff.Schema.SQLServer.Generates.Generates;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Front
{
    public partial class ProgressForm : Form
    {
        private Generate genData1;
        private Generate genData2;
        private bool IsProcessing = false;
        private Database origen = null;
        private Database destino = null;
        private Database origenClone = null;

        public ProgressForm(string DatabaseName1, string DatabaseName2, Generate genData1, Generate genData2)
        {
            InitializeComponent();
            databaseProgressControl1.Maximum = Generate.MaxValue;
            databaseProgressControl2.Maximum = Generate.MaxValue;
            databaseProgressControl1.DatabaseName = DatabaseName1;
            databaseProgressControl2.DatabaseName = DatabaseName2;
            this.genData1 = genData1;
            this.genData2 = genData2;
        }

        public Database Source
        {
            get { return origen; }
        }

        public Database Destination
        {
            get { return destino; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            this.Close();
            this.Cursor = Cursors.Default;
        }

        private void ProgressForm_Activated(object sender, EventArgs e)
        {

            if (!IsProcessing)
            {
                this.Refresh();
                IsProcessing = false;
                genData1.OnProgress += new DBDiff.Schema.Events.ProgressEventHandler.ProgressHandler(genData1_OnProgress);
                genData2.OnProgress += new DBDiff.Schema.Events.ProgressEventHandler.ProgressHandler(genData2_OnProgress);
                /*Thread t1 = new Thread(delegate()
                {*/
                    origen = genData1.Process();
                /*});
                Thread t2 = new Thread(delegate()
                {*/
                    destino = genData2.Process();

                    origenClone = (Database)origen.Clone(null);
                /*});
                t1.Start();
                t2.Start();
                t1.Join();
                t2.Join();
                */
                destino = Generate.Compare(origen, destino);
                origen = origenClone;

                databaseProgressControl1.Message = "Complete";
                databaseProgressControl2.Message = "Complete";
                databaseProgressControl1.Value = Generate.MaxValue;
                databaseProgressControl2.Value = Generate.MaxValue;
                this.Dispose();
            }
        }

        void genData2_OnProgress(DBDiff.Schema.Events.ProgressEventArgs e)
        {
            databaseProgressControl1.Value = e.Progress;
            databaseProgressControl1.Message = e.Message;
        }

        void genData1_OnProgress(DBDiff.Schema.Events.ProgressEventArgs e)
        {
            databaseProgressControl2.Value = e.Progress;
            databaseProgressControl2.Message = e.Message;
        }
    }
}
