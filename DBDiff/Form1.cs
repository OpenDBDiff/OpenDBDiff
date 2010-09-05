using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DBDiff.XmlConfig;
using DBDiff.Schema.Events;
using DBDiff.Schema.Options;
using DBDiff.Schema.SQLServer2000;
using DBDiff.Schema.SQLServer2000.Model;
using DBDiff.Schema.SQLServer2000.Compare;
using DBDiff.Schema.SQLServer;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.MySQL;
using DBDiff.Schema.MySQL.Model;

namespace DBDiff
{
    public partial class Form1 : Form
    {
        private OptionFilter filter = new OptionFilter();

        public Form1()
        {
            InitializeComponent();
        }

        private void ProcesarMySQL()
        {
            DBDiff.Schema.MySQL.Model.Database origen;
            DBDiff.Schema.MySQL.Model.Database destino;

            DBDiff.Schema.MySQL.Generate sql = new DBDiff.Schema.MySQL.Generate();
            sql.ConnectioString = txtConnectionOrigen.Text;
            origen = sql.Process(filter);

            sql.ConnectioString = txtConnectionDestino.Text;
            destino = sql.Process(filter);

            this.txtScript.SQLType = SQLEnum.SQLTypeEnum.MySQL;
            this.txtDiferencias.SQLType = SQLEnum.SQLTypeEnum.MySQL;
            //origen = DBDiff.Schema.MySQL.Generate.Compare(origen, destino);
            this.txtScript.Text = origen.ToSQL();
            //this.txtDiferencias.Text = origen.ToSQLDiff();
        }

        private void ProcesarSQL2005()
        {
            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectioString = txtConnectionOrigen.Text;            
            origen = sql.Process(filter);

            sql.ConnectioString = txtConnectionDestino.Text;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);
            this.txtScript.SQLType = SQLEnum.SQLTypeEnum.SQLServer;
            this.txtDiferencias.SQLType = SQLEnum.SQLTypeEnum.SQLServer;
            this.txtScript.Text = origen.ToSQL();
            this.txtDiferencias.Text = origen.ToSQLDiff();
        }

        private void ProcesarSQL2000()
        {
            DBDiff.Schema.SQLServer2000.Model.Database origen;
            DBDiff.Schema.SQLServer2000.Model.Database destino;
            /*Type type;
            Assembly ass = Assembly.GetExecutingAssembly();
            ConfigProvider provider = ConfigProviders.GetProvider("SQLServer2000");
            type = ass.GetType(provider.Library);
            Generate sql = (Generate)System.Activator.CreateInstance(type);*/
            DBDiff.Schema.SQLServer2000.Generate sql = new DBDiff.Schema.SQLServer2000.Generate();

            lblMessage.Text = "Leyendo tablas de origen...";
            sql.OnTableProgress += new Progress.ProgressHandler(sql_OnTableProgress);
            sql.ConnectioString = txtConnectionOrigen.Text;
            origen = sql.Process();

            sql.ConnectioString = txtConnectionDestino.Text;
            lblMessage.Text = "Leyendo tablas de destino...";
            destino = sql.Process();

            origen = DBDiff.Schema.SQLServer2000.Generate.Compare(origen, destino);
            this.txtScript.SQLType = SQLEnum.SQLTypeEnum.SQLServer;
            this.txtDiferencias.SQLType = SQLEnum.SQLTypeEnum.SQLServer;
            this.txtScript.Text = origen.ToSQL();
            this.txtDiferencias.Text = origen.ToSQLDiff();
            

        }

        private void sql_OnTableProgress(object sender, ProgressEventArgs e)
        {
            Application.DoEvents();
            progressBar1.Value = (int)e.Progress;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (optSQL2000.Checked) ProcesarSQL2000();
            if (optSQL2005.Checked) ProcesarSQL2005();
            if (optMySQL.Checked) ProcesarMySQL();
        }
    }
}