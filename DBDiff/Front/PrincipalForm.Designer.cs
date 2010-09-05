namespace DBDiff
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblMessage = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.txtDiferencias = new System.Windows.Forms.TextBox();
            this.GroupDb = new System.Windows.Forms.GroupBox();
            this.optSybase = new System.Windows.Forms.RadioButton();
            this.optMySQL = new System.Windows.Forms.RadioButton();
            this.optSQL2005 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btnOptions = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.btnCompare = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.GroupDb.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(9, 340);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(756, 26);
            this.progressBar1.TabIndex = 3;
            // 
            // lblMessage
            // 
            this.lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(6, 528);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(0, 13);
            this.lblMessage.TabIndex = 4;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(7, 234);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(781, 403);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtDiferencias);
            this.tabPage1.Controls.Add(this.progressBar1);
            this.tabPage1.Controls.Add(this.lblMessage);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(773, 377);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Sincronized Script";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // txtDiferencias
            // 
            this.txtDiferencias.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDiferencias.Location = new System.Drawing.Point(9, 6);
            this.txtDiferencias.Multiline = true;
            this.txtDiferencias.Name = "txtDiferencias";
            this.txtDiferencias.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDiferencias.Size = new System.Drawing.Size(753, 325);
            this.txtDiferencias.TabIndex = 6;
            // 
            // GroupDb
            // 
            this.GroupDb.Controls.Add(this.optSybase);
            this.GroupDb.Controls.Add(this.optMySQL);
            this.GroupDb.Controls.Add(this.optSQL2005);
            this.GroupDb.Location = new System.Drawing.Point(7, 0);
            this.GroupDb.Name = "GroupDb";
            this.GroupDb.Size = new System.Drawing.Size(665, 36);
            this.GroupDb.TabIndex = 9;
            this.GroupDb.TabStop = false;
            // 
            // optSybase
            // 
            this.optSybase.AutoSize = true;
            this.optSybase.Location = new System.Drawing.Point(291, 15);
            this.optSybase.Name = "optSybase";
            this.optSybase.Size = new System.Drawing.Size(84, 17);
            this.optSybase.TabIndex = 3;
            this.optSybase.Text = "Sybase 12.5";
            this.optSybase.UseVisualStyleBackColor = true;
            this.optSybase.Visible = false;
            this.optSybase.CheckedChanged += new System.EventHandler(this.optSybase_CheckedChanged);
            // 
            // optMySQL
            // 
            this.optMySQL.AutoSize = true;
            this.optMySQL.Location = new System.Drawing.Point(163, 15);
            this.optMySQL.Name = "optMySQL";
            this.optMySQL.Size = new System.Drawing.Size(122, 17);
            this.optMySQL.TabIndex = 2;
            this.optMySQL.Text = "MySQL 5.0 or higher";
            this.optMySQL.UseVisualStyleBackColor = true;
            this.optMySQL.Visible = false;
            this.optMySQL.CheckedChanged += new System.EventHandler(this.optMySQL_CheckedChanged);
            // 
            // optSQL2005
            // 
            this.optSQL2005.AutoSize = true;
            this.optSQL2005.Checked = true;
            this.optSQL2005.Location = new System.Drawing.Point(6, 12);
            this.optSQL2005.Name = "optSQL2005";
            this.optSQL2005.Size = new System.Drawing.Size(107, 17);
            this.optSQL2005.TabIndex = 1;
            this.optSQL2005.TabStop = true;
            this.optSQL2005.Text = "SQL Server 2005";
            this.optSQL2005.UseVisualStyleBackColor = true;
            this.optSQL2005.CheckedChanged += new System.EventHandler(this.optSQL2005_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(7, 38);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(430, 190);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Source Connection";
            // 
            // groupBox3
            // 
            this.groupBox3.Location = new System.Drawing.Point(445, 38);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(430, 190);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Target Connection";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.CheckFileExists = true;
            this.saveFileDialog1.DefaultExt = "SQL";
            this.saveFileDialog1.Filter = "SQL File|*.sql";
            // 
            // btnOptions
            // 
            this.btnOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOptions.Image = global::DBDiff.Properties.Resources.Control_panel_2;
            this.btnOptions.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnOptions.Location = new System.Drawing.Point(790, 314);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(109, 55);
            this.btnOptions.TabIndex = 15;
            this.btnOptions.Text = "Options";
            this.btnOptions.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnOptions.UseVisualStyleBackColor = true;
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.Enabled = false;
            this.btnCopy.Image = global::DBDiff.Properties.Resources.Copy;
            this.btnCopy.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCopy.Location = new System.Drawing.Point(790, 436);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(109, 55);
            this.btnCopy.TabIndex = 14;
            this.btnCopy.Text = "Copy Clipboard";
            this.btnCopy.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveAs.Enabled = false;
            this.btnSaveAs.Image = global::DBDiff.Properties.Resources.Floppy;
            this.btnSaveAs.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSaveAs.Location = new System.Drawing.Point(790, 375);
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(109, 55);
            this.btnSaveAs.TabIndex = 13;
            this.btnSaveAs.Text = "Save As";
            this.btnSaveAs.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSaveAs.UseVisualStyleBackColor = true;
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // btnCompare
            // 
            this.btnCompare.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCompare.Image = global::DBDiff.Properties.Resources.Compare;
            this.btnCompare.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCompare.Location = new System.Drawing.Point(790, 253);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(109, 55);
            this.btnCompare.TabIndex = 6;
            this.btnCompare.Text = "Compare";
            this.btnCompare.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCompare.UseVisualStyleBackColor = true;
            this.btnCompare.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(902, 658);
            this.Controls.Add(this.btnOptions);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnSaveAs);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnCompare);
            this.Controls.Add(this.GroupDb);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Open DBDiff Beta 1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.GroupDb.ResumeLayout(false);
            this.GroupDb.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnCompare;
        private System.Windows.Forms.GroupBox GroupDb;
        private System.Windows.Forms.RadioButton optSQL2005;
        private System.Windows.Forms.RadioButton optMySQL;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton optSybase;
        private System.Windows.Forms.TextBox txtDiferencias;
        private System.Windows.Forms.Button btnSaveAs;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnOptions;
    }
}

