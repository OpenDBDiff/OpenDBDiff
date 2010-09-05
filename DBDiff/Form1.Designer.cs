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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblMessage = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.txtScript = new DBDiff.SQLRichTextBox();
            this.txtDiferencias = new DBDiff.SQLRichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.chkCompExtendedProperties = new System.Windows.Forms.CheckBox();
            this.chkCompUDT = new System.Windows.Forms.CheckBox();
            this.chkCompFunciones = new System.Windows.Forms.CheckBox();
            this.chkCompVistas = new System.Windows.Forms.CheckBox();
            this.chkCompStoreProcedure = new System.Windows.Forms.CheckBox();
            this.chkCompXMLSchemas = new System.Windows.Forms.CheckBox();
            this.chkCompSchemas = new System.Windows.Forms.CheckBox();
            this.chkCompTablaOpciones = new System.Windows.Forms.CheckBox();
            this.chkCompIndices = new System.Windows.Forms.CheckBox();
            this.chkCompConstraints = new System.Windows.Forms.CheckBox();
            this.chkCompTriggers = new System.Windows.Forms.CheckBox();
            this.chkCompTablas = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.txtConnectionOrigen = new System.Windows.Forms.TextBox();
            this.txtConnectionDestino = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.optMySQL = new System.Windows.Forms.RadioButton();
            this.optSQL2005 = new System.Windows.Forms.RadioButton();
            this.optSQL2000 = new System.Windows.Forms.RadioButton();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(9, 587);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(858, 26);
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
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(7, 95);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(883, 650);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtScript);
            this.tabPage1.Controls.Add(this.txtDiferencias);
            this.tabPage1.Controls.Add(this.progressBar1);
            this.tabPage1.Controls.Add(this.lblMessage);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(875, 624);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Script";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // txtScript
            // 
            this.txtScript.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.txtScript.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtScript.Location = new System.Drawing.Point(6, 3);
            this.txtScript.Name = "txtScript";
            this.txtScript.Size = new System.Drawing.Size(436, 558);
            this.txtScript.SQLType = DBDiff.SQLEnum.SQLTypeEnum.SQLServer;
            this.txtScript.TabIndex = 6;
            // 
            // txtDiferencias
            // 
            this.txtDiferencias.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDiferencias.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDiferencias.Location = new System.Drawing.Point(448, 3);
            this.txtDiferencias.Name = "txtDiferencias";
            this.txtDiferencias.Size = new System.Drawing.Size(421, 558);
            this.txtDiferencias.SQLType = DBDiff.SQLEnum.SQLTypeEnum.SQLServer;
            this.txtDiferencias.TabIndex = 5;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(875, 624);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Detalle";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.chkCompExtendedProperties);
            this.tabPage3.Controls.Add(this.chkCompUDT);
            this.tabPage3.Controls.Add(this.chkCompFunciones);
            this.tabPage3.Controls.Add(this.chkCompVistas);
            this.tabPage3.Controls.Add(this.chkCompStoreProcedure);
            this.tabPage3.Controls.Add(this.chkCompXMLSchemas);
            this.tabPage3.Controls.Add(this.chkCompSchemas);
            this.tabPage3.Controls.Add(this.chkCompTablaOpciones);
            this.tabPage3.Controls.Add(this.chkCompIndices);
            this.tabPage3.Controls.Add(this.chkCompConstraints);
            this.tabPage3.Controls.Add(this.chkCompTriggers);
            this.tabPage3.Controls.Add(this.chkCompTablas);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(875, 624);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Opciones";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // chkCompExtendedProperties
            // 
            this.chkCompExtendedProperties.AutoSize = true;
            this.chkCompExtendedProperties.Checked = true;
            this.chkCompExtendedProperties.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCompExtendedProperties.Location = new System.Drawing.Point(15, 271);
            this.chkCompExtendedProperties.Name = "chkCompExtendedProperties";
            this.chkCompExtendedProperties.Size = new System.Drawing.Size(169, 17);
            this.chkCompExtendedProperties.TabIndex = 11;
            this.chkCompExtendedProperties.Text = "Comparar Extended Properties";
            this.chkCompExtendedProperties.UseVisualStyleBackColor = true;
            // 
            // chkCompUDT
            // 
            this.chkCompUDT.AutoSize = true;
            this.chkCompUDT.Checked = true;
            this.chkCompUDT.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCompUDT.Location = new System.Drawing.Point(15, 248);
            this.chkCompUDT.Name = "chkCompUDT";
            this.chkCompUDT.Size = new System.Drawing.Size(154, 17);
            this.chkCompUDT.TabIndex = 10;
            this.chkCompUDT.Text = "Comparar User Data Types";
            this.chkCompUDT.UseVisualStyleBackColor = true;
            // 
            // chkCompFunciones
            // 
            this.chkCompFunciones.AutoSize = true;
            this.chkCompFunciones.Checked = true;
            this.chkCompFunciones.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCompFunciones.Location = new System.Drawing.Point(15, 225);
            this.chkCompFunciones.Name = "chkCompFunciones";
            this.chkCompFunciones.Size = new System.Drawing.Size(123, 17);
            this.chkCompFunciones.TabIndex = 9;
            this.chkCompFunciones.Text = "Comparar Funciones";
            this.chkCompFunciones.UseVisualStyleBackColor = true;
            // 
            // chkCompVistas
            // 
            this.chkCompVistas.AutoSize = true;
            this.chkCompVistas.Checked = true;
            this.chkCompVistas.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCompVistas.Location = new System.Drawing.Point(15, 202);
            this.chkCompVistas.Name = "chkCompVistas";
            this.chkCompVistas.Size = new System.Drawing.Size(102, 17);
            this.chkCompVistas.TabIndex = 8;
            this.chkCompVistas.Text = "Comparar Vistas";
            this.chkCompVistas.UseVisualStyleBackColor = true;
            // 
            // chkCompStoreProcedure
            // 
            this.chkCompStoreProcedure.AutoSize = true;
            this.chkCompStoreProcedure.Checked = true;
            this.chkCompStoreProcedure.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCompStoreProcedure.Location = new System.Drawing.Point(15, 179);
            this.chkCompStoreProcedure.Name = "chkCompStoreProcedure";
            this.chkCompStoreProcedure.Size = new System.Drawing.Size(156, 17);
            this.chkCompStoreProcedure.TabIndex = 7;
            this.chkCompStoreProcedure.Text = "Comparar Store Procedures";
            this.chkCompStoreProcedure.UseVisualStyleBackColor = true;
            // 
            // chkCompXMLSchemas
            // 
            this.chkCompXMLSchemas.AutoSize = true;
            this.chkCompXMLSchemas.Checked = true;
            this.chkCompXMLSchemas.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCompXMLSchemas.Location = new System.Drawing.Point(15, 156);
            this.chkCompXMLSchemas.Name = "chkCompXMLSchemas";
            this.chkCompXMLSchemas.Size = new System.Drawing.Size(143, 17);
            this.chkCompXMLSchemas.TabIndex = 6;
            this.chkCompXMLSchemas.Text = "Comparar XML Schemas";
            this.chkCompXMLSchemas.UseVisualStyleBackColor = true;
            // 
            // chkCompSchemas
            // 
            this.chkCompSchemas.AutoSize = true;
            this.chkCompSchemas.Checked = true;
            this.chkCompSchemas.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCompSchemas.Location = new System.Drawing.Point(15, 133);
            this.chkCompSchemas.Name = "chkCompSchemas";
            this.chkCompSchemas.Size = new System.Drawing.Size(118, 17);
            this.chkCompSchemas.TabIndex = 5;
            this.chkCompSchemas.Text = "Comparar Schemas";
            this.chkCompSchemas.UseVisualStyleBackColor = true;
            // 
            // chkCompTablaOpciones
            // 
            this.chkCompTablaOpciones.AutoSize = true;
            this.chkCompTablaOpciones.Checked = true;
            this.chkCompTablaOpciones.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCompTablaOpciones.Location = new System.Drawing.Point(15, 110);
            this.chkCompTablaOpciones.Name = "chkCompTablaOpciones";
            this.chkCompTablaOpciones.Size = new System.Drawing.Size(160, 17);
            this.chkCompTablaOpciones.TabIndex = 4;
            this.chkCompTablaOpciones.Text = "Comparar Opciones de tabla";
            this.chkCompTablaOpciones.UseVisualStyleBackColor = true;
            // 
            // chkCompIndices
            // 
            this.chkCompIndices.AutoSize = true;
            this.chkCompIndices.Checked = true;
            this.chkCompIndices.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCompIndices.Location = new System.Drawing.Point(15, 87);
            this.chkCompIndices.Name = "chkCompIndices";
            this.chkCompIndices.Size = new System.Drawing.Size(108, 17);
            this.chkCompIndices.TabIndex = 3;
            this.chkCompIndices.Text = "Comparar Indices";
            this.chkCompIndices.UseVisualStyleBackColor = true;
            // 
            // chkCompConstraints
            // 
            this.chkCompConstraints.AutoSize = true;
            this.chkCompConstraints.Checked = true;
            this.chkCompConstraints.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCompConstraints.Location = new System.Drawing.Point(15, 64);
            this.chkCompConstraints.Name = "chkCompConstraints";
            this.chkCompConstraints.Size = new System.Drawing.Size(126, 17);
            this.chkCompConstraints.TabIndex = 2;
            this.chkCompConstraints.Text = "Comparar Constraints";
            this.chkCompConstraints.UseVisualStyleBackColor = true;
            // 
            // chkCompTriggers
            // 
            this.chkCompTriggers.AutoSize = true;
            this.chkCompTriggers.Checked = true;
            this.chkCompTriggers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCompTriggers.Location = new System.Drawing.Point(15, 41);
            this.chkCompTriggers.Name = "chkCompTriggers";
            this.chkCompTriggers.Size = new System.Drawing.Size(112, 17);
            this.chkCompTriggers.TabIndex = 1;
            this.chkCompTriggers.Text = "Comparar Triggers";
            this.chkCompTriggers.UseVisualStyleBackColor = true;
            // 
            // chkCompTablas
            // 
            this.chkCompTablas.AutoSize = true;
            this.chkCompTablas.Checked = true;
            this.chkCompTablas.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCompTablas.Location = new System.Drawing.Point(15, 18);
            this.chkCompTablas.Name = "chkCompTablas";
            this.chkCompTablas.Size = new System.Drawing.Size(106, 17);
            this.chkCompTablas.TabIndex = 0;
            this.chkCompTablas.Text = "Comparar Tablas";
            this.chkCompTablas.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(682, 49);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(208, 35);
            this.button1.TabIndex = 6;
            this.button1.Text = "Comparar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtConnectionOrigen
            // 
            this.txtConnectionOrigen.Location = new System.Drawing.Point(7, 42);
            this.txtConnectionOrigen.Name = "txtConnectionOrigen";
            this.txtConnectionOrigen.Size = new System.Drawing.Size(665, 20);
            this.txtConnectionOrigen.TabIndex = 7;
            this.txtConnectionOrigen.Text = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestDataType" +
                "Formula1;Data Source=PC-ES\\SQL2005";
            // 
            // txtConnectionDestino
            // 
            this.txtConnectionDestino.Location = new System.Drawing.Point(7, 69);
            this.txtConnectionDestino.Name = "txtConnectionDestino";
            this.txtConnectionDestino.Size = new System.Drawing.Size(665, 20);
            this.txtConnectionDestino.TabIndex = 8;
            this.txtConnectionDestino.Text = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestDataType" +
                "Formula2;Data Source=PC-ES\\SQL2005";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.optMySQL);
            this.groupBox1.Controls.Add(this.optSQL2005);
            this.groupBox1.Controls.Add(this.optSQL2000);
            this.groupBox1.Location = new System.Drawing.Point(7, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(665, 36);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Base de datos";
            // 
            // optMySQL
            // 
            this.optMySQL.AutoSize = true;
            this.optMySQL.Location = new System.Drawing.Point(241, 15);
            this.optMySQL.Name = "optMySQL";
            this.optMySQL.Size = new System.Drawing.Size(122, 17);
            this.optMySQL.TabIndex = 2;
            this.optMySQL.Text = "MySQL 5.0 or higher";
            this.optMySQL.UseVisualStyleBackColor = true;
            // 
            // optSQL2005
            // 
            this.optSQL2005.AutoSize = true;
            this.optSQL2005.Checked = true;
            this.optSQL2005.Location = new System.Drawing.Point(119, 15);
            this.optSQL2005.Name = "optSQL2005";
            this.optSQL2005.Size = new System.Drawing.Size(107, 17);
            this.optSQL2005.TabIndex = 1;
            this.optSQL2005.TabStop = true;
            this.optSQL2005.Text = "SQL Server 2005";
            this.optSQL2005.UseVisualStyleBackColor = true;
            // 
            // optSQL2000
            // 
            this.optSQL2000.AutoSize = true;
            this.optSQL2000.Location = new System.Drawing.Point(6, 15);
            this.optSQL2000.Name = "optSQL2000";
            this.optSQL2000.Size = new System.Drawing.Size(107, 17);
            this.optSQL2000.TabIndex = 0;
            this.optSQL2000.Text = "SQL Server 2000";
            this.optSQL2000.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(902, 746);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtConnectionDestino);
            this.Controls.Add(this.txtConnectionOrigen);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Open DBDiff";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtConnectionOrigen;
        private System.Windows.Forms.TextBox txtConnectionDestino;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton optSQL2005;
        private System.Windows.Forms.RadioButton optSQL2000;
        private SQLRichTextBox txtDiferencias;
        private SQLRichTextBox txtScript;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.CheckBox chkCompTablaOpciones;
        private System.Windows.Forms.CheckBox chkCompIndices;
        private System.Windows.Forms.CheckBox chkCompConstraints;
        private System.Windows.Forms.CheckBox chkCompTriggers;
        private System.Windows.Forms.CheckBox chkCompTablas;
        private System.Windows.Forms.CheckBox chkCompXMLSchemas;
        private System.Windows.Forms.CheckBox chkCompSchemas;
        private System.Windows.Forms.CheckBox chkCompFunciones;
        private System.Windows.Forms.CheckBox chkCompVistas;
        private System.Windows.Forms.CheckBox chkCompStoreProcedure;
        private System.Windows.Forms.CheckBox chkCompUDT;
        private System.Windows.Forms.CheckBox chkCompExtendedProperties;
        private System.Windows.Forms.RadioButton optMySQL;
    }
}

