using System.ComponentModel;
using System.Windows.Forms;

namespace DBDiff.Front
{
    partial class ProgressForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressForm));
            this.gradientPanel1 = new System.Windows.Forms.Panel();
            this.lblName = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.databaseProgressControl2 = new DBDiff.Front.DatabaseProgressControl();
            this.databaseProgressControl1 = new DBDiff.Front.DatabaseProgressControl();
            this.gradientPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gradientPanel1
            // 
            this.gradientPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gradientPanel1.Controls.Add(this.lblName);
            this.gradientPanel1.Location = new System.Drawing.Point(0, 0);
            this.gradientPanel1.Name = "gradientPanel1";
            this.gradientPanel1.Size = new System.Drawing.Size(499, 58);
            this.gradientPanel1.TabIndex = 27;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(12, 16);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(200, 24);
            this.lblName.TabIndex = 24;
            this.lblName.Text = "Compare Progress";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.databaseProgressControl2);
            this.panel1.Controls.Add(this.databaseProgressControl1);
            this.panel1.Location = new System.Drawing.Point(0, 57);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(499, 173);
            this.panel1.TabIndex = 30;
            // 
            // databaseProgressControl2
            // 
            this.databaseProgressControl2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.databaseProgressControl2.DatabaseName = "Database1:";
            this.databaseProgressControl2.Location = new System.Drawing.Point(12, 92);
            this.databaseProgressControl2.Maximum = 100;
            this.databaseProgressControl2.Message = "";
            this.databaseProgressControl2.Name = "databaseProgressControl2";
            this.databaseProgressControl2.Size = new System.Drawing.Size(472, 64);
            this.databaseProgressControl2.TabIndex = 35;
            this.databaseProgressControl2.Value = 0;
            // 
            // databaseProgressControl1
            // 
            this.databaseProgressControl1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.databaseProgressControl1.DatabaseName = "Database1:";
            this.databaseProgressControl1.Location = new System.Drawing.Point(12, 15);
            this.databaseProgressControl1.Maximum = 100;
            this.databaseProgressControl1.Message = "";
            this.databaseProgressControl1.Name = "databaseProgressControl1";
            this.databaseProgressControl1.Size = new System.Drawing.Size(472, 64);
            this.databaseProgressControl1.TabIndex = 34;
            this.databaseProgressControl1.Value = 0;
            // 
            // ProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(496, 230);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.gradientPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Running";
            this.Activated += new System.EventHandler(this.ProgressForm_Activated);
            this.gradientPanel1.ResumeLayout(false);
            this.gradientPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel gradientPanel1;
        private Label lblName;
        private Panel panel1;
        private DatabaseProgressControl databaseProgressControl1;
        private DatabaseProgressControl databaseProgressControl2;
    }
}