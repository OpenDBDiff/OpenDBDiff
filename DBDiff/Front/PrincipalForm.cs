using DBDiff.Schema;
using DBDiff.Schema.Misc;
using DBDiff.Schema.Model;
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

namespace DBDiff.Front
{
    public partial class PrincipalForm : Form
    {

        private Project ActiveProject;
        private IFront SourceSelector;
        private IFront DestinationSelector;
        private IOption Options;
        private List<ISchemaBase> _selectedSchemas = new List<ISchemaBase>();

        List<IProjectHandler> ProjectHandlers = new List<IProjectHandler>();
        DBDiff.Front.IProjectHandler ProjectSelectorHandler;


        IDatabase selectedOrigin;
        IDatabase selectedDestination;
        private readonly string[] ProjectHandlerAssemblies = new string[1] { "DBDiff.Schema.SQLServer" };

        public PrincipalForm()
        {
            InitializeComponent();

            this.Text += Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void StartComparision()
        {
            ProgressForm progress = null;
            string errorLocation = null;
            try
            {
                if ((!String.IsNullOrEmpty(ProjectSelectorHandler.GetSourceDatabaseName()) &&
                     (!String.IsNullOrEmpty(ProjectSelectorHandler.GetDestinationDatabaseName()))))
                {
                    Options = this.ProjectSelectorHandler.GetProjectOptions();
                    IGenerator sourceGenerator = this.ProjectSelectorHandler.SetSourceGenerator(SourceSelector.ConnectionString, Options);
                    IGenerator destinationGenerator = this.ProjectSelectorHandler.SetDestinationGenerator(DestinationSelector.ConnectionString, Options);
                    IDatabaseComparer databaseComparer = this.ProjectSelectorHandler.GetDatabaseComparer();

                    progress = new ProgressForm("Source Database", "Destination Database", destinationGenerator, sourceGenerator, databaseComparer);
                    progress.ShowDialog(this);
                    if (progress.Error != null)
                    {
                        throw new SchemaException(progress.Error.Message, progress.Error);
                    }

                    selectedOrigin = progress.Source;
                    selectedDestination = progress.Destination;

                    txtSyncScript.ConfigurationManager.Language = this.ProjectSelectorHandler.GetScriptLanguage();
                    txtSyncScript.IsReadOnly = false;
                    txtSyncScript.Styles.LineNumber.BackColor = Color.White;
                    txtSyncScript.Styles.LineNumber.IsVisible = false;
                    errorLocation = "Generating Synchronized Script";
                    txtSyncScript.Text = selectedDestination.ToSqlDiff(this._selectedSchemas).ToSQL();
                    txtSyncScript.IsReadOnly = true;
                    schemaTreeView1.DatabaseSource = selectedDestination;
                    schemaTreeView1.DatabaseDestination = selectedOrigin;
                    schemaTreeView1.OnSelectItem += new SchemaTreeView.SchemaHandler(schemaTreeView1_OnSelectItem);
                    schemaTreeView1_OnSelectItem(schemaTreeView1.SelectedNode);
                    textBox1.Text = selectedOrigin.ActionMessage.Message;

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

            IDatabase database = (IDatabase)schemaTreeView1.DatabaseSource;
            if (database.Find(ObjectFullName) != null)
            {
                if (database.Find(ObjectFullName).Status != Enums.ObjectStatusType.DropStatus)
                {
                    txtNewObject.Text = database.Find(ObjectFullName).ToSql();
                    if (database.Find(ObjectFullName).Status == Enums.ObjectStatusType.OriginalStatus)
                    {
                        btnUpdate.Enabled = false;
                    }
                    else
                    {
                        btnUpdate.Enabled = true;
                    }
                    if (database.Find(ObjectFullName).ObjectType == Enums.ObjectType.Table)
                    {
                        btnCompareTableData.Enabled = true;
                    }
                    else
                    {
                        btnCompareTableData.Enabled = false;
                    }
                }
            }

            database = (IDatabase)schemaTreeView1.DatabaseDestination;
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

            var db = schemaTreeView1.DatabaseSource as IDatabase;
            if (db != null)
            {
                this._selectedSchemas = this.schemaTreeView1.GetCheckedSchemas();
                this.txtSyncScript.IsReadOnly = false;
                this.txtSyncScript.Text = db.ToSqlDiff(this._selectedSchemas).ToSQL();
                this.txtSyncScript.IsReadOnly = false;
            }
        }
        private void btnCompareTableData_Click(object sender, EventArgs e)
        {
            TreeView tree = (TreeView)schemaTreeView1.Controls.Find("treeView1", true)[0];
            ISchemaBase selected = (ISchemaBase)tree.SelectedNode.Tag;
            DataCompareForm dataCompare = new DataCompareForm(selected, SourceSelector.ConnectionString, DestinationSelector.ConnectionString);
            dataCompare.Show();
        }
        private void btnCompare_Click(object sender, EventArgs e)
        {
            string errorLocation = "Processing Compare";
            try
            {
                Cursor = Cursors.WaitCursor;
                _selectedSchemas = schemaTreeView1.GetCheckedSchemas();
                StartComparision();
                schemaTreeView1.SetCheckedSchemas(_selectedSchemas);
                errorLocation = "Saving Connections";
                Project.SaveLastConfiguration(SourceSelector.ConnectionString, DestinationSelector.ConnectionString);
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

        private void UnloadProjectHandler()
        {
            if (ProjectSelectorHandler != null)
            {
                PanelSource.Controls.Remove((Control)SourceSelector);
                PanelDestination.Controls.Remove((Control)DestinationSelector);
                ProjectSelectorHandler.Unload();
                ProjectSelectorHandler = null;
            }
        }

        private void LoadProjectHandler(IProjectHandler projectHandler)
        {
            UnloadProjectHandler();
            ProjectSelectorHandler = projectHandler;
            SourceSelector = ProjectSelectorHandler.CreateSourceSelector();
            DestinationSelector = ProjectSelectorHandler.CreateDestinationSelector();
            PanelSource.Controls.Add(SourceSelector.Control);
            PanelDestination.Controls.Add(DestinationSelector.Control);
        }

        private void LoadProjectHandler<T>() where T : IProjectHandler, new()
        {
            var handler = new T();
            LoadProjectHandler(handler);
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
                    var db = schemaTreeView1.DatabaseSource as IDatabase;
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
            catch (Exception ex)
            {
                MessageBox.Show("An error ocurred while trying to copying the text to the clipboard");
                System.Diagnostics.Trace.WriteLine("ERROR: +" + ex.Message);
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

                            IDatabase database = (IDatabase)schemaTreeView1.DatabaseSource;

                            if (database.Find(selected.FullName) != null)
                            {
                                switch (selected.ObjectType)
                                {
                                    case Enums.ObjectType.Table:
                                        {
                                            switch (selected.Status)
                                            {
                                                case Enums.ObjectStatusType.CreateStatus: result += Updater.createNew(selected, DestinationSelector.ConnectionString); break;
                                                case Enums.ObjectStatusType.AlterStatus: result += Updater.alter(selected, DestinationSelector.ConnectionString); break;
                                                case Enums.ObjectStatusType.AlterWhitespaceStatus: result += Updater.alter(selected, DestinationSelector.ConnectionString); break;
                                                default: result += "Nothing could be found to do for table " + selected.Name + ".\r\n"; break;
                                            }
                                        }
                                        break;
                                    case Enums.ObjectType.StoredProcedure:
                                        {
                                            switch (selected.Status)
                                            {
                                                case Enums.ObjectStatusType.CreateStatus: result += Updater.createNew(selected, DestinationSelector.ConnectionString); break;
                                                case Enums.ObjectStatusType.AlterStatus: result += Updater.alter(selected, DestinationSelector.ConnectionString); break;
                                                case Enums.ObjectStatusType.AlterWhitespaceStatus: result += Updater.alter(selected, DestinationSelector.ConnectionString); break;
                                                default: result += "Nothing could be found to do for stored procedure " + selected.Name + ".\r\n"; break;
                                            }
                                        }
                                        break;
                                    case Enums.ObjectType.Function:
                                        {
                                            switch (selected.Status)
                                            {
                                                case Enums.ObjectStatusType.CreateStatus: result += Updater.createNew(selected, DestinationSelector.ConnectionString); break;
                                                case Enums.ObjectStatusType.AlterStatus: result += Updater.alter(selected, DestinationSelector.ConnectionString); break;
                                                case Enums.ObjectStatusType.AlterWhitespaceStatus: result += Updater.alter(selected, DestinationSelector.ConnectionString); break;
                                                case Enums.ObjectStatusType.AlterStatus | Enums.ObjectStatusType.AlterBodyStatus: result += Updater.alter(selected, DestinationSelector.ConnectionString); break;
                                                default: result += "Nothing could be found to do for function " + selected.Name + ".\r\n"; break;
                                            }
                                        }
                                        break;
                                    case Enums.ObjectType.View:
                                        {
                                            switch (selected.Status)
                                            {
                                                case Enums.ObjectStatusType.CreateStatus: result += Updater.createNew(selected, DestinationSelector.ConnectionString); break;
                                                case Enums.ObjectStatusType.AlterStatus: result += Updater.alter(selected, DestinationSelector.ConnectionString); break;
                                                case Enums.ObjectStatusType.AlterWhitespaceStatus: result += Updater.alter(selected, DestinationSelector.ConnectionString); break;
                                                case Enums.ObjectStatusType.AlterStatus | Enums.ObjectStatusType.AlterBodyStatus: result += Updater.alter(selected, DestinationSelector.ConnectionString); break;
                                                default: result += "Nothing could be found to do for view " + selected.Name + ".\r\n"; break;
                                            }
                                        }
                                        break;
                                    default:
                                        {
                                            switch (selected.Status)
                                            {
                                                case Enums.ObjectStatusType.CreateStatus: result += Updater.addNew(selected, DestinationSelector.ConnectionString); break;
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

            if (Options.Comparison.ReloadComparisonOnUpdate)
            {
                StartComparision();
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
                                case Enums.ObjectStatusType.CreateStatus: result += Updater.createNew(item, DestinationSelector.ConnectionString); break;
                                case Enums.ObjectStatusType.AlterStatus: result += Updater.alter(item, DestinationSelector.ConnectionString); break;
                            }
                        }
                    }
                }
                if (result == string.Empty)
                {
                    result = "Update successful";
                }
                MessageBox.Show(result);
                StartComparision();
            }
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            OptionForm form = new OptionForm(this.ProjectSelectorHandler);
            form.Show(Owner, Options);
        }

        private void LoadProjectHandlers(string[] assemblyNames)
        {
            Type projectHandlerType = typeof(IProjectHandler);
            ProjectHandlers.Clear();
            toolProjectTypes.Items.Clear();
            List<Type> projectHandlerTypes = new List<Type>();
            List<Assembly> loadedAssemblies = new List<System.Reflection.Assembly>();
            for (int i = 0; i < assemblyNames.Length; i++)
            {
                loadedAssemblies.Add(Assembly.Load(assemblyNames[i]));
            }

            foreach (var assembly in loadedAssemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (projectHandlerType.IsAssignableFrom(type) && type.IsClass)
                    {
                        projectHandlerTypes.Add(type);
                    }
                }
            }
            foreach (var handlerType in projectHandlerTypes)
            {
                ProjectHandlers.Add(Activator.CreateInstance(handlerType) as IProjectHandler);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadProjectHandlers(ProjectHandlerAssemblies);
            foreach (var projectHandler in ProjectHandlers)
            {
                toolProjectTypes.Items.Add(projectHandler);
            }

            Project LastConfiguration = Project.GetLastConfiguration();
            if (LastConfiguration != null)
            {
                if (SourceSelector != null)
                    SourceSelector.ConnectionString = LastConfiguration.ConnectionStringSource;
                if (DestinationSelector != null)
                    DestinationSelector.ConnectionString = LastConfiguration.ConnectionStringDestination;
            }
            if (toolProjectTypes.SelectedItem == null && toolProjectTypes.Items.Count > 0)
            {
                toolProjectTypes.SelectedIndex = 0;
            }


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

            txtSyncScript.Text = "";
        }

        private void panel2_Resize(object sender, EventArgs e)
        {
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //panel2.Left = Math.Max(this.btnProject.Right + this.btnProject.Left, (Width - panel2.Width) / 2);
        }

        private void btnSaveProject_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (ActiveProject == null)
                {
                    ActiveProject = new Project
                    {
                        ConnectionStringSource = ProjectSelectorHandler.GetSourceConnectionString(),
                        ConnectionStringDestination = ProjectSelectorHandler.GetDestinationConnectionString(),
                        Name = String.Format("[{0}].[{1}] - [{2}].[{3}]",

                                                        ProjectSelectorHandler.GetSourceServerName(),
                                                        ProjectSelectorHandler.GetSourceDatabaseName(),
                                                        ProjectSelectorHandler.GetDestinationServerName(),
                                                        ProjectSelectorHandler.GetDestinationDatabaseName()),
                        Options = Options ?? ProjectSelectorHandler.GetProjectOptions(),
                        Type = Project.ProjectType.SQLServer
                    };
                }
                ActiveProject.Id = Project.Save(ActiveProject);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        SourceSelector.ConnectionString = "";
                        DestinationSelector.ConnectionString = "";
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
                    SourceSelector.ConnectionString = itemSelected.ConnectionStringSource;
                    DestinationSelector.ConnectionString = itemSelected.ConnectionStringDestination;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNewProject_Click(object sender, EventArgs e)
        {
            SourceSelector.ConnectionString = "";
            DestinationSelector.ConnectionString = "";
            ActiveProject = null;
        }

        private void toolProjectTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            UnloadProjectHandler();
            if (toolProjectTypes.SelectedItem != null)
            {
                var handler = toolProjectTypes.SelectedItem as IProjectHandler;
                LoadProjectHandler(handler);
            }
        }
    }
}
