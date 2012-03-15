using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DBDiff.Schema;
using DBDiff.Schema.Events;
using DBDiff.Schema.Misc;
using DBDiff.Schema.SQLServer.Generates.Front;
using DBDiff.Schema.SQLServer.Generates.Generates;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.SQLServer.Generates.Options;
using DBDiff.Settings;
using DBDiff.Schema.Model;

namespace DBDiff.Front
{
    public partial class DataCompareForm : Form
    {
        public DataCompareForm(ISchemaBase Selected, string SrcConnectionString, string DestConnectionString)
        {
            InitializeComponent();
            this.selected = Selected;
            this.srcConnectionString = SrcConnectionString;
            this.destConnectionString = DestConnectionString;
            this.ClientSize = new System.Drawing.Size(1050, 600);
            
            //Label lblSrc
            lblSrc.Text = "Source";
            lblSrc.Font = new Font("Verdana", 14, FontStyle.Bold);
            lblSrc.Width = 150;
            lblSrc.Location = new System.Drawing.Point(10, 0);
            this.Controls.Add(lblSrc);
            //Label lblDest
            lblDest.Text = "Destination";
            lblDest.Font = new Font("Verdana", 14, FontStyle.Bold);
            lblDest.Width = 150;
            lblDest.Location = new System.Drawing.Point(520, 0);
            this.Controls.Add(lblDest);
            //Label lblAdded
            lblAdded.Text = "Added";
            lblAdded.Font = new Font("Verdana", 9, FontStyle.Regular);
            lblAdded.Width = 100;
            lblAdded.Location = new System.Drawing.Point(560, 20);
            this.pnlControl.Controls.Add(lblAdded);
            //Label lblModified
            lblModified.Text = "Modified";
            lblModified.Font = new Font("Verdana", 9, FontStyle.Regular);
            lblModified.Width = 100;
            lblModified.Location = new System.Drawing.Point(710, 20);
            this.pnlControl.Controls.Add(lblModified);

            //DataGridView srcDgv
            srcDgv.Width = 500;
            srcDgv.Height = 500;
            srcDgv.Location = new System.Drawing.Point(10, 25);
            this.Controls.Add(srcDgv);
            //DataGridView destDgv
            destDgv.Width = 500;
            destDgv.Height = 500;
            destDgv.Location = new System.Drawing.Point(520, 25);
            this.Controls.Add(destDgv);

            //Panel pnlControl
            pnlControl.Width = 1050;
            pnlControl.Height = 200;
            pnlControl.BackColor = Color.White;
            pnlControl.Location = new System.Drawing.Point(0, 530);
            this.Controls.Add(pnlControl);
            //Panel pnlAdded
            pnlAdded.Width = 30;
            pnlAdded.Height = 20;
            pnlAdded.Location = new System.Drawing.Point(520, 15);
            pnlAdded.BackColor = Color.Green;
            this.pnlControl.Controls.Add(pnlAdded);
            //Panel pnlModified
            pnlModified.Width = 30;
            pnlModified.Height = 20;
            pnlModified.Location = new System.Drawing.Point(670, 15);
            pnlModified.BackColor = Color.Blue;
            this.pnlControl.Controls.Add(pnlModified);

            //Button btnRowToRow
            btnRowToRow.Width = 120;
            btnRowToRow.Height = 30;
            btnRowToRow.Location = new System.Drawing.Point(10, 10);
            btnRowToRow.Name = "btnRowToRow";
            btnRowToRow.Text = "Update row --> row";
            btnRowToRow.BackColor = Color.LightGray;
            btnRowToRow.Click += new System.EventHandler(this.btnRowToRow_Click);
            this.pnlControl.Controls.Add(btnRowToRow);
            //Button btnMerge
            btnMerge.Width = 100;
            btnMerge.Height = 30;
            btnMerge.Location = new System.Drawing.Point(150, 10);
            btnMerge.Name = "btnMerge";
            btnMerge.Text = "Merge all -->";
            btnMerge.BackColor = Color.LightGray;
            btnMerge.Click += new System.EventHandler(this.btnMerge_Click);
            this.pnlControl.Controls.Add(btnMerge);
            //Button btnUpdateRow
            btnUpdateRow.Width = 100;
            btnUpdateRow.Height = 30;
            btnUpdateRow.Location = new System.Drawing.Point(290, 10);
            btnUpdateRow.Name = "btnUpdateRow";
            btnUpdateRow.Text = "Update row -->";
            btnUpdateRow.BackColor = Color.LightGray;
            btnUpdateRow.Click += new System.EventHandler(this.btnUpdateRow_Click);
            this.pnlControl.Controls.Add(btnUpdateRow);
            //Button btnCommitChanges
            btnCommitChanges.Width = 100;
            btnCommitChanges.Height = 30;
            btnCommitChanges.Location = new System.Drawing.Point(900, 10);
            btnCommitChanges.Name = "btnCommitChanges";
            btnCommitChanges.Text = "Commit";
            btnCommitChanges.BackColor = Color.LightGray;
            btnCommitChanges.Enabled = false;
            btnCommitChanges.Click += new System.EventHandler(btnCommitChanges_Click);
            this.pnlControl.Controls.Add(btnCommitChanges);

            doCompare();
        }

