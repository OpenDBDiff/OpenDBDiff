using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DBDiff.XmlConfig;
using DBDiff.Front;
using DBDiff.Schema.Events;
using DBDiff.Schema.Misc;
/*using DBDiff.Schema.SQLServer2000;
using DBDiff.Schema.SQLServer2000.Model;
using DBDiff.Schema.SQLServer2000.Compare;
/*using DBDiff.Schema.Sybase;
using DBDiff.Schema.Sybase.Options;
using DBDiff.Schema.Sybase.Model;*/
using DBDiff.Schema.SQLServer;
using DBDiff.Schema.SQLServer.Options;
using DBDiff.Schema.SQLServer.Model;
/*using DBDiff.Schema.MySQL;
using DBDiff.Schema.MySQL.Options;
using DBDiff.Schema.MySQL.Model;
*/
namespace DBDiff
{
    public partial class Form1 : Form
    {
        private SqlOption SqlFilter = new SqlOption();
        //private MySqlOption MySQLfilter = new MySqlOption();
        //private AseOption AseFilter = new AseOption();

        private DBDiff.Front.IFront mySqlConnectFront1;
        private DBDiff.Front.IFront mySqlConnectFront2;

        public Form1()
        {
            InitializeComponent();
            ShowSQL2005();
        }

        /*private void ProcesarSybase()
        {
            DBDiff.Schema.Sybase.Model.Database origen;
            DBDiff.Schema.Sybase.Model.Database destino;

            DBDiff.Schema.Sybase.Generate sql = new DBDiff.Schema.Sybase.Generate();
            sql.ConnectioString = txtConnectionOrigen.Text;
            
            AseFilter.OptionFilter.FilterTrigger = false;

            origen = sql.Process(AseFilter);

            sql.ConnectioString = txtConnectionDestino.Text;
            destino = sql.Process(AseFilter);

            this.txtScript.SQLType = SQLEnum.SQLTypeEnum.Sybase;
            this.txtDiferencias.SQLType = SQLEnum.SQLTypeEnum.Sybase;
            //origen = DBDiff.Schema.Sybase.Generate.Compare(origen, destino);

            this.txtScript.Text = origen.ToSQL();
            //this.txtDiferencias.Text = origen.ToSQLDiff();
        }*/

        /*private void ProcesarMySQL()
        {
            DBDiff.Schema.MySQL.Model.Database origen;
            DBDiff.Schema.MySQL.Model.Database destino;

            DBDiff.Schema.MySQL.Generate sql = new DBDiff.Schema.MySQL.Generate();
            sql.ConnectioString = mySqlConnectFront1.ConnectionString;
            origen = sql.Process(MySQLfilter);

            sql.ConnectioString = mySqlConnectFront2.ConnectionString;
            destino = sql.Process(MySQLfilter);

            //this.txtScript.SQLType = SQLEnum.SQLTypeEnum.MySQL;
            //this.txtDiferencias.SQLType = SQLEnum.SQLTypeEnum.MySQL;
            origen = DBDiff.Schema.MySQL.Generate.Compare(origen, destino);
            this.txtDiferencias.Text = origen.ToSQLDiff();
        }
        */
        private void ProcesarSQL2005()
        {
            try
            {
                DBDiff.Schema.SQLServer.Model.Database origen;
                DBDiff.Schema.SQLServer.Model.Database destino;

                if ((!String.IsNullOrEmpty(mySqlConnectFront1.DatabaseName) && (!String.IsNullOrEmpty(mySqlConnectFront2.DatabaseName))))
                {
                    DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
                    sql.ConnectionString = mySqlConnectFront1.ConnectionString;
                    origen = sql.Process(SqlFilter);

                    sql.ConnectionString = mySqlConnectFront2.ConnectionString;
                    destino = sql.Process(SqlFilter);

                    origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);
                    //this.txtScript.SQLType = SQLEnum.SQLTypeEnum.SQLServer;
                    //this.txtDiferencias.SQLType = SQLEnum.SQLTypeEnum.SQLServer;
                    this.txtDiferencias.Type = SQLRichControl.SQLTextControl.SQLType.SQLServer;
                    this.txtDiferencias.Text = origen.ToSqlDiff().ToSQL();
                    this.schemaTreeView1.Database = origen;

                    btnCopy.Enabled = true;
                    btnSaveAs.Enabled = true;
                }
                else
                    MessageBox.Show(Owner, "Please select a valid connection string", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SchemaException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SchemaException("Invalid database connection. Please check the database connection\r\n" + ex.Message);
            }
        }

