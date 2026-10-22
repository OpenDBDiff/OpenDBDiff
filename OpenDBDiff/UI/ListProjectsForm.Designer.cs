using System.ComponentModel;
using System.Windows.Forms;

namespace OpenDBDiff.UI
{
    sealed partial class ListProjectsForm
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "ListViewItem",
            "ListViewSubItem1",
            "ListViewSubItem2"}, 0);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListProjectsForm));
            this.ProjectsListView = new System.Windows.Forms.ListView();
            this.ProjectNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SourceConnectionStringColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.DestinationConnectionStringColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.IconsImageList = new System.Windows.Forms.ImageList(this.components);
            this.ActionsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RenameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.FilterPanel = new System.Windows.Forms.Panel();
            this.FilterLabel = new System.Windows.Forms.Label();
            this.FilterTextBox = new System.Windows.Forms.TextBox();
            this.ActionsContextMenuStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.FilterPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ProjectsListView
            // 
            this.ProjectsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ProjectNameColumnHeader,
            this.SourceConnectionStringColumnHeader,
            this.DestinationConnectionStringColumnHeader});
            this.ProjectsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProjectsListView.FullRowSelect = true;
            this.ProjectsListView.GridLines = true;
            this.ProjectsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
            this.ProjectsListView.HideSelection = false;
            this.ProjectsListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.ProjectsListView.LabelWrap = false;
            this.ProjectsListView.Location = new System.Drawing.Point(0, 30);
            this.ProjectsListView.MultiSelect = false;
            this.ProjectsListView.Name = "ProjectsListView";
            this.ProjectsListView.Size = new System.Drawing.Size(860, 252);
            this.ProjectsListView.SmallImageList = this.IconsImageList;
            this.ProjectsListView.TabIndex = 5;
            this.ProjectsListView.UseCompatibleStateImageBehavior = false;
            this.ProjectsListView.View = System.Windows.Forms.View.Details;
            this.ProjectsListView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.ProjectsListView_AfterLabelEdit);
            this.ProjectsListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ProjectsListView_ColumnClick);
            this.ProjectsListView.SelectedIndexChanged += new System.EventHandler(this.ProjectsListView_SelectedIndexChanged);
            this.ProjectsListView.DoubleClick += new System.EventHandler(this.ProjectsListView_DoubleClick);
            // 
            // ProjectNameColumnHeader
            // 
            this.ProjectNameColumnHeader.Text = "Project";
            this.ProjectNameColumnHeader.Width = 100;
            //
            // SourceConnectionStringColumnHeader
            //
            this.SourceConnectionStringColumnHeader.Text = "Source";
            this.SourceConnectionStringColumnHeader.Width = 150;
            //
            // DestinationConnectionStringColumnHeader
            //
            this.DestinationConnectionStringColumnHeader.Text = "Destination";
            this.DestinationConnectionStringColumnHeader.Width = 150;
            // 
            // IconsImageList
            // 
            this.IconsImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("IconsImageList.ImageStream")));
            this.IconsImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.IconsImageList.Images.SetKeyName(0, "database_yellow.png");
            // 
            // ActionsContextMenuStrip
            // 
            this.ActionsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenToolStripMenuItem,
            this.RenameToolStripMenuItem,
            this.DeleteToolStripMenuItem});
            this.ActionsContextMenuStrip.Name = "ActionsContextMenuStrip";
            this.ActionsContextMenuStrip.Size = new System.Drawing.Size(118, 70);
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.OpenToolStripMenuItem.Text = "&Open";
            this.OpenToolStripMenuItem.Click += new System.EventHandler(this.mnuItemOpen_Click);
            // 
            // RenameToolStripMenuItem
            // 
            this.RenameToolStripMenuItem.Name = "RenameToolStripMenuItem";
            this.RenameToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.RenameToolStripMenuItem.Text = "&Rename";
            this.RenameToolStripMenuItem.Click += new System.EventHandler(this.mnuItemRename_Click);
            // 
            // DeleteToolStripMenuItem
            // 
            this.DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem";
            this.DeleteToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.DeleteToolStripMenuItem.Text = "&Delete";
            this.DeleteToolStripMenuItem.Click += new System.EventHandler(this.mnuItemDelete_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 282);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(860, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(214, 17);
            this.toolStripStatusLabel1.Text = "Right-click on project for more actions.";
            //
            // FilterPanel
            //
            this.FilterPanel.Controls.Add(this.FilterTextBox);
            this.FilterPanel.Controls.Add(this.FilterLabel);
            this.FilterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.FilterPanel.Location = new System.Drawing.Point(0, 0);
            this.FilterPanel.Name = "FilterPanel";
            this.FilterPanel.Size = new System.Drawing.Size(860, 30);
            this.FilterPanel.TabIndex = 0;
            //
            // FilterLabel
            //
            this.FilterLabel.AutoSize = true;
            this.FilterLabel.Location = new System.Drawing.Point(6, 8);
            this.FilterLabel.Name = "FilterLabel";
            this.FilterLabel.Size = new System.Drawing.Size(32, 13);
            this.FilterLabel.TabIndex = 0;
            this.FilterLabel.Text = "&Filter:";
            //
            // FilterTextBox
            //
            this.FilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterTextBox.Location = new System.Drawing.Point(44, 5);
            this.FilterTextBox.Name = "FilterTextBox";
            this.FilterTextBox.Size = new System.Drawing.Size(810, 20);
            this.FilterTextBox.TabIndex = 1;
            this.FilterTextBox.TextChanged += new System.EventHandler(this.FilterTextBox_TextChanged);
            this.FilterTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterTextBox_KeyDown);
            //
            // ListProjectsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(860, 304);
            this.ContextMenuStrip = this.ActionsContextMenuStrip;
            this.Controls.Add(this.ProjectsListView);
            this.Controls.Add(this.FilterPanel);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "ListProjectsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "OpenDBDiff projects";
            this.Shown += new System.EventHandler(this.ListProjectsForm_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListProjectsForm_KeyDown);
            this.ActionsContextMenuStrip.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.FilterPanel.ResumeLayout(false);
            this.FilterPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private ListView ProjectsListView;
        private ColumnHeader ProjectNameColumnHeader;
        private ColumnHeader SourceConnectionStringColumnHeader;
        private ImageList IconsImageList;
        private ContextMenuStrip ActionsContextMenuStrip;
        private ToolStripMenuItem OpenToolStripMenuItem;
        private ToolStripMenuItem RenameToolStripMenuItem;
        private ToolStripMenuItem DeleteToolStripMenuItem;
        private ColumnHeader DestinationConnectionStringColumnHeader;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private Panel FilterPanel;
        private Label FilterLabel;
        private TextBox FilterTextBox;
    }
}
