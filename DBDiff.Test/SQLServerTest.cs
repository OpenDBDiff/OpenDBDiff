using System;
using System.Text;
using System.Collections.Generic;
using System.Resources;
using System.Threading;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DBDiff.Schema.Events;
using DBDiff.Schema.SQLServer.Options;
using DBDiff.Schema.SQLServer;
using DBDiff.Schema.SQLServer.Model;

namespace DBDiff.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class SQLServerTest
    {
        //private const string DATABASE_NAME = "VIRTUAL-XP2\\SQLEXPRESS;User ID=sa;Password=1";
        private const string DATABASE_NAME = "(LOCAL);User ID=sa;Password=";

        public SQLServerTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion
        [Description("Testea Velocidad con la base ISMM."), TestMethod]
        public void TestSpeedISMM()
        {
            SqlConnection conn1 = new SqlConnection();
            SqlConnection conn2 = new SqlConnection();
            SqlCommand com1;
            SqlCommand com2;

            string expected = "";
            expected += "USE AFULL\r\nGO\r\n\r\n";
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen = null;
            DBDiff.Schema.SQLServer.Model.Database destino = null;
            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();

            com1 = new SqlCommand("DBCC DROPCLEANBUFFERS");
            com2 = new SqlCommand("DBCC FREEPROCCACHE");
            conn2.ConnectionString = "Persist Security Info=False;Initial Catalog=AFULL;Data Source=" + DATABASE_NAME;
            conn2.Open();
            com1.Connection = conn2;
            com2.Connection = conn2;
            com1.ExecuteNonQuery();
            com2.ExecuteNonQuery();
            com1.Dispose();
            com2.Dispose();
            conn2.Close();
            
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=AFULL;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            com1 = new SqlCommand("DBCC DROPCLEANBUFFERS");
            com2 = new SqlCommand("DBCC FREEPROCCACHE");
            conn2.ConnectionString = "Persist Security Info=False;Initial Catalog=AFULL;Data Source=" + DATABASE_NAME;
            conn2.Open();
            com1.Connection = conn2;
            com2.Connection = conn2;
            com1.ExecuteNonQuery();
            com2.ExecuteNonQuery();
            com1.Dispose();
            com2.Dispose();
            conn2.Close();

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=AFULL;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ENABLED/DISABLED de Check Constraint."), TestMethod]
        public void TestConstraintCheckEnabled()
        {
            string expected = "";
            expected += "USE TestConstraintCheckEnabled1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] DROP CONSTRAINT [CK_Tabla1]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ADD CONSTRAINT [CK_Tabla1] CHECK  (([ID]>(1)))\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla2] NOCHECK CONSTRAINT [CK_Tabla2]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] CHECK CONSTRAINT [CK_Tabla1]\r\nGO\r\n";
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintCheckEnabled1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintCheckEnabled2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD/DROP/CHANGE Column Position."), TestMethod]
        public void TestColumnPosition()
        {
            string expected = "";
            expected += "USE TestColumnPosition1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[Tabla3] DROP CONSTRAINT [PK_Tabla3]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla2] DROP COLUMN [Campo1]\r\nGO\r\n";
            expected += "CREATE TABLE [dbo].[TempTabla1]\r\n(\r\n";
            expected += "\t[ID] [int] NOT NULL,\r\n";
            expected += "\t[Campo1] [int] NOT NULL,\r\n";
            expected += "\t[Campo3] [int] NOT NULL,\r\n";
            expected += "\t[Campo2] [int] NOT NULL\r\n";
            expected += ") ON [PRIMARY]\r\nGO\r\n\r\n";
            expected += "INSERT INTO [dbo].[TempTabla1] ([ID],[Campo1],[Campo2],[Campo3]) SELECT [ID],[Campo1],[Campo2],[Campo3] FROM [dbo].[Tabla1]\r\n";
            expected += "DROP TABLE [dbo].[Tabla1]\r\nGO\r\n";
            expected += "EXEC sp_rename N'[dbo].[TempTabla1]',N'Tabla1', 'OBJECT'\r\nGO\r\n\r\n";
            expected += "CREATE TABLE [dbo].[TempTabla3]\r\n(\r\n";
            expected += "\t[ID] [int] NOT NULL,\r\n";
            expected += "\t[Campo0] [int] NOT NULL,\r\n";
            expected += "\t[Campo1] [int] NOT NULL,\r\n";
            expected += "\t[Campo2] [nchar] (10) COLLATE Modern_Spanish_CI_AS NULL\r\n";
            expected += ") ON [PRIMARY]\r\nGO\r\n\r\n";
            expected += "INSERT INTO [dbo].[TempTabla3] ([ID],[Campo1],[Campo2],[Campo0]) SELECT [ID],[Campo1],[Campo2],0 FROM [dbo].[Tabla3]\r\n";
            expected += "DROP TABLE [dbo].[Tabla3]\r\nGO\r\n";
            expected += "EXEC sp_rename N'[dbo].[TempTabla3]',N'Tabla3', 'OBJECT'\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[Tabla3] ADD CONSTRAINT [PK_Tabla3] PRIMARY KEY CLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID] ASC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnPosition1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnPosition2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD/DROP/CHANGE Column Position Add."), TestMethod]
        public void TestColumnPositionAdd()
        {
            string expected = "";
            expected += "USE TestColumnPositionMultiple1\r\nGO\r\n\r\n";
            expected += "CREATE TABLE [dbo].[TempTabla1]\r\n\t(\r\n";
            expected += "\t[ID0] [int] NULL,\r\n";
            expected += "\t[ID] [int] NOT NULL,\r\n";
            expected += "\t[Campo00] [int] NULL,\r\n";
            expected += "\t[Campo0] [int] NULL,\r\n";
            expected += "\t[Campo2] [int] NOT NULL,\r\n";
            expected += "\t[Campo3] [int] NULL\r\n";
            expected += ") ON [PRIMARY]\r\nGO\r\n\r\n";
            expected += "INSERT INTO [dbo].[TempTabla1] ([ID],[Campo2]) SELECT [ID],[Campo2] FROM [dbo].[Tabla1]\r\n";
            expected += "DROP TABLE [dbo].[Tabla1]\r\nGO\r\n";
            expected += "EXEC sp_rename N'[dbo].[TempTabla1]',N'Tabla1', 'OBJECT'\r\nGO\r\n";


            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnPositionMultiple1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnPositionMultiple2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD, DROP y ALTER de Check Constraint."), TestMethod]
        public void TestConstraintCheckAlter()
        {
            string expected = "";
            expected += "USE TestConstraintCheck1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[TableAlterCheck] DROP CONSTRAINT [CK_TableAlterCheck_12]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TableAlterCheck] DROP CONSTRAINT [CK_TableAlterCheck_333]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TableAlterCheck] DROP CONSTRAINT [CK_TableAlterCheck]\r\nGO\r\n";                        
            expected += "ALTER TABLE [dbo].[TableAlterCheck] ADD CONSTRAINT [CK_TableAlterCheck_1] CHECK  (([CampoCheck2]*(2)>=(1)))\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TableAlterCheck] ADD CONSTRAINT [CK_TableAlterCheck] CHECK  (([CampoCheck]>(2)))\r\nGO\r\n";                        
            expected += "ALTER TABLE [dbo].[TableAlterCheck] WITH NOCHECK ADD CONSTRAINT [CK_TableAlterCheck_12] CHECK NOT FOR REPLICATION (([CampoCheck5]>(1)))\r\nGO\r\n";            
            expected += "ALTER TABLE [dbo].[TableAlterCheck] NOCHECK CONSTRAINT [CK_TableAlterCheck_t]\r\nGO\r\n";
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintCheck1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintCheck2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER de Default Constraint en una PK."), TestMethod]
        public void TestConstraintDefaultPK()
        {
            string expected = "";
            expected += "USE TestConstraintDefaultPK1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[Table_1] DROP CONSTRAINT [DF_Table_1_ID2]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Table_1] ADD CONSTRAINT [DF_Table_1_ID2] DEFAULT ((60)) FOR [ID2]\r\nGO\r\n";
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintDefaultPK1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintDefaultPK2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER de Check Constraint Rebuild."), TestMethod]
        public void TestConstraintCheckAlterRebuild()
        {
            string expected = "";
            expected += "USE TestConstraintCheckRebuil1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] DROP CONSTRAINT [CK_Tabla1]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] DROP CONSTRAINT [PK_Tabla1]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [ID] [smallint] NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ADD CONSTRAINT [CK_Tabla1] CHECK  (([ID]>(1)))\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ADD CONSTRAINT [PK_Tabla1] PRIMARY KEY CLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID] ASC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintCheckRebuil1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintCheckRebuil2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER de Default Constraint Rebuild."), TestMethod]
        public void TestConstraintDefaultAlterRebuild()
        {
            string expected = "";
            expected += "USE TestConstraintDefaultRebuild1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] DROP CONSTRAINT [DF_Tabla1_Campo2]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] DROP CONSTRAINT [PK_Tabla1]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [ID] [smallint] NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ADD CONSTRAINT [DF_Tabla1_Campo2] DEFAULT (N'''888999') FOR [Campo2]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ADD CONSTRAINT [PK_Tabla1] PRIMARY KEY CLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID] ASC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";            
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintDefaultRebuild1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintDefaultRebuild2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD, DROP y ALTER de Default Constraint."), TestMethod]
        public void TestConstraintDefaultAlter()
        {
            string expected = "";
            expected += "USE TestConstraintDefaultSimple1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] DROP CONSTRAINT [DF_Tabla1_Campo2]\r\nGO\r\n";            
            expected += "ALTER TABLE [dbo].[Tabla1] DROP CONSTRAINT [DF_Tabla1_Campo1]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ADD CONSTRAINT [DF_Tabla1_Campo3] DEFAULT ('Hola2') FOR [Campo3]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ADD CONSTRAINT [DF_Tabla1_Campo1] DEFAULT ('HolaAAAAAA') FOR [Campo1]\r\nGO\r\n";            
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintDefaultSimple1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintDefaultSimple2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD de Columna con Default."), TestMethod]
        public void TestConstraintDefaultAddColumn()
        {
            string expected = "";
            expected += "USE TestConstraintDefaultAddColumn1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ADD \r\n";
            expected += "[Campo2] [int] NOT NULL CONSTRAINT [DF_Tabla1_Campo2] DEFAULT ((2))\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintDefaultAddColumn1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintDefaultAddColumn2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER/ADD/DROP de Synonyms."), TestMethod]
        public void TestSynonyms()
        {
            string expected = "";

            expected = Scripts.TestSynonyms;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestSinonimos1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestSinonimos2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER de Constraint Foreign Key dependientes."), TestMethod]
        public void TestConstraintFKTree()
        {
            string expected = "";

            expected = Scripts.TestConstraintFKTree1;

            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintFKTree1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintFKTree2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea Enabled/Disabled de Constraint Foreign Key."), TestMethod]
        public void TestConstraintFKEnabled()
        {
            string expected = Scripts.TestConstraintFKDisable;

            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintFKDisable1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintFKDisable2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD/DROP de Constraint Foreign Key."), TestMethod]
        public void TestConstraintFKSimple()
        {
            string expected = "";
            expected += "USE TestConstraintFKSimple1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[TablaPadre] DROP CONSTRAINT [FK_TablaPadre_TablaPadre]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaHija] ADD CONSTRAINT [FK_TablaHija_TablaPadre] FOREIGN KEY\r\n\t(\r\n";
            expected += "\t\t[TablaPadreId],\r\n";
            expected += "\t\t[TablaPadreId2]\r\n";
            expected += "\t)\r\n";
            expected += "\tREFERENCES [dbo].[TablaPadre]\r\n\t(\r\n";
            expected += "\t\t[ID],\r\n";
            expected += "\t\t[ID2]\r\n";
            expected += "\t)\r\nGO\r\n";
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintFKSimple1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintFKSimple2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER COLUMN y ADD y DROP de una Check Constraint."), TestMethod]
        public void TestConstraintCheckAlterColumn()
        {
            string expected = Scripts.TestConstraintCheckAlterColumn;
            
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintCheckAlterColumn1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintCheckAlterColumn2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD, DROP y ALTER de PK Constraint con UserDataTypes."), TestMethod]
        public void TestConstraintPKDataType()
        {
            string expected = "";
            expected += "USE TestConstraintPKType1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] DROP CONSTRAINT [PK_Tabla1]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] DROP COLUMN [ID3]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [ID2] [int] NOT NULL\r\nGO\r\n";
            expected += "DROP TYPE [dbo].[Tipo1]\r\nGO\r\n";
            expected += "CREATE TYPE [dbo].[Tipo1] FROM [int] NOT NULL\r\nGO\r\n";
            expected += "CREATE TABLE [dbo].[TempTabla1]\r\n(\r\n";
            expected += "\t[ID] [int] NOT NULL,\r\n";
            expected += "\t[ID2] [Tipo1] NOT NULL,\r\n";
            expected += "\t[ID4] [int] NOT NULL,\r\n";
            expected += "\t[ID3] AS ([ID]+[ID2]) PERSISTED,\r\n";
            expected += "\t[Campo1] [varchar] (50) COLLATE Modern_Spanish_CI_AS NULL\r\n";            
            expected += ") ON [PRIMARY]\r\nGO\r\n\r\n";
            expected += "INSERT INTO [dbo].[TempTabla1] ([ID],[ID2],[Campo1],[ID4]) SELECT [ID],[ID2],[Campo1],0 FROM [dbo].[Tabla1]\r\n";
            expected += "DROP TABLE [dbo].[Tabla1]\r\nGO\r\n";
            expected += "EXEC sp_rename N'[dbo].[TempTabla1]',N'Tabla1', 'OBJECT'\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ADD CONSTRAINT [PK_Tabla1] PRIMARY KEY CLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID] ASC,\r\n";
            expected += "\t\t[ID2] DESC,\r\n";
            expected += "\t\t[ID4] ASC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO";          
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintPKType1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintPKType2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD, DROP y ALTER de PK Constraint."), TestMethod]
        public void TestConstraintPKAlter()
        {
            string expected = "";
            expected += "USE TestConstraintPK1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[TablaAlterPK] DROP CONSTRAINT [PK_TablaAlterPK]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaDropPK] DROP CONSTRAINT [PK_TablaDropPK]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaAddPKCover] ADD CONSTRAINT [PK_TablaAddPKCover] PRIMARY KEY CLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID1] ASC,\r\n";
            expected += "\t\t[ID2] DESC,\r\n";
            expected += "\t\t[ID3] ASC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaAlterPK] ADD CONSTRAINT [PK_TablaAlterPK] PRIMARY KEY CLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID] DESC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaAddPK] ADD CONSTRAINT [PK_TablaAddPK] PRIMARY KEY CLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID] ASC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintPK1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintPK2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER de File Group en Tabla e Indices."), TestMethod]
        public void TestFileGroupTable()
        {
            string expected = "";
            expected += "USE TestTableFileGroup1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[TablaAlterFileGroupIndex] DROP CONSTRAINT [PK_TablaAlterFileGroupIndex]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaAlterFileGroup] DROP CONSTRAINT [PK_TablaAlterFileGroup2] WITH (MOVE TO [PRIMARY])\r\nGO\r\n";            
            expected += "DROP INDEX [IX_TablaAlterFileGroupNon] ON [dbo].[TablaAlterFileGroupNon]\r\nGO\r\n";
            expected += "DROP INDEX [IX_TablaAlterFileGroupIndex] ON [dbo].[TablaAlterFileGroupIndex] WITH (MOVE TO [PRIMARY])\r\nGO\r\n";
            expected += "CREATE TABLE [dbo].[TempTablaAlterFileGroupHeap]\r\n(\r\n";
            expected += "\t[ID] [int] NOT NULL\r\n";
            expected += ") ON [PRIMARY]\r\nGO\r\n\r\n";
            expected += "INSERT INTO [dbo].[TempTablaAlterFileGroupHeap] ([ID]) SELECT [ID] FROM [dbo].[TablaAlterFileGroupHeap]\r\n";
            expected += "DROP TABLE [dbo].[TablaAlterFileGroupHeap]\r\nGO\r\n";
            expected += "EXEC sp_rename N'[dbo].[TempTablaAlterFileGroupHeap]',N'TablaAlterFileGroupHeap', 'OBJECT'\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[TablaAlterFileGroup] ADD CONSTRAINT [PK_TablaAlterFileGroup2] PRIMARY KEY CLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID] ASC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaAlterFileGroupIndex] ADD CONSTRAINT [PK_TablaAlterFileGroupIndex] PRIMARY KEY NONCLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID] ASC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";
            expected += "CREATE NONCLUSTERED INDEX [IX_TablaAlterFileGroupNon] ON [dbo].[TablaAlterFileGroupNon]\r\n(\r\n";
            expected += "\t[Valor] ASC\r\n";
            expected += ") WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";
            expected += "CREATE CLUSTERED INDEX [IX_TablaAlterFileGroupIndex] ON [dbo].[TablaAlterFileGroupIndex]\r\n(\r\n";
            expected += "\t[Valor] ASC\r\n";
            expected += ") WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";
            expected += "ALTER DATABASE [TestTableFileGroup1] REMOVE FILE [TestTableFileGroupSecond1]\r\nGO\r\n";
            expected += "ALTER DATABASE [TestTableFileGroup1] REMOVE FILEGROUP [SECONDARY]\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestTableFileGroup1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestTableFileGroup2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea Add de File Group."), TestMethod]
        public void TestFileGroupSimpleAdd()
        {
            string expected = "";
            expected += "USE TestFileGroupSimple1\r\nGO\r\n\r\n";
            expected += "ALTER DATABASE [TestFileGroupSimple1] ADD FILEGROUP [SECONDARY]\r\nGO\r\n";
            expected += "ALTER DATABASE [TestFileGroupSimple1]\r\n";
            expected += "ADD FILE ( NAME = N'Test', FILENAME = N'C:\\Archivos de programa\\Microsoft SQL Server\\MSSQL.1\\MSSQL\\Data\\TestFileGroupSimple1_DB.ndf' , SIZE = 384000KB , FILEGROWTH = 128000KB) TO FILEGROUP [SECONDARY]\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestFileGroupSimple1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestFileGroupSimple2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER de File Group en Tabla con campo BLOB."), TestMethod]
        public void TestFileGroupTableText()
        {
            string expected = "";
            expected += "USE TestTableFileGroupText1\r\nGO\r\n\r\n";
            expected += "CREATE TABLE [dbo].[TempTabla1]\r\n(\r\n";
            expected += "\t[ID] [int] NOT NULL,\r\n";
            expected += "\t[Text] [text] COLLATE Modern_Spanish_CI_AS NOT NULL\r\n";
            expected += ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]\r\nGO\r\n\r\n";
            expected += "INSERT INTO [dbo].[TempTabla1] ([ID],[Text]) SELECT [ID],[Text] FROM [dbo].[Tabla1]\r\n";
            expected += "DROP TABLE [dbo].[Tabla1]\r\nGO\r\n";
            expected += "EXEC sp_rename N'[dbo].[TempTabla1]',N'Tabla1', 'OBJECT'\r\nGO\r\n\r\n";
            expected += "ALTER DATABASE [TestTableFileGroupText1] REMOVE FILE [Hola]\r\nGO\r\n";
            expected += "ALTER DATABASE [TestTableFileGroupText1] REMOVE FILEGROUP [TEXTGROUP]\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestTableFileGroupText1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestTableFileGroupText2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER COLUMN de columas Timestamp."), TestMethod]
        public void TestColumnTimestamp()
        {
            string expected = "USE TestColumnTimestamp1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] DROP CONSTRAINT [PK_Tabla1]\r\nGO\r\n";
            expected += "CREATE TABLE [dbo].[TempTabla1]\r\n(\r\n";
            expected += "\t[ID] [int] NOT NULL,\r\n";
            expected += "\t[Campo1] [varchar] (50) COLLATE Modern_Spanish_CI_AS NULL,\r\n";
            expected += "\t[CampoT] [timestamp] NULL\r\n";
            expected += ") ON [PRIMARY]\r\nGO\r\n";
            expected += "INSERT INTO [dbo].[TempTabla1] ([ID],[Campo1]) SELECT [ID],[Campo1] FROM [dbo].[Tabla1]\r\n";
            expected += "DROP TABLE [dbo].[Tabla1]\r\nGO\r\n";
            expected += "EXEC sp_rename N'[dbo].[TempTabla1]',N'Tabla1', 'OBJECT'\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ADD CONSTRAINT [PK_Tabla1] PRIMARY KEY CLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID] ASC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";


            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnTimestamp1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnTimestamp2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD,ALTER Y DROP COLUMN de columas Identity."), TestMethod]
        public void TestColumnIdentity()
        {
            string expected = Scripts.TestColumnIdentity;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnIdentity1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnIdentity2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD y ALTER COLUMN de columas XML."), TestMethod]
        public void TestColumnXML()
        {
            string expected = "USE TestColumnXML1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[TablaXML2] ALTER COLUMN [XML1] [xml](CONTENT [dbo].AdditionalContactInfoSchemaCollection) NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaXML2] ALTER COLUMN [XML2] [xml](CONTENT [dbo].HRResumeSchemaCollection) NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaXML2] ALTER COLUMN [XML3] [xml](CONTENT [dbo].HRResumeSchemaCollection) NOT NULL\r\nGO\r\n";
            expected += "CREATE TABLE [dbo].[TempTablaXML1]\r\n(\r\n";
            expected += "\t[ID] [int] NOT NULL,\r\n";
            expected += "\t[XML1] [xml] NOT NULL,\r\n";
            expected += "\t[XML2] [xml] NULL,\r\n";
            expected += "\t[XML3] [xml](CONTENT [dbo].HRResumeSchemaCollection) NOT NULL,\r\n";
            expected += "\t[XML4] [xml](DOCUMENT [dbo].HRResumeSchemaCollection) NULL,\r\n";
            expected += "\t[XML5] [xml](CONTENT [dbo].AdditionalContactInfoSchemaCollection) NULL\r\n)\r\nGO\r\n\r\n";
            expected += "INSERT INTO [dbo].[TempTablaXML1] ([ID]) SELECT [ID] FROM [dbo].[TablaXML1]\r\n";
            expected += "DROP TABLE [dbo].[TablaXML1]\r\nGO\r\n";
            expected += "EXEC sp_rename N'[dbo].[TempTablaXML1]',N'TablaXML1', 'OBJECT'\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption(false);
            filter.Ignore.FilterTable = true;

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnXML1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnXML2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", ""), expected.Replace(" ", "").Replace("\r\n", ""), "Error");
        }

        [Description("Testea ADD COLUMN de todos los Tipos de Datos."), TestMethod]
        public void TestColumn()
        {
            string expected = "USE TestColumnAdd1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[TablaAdd] ADD\r\n";
            expected += "[Campo1] [bigint] NULL,\r\n";
            expected += "[Campo2] [binary] (50) NULL,\r\n";
            expected += "[Campo3] [bit] NULL,\r\n";
            expected += "[Campo4] [char] (10) COLLATE Modern_Spanish_CI_AS NULL,\r\n";
            expected += "[Campo5] [datetime] NULL,\r\n";
            expected += "[Campo6] [decimal] (18,0) NULL,\r\n";
            expected += "[Campo7] [float] NULL,\r\n";
            expected += "[Campo8] [image] NULL,\r\n";
            expected += "[Campo9] [money] NULL,\r\n";
            expected += "[Campo10] [ntext] COLLATE Modern_Spanish_CI_AS NULL,\r\n";
            expected += "[Campo11] [numeric] (18,0) NULL,\r\n";
            expected += "[Campo12] [nvarchar] (50) COLLATE Modern_Spanish_CI_AS NULL,\r\n";
            expected += "[Campo13] [nvarchar] (max) COLLATE Modern_Spanish_CI_AS NULL,\r\n";
            expected += "[campo14] [real] NULL,\r\n";
            expected += "[campo15] [smalldatetime] NULL,\r\n";
            expected += "[Campo16] [smallint] NULL,\r\n";
            expected += "[Campo17] [sql_variant] NULL,\r\n";
            expected += "[Campo18] [text] COLLATE Modern_Spanish_CI_AS NULL,\r\n";
            expected += "[Campo19] [timestamp] NULL,\r\n";
            expected += "[Campo20] [tinyint] NULL,\r\n";
            expected += "[Campo21] [uniqueidentifier] NULL,\r\n";
            expected += "[Campo22] [varbinary] (50) NULL,\r\n";
            expected += "[Campo23] [varbinary] (max) NULL,\r\n";
            expected += "[Campo24] [varchar] (50) COLLATE Modern_Spanish_CI_AS NULL,\r\n";
            expected += "[Campo25] [varchar] (max) COLLATE Modern_Spanish_CI_AS NULL,\r\n";
            expected += "[Campo26] [xml] NULL\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption(false);
            filter.Ignore.FilterTable = true;

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnAdd1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnAdd2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", ""), expected.Replace(" ", "").Replace("\r\n", ""), "Error");
        }

        [Description("Testea ADD y ALTER COLUMN de columnas RowGuid."), TestMethod]
        public void TestColumnRowGuid()
        {
            string expected = "USE TestColumnRowGuid1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[TableAlterRowGuid3] DROP CONSTRAINT [DF_TableAlterRowGuid3_Guid1]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TableAddRowGuid2] ADD\r\n";
            expected += "[Campo1] [uniqueidentifier] NOT NULL ROWGUIDCOL CONSTRAINT [DF_TableAddRowGuid2_Campo1] DEFAULT (newsequentialid())\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TableAddRowGuid1] ADD\r\n";
            expected += "[guid] [uniqueidentifier] NOT NULL ROWGUIDCOL CONSTRAINT [DF_TableAddRowGuid1_guid] DEFAULT (newid()),\r\n";
            expected += "[guid2] [uniqueidentifier] NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TableAlterRowGuid3] ADD CONSTRAINT [DF_TableAlterRowGuid3_Guid1] DEFAULT (newsequentialid()) FOR [Guid1]\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption(false);
            filter.Ignore.FilterTable = true;

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnRowGuid1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnRowGuid2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", ""), expected.Replace(" ", "").Replace("\r\n", ""), "Error");
        }

        [Description("Testea ALTER COLUMN de Campos NULL."), TestMethod]
        public void TestColumnNull()
        {
            string expected = "USE TestColumnNull1\r\nGO\r\n\r\n";
            expected += "UPDATE [dbo].[TablaColumnasNull] SET [Campo1] = 0x WHERE [Campo1] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaColumnasNull] ALTER COLUMN [Campo2] [nchar] (10) COLLATE Modern_Spanish_CI_AS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaColumnasNull] ALTER COLUMN [Campo1] [varbinary] (50) NOT NULL\r\nGO\r\n";
            
            string result;
            SqlOption filter = new SqlOption(false);
            filter.Ignore.FilterTable = true;

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnNull1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnNull2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", ""), expected.Replace(" ", "").Replace("\r\n", ""), "Error");
        }

        [Description("Testea ALTER, CREATE Y DROP COLUMN de Tipos Formula."), TestMethod]
        public void TestColumnFormula()
        {
            string expected = Scripts.TestColumnFormula;
            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnFormula1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnFormula2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER y ADD COLUMN con Collations."), TestMethod]
        public void TestColumnCollation()
        {
            string expected = "USE TestCollate1\r\nGO\r\n\r\n";            
            expected += "ALTER TABLE [dbo].[TablaAlterCollation] ALTER COLUMN [Campo2] [varchar] (50) COLLATE Modern_Spanish_CI_AS NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaAlterCollation] ALTER COLUMN [Campo3] [varchar] (50) COLLATE Modern_Spanish_BIN2 NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaAddCollection] ADD\r\n";
            expected += "[Campo2] [nchar] (10) COLLATE Greek_CI_AS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaAlterCollation] ALTER COLUMN [Campo1] [varchar] (50) COLLATE Hebrew_CI_AS NOT NULL\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption(false);
            filter.Ignore.FilterTable = true;

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestCollate1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestCollate2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", ""), expected.Replace(" ", "").Replace("\r\n", ""), "Error");
        }

        [Description("Testea ALTER COLUMN de Tipos de Datos con tamaños variables."), TestMethod]
        public void TestColumnSize()
        {
            string expected = "USE TestOption1\r\nGO\r\n\r\n";            
            expected += "ALTER TABLE [dbo].[TablaColumnasSize1] ALTER COLUMN [Campo8] [nvarchar] (55) COLLATE Modern_Spanish_CI_AS NOT NULL\r\nGO\r\n";            
            expected += "ALTER TABLE [dbo].[TablaColumnasSize1] ALTER COLUMN [Campo10] [binary] (100) NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaColumnasSize1] ALTER COLUMN [Campo12] [real] NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaColumnasSize1] ALTER COLUMN [Campo7] [nchar] (20) COLLATE Modern_Spanish_CI_AS NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaColumnasSize1] ALTER COLUMN [Campo2] [varchar] (100) COLLATE Modern_Spanish_CI_AS NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaColumnasSize1] ALTER COLUMN [Campo4] [numeric] (18,10) NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaColumnasSize1] ALTER COLUMN [Campo6] [char] (20) COLLATE Modern_Spanish_CI_AS NOT NULL\r\nGO\r\n";
            
                                    
            string result;
            SqlOption filter = new SqlOption(false);
            filter.Ignore.FilterTable = true;

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestOption1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestOption2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", ""), expected.Replace(" ", "").Replace("\r\n", ""), "Error");
        }

        [Description("Testea CREATE y DROP de Triggers."), TestMethod]
        public void TestTriggers()
        {
            string expected = "USE TestOption1\r\nGO\r\n";
            expected += "DROP TRIGGER [dbo].[TablaTriggerAction2]\r\nGO\r\n";
            expected += "DROP TRIGGER [dbo].[TablaTriggerAction]\r\nGO\r\n";
            expected += "CREATE TRIGGER [dbo].[TablaTriggerAction]\r\n";
            expected += "   ON [dbo].[TablaTrigger]\r\n";
            expected += "   AFTER INSERT,DELETE,UPDATE\r\n";
            expected += "AS \r\n";
            expected += "BEGIN\r\n";
            expected += "	  SET NOCOUNT ON;\r\n";
            expected += "END;\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption(false);
            filter.Ignore.FilterTrigger = true;

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestOption1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestOption2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", ""), expected.Replace(" ", "").Replace("\r\n", ""), "Error");
        }

        [Description("Testea ENABLE y DISABLE de Triggers."), TestMethod]
        public void TestTriggersDisabled()
        {
            string expected = "USE TestTriggerDisabled1\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla2] DISABLE TRIGGER [TR2]\r\nGO\r\n";            
            expected += "ALTER TABLE [dbo].[Tabla1] ENABLE TRIGGER [TR1]\r\nGO\r\n";
            
            string result;
            SqlOption filter = new SqlOption(false);
            filter.Ignore.FilterTrigger = true;

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestTriggerDisabled1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestTriggerDisabled2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", ""), expected.Replace(" ", "").Replace("\r\n", ""), "Error");
        }

        [Description("Testea CREATE y DROP de Triggers de DDL Triggers."), TestMethod]
        public void TestTriggersDDL()
        {
            string expected = "USE TestDDLTriger1\r\nGO\r\n\r\n";
            expected += "DROP TRIGGER [TR3Login] ON DATABASE\r\nGO\r\n";
            expected += "DROP TRIGGER [TR1Login] ON DATABASE\r\nGO\r\n";
            expected += "CREATE TRIGGER [TR2Login] \r\n";
            expected += "ON DATABASE \r\n";
            expected += "FOR ALTER_TABLE \r\n \r\n";
            expected += "AS \r\n";
            expected += "\tPRINT 'Login Event Issued.'\r\nGO\r\n";
            expected += "CREATE TRIGGER [TR3Login] \r\n";
            expected += "ON DATABASE \r\n";
            expected += "FOR DROP_TABLE \r\n \r\n";
            expected += "AS \r\n";
            expected += "\tPRINT 'Login Event Issued.'\r\n\r\n\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDDLTriger1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDDLTriger2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD y DROP de User Data Types utilizados en PK."), TestMethod]
        public void TestDataTypePK()
        {
            string expected = Scripts.TestDataTypePK;

            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypePK1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypePK2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace("\r\n", ""), expected.Replace("\r\n", ""), "Error");
        }

        [Description("Testea ADD y DROP de User Data Types utilizados en tablas."), TestMethod]
        public void TestDataTypeUsed()
        {
            string expected = Scripts.TestDataTypeUsed;
            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypeUseTable1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypeUseTable2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace("\r\n", ""), expected.Replace("\r\n", ""), "Error");
        }

        [Description("Testea ADD y DROP de User Data Types utilizados en tablas con campos Formula."), TestMethod]
        public void TestDataTypeUsedFormula()
        {
            string expected = Scripts.TestDataTypeFormula;
            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypeFormula1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypeFormula2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD y DROP de User Data Types."), TestMethod]
        public void TestDataTypeSimple()
        {
            string expected = "USE TestDataTypeSimple1\r\nGO\r\n\r\n";

            expected += "CREATE TYPE [dbo].[TipoAdd2NotNull] FROM [text] NOT NULL\r\nGO\r\n";
            expected += "CREATE TYPE [dbo].[TipoAddNumeric] FROM [numeric] (15,15) NOT NULL\r\nGO\r\n";
            expected += "CREATE TYPE [dbo].[TipoAdd1] FROM [char](50) NULL\r\nGO\r\n";
            expected += "DROP TYPE [dbo].[TipoUpdateSize1]\r\nGO\r\n";
            expected += "CREATE TYPE [dbo].[TipoUpdateSize1] FROM [varchar](50) NOT NULL\r\nGO\r\n";                        
            expected += "DROP TYPE [dbo].[TipoDelete1]\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption(true);
            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypeSimple1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypeSimple2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result, expected, "Error");
        }

        [Description("Testea Bind y Unbind de User Data Types (default y rules)."), TestMethod]
        public void TestDataTypeBind()
        {
            string expected = "USE TestDataTypeBind1\r\nGO\r\n\r\n";
            expected += "EXEC sp_unbindrule @objname=N'TestRemoveRule'\r\nGO\r\n";
            expected += "EXEC sp_unbindefault @objname=N'TestAddDefault'\r\nGO\r\n";
            expected += "DROP RULE [dbo].[pattern_rule]\r\nGO\r\n";
            expected += "CREATE RULE [dbo].[pattern_rule] \r\nAS\r\n";
            expected += "@value LIKE '__-%[0-9]'\r\nGO\r\n";
            expected += "EXEC sp_bindefault N'2', N'TestRemoveDefault'\r\nGO\r\n";                        
            expected += "EXEC sp_bindrule N'pattern_rule', N'TestAddRule','futureonly'\r\nGO\r\n";
                        
            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypeBind1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypeBind2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result, expected, "Error");
        }

        [Description("Testea Table Options."), TestMethod]
        public void TestOptions()
        {
            string expected = "USE TestOption1\r\nGO\r\n\r\n";
            expected += "EXEC sp_tableoption TablaOpcion3, 'text in row','off'\r\nGO\r\n";
            expected += "EXEC sp_tableoption TablaOpcion3, 'vardecimal storage format','0'\r\nGO\r\n";
            expected += "EXEC sp_tableoption TablaOpcion2, 'text in row','off'\r\nGO\r\n";
            expected += "EXEC sp_tableoption TablaOpcion2, 'text in row',500\r\nGO\r\n";
            expected += "EXEC sp_tableoption TablaOpcion, 'text in row',500\r\nGO\r\n";
            expected += "EXEC sp_tableoption TablaOpcion, 'large value types out of row',1\r\nGO\r\n";
            expected += "EXEC sp_tableoption TablaOpcion, 'vardecimal storage format','1'\r\nGO\r\n";
            
            string result;
            SqlOption filter = new SqlOption(false);
            filter.Ignore.FilterTableOption = true;

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestOption1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestOption2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);
            
            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);
            
            result = origen.ToSQLDiff();

            Assert.AreEqual(result,expected,"Error");
        }

        [Description("Testea ALTER COLUMN a NOT NULL de campos Enteros."), TestMethod]
        public void TestColumnNotNullInteger()
        {
            string expected = Scripts.TestColumnNotNullEntero1;

            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnNotNullEntero1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnNotNullEntero2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER COLUMN a NOT NULL de campos Real/Float."), TestMethod]
        public void TestColumnNotNullReal()
        {
            string expected = Scripts.TestColumnNotNullReal1;
            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnNotNullReal1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnNotNullReal2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER COLUMN a NOT NULL de campos Text."), TestMethod]
        public void TestColumnNotNullText()
        {
            string expected = Scripts.TestColumnNotNullText1;

            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnNotNullText1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnNotNullText2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER COLUMN a NOT NULL de campos Date."), TestMethod]
        public void TestColumnNotNullDate()
        {
            string expected = Scripts.TestColumnNotNullDate1;

            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnNotNullDate1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestColumnNotNullDate2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER COLUMN de una Default Constraint."), TestMethod]
        public void TestConstraintDefaultAlterColumnSimple()
        {
            string expected = Scripts.TestConstraintDefaultAlterColumnSimple1;

            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintDefaultAlterColumnSimple1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintDefaultAlterColumnSimple2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER COLUMN y ADD y DROP de una Default Constraint."), TestMethod]
        public void TestConstraintDefaultAlterColumn()
        {
            string expected = Scripts.TestConstraintDefaultAlterColumn;

            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintDefaultAlterColumn1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintDefaultAlterColumn2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER COLUMN de una columna Computed con FK Constraint."), TestMethod]
        public void TestConstraintFKType()
        {
            string expected = Scripts.TestConstraintFKType;

            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintFKType1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintFKType2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER de una FK Constraint."), TestMethod]
        public void TestConstraintFKAlter()
        {
            string expected = "USE TestConstraintFKAlter1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[TablaHija] DROP CONSTRAINT [FK_TablaHija_Tabla1]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaHija] WITH NOCHECK ADD CONSTRAINT [FK_TablaHija_Tabla1] FOREIGN KEY\r\n\t(\r\n";
            expected += "\t\t[CampoID]\r\n";
            expected += "\t)\r\n";
            expected += "\tREFERENCES [dbo].[Tabla1]\r\n\t(\r\n";
            expected += "\t\t[ID]\r\n";
            expected += "\t) ON DELETE CASCADE ON UPDATE SET DEFAULT NOT FOR REPLICATION\r\nGO\r\n";


            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintFKAlter1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintFKAlter2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD/DROP/ALTER de una tabla con nombres largos."), TestMethod]
        public void TestNombreLargo()
        {
            string expected = Scripts.TestNombreLargo;

            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=Test Nombre Largo1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=Test Nombre Largo2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD/DROP de una tabla."), TestMethod]
        public void TestTableAddDrop()
        {
            string expected = Scripts.TestTableAdd;
            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestTableAdd1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestTableAdd2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER/ADD/DROP de Index."), TestMethod]
        public void TestIndex()
        {
            string expected = "";

            expected = Scripts.TestIndexSimple;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestIndexSimple1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestIndexSimple2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea DISABLED/REBUILD de Index."), TestMethod]
        public void TestIndexEnabled()
        {
            string expected = "";

            expected = Scripts.TestIndexEnabled;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestIndexEnabled1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestIndexEnabled2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER/ADD/DROP de Index con User Types modificados."), TestMethod]
        public void TestIndexUserType()
        {
            string expected = "";

            expected = Scripts.TestIndexuserType;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestIndexUserType1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestIndexUserType2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD de Index XML."), TestMethod]
        public void TestIndexXML()
        {
            string expected = "";

            expected = Scripts.TestIndexXML;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestIndexXML1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestIndexXML2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER VIEW con SCHEMABINDING."), TestMethod]
        public void TestViewSchemaBinding()
        {
            string expected = "";

            expected = Scripts.TestViewSchemaBinding;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestView1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestView2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD/DROP VIEW."), TestMethod]
        public void TestViewComun()
        {
            string expected = "";

            expected = Scripts.TestViewComun;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestViewComun1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestViewComun2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea DROP Table dependientes con FK."), TestMethod]
        public void TestConstraintFKRemove()
        {
            string expected = "";

            expected = Scripts.TestConstraintFKRemove;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintFKRemoveTable1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintFKRemoveTable2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea DROP Table con UserDataTypes modificados"), TestMethod]
        public void TestDataTypeDropTable()
        {
            string expected = "";

            expected = Scripts.TestDataTypeDropTable;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypeDropTable1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypeDropTable2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea DROP Column con UserDataTypes modificados"), TestMethod]
        public void TestDataTypeDropColumn()
        {
            string expected = "";

            expected = Scripts.TestDataTypeDeleteColumn;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypeDeleteColumn1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypeDeleteColumn2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER TABLES en tablas con FK circulares"), TestMethod]
        public void TestConstraintFKCircular()
        {
            string expected = "";

            expected = Scripts.TestConstratinFKCircular;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintFKCircular1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestConstraintFKCircular2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD/DROP de SCHEMAS"), TestMethod]
        public void TestSchema()
        {
            string expected = "";

            expected = Scripts.TestSchema;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestSchema1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestSchema2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }


        [Description("Testea ADD/ALTER/DROP de Functiones simples"), TestMethod]
        public void TestFunctionSimple()
        {
            string expected = "";

            expected = Scripts.TestFunctionSimple;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestFunctionSimple1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestFunctionSimple2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD/ALTER/DROP de Functiones con distintos Owners (Schemas)"), TestMethod]
        public void TestFunctionOwner()
        {
            string expected = "";

            expected = Scripts.TestFunctionOwner;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestFunctionOwner1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestFunctionOwner2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER de tablas con Functiones con SCHEMABINDING"), TestMethod]
        public void TestFunctionSchema()
        {
            string expected = "";

            expected = Scripts.TestFunctionSchemaBound1;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestFunctionSchemaBound1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestFunctionSchemaBound2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD/ALTER/DROP de tablas y Functiones con SCHEMABINDING"), TestMethod]
        public void TestFunctionSchemaChange()
        {
            string expected = "";

            expected = Scripts.TestFunctionSchemaBoundChange1;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestFunctionSchemaBoundChange1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestFunctionSchemaBoundChange2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER de tablas con columnas con el mismo nombre, pero distino el mayuscula/minisculas"), TestMethod]
        public void TestTableChangeAll()
        {
            string expected = "";

            expected = Scripts.TestTableChangeAll;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestTableChangeAll1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestTableChangeAll2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }
        [Description("Testea Aladia contra ISMM"), TestMethod]
        public void TestAladiaISMM()
        {
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=ISMM;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=Aladia30;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            string result = origen.ToSQLDiff();

            Assert.AreEqual("","", "Error");
        }

        [Description("Testea ALTER de Functiones con teniendo en cuenta comentarios y fin de lineas"), TestMethod]
        public void TestFunctionEnter()
        {
            string expected = "";

            expected = Scripts.TestFuncionEnter;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestFunctionEnter1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestFunctionEnter2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER, CREATE y DROP de Functiones con teniendo en cuenta dependencias y schema binding"), TestMethod]
        public void TestFunctionSchemaBoundMultiple()
        {
            string expected = "";

            expected = Scripts.TestFunctionSchemaBoundMultiple1;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestFunctionSchemaBoundMultiple1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestFunctionSchemaBoundMultiple2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER, CREATE y DROP de XML Schemas"), TestMethod]
        public void TestXMLSchema()
        {
            string expected = "";

            expected = Scripts.TestXMLSchema;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestXMLSchema1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestXMLSchema2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER de UserDataType con Vistas"), TestMethod]
        public void TestDataTypeView()
        {
            string expected = "";

            expected = Scripts.TestDataTypeView;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypeView1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypeView2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER de UserDataType con Functiones"), TestMethod]
        public void TestDataTypeFunction()
        {
            string expected = "";

            expected = Scripts.TestDataTypeFunction;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypeFunction1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypeFunction2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD/DROP de UserDataType con Tablas con Identitys"), TestMethod]
        public void TestDataTypeRebuild()
        {
            string expected = "";

            expected = Scripts.TestDataTypeRebuild;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypeRebuild1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypeRebuild2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER de Columnas con Defaults Constraint y UserDataType"), TestMethod]
        public void TestDataTypeDefaultSimple()
        {
            string expected = "";

            expected = Scripts.TestDataTypeDefaultSimple;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypeDefaultSimple1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypeDefaultSimple2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER/ADD/DROP de Columnas con Defaults Constraint, Store, Funciones, Triggers y UserDataType"), TestMethod]
        public void TestDataTypeDefaultComplex()
        {
            string expected = "";

            expected = Scripts.TestDataTypeDefaultComplex;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypeDefaultComplex1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestDataTypeDefaultComplex2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER de Funciones con Check Coinstraint"), TestMethod]
        public void TestFunctionConstraintCheck()
        {
            string expected = "";

            expected = Scripts.TestFunctionConstraintCheck;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestFunctionConstraintCheck1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestFunctionConstraintCheck2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER de Tabla con funciones asociadas"), TestMethod]
        public void TestFunctionSchemaBoundTable()
        {
            string expected = "";

            expected = Scripts.TestFunctionSchemaBoundTable;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestFunctionSchemaBoundTable1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestFunctionSchemaBoundTable2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea UDTs"), TestMethod]
        public void TestUDT()
        {
            string expected = "";

            expected = Scripts.TestUDT;
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestUDT1;Data Source=" + DATABASE_NAME;
            origen = sql.Process(filter);

            sql.ConnectionString = @"Persist Security Info=False;Initial Catalog=TestUDT2;Data Source=" + DATABASE_NAME;
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }
    }
}
