namespace DBDiff.Front
{
    partial class SchemaTreeView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SchemaTreeView));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.chkOld = new System.Windows.Forms.CheckBox();
            this.chkNew = new System.Windows.Forms.CheckBox();
            this.chkDiferent = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 50);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(262, 150);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Folder");
            this.imageList1.Images.SetKeyName(1, "Table");
            this.imageList1.Images.SetKeyName(2, "Procedure");
            this.imageList1.Images.SetKeyName(3, "User");
            this.imageList1.Images.SetKeyName(4, "Column");
            this.imageList1.Images.SetKeyName(5, "Index");
            this.imageList1.Images.SetKeyName(6, "Rol");
            this.imageList1.Images.SetKeyName(7, "Schema");
            this.imageList1.Images.SetKeyName(8, "View");
            this.imageList1.Images.SetKeyName(9, "Function");
            this.imageList1.Images.SetKeyName(10, "XMLSchema");
            this.imageList1.Images.SetKeyName(11, "Database");
            this.imageList1.Images.SetKeyName(12, "UDT");
            this.imageList1.Images.SetKeyName(13, "Assembly");
            this.imageList1.Images.SetKeyName(14, "PartitionFunction");
            this.imageList1.Images.SetKeyName(15, "PartitionScheme");
            // 
            // chkOld
            // 
            this.chkOld.AutoSize = true;
            this.chkOld.Checked = true;
            this.chkOld.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOld.Enabled = false;
            this.chkOld.Location = new System.Drawing.Point(4, 4);
            this.chkOld.Name = "chkOld";
            this.chkOld.Size = new System.Drawing.Size(125, 17);
            this.chkOld.TabIndex = 1;
            this.chkOld.Text = "Filter Missing Objects";
            this.chkOld.UseVisualStyleBackColor = true;
            this.chkOld.CheckedChanged += new System.EventHandler(this.chkOld_CheckedChanged);
            // 
            // chkNew
            // 
            this.chkNew.AutoSize = true;
            this.chkNew.Checked = true;
            this.chkNew.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNew.Enabled = false;
            this.chkNew.Location = new System.Drawing.Point(4, 27);
            this.chkNew.Name = "chkNew";
            this.chkNew.Size = new System.Drawing.Size(112, 17);
            this.chkNew.TabIndex = 2;
            this.chkNew.Text = "Filter New Objects";
            this.chkNew.UseVisualStyleBackColor = true;
            this.chkNew.CheckedChanged += new System.EventHandler(this.chkNew_CheckedChanged);
            // 
            // chkDiferent
            // 
            this.chkDiferent.AutoSize = true;
            this.chkDiferent.Checked = true;
            this.chkDiferent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDiferent.Enabled = false;
            this.chkDiferent.Location = new System.Drawing.Point(135, 4);
            this.chkDiferent.Name = "chkDiferent";
            this.chkDiferent.Size = new System.Drawing.Size(130, 17);
            this.chkDiferent.TabIndex = 3;
            this.chkDiferent.Text = "Filter Different Objects";
            this.chkDiferent.UseVisualStyleBackColor = true;
            this.chkDiferent.CheckedChanged += new System.EventHandler(this.chkDiferent_CheckedChanged);
            // 
            // SchemaTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkDiferent);
            this.Controls.Add(this.chkNew);
            this.Controls.Add(this.chkOld);
            this.Controls.Add(this.treeView1);
            this.Name = "SchemaTreeView";
            this.Size = new System.Drawing.Size(266, 203);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.CheckBox chkOld;
        private System.Windows.Forms.CheckBox chkNew;
        private System.Windows.Forms.CheckBox chkDiferent;
        private System.Windows.Forms.ImageList imageList1;
    }
}
