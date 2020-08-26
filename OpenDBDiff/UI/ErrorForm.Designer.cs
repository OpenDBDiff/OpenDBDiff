namespace OpenDBDiff.UI
{
    partial class ErrorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorForm));
            this.ErrorPanel = new System.Windows.Forms.Panel();
            this.FindIssueButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.CopyButton = new System.Windows.Forms.Button();
            this.ErrorLabel = new System.Windows.Forms.Label();
            this.ErrorInformationTextBox = new System.Windows.Forms.TextBox();
            this.ReportIssueButton = new System.Windows.Forms.Button();
            this.ErrorPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // ErrorPanel
            //
            this.ErrorPanel.Controls.Add(this.ReportIssueButton);
            this.ErrorPanel.Controls.Add(this.FindIssueButton);
            this.ErrorPanel.Controls.Add(this.CloseButton);
            this.ErrorPanel.Controls.Add(this.CopyButton);
            this.ErrorPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ErrorPanel.Location = new System.Drawing.Point(0, 299);
            this.ErrorPanel.Name = "panel1";
            this.ErrorPanel.Size = new System.Drawing.Size(756, 30);
            this.ErrorPanel.TabIndex = 0;
            //
            // FindIssueButton
            //
            this.FindIssueButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.FindIssueButton.Location = new System.Drawing.Point(84, 4);
            this.FindIssueButton.Name = "btnFindIssue";
            this.FindIssueButton.Size = new System.Drawing.Size(131, 23);
            this.FindIssueButton.TabIndex = 2;
            this.FindIssueButton.Text = "Search for error on web";
            this.FindIssueButton.UseVisualStyleBackColor = true;
            this.FindIssueButton.Click += new System.EventHandler(this.btnFindIssue_Click);
            //
            // CloseButton
            //
            this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Location = new System.Drawing.Point(678, 4);
            this.CloseButton.Name = "btnClose";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 1;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.btnClose_Click);
            //
            // CopyButton
            //
            this.CopyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CopyButton.Location = new System.Drawing.Point(3, 4);
            this.CopyButton.Name = "btnCopy";
            this.CopyButton.Size = new System.Drawing.Size(75, 23);
            this.CopyButton.TabIndex = 0;
            this.CopyButton.Text = "Copy error";
            this.CopyButton.UseVisualStyleBackColor = true;
            this.CopyButton.Click += new System.EventHandler(this.btnCopy_Click);
            //
            // ErrorLabel
            //
            this.ErrorLabel.AutoSize = true;
            this.ErrorLabel.Location = new System.Drawing.Point(13, 13);
            this.ErrorLabel.Name = "lblError";
            this.ErrorLabel.Size = new System.Drawing.Size(254, 13);
            this.ErrorLabel.TabIndex = 1;
            this.ErrorLabel.Text = "An unexpected error has occured during processing:";
            //
            // ErrorInformationTextBox
            //
            this.ErrorInformationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ErrorInformationTextBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ErrorInformationTextBox.Location = new System.Drawing.Point(12, 29);
            this.ErrorInformationTextBox.Multiline = true;
            this.ErrorInformationTextBox.Name = "txtErrorInformation";
            this.ErrorInformationTextBox.ReadOnly = true;
            this.ErrorInformationTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ErrorInformationTextBox.Size = new System.Drawing.Size(732, 264);
            this.ErrorInformationTextBox.TabIndex = 2;
            //
            // ReportIssueButton
            //
            this.ReportIssueButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ReportIssueButton.Location = new System.Drawing.Point(221, 4);
            this.ReportIssueButton.Name = "ReportIssueButton";
            this.ReportIssueButton.Size = new System.Drawing.Size(105, 23);
            this.ReportIssueButton.TabIndex = 3;
            this.ReportIssueButton.Text = "Report issue";
            this.ReportIssueButton.UseVisualStyleBackColor = true;
            this.ReportIssueButton.Click += new System.EventHandler(this.ReportIssueButton_Click);
            //
            // ErrorForm
            //
            this.AcceptButton = this.CopyButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CloseButton;
            this.ClientSize = new System.Drawing.Size(756, 329);
            this.Controls.Add(this.ErrorInformationTextBox);
            this.Controls.Add(this.ErrorLabel);
            this.Controls.Add(this.ErrorPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ErrorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Error information";
            this.Load += new System.EventHandler(this.ErrorForm_Load);
            this.ErrorPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel ErrorPanel;
        private System.Windows.Forms.Button FindIssueButton;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button CopyButton;
        private System.Windows.Forms.Label ErrorLabel;
        private System.Windows.Forms.TextBox ErrorInformationTextBox;
        private System.Windows.Forms.Button ReportIssueButton;
    }
}
