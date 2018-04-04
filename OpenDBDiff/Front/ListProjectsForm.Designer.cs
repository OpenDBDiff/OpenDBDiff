using System.ComponentModel;
using System.Windows.Forms;

namespace OpenDBDiff.Front
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
            "test",
            "test"}, -1);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListProjectsForm));
            this.ProjectsListView = new System.Windows.Forms.ListView();
            this.IconColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ConnectionColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.IconsImageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // ProjectsListView
            // 
            this.ProjectsListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ProjectsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.IconColumnHeader,
            this.ConnectionColumnHeader});
            this.ProjectsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.ProjectsListView.HideSelection = false;
            this.ProjectsListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.ProjectsListView.LabelWrap = false;
            this.ProjectsListView.Location = new System.Drawing.Point(3, 5);
            this.ProjectsListView.MultiSelect = false;
            this.ProjectsListView.Name = "ProjectsListView";
            this.ProjectsListView.Size = new System.Drawing.Size(420, 295);
            this.ProjectsListView.SmallImageList = this.IconsImageList;
            this.ProjectsListView.TabIndex = 5;
            this.ProjectsListView.UseCompatibleStateImageBehavior = false;
            this.ProjectsListView.View = System.Windows.Forms.View.List;
            this.ProjectsListView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listView1_AfterLabelEdit);
            this.ProjectsListView.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.ProjectsListView.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // IconColumnHeader
            // 
            this.IconColumnHeader.Width = 500;
            // 
            // ConnectionColumnHeader
            // 
            this.ConnectionColumnHeader.Width = 600;
            // 
            // IconsImageList
            // 
            this.IconsImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("IconsImageList.ImageStream")));
            this.IconsImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.IconsImageList.Images.SetKeyName(0, "database_yellow.png");
            // 
            // ListProjectsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(426, 304);
            this.Controls.Add(this.ProjectsListView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ListProjectsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Projects";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListProjectsForm_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion
        private ListView ProjectsListView;
        private ColumnHeader IconColumnHeader;
        private ColumnHeader ConnectionColumnHeader;
        private ImageList IconsImageList;
    }
}