        /*private void ProcesarSQL2000()
        {
            DBDiff.Schema.SQLServer2000.Model.Database origen;
            DBDiff.Schema.SQLServer2000.Model.Database destino;

            DBDiff.Schema.SQLServer2000.Generate sql = new DBDiff.Schema.SQLServer2000.Generate();

            lblMessage.Text = "Leyendo tablas de origen...";
            sql.OnTableProgress += new Progress.ProgressHandler(sql_OnTableProgress);
            //sql.ConnectioString = txtConnectionOrigen.Text;
            origen = sql.Process();

            //sql.ConnectioString = txtConnectionDestino.Text;
            lblMessage.Text = "Leyendo tablas de destino...";
            destino = sql.Process();

            origen = DBDiff.Schema.SQLServer2000.Generate.Compare(origen, destino);
            //this.txtScript.SQLType = SQLEnum.SQLTypeEnum.SQLServer;
            //this.txtDiferencias.SQLType = SQLEnum.SQLTypeEnum.SQLServer;
            this.txtDiferencias.Text = origen.ToSQLDiff();
            

        }
        */
        private void sql_OnTableProgress(object sender, ProgressEventArgs e)
        {
            Application.DoEvents();
            progressBar1.Value = (int)e.Progress;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                //if (optSQL2000.Checked) ProcesarSQL2000();
                if (optSQL2005.Checked) ProcesarSQL2005();
                //if (optMySQL.Checked) ProcesarMySQL();
                //if (optSybase.Checked) ProcesarSybase();                
            }
            catch (Exception ex)
            {
                MessageBox.Show(Owner, ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void optMySQL_CheckedChanged(object sender, EventArgs e)
        {
            /*if (optMySQL.Checked)
            {
                ShowMySQL();
            }
            else
            {
                this.groupBox2.Controls.Remove((System.Windows.Forms.Control)this.mySqlConnectFront1);
                this.groupBox3.Controls.Remove((System.Windows.Forms.Control)this.mySqlConnectFront2);
            }*/
        }

        /*private void ShowMySQL()
        {
            this.mySqlConnectFront2 = new DBDiff.Schema.MySQL.Front.MySqlConnectFront();
            this.mySqlConnectFront1 = new DBDiff.Schema.MySQL.Front.MySqlConnectFront();
            this.mySqlConnectFront1.Location = new System.Drawing.Point(5, 19);
            this.mySqlConnectFront1.Name = "mySqlConnectFront1";
            this.mySqlConnectFront1.Size = new System.Drawing.Size(420, 190);
            this.mySqlConnectFront1.TabIndex = 10;
            this.mySqlConnectFront2.Location = new System.Drawing.Point(5, 19);
            this.mySqlConnectFront2.Name = "mySqlConnectFront2";
            this.mySqlConnectFront2.Size = new System.Drawing.Size(420, 190);
            this.mySqlConnectFront2.TabIndex = 10;
            this.mySqlConnectFront1.Visible = true;
            this.mySqlConnectFront2.Visible = true;
            this.groupBox3.Controls.Add((System.Windows.Forms.Control)this.mySqlConnectFront2);
            this.groupBox2.Controls.Add((System.Windows.Forms.Control)this.mySqlConnectFront1);
        }
        */
        private void ShowSQL2005()
        {
            this.mySqlConnectFront2 = new DBDiff.Schema.SQLServer.Front.SqlServerConnectFront();
            this.mySqlConnectFront1 = new DBDiff.Schema.SQLServer.Front.SqlServerConnectFront();
            this.mySqlConnectFront1.Location = new System.Drawing.Point(5, 19);
            this.mySqlConnectFront1.Name = "mySqlConnectFront1";
            this.mySqlConnectFront1.Dock = DockStyle.Fill;
            this.mySqlConnectFront1.TabIndex = 10;
            this.mySqlConnectFront2.Location = new System.Drawing.Point(5, 19);
            this.mySqlConnectFront2.Name = "mySqlConnectFront2";
            this.mySqlConnectFront2.Dock = DockStyle.Fill;
            this.mySqlConnectFront2.TabIndex = 10;
            this.mySqlConnectFront1.Visible = true;
            this.mySqlConnectFront2.Visible = true;
            ((DBDiff.Schema.SQLServer.Front.SqlServerConnectFront)this.mySqlConnectFront1).UserName = "sa";
            ((DBDiff.Schema.SQLServer.Front.SqlServerConnectFront)this.mySqlConnectFront1).Password = "";
            ((DBDiff.Schema.SQLServer.Front.SqlServerConnectFront)this.mySqlConnectFront1).ServerName = ".";
            ((DBDiff.Schema.SQLServer.Front.SqlServerConnectFront)this.mySqlConnectFront2).UserName = "sa";
            ((DBDiff.Schema.SQLServer.Front.SqlServerConnectFront)this.mySqlConnectFront2).Password = "";
            ((DBDiff.Schema.SQLServer.Front.SqlServerConnectFront)this.mySqlConnectFront2).ServerName = ".";
            ((DBDiff.Schema.SQLServer.Front.SqlServerConnectFront)this.mySqlConnectFront1).DatabaseIndex = 1;
            ((DBDiff.Schema.SQLServer.Front.SqlServerConnectFront)this.mySqlConnectFront2).DatabaseIndex = 2;
            this.groupBox3.Controls.Add((System.Windows.Forms.Control)this.mySqlConnectFront2);
            this.groupBox2.Controls.Add((System.Windows.Forms.Control)this.mySqlConnectFront1);
        }

        private void optSQL2005_CheckedChanged(object sender, EventArgs e)
        {
            if (optSQL2005.Checked)
            {
                ShowSQL2005();
            }
            else
            {
                this.groupBox2.Controls.Remove((System.Windows.Forms.Control)this.mySqlConnectFront1);
                this.groupBox3.Controls.Remove((System.Windows.Forms.Control)this.mySqlConnectFront2);
            }

        }

        private void optSybase_CheckedChanged(object sender, EventArgs e)
        {
            /*if (optSybase.Checked)
            {
                this.mySqlConnectFront2 = new DBDiff.Schema.Sybase.Front.AseConnectFront();
                this.mySqlConnectFront1 = new DBDiff.Schema.Sybase.Front.AseConnectFront();
                this.mySqlConnectFront1.Location = new System.Drawing.Point(5, 19);
                this.mySqlConnectFront1.Name = "mySqlConnectFront1";
                this.mySqlConnectFront1.Size = new System.Drawing.Size(410, 214);
                this.mySqlConnectFront1.TabIndex = 10;
                this.mySqlConnectFront2.Location = new System.Drawing.Point(5, 19);
                this.mySqlConnectFront2.Name = "mySqlConnectFront2";
                this.mySqlConnectFront2.Size = new System.Drawing.Size(410, 214);
                this.mySqlConnectFront2.TabIndex = 10;
                this.mySqlConnectFront1.Visible = true;
                this.mySqlConnectFront2.Visible = true;
                this.groupBox3.Controls.Add((System.Windows.Forms.Control)this.mySqlConnectFront2);
                this.groupBox2.Controls.Add((System.Windows.Forms.Control)this.mySqlConnectFront1);
            }
            else
            {
                this.groupBox2.Controls.Remove((System.Windows.Forms.Control)this.mySqlConnectFront1);
                this.groupBox3.Controls.Remove((System.Windows.Forms.Control)this.mySqlConnectFront2);
            }*/
        }

        private void txtConnectionOrigen_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog(this.Owner);
            if (!String.IsNullOrEmpty(saveFileDialog1.FileName))
            {
                StreamWriter writer = new StreamWriter(saveFileDialog1.FileName, false);
                writer.Write(txtDiferencias.Text);
                writer.Close();
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtDiferencias.Text);
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            OptionForm form = new OptionForm();
            form.Show(this.Owner, SqlFilter);
        }
    }
}