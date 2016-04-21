using System.ComponentModel;
using System.Windows.Forms;

namespace DBDiff.Front
{
    partial class SchemaTreeView
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
            this.chkDifferent = new System.Windows.Forms.CheckBox();
            this.chkShowUnchangedItems = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            //
            // treeView1
            //
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.CheckBoxes = true;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 50);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(262, 150);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
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
            this.chkOld.Location = new System.Drawing.Point(141, 27);
            this.chkOld.Name = "chkOld";
            this.chkOld.Size = new System.Drawing.Size(117, 17);
            this.chkOld.TabIndex = 1;
            this.chkOld.Text = "Show missing items";
            this.chkOld.UseVisualStyleBackColor = true;
            this.chkOld.CheckedChanged += new System.EventHandler(this.FilterCheckbox_CheckedChanged);
            //
            // chkNew
            //
            this.chkNew.AutoSize = true;
            this.chkNew.Checked = true;
            this.chkNew.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNew.Location = new System.Drawing.Point(4, 27);
            this.chkNew.Name = "chkNew";
            this.chkNew.Size = new System.Drawing.Size(103, 17);
            this.chkNew.TabIndex = 2;
            this.chkNew.Text = "Show new items";
            this.chkNew.UseVisualStyleBackColor = true;
            this.chkNew.CheckedChanged += new System.EventHandler(this.FilterCheckbox_CheckedChanged);
            //
            // chkDifferent
            //
            this.chkDifferent.AutoSize = true;
            this.chkDifferent.Checked = true;
            this.chkDifferent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDifferent.Location = new System.Drawing.Point(141, 4);
            this.chkDifferent.Name = "chkDifferent";
            this.chkDifferent.Size = new System.Drawing.Size(125, 17);
            this.chkDifferent.TabIndex = 3;
            this.chkDifferent.Text = "Show changed items";
            this.chkDifferent.UseVisualStyleBackColor = true;
            this.chkDifferent.CheckedChanged += new System.EventHandler(this.FilterCheckbox_CheckedChanged);
            //
            // chkShowUnchangedItems
            //
            this.chkShowUnchangedItems.AutoSize = true;
            this.chkShowUnchangedItems.Checked = true;
            this.chkShowUnchangedItems.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowUnchangedItems.Location = new System.Drawing.Point(3, 4);
            this.chkShowUnchangedItems.Name = "chkShowUnchangedItems";
            this.chkShowUnchangedItems.Size = new System.Drawing.Size(137, 17);
            this.chkShowUnchangedItems.TabIndex = 4;
            this.chkShowUnchangedItems.Text = "Show unchanged items";
            this.chkShowUnchangedItems.UseVisualStyleBackColor = true;
            this.chkShowUnchangedItems.CheckedChanged += new System.EventHandler(this.FilterCheckbox_CheckedChanged);
            //
            // SchemaTreeView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkShowUnchangedItems);
            this.Controls.Add(this.chkDifferent);
            this.Controls.Add(this.chkNew);
            this.Controls.Add(this.chkOld);
            this.Controls.Add(this.treeView1);
            this.Name = "SchemaTreeView";
            this.Size = new System.Drawing.Size(266, 203);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TreeView treeView1;
        private CheckBox chkOld;
        private CheckBox chkNew;
        private CheckBox chkDifferent;
        private ImageList imageList1;
        private CheckBox chkShowUnchangedItems;
    }
}
