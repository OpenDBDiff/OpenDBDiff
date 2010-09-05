namespace DBDiff.Schema.SQLServer.Front
{
    partial class SqlServerPanel
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.sqlServerConnectFront2 = new DBDiff.Schema.SQLServer.Front.SqlServerConnectFront();
            this.sqlServerConnectFront1 = new DBDiff.Schema.SQLServer.Front.SqlServerConnectFront();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.sqlServerConnectFront1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(410, 197);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source Connection";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.sqlServerConnectFront2);
            this.groupBox2.Location = new System.Drawing.Point(419, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(405, 197);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Target Connection";
            // 
            // sqlServerConnectFront2
            // 
            this.sqlServerConnectFront2.ConnectionType = global::DBDiff.Schema.SQLServer.Properties.Settings.Default.SQLServerConnectionTypeDestino;
            this.sqlServerConnectFront2.DatabaseIndex = global::DBDiff.Schema.SQLServer.Properties.Settings.Default.SQLServerDatabaseDestino;
            this.sqlServerConnectFront2.DataBindings.Add(new System.Windows.Forms.Binding("UserName", global::DBDiff.Schema.SQLServer.Properties.Settings.Default, "SQLServerUserDestino", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.sqlServerConnectFront2.DataBindings.Add(new System.Windows.Forms.Binding("ConnectionType", global::DBDiff.Schema.SQLServer.Properties.Settings.Default, "SQLServerConnectionTypeDestino", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.sqlServerConnectFront2.DataBindings.Add(new System.Windows.Forms.Binding("ServerName", global::DBDiff.Schema.SQLServer.Properties.Settings.Default, "SQLServerNameDestino", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.sqlServerConnectFront2.DataBindings.Add(new System.Windows.Forms.Binding("DatabaseIndex", global::DBDiff.Schema.SQLServer.Properties.Settings.Default, "SQLServerDatabaseDestino", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.sqlServerConnectFront2.Location = new System.Drawing.Point(6, 19);
            this.sqlServerConnectFront2.Name = "sqlServerConnectFront2";
            this.sqlServerConnectFront2.ServerName = global::DBDiff.Schema.SQLServer.Properties.Settings.Default.SQLServerNameDestino;
            this.sqlServerConnectFront2.Size = new System.Drawing.Size(390, 169);
            this.sqlServerConnectFront2.TabIndex = 0;
            this.sqlServerConnectFront2.UserName = global::DBDiff.Schema.SQLServer.Properties.Settings.Default.SQLServerUserDestino;
            // 
            // sqlServerConnectFront1
            // 
            this.sqlServerConnectFront1.ConnectionType = global::DBDiff.Schema.SQLServer.Properties.Settings.Default.SQLServerConnectionTypeOrigen;
            this.sqlServerConnectFront1.DatabaseIndex = global::DBDiff.Schema.SQLServer.Properties.Settings.Default.SQLServerDatabaseOrigen;
            this.sqlServerConnectFront1.DataBindings.Add(new System.Windows.Forms.Binding("UserName", global::DBDiff.Schema.SQLServer.Properties.Settings.Default, "SQLServerUserOrigen", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.sqlServerConnectFront1.DataBindings.Add(new System.Windows.Forms.Binding("ConnectionType", global::DBDiff.Schema.SQLServer.Properties.Settings.Default, "SQLServerConnectionTypeOrigen", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.sqlServerConnectFront1.DataBindings.Add(new System.Windows.Forms.Binding("ServerName", global::DBDiff.Schema.SQLServer.Properties.Settings.Default, "SQLServerNameOrigen", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.sqlServerConnectFront1.DataBindings.Add(new System.Windows.Forms.Binding("DatabaseIndex", global::DBDiff.Schema.SQLServer.Properties.Settings.Default, "SQLServerDatabaseOrigen", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.sqlServerConnectFront1.Location = new System.Drawing.Point(6, 19);
            this.sqlServerConnectFront1.Name = "sqlServerConnectFront1";
            this.sqlServerConnectFront1.ServerName = global::DBDiff.Schema.SQLServer.Properties.Settings.Default.SQLServerNameOrigen;
            this.sqlServerConnectFront1.Size = new System.Drawing.Size(390, 169);
            this.sqlServerConnectFront1.TabIndex = 0;
            this.sqlServerConnectFront1.UserName = global::DBDiff.Schema.SQLServer.Properties.Settings.Default.SQLServerUserOrigen;
            // 
            // SqlServerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "SqlServerPanel";
            this.Size = new System.Drawing.Size(826, 205);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SqlServerConnectFront sqlServerConnectFront1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private SqlServerConnectFront sqlServerConnectFront2;
    }
}
