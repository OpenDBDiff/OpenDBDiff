using OpenDBDiff.Abstractions.Schema.Model;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace OpenDBDiff.UI
{
    public partial class DataCompareForm : Form
    {
        public DataCompareForm(ISchemaBase Selected, string SrcConnectionString, string DestConnectionString)
        {
            InitializeComponent();
            this.selected = Selected;
            this.srcConnectionString = SrcConnectionString;
            this.destConnectionString = DestConnectionString;

            doCompare();
        }

        private void doCompare()
        {
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

        private void btnCommitChanges_Click(object sender, EventArgs e)
        {
            DataTable destination = (DataTable)destDgv.DataSource;
            DataTable edits = destination.GetChanges();
            if (Updater.CommitTable(edits, selected.FullName, destConnectionString))
            {
                destination.AcceptChanges();
                doCompare();
                btnCommitChanges.Enabled = false;
            }
        }

        private void btnUpdateRow_Click(object sender, EventArgs e)
        {
            DataTable source = (DataTable)srcDgv.DataSource;
            DataTable destination = (DataTable)destDgv.DataSource;

            object[] sourceItems = source.Rows[srcDgv.CurrentRow.Index].ItemArray;

            for (int i = 0; i < destination.Columns.Count; i++)
            {
                if (destination.Columns[i].Unique)
                {
                    if (destination.Rows.Find(sourceItems[i]) == null && destination.Columns[i].AutoIncrement)
                    {
                        sourceItems[i] = null;
                    }
                }
            }

            destination.BeginLoadData();
            destination.LoadDataRow(sourceItems, false);
            destination.EndLoadData();
            btnCommitChanges.Enabled = true;
        }

        private void btnMerge_Click(object sender, EventArgs e)
        {
            DataTable source = (DataTable)srcDgv.DataSource;
            DataTable destination = (DataTable)destDgv.DataSource;

            destination.Merge(source, true);
            foreach (DataRow dr in destination.Rows)
            {
                if (dr.RowState == DataRowState.Unchanged)
                {
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
    }
}
