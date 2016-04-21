using DBDiff.Schema;
using DBDiff.Schema.Misc;
using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Front;
using DBDiff.Schema.SQLServer.Generates.Generates;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.SQLServer.Generates.Options;
using DBDiff.Settings;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Assembly = System.Reflection.Assembly;

/*using DBDiff.Schema.SQLServer2000;
using DBDiff.Schema.SQLServer2000.Model;
using DBDiff.Schema.SQLServer2000.Compare;
/*using DBDiff.Schema.Sybase;
using DBDiff.Schema.Sybase.Options;
using DBDiff.Schema.Sybase.Model;*/

/*using DBDiff.Schema.MySQL;
using DBDiff.Schema.MySQL.Options;
using DBDiff.Schema.MySQL.Model;
*/

namespace DBDiff.Front
{
    public partial class Form1 : Form
    {
        //private MySqlOption MySQLfilter = new MySqlOption();
        //private AseOption AseFilter = new AseOption();

        private Project ActiveProject;
        private IFront mySqlConnectFront1;
        private IFront mySqlConnectFront2;
        private readonly SqlOption SqlFilter = new SqlOption();
        private List<ISchemaBase> _selectedSchemas = new List<ISchemaBase>();

        public Form1()
        {
            InitializeComponent();

            this.Text += Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        /*private void ProcessSybase()
        {
            DBDiff.Schema.Sybase.Model.Database origin;
            DBDiff.Schema.Sybase.Model.Database destination;

            DBDiff.Schema.Sybase.Generate sql = new DBDiff.Schema.Sybase.Generate();
            sql.ConnectioString = txtConnectionOrigen.Text;

            AseFilter.OptionFilter.FilterTrigger = false;

            origin = sql.Process(AseFilter);

            sql.ConnectioString = txtConnectionDestination.Text;
            destination = sql.Process(AseFilter);

            this.txtScript.SQLType = SQLEnum.SQLTypeEnum.Sybase;
            this.txtDiferencias.SQLType = SQLEnum.SQLTypeEnum.Sybase;
            //origin = DBDiff.Schema.Sybase.Generate.Compare(origin, destination);

            this.txtScript.Text = origin.ToSQL();
            //this.txtDiferencias.Text = origin.ToSQLDiff();
        }*/

        /*private void ProcessMySQL()
        {
            DBDiff.Schema.MySQL.Model.Database origin;
            DBDiff.Schema.MySQL.Model.Database destination;

            DBDiff.Schema.MySQL.Generate sql = new DBDiff.Schema.MySQL.Generate();
            sql.ConnectioString = mySqlConnectFront1.ConnectionString;
            origin = sql.Process(MySQLfilter);

            sql.ConnectioString = mySqlConnectFront2.ConnectionString;
            destination = sql.Process(MySQLfilter);

            //this.txtScript.SQLType = SQLEnum.SQLTypeEnum.MySQL;
            //this.txtDiferencias.SQLType = SQLEnum.SQLTypeEnum.MySQL;
            origin = DBDiff.Schema.MySQL.Generate.Compare(origin, destination);
            this.txtDiferencias.Text = origin.ToSQLDiff();
        }
        */

        private void ProcessSQL2005()
        {
            ProgressForm progress = null;
            string errorLocation = null;
            try
            {
                Database origin;
                Database destination;

                if ((!String.IsNullOrEmpty(mySqlConnectFront1.DatabaseName) &&
                     (!String.IsNullOrEmpty(mySqlConnectFront2.DatabaseName))))
                {
                    Generate sql1 = new Generate();
                    Generate sql2 = new Generate();

                    sql1.ConnectionString = mySqlConnectFront1.ConnectionString;
                    sql1.Options = SqlFilter;

                    sql2.ConnectionString = mySqlConnectFront2.ConnectionString;
                    sql2.Options = SqlFilter;

                    progress = new ProgressForm("Source Database", "Destination Database", sql2, sql1);
                    progress.ShowDialog(this);
                    if (progress.Error != null)
                    {
                        throw new SchemaException(progress.Error.Message, progress.Error);
                    }

                    origin = progress.Source;
                    destination = progress.Destination;

                    txtSyncScript.ConfigurationManager.Language = "mssql";
                    txtSyncScript.IsReadOnly = false;
                    txtSyncScript.Styles.LineNumber.BackColor = Color.White;
                    txtSyncScript.Styles.LineNumber.IsVisible = false;
                    errorLocation = "Generating Synchronized Script";
                    txtSyncScript.Text = destination.ToSqlDiff(_selectedSchemas).ToSQL();
                    txtSyncScript.IsReadOnly = true;
                    schemaTreeView1.DatabaseSource = destination;
                    schemaTreeView1.DatabaseDestination = origin;
                    schemaTreeView1.OnSelectItem += new SchemaTreeView.SchemaHandler(schemaTreeView1_OnSelectItem);
                    schemaTreeView1_OnSelectItem(schemaTreeView1.SelectedNode);
                    textBox1.Text = origin.ActionMessage.Message;

                    btnCopy.Enabled = true;
                    btnSaveAs.Enabled = true;
                    btnUpdateAll.Enabled = true;
                }
                else
                    MessageBox.Show(Owner, "Please select a valid connection string", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                if (errorLocation == null && progress != null)
                {
                    errorLocation = String.Format("{0} (while {1})", progress.ErrorLocation, progress.ErrorMostRecentProgress ?? "initializing");
                }

                throw new SchemaException("Error " + (errorLocation ?? " Comparing Databases"), ex);
            }
        }

        private void schemaTreeView1_OnSelectItem(string ObjectFullName)
        {
            if (ObjectFullName == null) return;

            txtNewObject.IsReadOnly = false;
            txtOldObject.IsReadOnly = false;
            txtNewObject.Text = "";
            txtOldObject.Text = "";

            Database database = (Database)schemaTreeView1.DatabaseSource;
            if (database.Find(ObjectFullName) != null)
            {
                if (database.Find(ObjectFullName).Status != Enums.ObjectStatusType.DropStatus)
                {
                    txtNewObject.Text = database.Find(ObjectFullName).ToSql();
                    if (database.Find(ObjectFullName).Status == Enums.ObjectStatusType.OriginalStatus)
                    {
                        btnUpdate.Enabled = false;
                    }
                    else {
                        btnUpdate.Enabled = true;
                    }
                    if (database.Find(ObjectFullName).ObjectType == Enums.ObjectType.Table)
                    {
                        btnCompareTableData.Enabled = true;
                    }
                    else {
                        btnCompareTableData.Enabled = false;
                    }
                }
            }

            database = (Database)schemaTreeView1.DatabaseDestination;
            if (database.Find(ObjectFullName) != null)
            {
                if (database.Find(ObjectFullName).Status != Enums.ObjectStatusType.CreateStatus)
                    txtOldObject.Text = database.Find(ObjectFullName).ToSql();
            }
            txtNewObject.IsReadOnly = true;
            txtOldObject.IsReadOnly = true;

            var diff = (new SideBySideDiffBuilder(new Differ())).BuildDiffModel(txtOldObject.Text, txtNewObject.Text);

            var sb = new StringBuilder();
            DiffPiece newLine, oldLine;
            var markers = new Marker[] { txtDiff.Markers[0], txtDiff.Markers[1], txtDiff.Markers[2], txtDiff.Markers[3] };
            foreach (var marker in markers) marker.Symbol = MarkerSymbol.Background;
            markers[0].BackColor = Color.LightGreen;
            markers[1].BackColor = Color.LightCyan;
            markers[2].BackColor = Color.LightSalmon;
            markers[3].BackColor = Color.PeachPuff;

            var indexes = new List<int>[] { new List<int>(), new List<int>(), new List<int>(), new List<int>() };
            var index = 0;
            for (var i = 0; i < Math.Max(diff.NewText.Lines.Count, diff.OldText.Lines.Count); i++)
            {
                newLine = i < diff.NewText.Lines.Count ? diff.NewText.Lines[i] : null;
                oldLine = i < diff.OldText.Lines.Count ? diff.OldText.Lines[i] : null;
                if (oldLine.Type == ChangeType.Inserted)
                {
                    sb.AppendLine(" " + oldLine.Text);
                }
                else if (oldLine.Type == ChangeType.Deleted)
                {
                    sb.AppendLine("- " + oldLine.Text);
                    indexes[2].Add(index);
                }
                else if (oldLine.Type == ChangeType.Modified)
                {
                    sb.AppendLine("* " + newLine.Text);
                    indexes[1].Add(index++);
                    sb.AppendLine("* " + oldLine.Text);
                    indexes[3].Add(index);
                }
                else if (oldLine.Type == ChangeType.Imaginary)
                {
                    sb.AppendLine("+ " + newLine.Text);
                    indexes[0].Add(index);
                }
                else if (oldLine.Type == ChangeType.Unchanged)
                {
                    sb.AppendLine("  " + oldLine.Text);
                }
                index++;
            }
            txtDiff.Text = sb.ToString();
            for (var i = 0; i < 4; i++)
            {
                foreach (var ind in indexes[i])
                {
                    txtDiff.Lines[ind].AddMarker(markers[i]);
                }
            }
        }

        void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Refresh script when tab is shown
            if (tabControl1.SelectedIndex != 1)
            {
                return;
            }

            var db = schemaTreeView1.DatabaseSource as Database;
            if (db != null)
            {
                this._selectedSchemas = this.schemaTreeView1.GetCheckedSchemas();
                this.txtSyncScript.IsReadOnly = false;
                this.txtSyncScript.Text = db.ToSqlDiff(this._selectedSchemas).ToSQL();
                this.txtSyncScript.IsReadOnly = false;
            }
        }

        /*private void ProcessSQL2000()
        {
            DBDiff.Schema.SQLServer2000.Model.Database origin;
            DBDiff.Schema.SQLServer2000.Model.Database destination;

            DBDiff.Schema.SQLServer2000.Generate sql = new DBDiff.Schema.SQLServer2000.Generate();

            lblMessage.Text = "Leyendo tablas de origin...";
            sql.OnTableProgress += new Progress.ProgressHandler(sql_OnTableProgress);
            //sql.ConnectioString = txtConnectionOrigen.Text;
            origin = sql.Process();

            //sql.ConnectioString = txtConnectionDestination.Text;
            lblMessage.Text = "Leyendo tablas de destination...";
            destination = sql.Process();

            origin = DBDiff.Schema.SQLServer2000.Generate.Compare(origin, destination);
            //this.txtScript.SQLType = SQLEnum.SQLTypeEnum.SQLServer;
            //this.txtDiferencias.SQLType = SQLEnum.SQLTypeEnum.SQLServer;
            this.txtDiferencias.Text = origin.ToSQLDiff();


        }
        */
        private void btnCompareTableData_Click(object sender, EventArgs e)
        {
            TreeView tree = (TreeView)schemaTreeView1.Controls.Find("treeView1", true)[0];
            ISchemaBase selected = (ISchemaBase)tree.SelectedNode.Tag;
            DataCompareForm dataCompare = new DataCompareForm(selected, mySqlConnectFront1.ConnectionString, mySqlConnectFront2.ConnectionString);
            dataCompare.Show();
        }
        private void btnCompare_Click(object sender, EventArgs e)
        {
            string errorLocation = "Processing Compare";
            try
            {
                Cursor = Cursors.WaitCursor;
                _selectedSchemas = schemaTreeView1.GetCheckedSchemas();
                //if (optSQL2000.Checked) ProcessSQL2000();
                if (optSQL2005.Checked) ProcessSQL2005();
                //if (optMySQL.Checked) ProcessMySQL();
                //if (optSybase.Checked) ProcessSybase();
                schemaTreeView1.SetCheckedSchemas(_selectedSchemas);
                errorLocation = "Saving Connections";
                Project.SaveLastConfiguration(mySqlConnectFront1.ConnectionString, mySqlConnectFront2.ConnectionString);
            }
            catch (Exception ex)
            {
                var exceptionList = new List<Exception>();
                exceptionList.Add(ex);
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    exceptionList.Insert(0, innerException);
                    innerException = innerException.InnerException;
                }

                var exceptionMsg = new StringBuilder();
                var prevMessage = exceptionList[0].Message;
                exceptionMsg.Append(this.Text);
                for (int i = 1; i < exceptionList.Count; ++i)
                {
                    if (exceptionList[i].Message != prevMessage)
                    {
                        exceptionMsg.AppendFormat("\r\n{0}", exceptionList[i].Message);
                        prevMessage = exceptionList[i].Message;
                    }
                }

                var ignoreSystem = new Regex(@"   at System\.[^\r\n]+\r\n|C:\\dev\\open-dbdiff\\");
                exceptionMsg.AppendFormat("\r\n{0}: {1}\r\n{2}", exceptionList[0].GetType().Name, exceptionList[0].Message, ignoreSystem.Replace(exceptionList[0].StackTrace, String.Empty));

                var ignoreChunks = new Regex(@": \[[^\)]*\)|\.\.\.\)|\'[^\']*\'|\([^\)]*\)|\" + '"' + @"[^\" + '"' + @"]*\" + '"' + @"|Source|Destination");
                var searchableError = ignoreChunks.Replace(exceptionMsg.ToString(), String.Empty);
                var searchableErrorBytes = Encoding.UTF8.GetBytes(searchableError);
                searchableErrorBytes = new MD5CryptoServiceProvider().ComputeHash(searchableErrorBytes);
                var searchHash = BitConverter.ToString(searchableErrorBytes).Replace("-", String.Empty);
                exceptionMsg.AppendFormat("\r\n\r\n{0}", searchHash);

                if (DialogResult.OK == MessageBox.Show(Owner, @"An unexpected error has occured during processing.
Clicking 'OK' will result in the following:

    1. The exception info below will be copied to the clipboard.

    2. Your default browser will search CodePlex for more details.

    • *Please* click 'Create New Work Item' and paste the error details
        into the Description field if there are no work items for this issue!
        (At least email the details to opendbdiff@gmail.com...)

    • Vote for existing work items; paste details into 'Add Comment'.

    • If possible, please attach a SQL script creating two dbs with
        the minimum necessary to reproduce the problem.

" + exceptionMsg.ToString(), "Error " + errorLocation, MessageBoxButtons.OKCancel, MessageBoxIcon.Error))
                {
                    try
                    {
                        System.Windows.Forms.Clipboard.SetText(exceptionMsg.ToString());
                        Process.Start("http://opendbiff.codeplex.com/workitem/list/basic?keywords=" + Uri.EscapeDataString(searchHash));
                    }
                    finally
                    {
                    }
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void optMySQL_CheckedChanged(object sender, EventArgs e)
        {
            /*if (optMySQL.Checked)
            {
                ShowMySQL();
            }
            else
            {
                this.groupBox2.Controls.Remove((System.Windows.Forms.Control)this.mySqlConnectFront1);
                this.groupBox3.Controls.Remove((System.Windows.Forms.Control)this.mySqlConnectFront2);
            }*/
        }

        /*private void ShowMySQL()
        {
            this.mySqlConnectFront2 = new DBDiff.Schema.MySQL.Front.MySqlConnectFront();
            this.mySqlConnectFront1 = new DBDiff.Schema.MySQL.Front.MySqlConnectFront();
            this.mySqlConnectFront1.Location = new System.Drawing.Point(5, 19);
            this.mySqlConnectFront1.Name = "mySqlConnectFront1";
            this.mySqlConnectFront1.Size = new System.Drawing.Size(420, 190);
            this.mySqlConnectFront1.TabIndex = 10;
            this.mySqlConnectFront2.Location = new System.Drawing.Point(5, 19);
            this.mySqlConnectFront2.Name = "mySqlConnectFront2";
            this.mySqlConnectFront2.Size = new System.Drawing.Size(420, 190);
            this.mySqlConnectFront2.TabIndex = 10;
            this.mySqlConnectFront1.Visible = true;
            this.mySqlConnectFront2.Visible = true;
            this.groupBox3.Controls.Add((System.Windows.Forms.Control)this.mySqlConnectFront2);
            this.groupBox2.Controls.Add((System.Windows.Forms.Control)this.mySqlConnectFront1);
        }
        */

        private void ShowSQL2005()
        {
            mySqlConnectFront2 = new SqlServerConnectFront();
            mySqlConnectFront1 = new SqlServerConnectFront();
            mySqlConnectFront1.Location = new Point(1, 1);
            mySqlConnectFront1.Name = "mySqlConnectFront1";
            mySqlConnectFront1.Anchor =
                (AnchorStyles)((int)AnchorStyles.Bottom + (int)AnchorStyles.Left + (int)AnchorStyles.Right);

            mySqlConnectFront1.TabIndex = 10;
            mySqlConnectFront1.Text = "Source Database:";
            mySqlConnectFront2.Location = new Point(1, 1);
            mySqlConnectFront2.Name = "mySqlConnectFront2";
            mySqlConnectFront2.Anchor =
                (AnchorStyles)((int)AnchorStyles.Bottom + (int)AnchorStyles.Left + (int)AnchorStyles.Right);
            mySqlConnectFront2.TabIndex = 10;
            mySqlConnectFront1.Visible = true;
            mySqlConnectFront2.Visible = true;
            mySqlConnectFront2.Text = "Destination Database:";
            ((SqlServerConnectFront)mySqlConnectFront1).UserName = "sa";
            ((SqlServerConnectFront)mySqlConnectFront1).Password = "";
            ((SqlServerConnectFront)mySqlConnectFront1).ServerName = "(local)";
            ((SqlServerConnectFront)mySqlConnectFront2).UserName = "sa";
            ((SqlServerConnectFront)mySqlConnectFront2).Password = "";
            ((SqlServerConnectFront)mySqlConnectFront2).ServerName = "(local)";
            ((SqlServerConnectFront)mySqlConnectFront1).DatabaseIndex = 1;
            ((SqlServerConnectFront)mySqlConnectFront2).DatabaseIndex = 2;
            PanelDestination.Controls.Add((Control)mySqlConnectFront2);
            PanelSource.Controls.Add((Control)mySqlConnectFront1);
        }

        private void optSQL2005_CheckedChanged(object sender, EventArgs e)
        {
            if (optSQL2005.Checked)
            {
                ShowSQL2005();
            }
            else
            {
                PanelSource.Controls.Remove((Control)mySqlConnectFront1);
                PanelDestination.Controls.Remove((Control)mySqlConnectFront2);
            }
        }

        private void optSybase_CheckedChanged(object sender, EventArgs e)
        {
            /*if (optSybase.Checked)
            {
                this.mySqlConnectFront2 = new DBDiff.Schema.Sybase.Front.AseConnectFront();
                this.mySqlConnectFront1 = new DBDiff.Schema.Sybase.Front.AseConnectFront();
                this.mySqlConnectFront1.Location = new System.Drawing.Point(5, 19);
                this.mySqlConnectFront1.Name = "mySqlConnectFront1";
                this.mySqlConnectFront1.Size = new System.Drawing.Size(410, 214);
                this.mySqlConnectFront1.TabIndex = 10;
                this.mySqlConnectFront2.Location = new System.Drawing.Point(5, 19);
                this.mySqlConnectFront2.Name = "mySqlConnectFront2";
                this.mySqlConnectFront2.Size = new System.Drawing.Size(410, 214);
                this.mySqlConnectFront2.TabIndex = 10;
                this.mySqlConnectFront1.Visible = true;
                this.mySqlConnectFront2.Visible = true;
                this.groupBox3.Controls.Add((System.Windows.Forms.Control)this.mySqlConnectFront2);
                this.groupBox2.Controls.Add((System.Windows.Forms.Control)this.mySqlConnectFront1);
            }
            else
            {
                this.groupBox2.Controls.Remove((System.Windows.Forms.Control)this.mySqlConnectFront1);
                this.groupBox3.Controls.Remove((System.Windows.Forms.Control)this.mySqlConnectFront2);
            }*/
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(saveFileDialog1.FileName) && !string.IsNullOrEmpty(Path.GetDirectoryName(saveFileDialog1.FileName)))
                {
                    saveFileDialog1.InitialDirectory = Path.GetDirectoryName(saveFileDialog1.FileName);
                    saveFileDialog1.FileName = Path.GetFileName(saveFileDialog1.FileName);
                }
                saveFileDialog1.ShowDialog(this);
                if (!String.IsNullOrEmpty(saveFileDialog1.FileName))
                {
                    var db = schemaTreeView1.DatabaseSource as Database;
                    if (db != null)
                    {
                        using (StreamWriter writer = new StreamWriter(saveFileDialog1.FileName, false))
                        {
                            this._selectedSchemas = this.schemaTreeView1.GetCheckedSchemas();
                            writer.Write(db.ToSqlDiff(this._selectedSchemas).ToSQL());
                            writer.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.Clipboard.SetText(txtSyncScript.Text);
            }
            finally
            {
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            TreeView tree = (TreeView)schemaTreeView1.Controls.Find("treeView1", true)[0];
            TreeNode dbArm = tree.Nodes[0];
            string result = "";

            foreach (TreeNode node in dbArm.Nodes)
            {
                if (node.Nodes.Count != 0)
                {
                    foreach (TreeNode subnode in node.Nodes)
                    {
                        if (subnode.Checked)
                        {
                            //ISchemaBase selected = (ISchemaBase)tree.SelectedNode.Tag;
                            ISchemaBase selected = (ISchemaBase)subnode.Tag;

                            Database database = (Database)schemaTreeView1.DatabaseSource;

                            if (database.Find(selected.FullName) != null)
                            {
                                switch (selected.ObjectType)
                                {
                                    case Enums.ObjectType.Table:
                                        {
                                            switch (selected.Status)
                                            {
                                                case Enums.ObjectStatusType.CreateStatus: result += Updater.createNew(selected, mySqlConnectFront2.ConnectionString); break;
                                                case Enums.ObjectStatusType.AlterStatus: result += Updater.alter(selected, mySqlConnectFront2.ConnectionString); break;
                                                case Enums.ObjectStatusType.AlterWhitespaceStatus: result += Updater.alter(selected, mySqlConnectFront2.ConnectionString); break;
                                                default: result += "Nothing could be found to do for table " + selected.Name + ".\r\n"; break;
                                            }
                                        }
                                        break;
                                    case Enums.ObjectType.StoredProcedure:
                                        {
                                            switch (selected.Status)
                                            {
                                                case Enums.ObjectStatusType.CreateStatus: result += Updater.createNew(selected, mySqlConnectFront2.ConnectionString); break;
                                                case Enums.ObjectStatusType.AlterStatus: result += Updater.alter(selected, mySqlConnectFront2.ConnectionString); break;
                                                case Enums.ObjectStatusType.AlterWhitespaceStatus: result += Updater.alter(selected, mySqlConnectFront2.ConnectionString); break;
                                                default: result += "Nothing could be found to do for stored procedure " + selected.Name + ".\r\n"; break;
                                            }
                                        }
                                        break;
                                    case Enums.ObjectType.Function:
                                        {
                                            switch (selected.Status)
                                            {
                                                case Enums.ObjectStatusType.CreateStatus: result += Updater.createNew(selected, mySqlConnectFront2.ConnectionString); break;
                                                case Enums.ObjectStatusType.AlterStatus: result += Updater.alter(selected, mySqlConnectFront2.ConnectionString); break;
                                                case Enums.ObjectStatusType.AlterWhitespaceStatus: result += Updater.alter(selected, mySqlConnectFront2.ConnectionString); break;
                                                case Enums.ObjectStatusType.AlterStatus | Enums.ObjectStatusType.AlterBodyStatus: result += Updater.alter(selected, mySqlConnectFront2.ConnectionString); break;
                                                default: result += "Nothing could be found to do for function " + selected.Name + ".\r\n"; break;
                                            }
                                        }
                                        break;
                                    case Enums.ObjectType.View:
                                        {
                                            switch (selected.Status)
                                            {
                                                case Enums.ObjectStatusType.CreateStatus: result += Updater.createNew(selected, mySqlConnectFront2.ConnectionString); break;
                                                case Enums.ObjectStatusType.AlterStatus: result += Updater.alter(selected, mySqlConnectFront2.ConnectionString); break;
                                                case Enums.ObjectStatusType.AlterWhitespaceStatus: result += Updater.alter(selected, mySqlConnectFront2.ConnectionString); break;
                                                case Enums.ObjectStatusType.AlterStatus | Enums.ObjectStatusType.AlterBodyStatus: result += Updater.alter(selected, mySqlConnectFront2.ConnectionString); break;
                                                default: result += "Nothing could be found to do for view " + selected.Name + ".\r\n"; break;
                                            }
                                        }
                                        break;
                                    default:
                                        {
                                            switch (selected.Status)
                                            {
                                                case Enums.ObjectStatusType.CreateStatus: result += Updater.addNew(selected, mySqlConnectFront2.ConnectionString); break;
                                                default: result += "Nothing could be found to do for " + selected.Name + ".\r\n"; break;
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            if (result == string.Empty)
            {
                result = "All successful";
            }
            MessageBox.Show(result);

            if (SqlFilter.Comparison.ReloadComparisonOnUpdate)
            {
                if (optSQL2005.Checked) ProcessSQL2005();
            }

            btnUpdate.Enabled = false;
        }

        private void btnUpdateAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to update all?", "Confirm update", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                TreeView tree = (TreeView)schemaTreeView1.Controls.Find("treeView1", true)[0];
                TreeNode database = tree.Nodes[0];
                string result = "";
                foreach (TreeNode tn in database.Nodes)
                {
                    foreach (TreeNode inner in tn.Nodes)
                    {
                        if (inner.Tag != null)
                        {
                            ISchemaBase item = (ISchemaBase)inner.Tag;
                            switch (item.Status)
                            {
                                case Enums.ObjectStatusType.CreateStatus: result += Updater.createNew(item, mySqlConnectFront2.ConnectionString); break;
                                case Enums.ObjectStatusType.AlterStatus: result += Updater.alter(item, mySqlConnectFront2.ConnectionString); break;
                            }
                        }
                    }
                }
                if (result == string.Empty)
                {
                    result = "Update successful";
                }
                MessageBox.Show(result);
                if (optSQL2005.Checked) ProcessSQL2005();
            }
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            OptionForm form = new OptionForm();
            form.Show(Owner, SqlFilter);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ShowSQL2005();
            txtNewObject.ConfigurationManager.Language = "mssql";
            txtNewObject.IsReadOnly = false;
            txtNewObject.Styles.LineNumber.BackColor = Color.White;
            txtNewObject.Styles.LineNumber.IsVisible = false;
            txtOldObject.ConfigurationManager.Language = "mssql";
            txtOldObject.IsReadOnly = false;
            txtOldObject.Styles.LineNumber.BackColor = Color.White;
            txtOldObject.Styles.LineNumber.IsVisible = false;
            txtDiff.ConfigurationManager.Language = "mssql";
            txtDiff.IsReadOnly = false;
            txtDiff.Styles.LineNumber.IsVisible = false;
            txtDiff.Margins[0].Width = 20;
            Project LastConfiguration = Project.GetLastConfiguration();
            if (LastConfiguration != null)
            {
                mySqlConnectFront1.ConnectionString = LastConfiguration.ConnectionStringSource;
                mySqlConnectFront2.ConnectionString = LastConfiguration.ConnectionStringDestination;
            }

            txtSyncScript.Text = "";
        }

        private void panel2_Resize(object sender, EventArgs e)
        {
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            panel2.Left = Math.Max(this.btnProject.Right + this.btnProject.Left, (Width - panel2.Width) / 2);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (ActiveProject == null)
                {
                    ActiveProject = new Project
                    {
                        ConnectionStringSource = mySqlConnectFront1.ConnectionString,
                        ConnectionStringDestination = mySqlConnectFront2.ConnectionString,
                        Name = String.Format("[{0}].[{1}] - [{2}].[{3}]",
                                                        ((SqlServerConnectFront)mySqlConnectFront1).ServerName,
                                                        mySqlConnectFront1.DatabaseName,
                                                        ((SqlServerConnectFront)mySqlConnectFront2).ServerName,
                                                        mySqlConnectFront2.DatabaseName),
                        Type = Project.ProjectType.SQLServer
                    };
                }
                ActiveProject.Id = Project.Save(ActiveProject);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnProject_Click(object sender, EventArgs e)
        {
            try
            {
                ListProjectsForm form = new ListProjectsForm(Project.GetAll());
                form.OnSelect += new ListProjectHandler(form_OnSelect);
                form.OnDelete += new ListProjectHandler(form_OnDelete);
                form.OnRename += new ListProjectHandler(form_OnRename);
                form.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void form_OnRename(Project itemSelected)
        {
            try
            {
                Project.Save(itemSelected);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void form_OnDelete(Project itemSelected)
        {
            try
            {
                Project.Delete(itemSelected.Id);
                if (ActiveProject != null)
                {
                    if (ActiveProject.Id == itemSelected.Id)
                    {
                        ActiveProject = null;
                        mySqlConnectFront1.ConnectionString = "";
                        mySqlConnectFront2.ConnectionString = "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void form_OnSelect(Project itemSelected)
        {
            try
            {
                if (itemSelected != null)
                {
                    ActiveProject = itemSelected;
                    mySqlConnectFront1.ConnectionString = itemSelected.ConnectionStringSource;
                    mySqlConnectFront2.ConnectionString = itemSelected.ConnectionStringDestination;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNewProject_Click(object sender, EventArgs e)
        {
            mySqlConnectFront1.ConnectionString = "";
            mySqlConnectFront2.ConnectionString = "";
            ActiveProject = null;
        }
    }
}
