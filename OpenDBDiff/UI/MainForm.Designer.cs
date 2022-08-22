using System.ComponentModel;
using System.Windows.Forms;
using ScintillaNET;

namespace OpenDBDiff.UI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      this.lblMessage = new System.Windows.Forms.Label();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.tabControl2 = new System.Windows.Forms.TabControl();
      this.tabPage4 = new System.Windows.Forms.TabPage();
      this.txtNewObject = new ScintillaNET.Scintilla();
      this.tabPage5 = new System.Windows.Forms.TabPage();
      this.txtOldObject = new ScintillaNET.Scintilla();
      this.tabPage6 = new System.Windows.Forms.TabPage();
      this.txtDiff = new ScintillaNET.Scintilla();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.lblDiffDeletions = new System.Windows.Forms.Label();
      this.lblDiffModifications = new System.Windows.Forms.Label();
      this.lblDiffAdditions = new System.Windows.Forms.Label();
      this.BtnDiffDown = new System.Windows.Forms.Button();
      this.BtnDiffUp = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.panel1 = new System.Windows.Forms.Panel();
      this.txtSyncScript = new ScintillaNET.Scintilla();
      this.tabPage3 = new System.Windows.Forms.TabPage();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
      this.PanelGlobal = new System.Windows.Forms.Panel();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.LeftDatabasePanel = new System.Windows.Forms.Panel();
      this.RightDatabasePanel = new System.Windows.Forms.Panel();
      this.SwapButton = new System.Windows.Forms.Button();
      this.btnNewProject = new System.Windows.Forms.Button();
      this.btnSaveProject = new System.Windows.Forms.Button();
      this.btnProject = new System.Windows.Forms.Button();
      this.toolMenu = new System.Windows.Forms.ToolStrip();
      this.toolOpenProject = new System.Windows.Forms.ToolStripButton();
      this.toolNewProject = new System.Windows.Forms.ToolStripButton();
      this.toolSaveProject = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.toolLblProjectType = new System.Windows.Forms.ToolStripLabel();
      this.toolProjectTypes = new System.Windows.Forms.ToolStripComboBox();
      this.PanelActions = new System.Windows.Forms.FlowLayoutPanel();
      this.btnCompare = new System.Windows.Forms.Button();
      this.btnOptions = new System.Windows.Forms.Button();
      this.btnSaveAs = new System.Windows.Forms.Button();
      this.btnCopy = new System.Windows.Forms.Button();
      this.btnUpdate = new System.Windows.Forms.Button();
      this.btnCompareTableData = new System.Windows.Forms.Button();
      this.btnUpdateAll = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.schemaTreeView1 = new OpenDBDiff.UI.SchemaTreeView();
      this.tabControl1.SuspendLayout();
      this.tabPage2.SuspendLayout();
      this.tabControl2.SuspendLayout();
      this.tabPage4.SuspendLayout();
      this.tabPage5.SuspendLayout();
      this.tabPage6.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.panel1.SuspendLayout();
      this.tabPage3.SuspendLayout();
      this.PanelGlobal.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      this.toolMenu.SuspendLayout();
      this.PanelActions.SuspendLayout();
      this.SuspendLayout();
      // 
      // lblMessage
      // 
      this.lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.lblMessage.AutoSize = true;
      this.lblMessage.Location = new System.Drawing.Point(9, 812);
      this.lblMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblMessage.Name = "lblMessage";
      this.lblMessage.Size = new System.Drawing.Size(0, 20);
      this.lblMessage.TabIndex = 4;
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.tabPage2);
      this.tabControl1.Controls.Add(this.tabPage1);
      this.tabControl1.Controls.Add(this.tabPage3);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.Location = new System.Drawing.Point(0, 269);
      this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(1366, 743);
      this.tabControl1.TabIndex = 3;
      this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.tabControl2);
      this.tabPage2.Controls.Add(this.groupBox2);
      this.tabPage2.Controls.Add(this.groupBox1);
      this.tabPage2.Location = new System.Drawing.Point(4, 29);
      this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabPage2.Size = new System.Drawing.Size(1358, 710);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Schema";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // tabControl2
      // 
      this.tabControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl2.Controls.Add(this.tabPage4);
      this.tabControl2.Controls.Add(this.tabPage5);
      this.tabControl2.Controls.Add(this.tabPage6);
      this.tabControl2.Location = new System.Drawing.Point(525, 77);
      this.tabControl2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabControl2.Name = "tabControl2";
      this.tabControl2.SelectedIndex = 0;
      this.tabControl2.Size = new System.Drawing.Size(819, 618);
      this.tabControl2.TabIndex = 3;
      this.tabControl2.SelectedIndexChanged += new System.EventHandler(this.tabControl2_SelectedIndexChanged);
      // 
      // tabPage4
      // 
      this.tabPage4.Controls.Add(this.txtNewObject);
      this.tabPage4.Location = new System.Drawing.Point(4, 29);
      this.tabPage4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabPage4.Name = "tabPage4";
      this.tabPage4.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabPage4.Size = new System.Drawing.Size(811, 585);
      this.tabPage4.TabIndex = 0;
      this.tabPage4.Text = "New object";
      this.tabPage4.UseVisualStyleBackColor = true;
      // 
      // txtNewObject
      // 
      this.txtNewObject.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtNewObject.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtNewObject.Location = new System.Drawing.Point(4, 5);
      this.txtNewObject.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.txtNewObject.Name = "txtNewObject";
      this.txtNewObject.Size = new System.Drawing.Size(803, 575);
      this.txtNewObject.TabIndex = 0;
      // 
      // tabPage5
      // 
      this.tabPage5.Controls.Add(this.txtOldObject);
      this.tabPage5.Location = new System.Drawing.Point(4, 29);
      this.tabPage5.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabPage5.Name = "tabPage5";
      this.tabPage5.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabPage5.Size = new System.Drawing.Size(811, 585);
      this.tabPage5.TabIndex = 1;
      this.tabPage5.Text = "Old object";
      this.tabPage5.UseVisualStyleBackColor = true;
      // 
      // txtOldObject
      // 
      this.txtOldObject.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtOldObject.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtOldObject.Location = new System.Drawing.Point(4, 5);
      this.txtOldObject.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.txtOldObject.Name = "txtOldObject";
      this.txtOldObject.Size = new System.Drawing.Size(803, 575);
      this.txtOldObject.TabIndex = 0;
      // 
      // tabPage6
      // 
      this.tabPage6.Controls.Add(this.txtDiff);
      this.tabPage6.Location = new System.Drawing.Point(4, 29);
      this.tabPage6.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabPage6.Name = "tabPage6";
      this.tabPage6.Size = new System.Drawing.Size(811, 585);
      this.tabPage6.TabIndex = 2;
      this.tabPage6.Text = "Diff";
      this.tabPage6.UseVisualStyleBackColor = true;
      // 
      // txtDiff
      // 
      this.txtDiff.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtDiff.Location = new System.Drawing.Point(0, 0);
      this.txtDiff.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.txtDiff.Name = "txtDiff";
      this.txtDiff.Size = new System.Drawing.Size(811, 585);
      this.txtDiff.TabIndex = 0;
      // 
      // groupBox2
      // 
      this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox2.Controls.Add(this.lblDiffDeletions);
      this.groupBox2.Controls.Add(this.lblDiffModifications);
      this.groupBox2.Controls.Add(this.lblDiffAdditions);
      this.groupBox2.Controls.Add(this.BtnDiffDown);
      this.groupBox2.Controls.Add(this.BtnDiffUp);
      this.groupBox2.Controls.Add(this.label3);
      this.groupBox2.Controls.Add(this.label2);
      this.groupBox2.Controls.Add(this.label1);
      this.groupBox2.Location = new System.Drawing.Point(525, 8);
      this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox2.Size = new System.Drawing.Size(820, 62);
      this.groupBox2.TabIndex = 2;
      this.groupBox2.TabStop = false;
      // 
      // lblDiffDeletions
      // 
      this.lblDiffDeletions.BackColor = System.Drawing.Color.Red;
      this.lblDiffDeletions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lblDiffDeletions.ForeColor = System.Drawing.Color.White;
      this.lblDiffDeletions.Location = new System.Drawing.Point(459, 20);
      this.lblDiffDeletions.Name = "lblDiffDeletions";
      this.lblDiffDeletions.Size = new System.Drawing.Size(52, 31);
      this.lblDiffDeletions.TabIndex = 11;
      this.lblDiffDeletions.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lblDiffModifications
      // 
      this.lblDiffModifications.BackColor = System.Drawing.Color.Blue;
      this.lblDiffModifications.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lblDiffModifications.ForeColor = System.Drawing.Color.White;
      this.lblDiffModifications.Location = new System.Drawing.Point(233, 20);
      this.lblDiffModifications.Name = "lblDiffModifications";
      this.lblDiffModifications.Size = new System.Drawing.Size(52, 31);
      this.lblDiffModifications.TabIndex = 10;
      this.lblDiffModifications.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lblDiffAdditions
      // 
      this.lblDiffAdditions.BackColor = System.Drawing.Color.Lime;
      this.lblDiffAdditions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lblDiffAdditions.ForeColor = System.Drawing.Color.Black;
      this.lblDiffAdditions.Location = new System.Drawing.Point(10, 20);
      this.lblDiffAdditions.Name = "lblDiffAdditions";
      this.lblDiffAdditions.Size = new System.Drawing.Size(51, 31);
      this.lblDiffAdditions.TabIndex = 9;
      this.lblDiffAdditions.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // BtnDiffDown
      // 
      this.BtnDiffDown.BackColor = System.Drawing.SystemColors.Window;
      this.BtnDiffDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
      this.BtnDiffDown.Font = new System.Drawing.Font("Wingdings 3", 10F);
      this.BtnDiffDown.Location = new System.Drawing.Point(754, 17);
      this.BtnDiffDown.Name = "BtnDiffDown";
      this.BtnDiffDown.Size = new System.Drawing.Size(50, 37);
      this.BtnDiffDown.TabIndex = 7;
      this.BtnDiffDown.Text = "q";
      this.BtnDiffDown.UseVisualStyleBackColor = false;
      this.BtnDiffDown.Visible = false;
      this.BtnDiffDown.Click += new System.EventHandler(this.BtnDiffDown_Click);
      // 
      // BtnDiffUp
      // 
      this.BtnDiffUp.BackColor = System.Drawing.SystemColors.Window;
      this.BtnDiffUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
      this.BtnDiffUp.Font = new System.Drawing.Font("Wingdings 3", 10F);
      this.BtnDiffUp.Location = new System.Drawing.Point(703, 17);
      this.BtnDiffUp.Name = "BtnDiffUp";
      this.BtnDiffUp.Size = new System.Drawing.Size(50, 37);
      this.BtnDiffUp.TabIndex = 6;
      this.BtnDiffUp.Text = "p";
      this.BtnDiffUp.UseVisualStyleBackColor = false;
      this.BtnDiffUp.Visible = false;
      this.BtnDiffUp.Click += new System.EventHandler(this.BtnDiffUp_Click);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(518, 25);
      this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(91, 20);
      this.label3.TabIndex = 5;
      this.label3.Text = "Drop object";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(292, 25);
      this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(114, 20);
      this.label2.TabIndex = 4;
      this.label2.Text = "Alter old object";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(68, 25);
      this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(137, 20);
      this.label1.TabIndex = 3;
      this.label1.Text = "Create new object";
      // 
      // groupBox1
      // 
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.groupBox1.Controls.Add(this.schemaTreeView1);
      this.groupBox1.Location = new System.Drawing.Point(9, 6);
      this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.groupBox1.Size = new System.Drawing.Size(502, 685);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.panel1);
      this.tabPage1.Controls.Add(this.lblMessage);
      this.tabPage1.Location = new System.Drawing.Point(4, 29);
      this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabPage1.Size = new System.Drawing.Size(1358, 710);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Synchronized script";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // panel1
      // 
      this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.panel1.Controls.Add(this.txtSyncScript);
      this.panel1.Location = new System.Drawing.Point(14, 9);
      this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(1329, 707);
      this.panel1.TabIndex = 6;
      // 
      // txtSyncScript
      // 
      this.txtSyncScript.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtSyncScript.Location = new System.Drawing.Point(0, 0);
      this.txtSyncScript.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.txtSyncScript.Name = "txtSyncScript";
      this.txtSyncScript.ReadOnly = true;
      this.txtSyncScript.Size = new System.Drawing.Size(1325, 703);
      this.txtSyncScript.TabIndex = 0;
      // 
      // tabPage3
      // 
      this.tabPage3.Controls.Add(this.textBox1);
      this.tabPage3.Location = new System.Drawing.Point(4, 29);
      this.tabPage3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabPage3.Name = "tabPage3";
      this.tabPage3.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.tabPage3.Size = new System.Drawing.Size(1358, 710);
      this.tabPage3.TabIndex = 2;
      this.tabPage3.Text = "Action report";
      this.tabPage3.UseVisualStyleBackColor = true;
      // 
      // textBox1
      // 
      this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBox1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBox1.Location = new System.Drawing.Point(14, 9);
      this.textBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.textBox1.Size = new System.Drawing.Size(1329, 707);
      this.textBox1.TabIndex = 0;
      // 
      // saveFileDialog1
      // 
      this.saveFileDialog1.DefaultExt = "sql";
      this.saveFileDialog1.Filter = "SQL File|*.sql";
      // 
      // PanelGlobal
      // 
      this.PanelGlobal.BackColor = System.Drawing.Color.White;
      this.PanelGlobal.Controls.Add(this.tableLayoutPanel1);
      this.PanelGlobal.Controls.Add(this.btnNewProject);
      this.PanelGlobal.Controls.Add(this.btnSaveProject);
      this.PanelGlobal.Controls.Add(this.btnProject);
      this.PanelGlobal.Dock = System.Windows.Forms.DockStyle.Top;
      this.PanelGlobal.Location = new System.Drawing.Point(0, 0);
      this.PanelGlobal.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.PanelGlobal.Name = "PanelGlobal";
      this.PanelGlobal.Size = new System.Drawing.Size(1513, 269);
      this.PanelGlobal.TabIndex = 10;
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel1.AutoSize = true;
      this.tableLayoutPanel1.ColumnCount = 3;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.Controls.Add(this.LeftDatabasePanel, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.RightDatabasePanel, 2, 0);
      this.tableLayoutPanel1.Controls.Add(this.SwapButton, 1, 0);
      this.tableLayoutPanel1.Location = new System.Drawing.Point(189, 5);
      this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 1;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(1384, 265);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // LeftDatabasePanel
      // 
      this.LeftDatabasePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.LeftDatabasePanel.Location = new System.Drawing.Point(4, 5);
      this.LeftDatabasePanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.LeftDatabasePanel.Name = "LeftDatabasePanel";
      this.LeftDatabasePanel.Size = new System.Drawing.Size(600, 254);
      this.LeftDatabasePanel.TabIndex = 10;
      // 
      // RightDatabasePanel
      // 
      this.RightDatabasePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.RightDatabasePanel.Location = new System.Drawing.Point(672, 5);
      this.RightDatabasePanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.RightDatabasePanel.Name = "RightDatabasePanel";
      this.RightDatabasePanel.Size = new System.Drawing.Size(600, 254);
      this.RightDatabasePanel.TabIndex = 11;
      // 
      // SwapButton
      // 
      this.SwapButton.Dock = System.Windows.Forms.DockStyle.Fill;
      this.SwapButton.Image = global::OpenDBDiff.Properties.Resources.arrow_ew;
      this.SwapButton.Location = new System.Drawing.Point(612, 5);
      this.SwapButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.SwapButton.Name = "SwapButton";
      this.SwapButton.Size = new System.Drawing.Size(52, 255);
      this.SwapButton.TabIndex = 12;
      this.toolTip1.SetToolTip(this.SwapButton, "Swap source and destination");
      this.SwapButton.UseVisualStyleBackColor = true;
      this.SwapButton.Click += new System.EventHandler(this.SwapButton_Click);
      // 
      // btnNewProject
      // 
      this.btnNewProject.BackColor = System.Drawing.SystemColors.ButtonFace;
      this.btnNewProject.Image = global::OpenDBDiff.Properties.Resources.new_window;
      this.btnNewProject.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnNewProject.Location = new System.Drawing.Point(10, 92);
      this.btnNewProject.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.btnNewProject.Name = "btnNewProject";
      this.btnNewProject.Size = new System.Drawing.Size(170, 51);
      this.btnNewProject.TabIndex = 15;
      this.btnNewProject.Text = "New project";
      this.btnNewProject.UseVisualStyleBackColor = false;
      this.btnNewProject.Click += new System.EventHandler(this.btnNewProject_Click);
      // 
      // btnSaveProject
      // 
      this.btnSaveProject.BackColor = System.Drawing.SystemColors.ButtonFace;
      this.btnSaveProject.Image = global::OpenDBDiff.Properties.Resources.diskette;
      this.btnSaveProject.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnSaveProject.Location = new System.Drawing.Point(10, 152);
      this.btnSaveProject.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.btnSaveProject.Name = "btnSaveProject";
      this.btnSaveProject.Size = new System.Drawing.Size(170, 51);
      this.btnSaveProject.TabIndex = 13;
      this.btnSaveProject.Text = "Save project";
      this.btnSaveProject.UseVisualStyleBackColor = false;
      this.btnSaveProject.Click += new System.EventHandler(this.btnSaveProject_Click);
      // 
      // btnProject
      // 
      this.btnProject.BackColor = System.Drawing.SystemColors.ButtonFace;
      this.btnProject.Image = global::OpenDBDiff.Properties.Resources.folder;
      this.btnProject.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnProject.Location = new System.Drawing.Point(10, 32);
      this.btnProject.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.btnProject.Name = "btnProject";
      this.btnProject.Size = new System.Drawing.Size(170, 51);
      this.btnProject.TabIndex = 12;
      this.btnProject.Text = "Open project";
      this.btnProject.UseVisualStyleBackColor = false;
      this.btnProject.Click += new System.EventHandler(this.btnProject_Click);
      // 
      // toolMenu
      // 
      this.toolMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
      this.toolMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolOpenProject,
            this.toolNewProject,
            this.toolSaveProject,
            this.toolStripSeparator1,
            this.toolLblProjectType,
            this.toolProjectTypes});
      this.toolMenu.Location = new System.Drawing.Point(0, 0);
      this.toolMenu.Name = "toolMenu";
      this.toolMenu.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
      this.toolMenu.Size = new System.Drawing.Size(1410, 38);
      this.toolMenu.TabIndex = 16;
      this.toolMenu.Visible = false;
      // 
      // toolOpenProject
      // 
      this.toolOpenProject.Image = global::OpenDBDiff.Properties.Resources.folder;
      this.toolOpenProject.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolOpenProject.Name = "toolOpenProject";
      this.toolOpenProject.Size = new System.Drawing.Size(143, 33);
      this.toolOpenProject.Text = "&Open Project";
      // 
      // toolNewProject
      // 
      this.toolNewProject.Image = global::OpenDBDiff.Properties.Resources.new_window;
      this.toolNewProject.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolNewProject.Name = "toolNewProject";
      this.toolNewProject.Size = new System.Drawing.Size(134, 33);
      this.toolNewProject.Text = "&New Project";
      // 
      // toolSaveProject
      // 
      this.toolSaveProject.Image = global::OpenDBDiff.Properties.Resources.diskette;
      this.toolSaveProject.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolSaveProject.Name = "toolSaveProject";
      this.toolSaveProject.Size = new System.Drawing.Size(136, 33);
      this.toolSaveProject.Text = "&Save Project";
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(6, 38);
      this.toolStripSeparator1.Visible = false;
      // 
      // toolLblProjectType
      // 
      this.toolLblProjectType.Name = "toolLblProjectType";
      this.toolLblProjectType.Size = new System.Drawing.Size(112, 33);
      this.toolLblProjectType.Text = "Project Type:";
      // 
      // toolProjectTypes
      // 
      this.toolProjectTypes.AutoSize = false;
      this.toolProjectTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.toolProjectTypes.Items.AddRange(new object[] {
            "SQL Sever 2005",
            "MySQL 5.0 or Higher",
            "Sybase 12.5"});
      this.toolProjectTypes.Name = "toolProjectTypes";
      this.toolProjectTypes.Size = new System.Drawing.Size(298, 33);
      this.toolProjectTypes.SelectedIndexChanged += new System.EventHandler(this.toolProjectTypes_SelectedIndexChanged);
      // 
      // PanelActions
      // 
      this.PanelActions.Controls.Add(this.btnCompare);
      this.PanelActions.Controls.Add(this.btnOptions);
      this.PanelActions.Controls.Add(this.btnSaveAs);
      this.PanelActions.Controls.Add(this.btnCopy);
      this.PanelActions.Controls.Add(this.btnUpdate);
      this.PanelActions.Controls.Add(this.btnCompareTableData);
      this.PanelActions.Controls.Add(this.btnUpdateAll);
      this.PanelActions.Controls.Add(this.button1);
      this.PanelActions.Dock = System.Windows.Forms.DockStyle.Right;
      this.PanelActions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.PanelActions.Location = new System.Drawing.Point(1366, 269);
      this.PanelActions.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.PanelActions.Name = "PanelActions";
      this.PanelActions.Size = new System.Drawing.Size(147, 743);
      this.PanelActions.TabIndex = 17;
      // 
      // btnCompare
      // 
      this.btnCompare.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnCompare.Image = global::OpenDBDiff.Properties.Resources.compare;
      this.btnCompare.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btnCompare.Location = new System.Drawing.Point(4, 5);
      this.btnCompare.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.btnCompare.Name = "btnCompare";
      this.btnCompare.Size = new System.Drawing.Size(142, 85);
      this.btnCompare.TabIndex = 4;
      this.btnCompare.Text = "Compare";
      this.btnCompare.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.btnCompare.UseVisualStyleBackColor = true;
      this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
      // 
      // btnOptions
      // 
      this.btnOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnOptions.Image = global::OpenDBDiff.Properties.Resources.setting_tools;
      this.btnOptions.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btnOptions.Location = new System.Drawing.Point(4, 100);
      this.btnOptions.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.btnOptions.Name = "btnOptions";
      this.btnOptions.Size = new System.Drawing.Size(142, 85);
      this.btnOptions.TabIndex = 5;
      this.btnOptions.Text = "Options";
      this.btnOptions.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.btnOptions.UseVisualStyleBackColor = true;
      this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
      // 
      // btnSaveAs
      // 
      this.btnSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSaveAs.Enabled = false;
      this.btnSaveAs.Image = global::OpenDBDiff.Properties.Resources.save_as;
      this.btnSaveAs.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btnSaveAs.Location = new System.Drawing.Point(4, 195);
      this.btnSaveAs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.btnSaveAs.Name = "btnSaveAs";
      this.btnSaveAs.Size = new System.Drawing.Size(142, 85);
      this.btnSaveAs.TabIndex = 6;
      this.btnSaveAs.Text = "Save as";
      this.btnSaveAs.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.btnSaveAs.UseVisualStyleBackColor = true;
      this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
      // 
      // btnCopy
      // 
      this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnCopy.Enabled = false;
      this.btnCopy.Image = global::OpenDBDiff.Properties.Resources.clipboard_invoice;
      this.btnCopy.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btnCopy.Location = new System.Drawing.Point(4, 290);
      this.btnCopy.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.btnCopy.Name = "btnCopy";
      this.btnCopy.Size = new System.Drawing.Size(142, 85);
      this.btnCopy.TabIndex = 7;
      this.btnCopy.Text = "Copy script";
      this.btnCopy.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.btnCopy.UseVisualStyleBackColor = true;
      this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
      // 
      // btnUpdate
      // 
      this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnUpdate.Enabled = false;
      this.btnUpdate.Image = global::OpenDBDiff.Properties.Resources.refresh_all;
      this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btnUpdate.Location = new System.Drawing.Point(4, 385);
      this.btnUpdate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.btnUpdate.Name = "btnUpdate";
      this.btnUpdate.Size = new System.Drawing.Size(142, 85);
      this.btnUpdate.TabIndex = 8;
      this.btnUpdate.Text = "Update selected";
      this.btnUpdate.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.btnUpdate.UseVisualStyleBackColor = true;
      this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
      // 
      // btnCompareTableData
      // 
      this.btnCompareTableData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnCompareTableData.Enabled = false;
      this.btnCompareTableData.Image = global::OpenDBDiff.Properties.Resources.table_analysis;
      this.btnCompareTableData.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btnCompareTableData.Location = new System.Drawing.Point(4, 480);
      this.btnCompareTableData.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.btnCompareTableData.Name = "btnCompareTableData";
      this.btnCompareTableData.Size = new System.Drawing.Size(142, 85);
      this.btnCompareTableData.TabIndex = 9;
      this.btnCompareTableData.Text = "Compare data";
      this.btnCompareTableData.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.btnCompareTableData.UseVisualStyleBackColor = true;
      this.btnCompareTableData.Click += new System.EventHandler(this.btnCompareTableData_Click);
      // 
      // btnUpdateAll
      // 
      this.btnUpdateAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnUpdateAll.Enabled = false;
      this.btnUpdateAll.Image = global::OpenDBDiff.Properties.Resources.database_refresh;
      this.btnUpdateAll.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btnUpdateAll.Location = new System.Drawing.Point(4, 616);
      this.btnUpdateAll.Margin = new System.Windows.Forms.Padding(4, 46, 4, 5);
      this.btnUpdateAll.Name = "btnUpdateAll";
      this.btnUpdateAll.Size = new System.Drawing.Size(142, 85);
      this.btnUpdateAll.TabIndex = 10;
      this.btnUpdateAll.Text = "Update all";
      this.btnUpdateAll.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.btnUpdateAll.UseVisualStyleBackColor = true;
      this.btnUpdateAll.Click += new System.EventHandler(this.btnUpdateAll_Click);
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(3, 709);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 11;
      this.button1.Text = "button1";
      this.button1.UseVisualStyleBackColor = true;
      // 
      // schemaTreeView1
      // 
      this.schemaTreeView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.schemaTreeView1.LeftDatabase = null;
      this.schemaTreeView1.Location = new System.Drawing.Point(10, 15);
      this.schemaTreeView1.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
      this.schemaTreeView1.Name = "schemaTreeView1";
      this.schemaTreeView1.RightDatabase = null;
      this.schemaTreeView1.ShowChangedItems = true;
      this.schemaTreeView1.ShowMissingItems = true;
      this.schemaTreeView1.ShowNewItems = true;
      this.schemaTreeView1.ShowUnchangedItems = true;
      this.schemaTreeView1.Size = new System.Drawing.Size(483, 660);
      this.schemaTreeView1.TabIndex = 0;
      // 
      // MainForm
      // 
      this.AcceptButton = this.btnCompare;
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1513, 1012);
      this.Controls.Add(this.tabControl1);
      this.Controls.Add(this.PanelActions);
      this.Controls.Add(this.PanelGlobal);
      this.Controls.Add(this.toolMenu);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.Name = "MainForm";
      this.Text = "OpenDBDiff";
      this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
      this.Load += new System.EventHandler(this.MainForm_Load);
      this.tabControl1.ResumeLayout(false);
      this.tabPage2.ResumeLayout(false);
      this.tabControl2.ResumeLayout(false);
      this.tabPage4.ResumeLayout(false);
      this.tabPage5.ResumeLayout(false);
      this.tabPage6.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.groupBox1.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.tabPage1.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.tabPage3.ResumeLayout(false);
      this.tabPage3.PerformLayout();
      this.PanelGlobal.ResumeLayout(false);
      this.PanelGlobal.PerformLayout();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.toolMenu.ResumeLayout(false);
      this.toolMenu.PerformLayout();
      this.PanelActions.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private Label lblMessage;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private Button btnCompare;
        private Button btnSaveAs;
        private SaveFileDialog saveFileDialog1;
        private Button btnCopy;
        private Button btnUpdate;
        private Button btnUpdateAll;
        private Button btnOptions;
        private TabPage tabPage2;
        private SchemaTreeView schemaTreeView1;
        private GroupBox groupBox1;
        private TabPage tabPage3;
        private TextBox textBox1;
        private Panel panel1;
        private Scintilla txtSyncScript;
        private Panel PanelGlobal;
        private Panel LeftDatabasePanel;
        private Panel RightDatabasePanel;
        private GroupBox groupBox2;
        private Label label1;
        private Label label3;
        private Label label2;
        private TabControl tabControl2;
        private TabPage tabPage4;
        private TabPage tabPage5;
        private Scintilla txtNewObject;
        private Scintilla txtOldObject;
        private Button btnSaveProject;
        private Button btnProject;
        private Button btnNewProject;
        private Button btnCompareTableData;
        private TabPage tabPage6;
        private Scintilla txtDiff;
        private ToolStrip toolMenu;
        private ToolStripButton toolOpenProject;
        private ToolStripButton toolNewProject;
        private ToolStripButton toolSaveProject;
        private ToolStripComboBox toolProjectTypes;
        private FlowLayoutPanel PanelActions;
        private ToolStripLabel toolLblProjectType;
        private TableLayoutPanel tableLayoutPanel1;
        private ToolStripSeparator toolStripSeparator1;
        private Button SwapButton;
        private ToolTip toolTip1;
        private Button BtnDiffDown;
        private Button BtnDiffUp;
        private Button button1;
        private Label lblDiffDeletions;
        private Label lblDiffModifications;
        private Label lblDiffAdditions;
    }
}
