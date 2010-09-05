namespace DBDiff.Schema.MySQL.Front
{
    partial class MySqlPanel
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mySqlConnectFront1 = new DBDiff.Schema.MySQL.Front.MySqlConnectFront();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.mySqlConnectFront2 = new DBDiff.Schema.MySQL.Front.MySqlConnectFront();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.mySqlConnectFront1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(433, 214);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source Connection";
            // 
            // mySqlConnectFront1
            // 
            this.mySqlConnectFront1.Location = new System.Drawing.Point(6, 14);
            this.mySqlConnectFront1.Name = "mySqlConnectFront1";
            this.mySqlConnectFront1.Size = new System.Drawing.Size(421, 191);
            this.mySqlConnectFront1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.mySqlConnectFront2);
            this.groupBox2.Location = new System.Drawing.Point(442, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(431, 214);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Target Connection";
            // 
            // mySqlConnectFront2
            // 
            this.mySqlConnectFront2.Location = new System.Drawing.Point(6, 16);
            this.mySqlConnectFront2.Name = "mySqlConnectFront2";
            this.mySqlConnectFront2.Size = new System.Drawing.Size(418, 189);
            this.mySqlConnectFront2.TabIndex = 1;
            // 
            // MySqlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MySqlPanel";
            this.Size = new System.Drawing.Size(879, 223);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private MySqlConnectFront mySqlConnectFront1;
        private MySqlConnectFront mySqlConnectFront2;
    }
}
