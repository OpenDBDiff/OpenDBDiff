using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace OpenDBDiff.UI
{
    partial class DataCompareForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataCompareForm));
            this.pnlControl = new System.Windows.Forms.Panel();
            this.lblAdded = new System.Windows.Forms.Label();
            this.lblModified = new System.Windows.Forms.Label();
            this.pnlAdded = new System.Windows.Forms.Panel();
            this.pnlModified = new System.Windows.Forms.Panel();
            this.btnRowToRow = new System.Windows.Forms.Button();
            this.btnMerge = new System.Windows.Forms.Button();
            this.btnUpdateRow = new System.Windows.Forms.Button();
            this.btnCommitChanges = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pnlSource = new System.Windows.Forms.Panel();
            this.lblSrc = new System.Windows.Forms.Label();
            this.srcDgv = new System.Windows.Forms.DataGridView();
            this.pnlDestination = new System.Windows.Forms.Panel();
            this.lblDestination = new System.Windows.Forms.Label();
            this.destDgv = new System.Windows.Forms.DataGridView();
            this.pnlControl.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.pnlSource.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.srcDgv)).BeginInit();
            this.pnlDestination.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.destDgv)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlControl
            // 
            this.pnlControl.BackColor = System.Drawing.Color.White;
            this.pnlControl.Controls.Add(this.btnCommitChanges);
            this.pnlControl.Controls.Add(this.lblAdded);
            this.pnlControl.Controls.Add(this.lblModified);
            this.pnlControl.Controls.Add(this.pnlAdded);
            this.pnlControl.Controls.Add(this.pnlModified);
            this.pnlControl.Controls.Add(this.btnRowToRow);
            this.pnlControl.Controls.Add(this.btnMerge);
            this.pnlControl.Controls.Add(this.btnUpdateRow);
            this.pnlControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlControl.Location = new System.Drawing.Point(0, 462);
            this.pnlControl.Name = "pnlControl";
            this.pnlControl.Size = new System.Drawing.Size(787, 48);
            this.pnlControl.TabIndex = 2;
            // 
            // lblAdded
            // 
            this.lblAdded.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAdded.Font = new System.Drawing.Font("Verdana", 9F);
            this.lblAdded.Location = new System.Drawing.Point(390, 14);
            this.lblAdded.Name = "lblAdded";
            this.lblAdded.Size = new System.Drawing.Size(100, 20);
            this.lblAdded.TabIndex = 0;
            this.lblAdded.Text = "Added";
            // 
            // lblModified
            // 
            this.lblModified.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblModified.Font = new System.Drawing.Font("Verdana", 9F);
            this.lblModified.Location = new System.Drawing.Point(540, 14);
            this.lblModified.Name = "lblModified";
            this.lblModified.Size = new System.Drawing.Size(100, 20);
            this.lblModified.TabIndex = 1;
            this.lblModified.Text = "Modified";
            // 
            // pnlAdded
            // 
            this.pnlAdded.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlAdded.BackColor = System.Drawing.Color.Green;
            this.pnlAdded.Location = new System.Drawing.Point(354, 14);
            this.pnlAdded.Name = "pnlAdded";
            this.pnlAdded.Size = new System.Drawing.Size(30, 20);
            this.pnlAdded.TabIndex = 2;
            // 
            // pnlModified
            // 
            this.pnlModified.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlModified.BackColor = System.Drawing.Color.Blue;
            this.pnlModified.Location = new System.Drawing.Point(504, 14);
            this.pnlModified.Name = "pnlModified";
            this.pnlModified.Size = new System.Drawing.Size(30, 20);
            this.pnlModified.TabIndex = 3;
            // 
            // btnRowToRow
            // 
            this.btnRowToRow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRowToRow.BackColor = System.Drawing.Color.LightGray;
            this.btnRowToRow.Location = new System.Drawing.Point(10, 6);
            this.btnRowToRow.Name = "btnRowToRow";
            this.btnRowToRow.Size = new System.Drawing.Size(120, 30);
            this.btnRowToRow.TabIndex = 4;
            this.btnRowToRow.Text = "Update row --> row";
            this.btnRowToRow.UseVisualStyleBackColor = false;
            this.btnRowToRow.Click += new System.EventHandler(this.btnRowToRow_Click);
            // 
            // btnMerge
            // 
            this.btnMerge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMerge.BackColor = System.Drawing.Color.LightGray;
            this.btnMerge.Location = new System.Drawing.Point(136, 6);
            this.btnMerge.Name = "btnMerge";
            this.btnMerge.Size = new System.Drawing.Size(100, 30);
            this.btnMerge.TabIndex = 5;
            this.btnMerge.Text = "Merge all -->";
            this.btnMerge.UseVisualStyleBackColor = false;
            this.btnMerge.Click += new System.EventHandler(this.btnMerge_Click);
            // 
            // btnUpdateRow
            // 
            this.btnUpdateRow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUpdateRow.BackColor = System.Drawing.Color.LightGray;
            this.btnUpdateRow.Location = new System.Drawing.Point(242, 6);
            this.btnUpdateRow.Name = "btnUpdateRow";
            this.btnUpdateRow.Size = new System.Drawing.Size(100, 30);
            this.btnUpdateRow.TabIndex = 6;
            this.btnUpdateRow.Text = "Update row -->";
            this.btnUpdateRow.UseVisualStyleBackColor = false;
            this.btnUpdateRow.Click += new System.EventHandler(this.btnUpdateRow_Click);
            // 
            // btnCommitChanges
            // 
            this.btnCommitChanges.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCommitChanges.BackColor = System.Drawing.Color.LightGray;
            this.btnCommitChanges.Enabled = false;
            this.btnCommitChanges.Location = new System.Drawing.Point(675, 6);
            this.btnCommitChanges.Name = "btnCommitChanges";
            this.btnCommitChanges.Size = new System.Drawing.Size(100, 30);
            this.btnCommitChanges.TabIndex = 7;
            this.btnCommitChanges.Text = "Commit";
            this.btnCommitChanges.UseVisualStyleBackColor = false;
            this.btnCommitChanges.Click += new System.EventHandler(this.btnCommitChanges_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pnlSource);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pnlDestination);
            this.splitContainer1.Size = new System.Drawing.Size(787, 462);
            this.splitContainer1.SplitterDistance = 353;
            this.splitContainer1.TabIndex = 3;
            // 
            // pnlSource
            // 
            this.pnlSource.Controls.Add(this.lblSrc);
            this.pnlSource.Controls.Add(this.srcDgv);
            this.pnlSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSource.Location = new System.Drawing.Point(0, 0);
            this.pnlSource.Name = "pnlSource";
            this.pnlSource.Size = new System.Drawing.Size(353, 462);
            this.pnlSource.TabIndex = 6;
            // 
            // lblSrc
            // 
            this.lblSrc.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSrc.Font = new System.Drawing.Font("Verdana", 14F, System.Drawing.FontStyle.Bold);
            this.lblSrc.Location = new System.Drawing.Point(0, 0);
            this.lblSrc.Name = "lblSrc";
            this.lblSrc.Size = new System.Drawing.Size(353, 23);
            this.lblSrc.TabIndex = 0;
            this.lblSrc.Text = "Source";
            // 
            // srcDgv
            // 
            this.srcDgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.srcDgv.Location = new System.Drawing.Point(0, 0);
            this.srcDgv.Name = "srcDgv";
            this.srcDgv.Size = new System.Drawing.Size(353, 462);
            this.srcDgv.TabIndex = 3;
            // 
            // pnlDestination
            // 
            this.pnlDestination.Controls.Add(this.lblDestination);
            this.pnlDestination.Controls.Add(this.destDgv);
            this.pnlDestination.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDestination.Location = new System.Drawing.Point(0, 0);
            this.pnlDestination.Name = "pnlDestination";
            this.pnlDestination.Size = new System.Drawing.Size(430, 462);
            this.pnlDestination.TabIndex = 6;
            // 
            // lblDestination
            // 
            this.lblDestination.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDestination.Font = new System.Drawing.Font("Verdana", 14F, System.Drawing.FontStyle.Bold);
            this.lblDestination.Location = new System.Drawing.Point(0, 0);
            this.lblDestination.Name = "lblDestination";
            this.lblDestination.Size = new System.Drawing.Size(430, 23);
            this.lblDestination.TabIndex = 0;
            this.lblDestination.Text = "Destination";
            // 
            // destDgv
            // 
            this.destDgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.destDgv.Location = new System.Drawing.Point(0, 0);
            this.destDgv.Name = "destDgv";
            this.destDgv.Size = new System.Drawing.Size(430, 462);
            this.destDgv.TabIndex = 3;
            // 
            // DataCompareForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 510);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.pnlControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(478, 250);
            this.Name = "DataCompareForm";
            this.Text = "DataCompare";
            this.pnlControl.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.pnlSource.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.srcDgv)).EndInit();
            this.pnlDestination.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.destDgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Button btnRowToRow;
        private Button btnMerge;
        private Button btnUpdateRow;
        private Button btnCommitChanges;
        private Label lblAdded;
        private Label lblModified;

        private Panel pnlControl;
        private Panel pnlAdded;
        private Panel pnlModified;
        private SplitContainer splitContainer1;
        private Panel pnlSource;
        private Label lblSrc;
        private DataGridView srcDgv;
        private Panel pnlDestination;
        private Label lblDestination;
        private DataGridView destDgv;
    }
}
