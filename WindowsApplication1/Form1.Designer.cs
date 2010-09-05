namespace WindowsApplication1
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
            this.sqlRichTextBox1 = new DBDiff.SQLRichTextBox();
            this.SuspendLayout();
            // 
            // sqlRichTextBox1
            // 
            this.sqlRichTextBox1.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sqlRichTextBox1.Location = new System.Drawing.Point(27, 30);
            this.sqlRichTextBox1.Name = "sqlRichTextBox1";
            this.sqlRichTextBox1.Size = new System.Drawing.Size(320, 240);
            this.sqlRichTextBox1.SQLType = DBDiff.SQLEnum.SQLTypeEnum.SQLServer;
            this.sqlRichTextBox1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 419);
            this.Controls.Add(this.sqlRichTextBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private DBDiff.SQLRichTextBox sqlRichTextBox1;
    }
}

