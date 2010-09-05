namespace SCide
{
	partial class frmDocument
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
			this.sciDocument = new ScintillaNet.Scintilla();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.sciDocument)).BeginInit();
			this.SuspendLayout();
			// 
			// sciDocument
			// 
			this.sciDocument.CurrentPos = 0;
			this.sciDocument.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sciDocument.LineWrap.VisualFlags = ScintillaNet.WrapVisualFlag.End;
			this.sciDocument.Location = new System.Drawing.Point(0, 0);
			this.sciDocument.Margins.Margin1.AutoToggleMarkerNumber = 0;
			this.sciDocument.Margins.Margin1.IsClickable = true;
			this.sciDocument.Margins.Margin2.Width = 16;
			this.sciDocument.Name = "sciDocument";
			this.sciDocument.Size = new System.Drawing.Size(292, 266);
			this.sciDocument.TabIndex = 0;
			this.sciDocument.SavePointLeft += new System.EventHandler(this.sciDocument_SavePointLeft);
			this.sciDocument.SavePointReached += new System.EventHandler(this.sciDocument_SavePointReached);
			// 
			// frmDocument
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.sciDocument);
			this.Name = "frmDocument";
			((System.ComponentModel.ISupportInitialize)(this.sciDocument)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private ScintillaNet.Scintilla sciDocument;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
	}
}