        private void doCompare(){
            DataTable srcTable = Updater.getData(selected, srcConnectionString);
            DataTable destTable = Updater.getData(selected, destConnectionString);

            srcDgv.MultiSelect = false;
            srcDgv.ReadOnly = true;
            srcDgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            srcDgv.RowHeadersVisible = false;
            srcDgv.DataSource = srcTable;
            srcDgv.Rows[0].Cells[0].Style.ForeColor = Color.Blue;

            destDgv.MultiSelect = false;
            destDgv.ReadOnly = true;
            destDgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            destDgv.RowHeadersVisible = false;
            destDgv.DataSource = destTable;
            destDgv.CellFormatting += new DataGridViewCellFormattingEventHandler(destDgv_CellFormatting);
        }

        private void destDgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataTable table = (DataTable)destDgv.DataSource;
            if (e.RowIndex < table.Rows.Count)
            {
                if (table.Rows[e.RowIndex].RowState == DataRowState.Added)
                {
                    e.CellStyle.ForeColor = Color.Green;
                }
                else if (table.Rows[e.RowIndex].RowState == DataRowState.Modified)
                {
                    e.CellStyle.ForeColor = Color.Blue;
                }
            }
        }

        private void btnCommitChanges_Click(object sender, EventArgs e) {
            DataTable destination = (DataTable)destDgv.DataSource;
            DataTable edits = destination.GetChanges();
            if (Updater.CommitTable(edits, destConnectionString))
            {
                destination.AcceptChanges();
                doCompare();
                btnCommitChanges.Enabled = false;
            }
        }

        private void btnUpdateRow_Click(object sender, EventArgs e) {
            DataTable source = (DataTable)srcDgv.DataSource;
            DataTable destination = (DataTable)destDgv.DataSource;

            object[] sourceItems = source.Rows[srcDgv.CurrentRow.Index].ItemArray;

            for (int i = 0; i < destination.Columns.Count; i++) {
                if (destination.Columns[i].Unique) {
                    if (destination.Rows.Find(sourceItems[i]) == null && destination.Columns[i].AutoIncrement) {
                        sourceItems[i] = null;
                    }
                }
            }

            destination.BeginLoadData();
            destination.LoadDataRow(sourceItems, false);
            destination.EndLoadData();
            btnCommitChanges.Enabled = true;
        }

        private void btnMerge_Click(object sender, EventArgs e) {
            DataTable source = (DataTable)srcDgv.DataSource;
            DataTable destination = (DataTable)destDgv.DataSource;

            destination.Merge(source, true);
            foreach(DataRow dr in destination.Rows){
                if (dr.RowState == DataRowState.Unchanged) {
                    dr.SetAdded();
                }
            }
            btnCommitChanges.Enabled = true;
        }

        private void btnRowToRow_Click(object sender, EventArgs e)
        {
            DataTable source = (DataTable)srcDgv.DataSource;
            DataTable destination = (DataTable)destDgv.DataSource;

            DataRow sourceRow = source.Rows[srcDgv.CurrentRow.Index];
            DataRow destinationRow = destination.Rows[destDgv.CurrentRow.Index];

            for (int i = 0; i < destination.Columns.Count; i++)
            {
                if (!destination.Columns[i].Unique)
                {
                    destinationRow[i] = sourceRow[i];
                }
            }
            btnCommitChanges.Enabled = true;
        }
        private ISchemaBase selected;
        private string srcConnectionString;
        private string destConnectionString;

        private Button btnRowToRow = new Button();
        private Button btnMerge = new Button();
        private Button btnUpdateRow = new Button();
        private Button btnCommitChanges = new Button();

        private Label lblSrc = new Label();
        private Label lblDest = new Label();
        private Label lblAdded = new Label();
        private Label lblModified = new Label();

        private Panel pnlControl = new Panel();
        private Panel pnlAdded = new Panel();
        private Panel pnlModified = new Panel();

        private DataGridView destDgv = new DataGridView();
        private DataGridView srcDgv = new DataGridView();
    }
}
