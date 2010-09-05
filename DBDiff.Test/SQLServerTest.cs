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
            conn2.ConnectionString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=AFULL;Data Source=(LOCAL)";
            conn2.Open();
            com1.Connection = conn2;
            com2.Connection = conn2;
            com1.ExecuteNonQuery();
            com2.ExecuteNonQuery();
            com1.Dispose();
            com2.Dispose();
            conn2.Close();
            
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=AFULL;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            com1 = new SqlCommand("DBCC DROPCLEANBUFFERS");
            com2 = new SqlCommand("DBCC FREEPROCCACHE");
            conn2.ConnectionString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=AFULL;Data Source=(LOCAL)";
            conn2.Open();
            com1.Connection = conn2;
            com2.Connection = conn2;
            com1.ExecuteNonQuery();
            com2.ExecuteNonQuery();
            com1.Dispose();
            com2.Dispose();
            conn2.Close();

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=AFULL;Data Source=(LOCAL)";
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
            expected += "ALTER TABLE [dbo].[Tabla1] CHECK CONSTRAINT [CK_Tabla1]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla2] NOCHECK CONSTRAINT [CK_Tabla2]\r\nGO\r\n";            
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintCheckEnabled1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintCheckEnabled2;Data Source=(LOCAL)";
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
            expected += "CREATE TABLE [dbo].[TempTabla3]\r\n(\r\n";
            expected += "\t[ID] [int] NOT NULL,\r\n";
            expected += "\t[Campo0] [int] NOT NULL,\r\n";
            expected += "\t[Campo1] [int] NOT NULL,\r\n";
            expected += "\t[Campo2] [nchar] (10) COLLATE Modern_Spanish_CI_AS NULL\r\n";
            expected += ") ON [PRIMARY]\r\nGO\r\n\r\n";
            expected += "INSERT INTO [dbo].[TempTabla3] ([ID],[Campo1],[Campo2],[Campo0]) SELECT [ID],[Campo1],[Campo2],0 FROM [dbo].[Tabla3]\r\n";
            expected += "DROP TABLE [dbo].[Tabla3]\r\nGO\r\n";
            expected += "EXEC sp_rename N'[dbo].[TempTabla3]',N'Tabla3', 'OBJECT'\r\nGO\r\n\r\n";
            expected += "CREATE TABLE [dbo].[TempTabla1]\r\n(\r\n";
            expected += "\t[ID] [int] NOT NULL,\r\n";
            expected += "\t[Campo1] [int] NOT NULL,\r\n";
            expected += "\t[Campo3] [int] NOT NULL,\r\n";
            expected += "\t[Campo2] [int] NOT NULL\r\n";
            expected += ") ON [PRIMARY]\r\nGO\r\n\r\n";
            expected += "INSERT INTO [dbo].[TempTabla1] ([ID],[Campo1],[Campo2],[Campo3]) SELECT [ID],[Campo1],[Campo2],[Campo3] FROM [dbo].[Tabla1]\r\n";
            expected += "DROP TABLE [dbo].[Tabla1]\r\nGO\r\n";
            expected += "EXEC sp_rename N'[dbo].[TempTabla1]',N'Tabla1', 'OBJECT'\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[Tabla3] ADD CONSTRAINT [PK_Tabla3] PRIMARY KEY CLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID] ASC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnPosition1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnPosition2;Data Source=(LOCAL)";
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
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnPositionMultiple1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnPositionMultiple2;Data Source=(LOCAL)";
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
            expected += "ALTER TABLE [dbo].[TableAlterCheck] WITH NOCHECK ADD CONSTRAINT [CK_TableAlterCheck_12] CHECK NOT FOR REPLICATION (([CampoCheck5]>(1)))\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TableAlterCheck] ADD CONSTRAINT [CK_TableAlterCheck] CHECK  (([CampoCheck]>(2)))\r\nGO\r\n";                        
            expected += "ALTER TABLE [dbo].[TableAlterCheck] NOCHECK CONSTRAINT [CK_TableAlterCheck_t]\r\nGO\r\n";
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintCheck1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintCheck2;Data Source=(LOCAL)";
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
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintDefaultPK1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintDefaultPK2;Data Source=(LOCAL)";
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
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintCheckRebuil1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintCheckRebuil2;Data Source=(LOCAL)";
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
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintDefaultRebuild1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintDefaultRebuild2;Data Source=(LOCAL)";
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
            expected += "ALTER TABLE [dbo].[Tabla1] ADD CONSTRAINT [DF_Tabla1_Campo1] DEFAULT ('HolaAAAAAA') FOR [Campo1]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ADD CONSTRAINT [DF_Tabla1_Campo3] DEFAULT ('Hola2') FOR [Campo3]\r\nGO\r\n";
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintDefaultSimple1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintDefaultSimple2;Data Source=(LOCAL)";
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
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintDefaultAddColumn1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintDefaultAddColumn2;Data Source=(LOCAL)";
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
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestSinonimos1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestSinonimos2;Data Source=(LOCAL)";
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
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintFKTree1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintFKTree2;Data Source=(LOCAL)";
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea Enabled/Disabled de Constraint Foreign Key."), TestMethod]
        public void TestConstraintFKEnabled()
        {
            string expected = "";
            expected += "USE TestConstraintFKDisable1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[TablaHija] DROP CONSTRAINT [FK_TablaHija_Tabla1]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] DROP CONSTRAINT [FK_Habilitar]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaHija] WITH NOCHECK ADD CONSTRAINT [FK_TablaHija_Tabla1] FOREIGN KEY\r\n\t(\r\n";
            expected += "\t\t[Tabla1Id]\r\n\t)\r\n";
            expected += "\tREFERENCES [dbo].[Tabla1]\r\n\t(\r\n";
            expected += "\t\t[ID]\r\n";
            expected += "\t) NOT FOR REPLICATION\r\nGO\r\n";

            expected += "ALTER TABLE [dbo].[Tabla1] ADD CONSTRAINT [FK_Habilitar] FOREIGN KEY\r\n\t(\r\n";
            expected += "\t\t[ID3]\r\n\t)\r\n";
            expected += "\tREFERENCES [dbo].[Tabla1]\r\n\t(\r\n";
            expected += "\t\t[ID]\r\n";
            expected += "\t)\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaHija] NOCHECK CONSTRAINT [FK_TablaHija_Tabla1]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] CHECK CONSTRAINT [FK_Habilitar]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] NOCHECK CONSTRAINT [FK_Deshabilitar]\r\nGO\r\n";            

            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintFKDisable1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintFKDisable2;Data Source=(LOCAL)";
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
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintFKSimple1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintFKSimple2;Data Source=(LOCAL)";
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER COLUMN y ADD y DROP de una Check Constraint."), TestMethod]
        public void TestConstraintCheckAlterColumn()
        {
            string expected = "";
            expected += "USE TestConstraintCheckAlterColumn1\r\nGO\r\n\r\n";            
            expected += "ALTER TABLE [dbo].[Tabla2] DROP CONSTRAINT [CK_Tabla2]\r\nGO\r\n";            
            expected += "ALTER TABLE [dbo].[Tabla1] DROP CONSTRAINT [CK_Tabla1]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla2] DROP CONSTRAINT [CK_Tabla2_1]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla2] DROP CONSTRAINT [PK_Tabla2]\r\nGO\r\n";            
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo1] [smallint] NOT NULL\r\nGO\r\n";
            expected += "CREATE TABLE [dbo].[TempTabla2]\r\n(\r\n";
            expected += "\t[ID] [int] NOT NULL,\r\n";
            expected += "\t[Campo1] [smallint] NOT NULL,\r\n";
            expected += "\t[Campo2] [int] NOT NULL,\r\n";
            expected += "\t[Campo3] AS (([Campo1]+[Campo2])+(1)) PERSISTED\r\n";
            expected += ") ON [PRIMARY]\r\nGO\r\n\r\n";
            expected += "INSERT INTO [dbo].[TempTabla2] ([ID],[Campo1],[Campo2]) SELECT [ID],[Campo1],[Campo2] FROM [dbo].[Tabla2]\r\n";
            expected += "DROP TABLE [dbo].[Tabla2]\r\nGO\r\n";
            expected += "EXEC sp_rename N'[dbo].[TempTabla2]',N'Tabla2', 'OBJECT'\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[Tabla2] ADD CONSTRAINT [CK_Tabla2_1] CHECK  (([Campo3]>(9)))\r\nGO\r\n";            
            expected += "ALTER TABLE [dbo].[Tabla1] ADD CONSTRAINT [CK_Tabla1] CHECK  (([Campo1]>(3)))\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla2] ADD CONSTRAINT [CK_Tabla2] CHECK  (([Campo1]>(4)))\r\nGO\r\n";            
            expected += "ALTER TABLE [dbo].[Tabla2] ADD CONSTRAINT [PK_Tabla2] PRIMARY KEY CLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID] ASC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";
            
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintCheckAlterColumn1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintCheckAlterColumn2;Data Source=(LOCAL)";
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
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintPKType1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintPKType2;Data Source=(LOCAL)";
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
            expected += "ALTER TABLE [dbo].[TablaAddPK] ADD CONSTRAINT [PK_TablaAddPK] PRIMARY KEY CLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID] ASC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaAlterPK] ADD CONSTRAINT [PK_TablaAlterPK] PRIMARY KEY CLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID] DESC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintPK1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintPK2;Data Source=(LOCAL)";
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
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestTableFileGroup1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestTableFileGroup2;Data Source=(LOCAL)";
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
            expected += "ADD FILE ( NAME = N'Test', FILENAME = N'C:\\Program Files\\Microsoft SQL Server\\MSSQL.1\\MSSQL\\DATA\\TestFileGroupSimple1_DB.ndf' , SIZE = 384000KB , FILEGROWTH = 128000KB) TO FILEGROUP [SECONDARY]\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestFileGroupSimple1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestFileGroupSimple2;Data Source=(LOCAL)";
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
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestTableFileGroupText1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestTableFileGroupText2;Data Source=(LOCAL)";
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
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnTimestamp1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnTimestamp2;Data Source=(LOCAL)";
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD,ALTER Y DROP COLUMN de columas Identity."), TestMethod]
        public void TestColumnIdentity()
        {
            string expected = "USE TestColumnIdentity1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[TablaAddIdentity1] DROP CONSTRAINT [PK_TablaAddIdentity1]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaAlterIdentityNotForReplication] DROP CONSTRAINT [PK_TablaAlterIdentityNotForReplication]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaDropIdentity1] DROP CONSTRAINT [PK_TablaDropIdentity]\r\nGO\r\n";                                    
            expected += "ALTER TABLE [dbo].[TablaDropColumnIdentity1] DROP COLUMN [ID]\r\nGO\r\n";
            expected += "CREATE TABLE [dbo].[TempTablaAddColumnIdentity1]\r\n(\r\n";
            expected += "\t[ID] [int] IDENTITY (1,1) NOT NULL,\r\n";
            expected += "\t[Campo2] [varchar] (50) COLLATE Modern_Spanish_CI_AS NULL\r\n) ON [PRIMARY]\r\nGO\r\n\r\n";
            expected += "SET IDENTITY_INSERT [dbo].[TempTablaAddColumnIdentity1] ON\r\n";
            expected += "INSERT INTO [dbo].[TempTablaAddColumnIdentity1] ([Campo2],[ID]) SELECT [Campo2],0 FROM [dbo].[TablaAddColumnIdentity1]\r\n";
            expected += "SET IDENTITY_INSERT [dbo].[TempTablaAddColumnIdentity1] OFF\r\nGO\r\n\r\n";
            expected += "DROP TABLE [dbo].[TablaAddColumnIdentity1]\r\nGO\r\n";
            expected += "EXEC sp_rename N'[dbo].[TempTablaAddColumnIdentity1]',N'TablaAddColumnIdentity1', 'OBJECT'\r\nGO\r\n\r\n";
            expected += "CREATE TABLE [dbo].[TempTablaAlterIdentityNotForReplication]\r\n(\r\n";
            expected += "\t[ID] [int] IDENTITY (1,1) NOT FOR REPLICATION NOT NULL,\r\n";
            expected += "\t[Campo2] [varchar] (50) COLLATE Modern_Spanish_CI_AS NULL,\r\n";
            expected += "\t[Campo3] [varchar] (50) COLLATE Modern_Spanish_CI_AS NULL\r\n) ON [PRIMARY]\r\nGO\r\n\r\n";
            expected += "SET IDENTITY_INSERT [dbo].[TempTablaAlterIdentityNotForReplication] ON\r\n";
            expected += "INSERT INTO [dbo].[TempTablaAlterIdentityNotForReplication] ([ID],[Campo2],[Campo3]) SELECT [ID],[Campo2],[Campo3] FROM [dbo].[TablaAlterIdentityNotForReplication]\r\n";
            expected += "SET IDENTITY_INSERT [dbo].[TempTablaAlterIdentityNotForReplication] OFF\r\nGO\r\n\r\n";
            expected += "DROP TABLE [dbo].[TablaAlterIdentityNotForReplication]\r\nGO\r\n";
            expected += "EXEC sp_rename N'[dbo].[TempTablaAlterIdentityNotForReplication]',N'TablaAlterIdentityNotForReplication', 'OBJECT'\r\nGO\r\n\r\n";
            expected += "CREATE TABLE [dbo].[TempTablaDropIdentity1]\r\n(\r\n";
            expected += "\t[ID] [int] NOT NULL,\r\n";
            expected += "\t[Campo] [varchar] (50) COLLATE Modern_Spanish_CI_AS NULL\r\n) ON [PRIMARY]\r\nGO\r\n\r\n";
            expected += "INSERT INTO [dbo].[TempTablaDropIdentity1] ([ID],[Campo]) SELECT [ID],[Campo] FROM [dbo].[TablaDropIdentity1]\r\n";
            expected += "DROP TABLE [dbo].[TablaDropIdentity1]\r\nGO\r\n";
            expected += "EXEC sp_rename N'[dbo].[TempTablaDropIdentity1]',N'TablaDropIdentity1', 'OBJECT'\r\nGO\r\n\r\n";
            expected += "CREATE TABLE [dbo].[TempTablaAddIdentity1]\r\n(\r\n";
            expected += "\t[ID] [int] IDENTITY (1,1) NOT NULL,\r\n";
            expected += "\t[Campo2] [varchar] (50) COLLATE Modern_Spanish_CI_AS NULL\r\n) ON [PRIMARY]\r\nGO\r\n\r\n";
            expected += "SET IDENTITY_INSERT [dbo].[TempTablaAddIdentity1] ON\r\n";
            expected += "INSERT INTO [dbo].[TempTablaAddIdentity1] ([ID],[Campo2]) SELECT [ID],[Campo2] FROM [dbo].[TablaAddIdentity1]\r\n";
            expected += "SET IDENTITY_INSERT [dbo].[TempTablaAddIdentity1] OFF\r\nGO\r\n\r\n";
            expected += "DROP TABLE [dbo].[TablaAddIdentity1]\r\nGO\r\n";
            expected += "EXEC sp_rename N'[dbo].[TempTablaAddIdentity1]',N'TablaAddIdentity1', 'OBJECT'\r\nGO\r\n\r\n";            
            expected += "ALTER TABLE [dbo].[TablaDropIdentity1] ADD CONSTRAINT [PK_TablaDropIdentity] PRIMARY KEY CLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID] ASC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaAlterIdentityNotForReplication] ADD CONSTRAINT [PK_TablaAlterIdentityNotForReplication] PRIMARY KEY CLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID] ASC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaAddIdentity1] ADD CONSTRAINT [PK_TablaAddIdentity1] PRIMARY KEY CLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID] ASC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";                                    
            string result;
            SqlOption filter = new SqlOption();

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnIdentity1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnIdentity2;Data Source=(LOCAL)";
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
            filter.OptionFilter.FilterTable = true;

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnXML1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnXML2;Data Source=(LOCAL)";
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
            filter.OptionFilter.FilterTable = true;

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnAdd1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnAdd2;Data Source=(LOCAL)";
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
            filter.OptionFilter.FilterTable = true;

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnRowGuid1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnRowGuid2;Data Source=(LOCAL)";
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
            expected += "ALTER TABLE [dbo].[TablaColumnasNull] ALTER COLUMN [Campo1] [varbinary] (50) NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaColumnasNull] ALTER COLUMN [Campo2] [nchar] (10) COLLATE Modern_Spanish_CI_AS NULL\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption(false);
            filter.OptionFilter.FilterTable = true;

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnNull1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnNull2;Data Source=(LOCAL)";
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", ""), expected.Replace(" ", "").Replace("\r\n", ""), "Error");
        }

        [Description("Testea ALTER, CREATE Y DROP COLUMN de Tipos Formula."), TestMethod]
        public void TestColumnFormula()
        {
            string expected = "USE TestColumnFormula1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[TablaFormula4] DROP CONSTRAINT [PK_TablaFormula4]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaFormulaPersisted] DROP CONSTRAINT [PK_TablaFormulaPersisted]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaDropFormula1] DROP COLUMN [CampoFormula1],[CampoFormula2]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaFormula2] ADD\r\n";
            expected += "[CampoFormula1] AS (([Campo1]+[Campo2])+'d') PERSISTED,\r\n";
            expected += "[CampoFormula2] AS ([ID]+(1))\r\nGO\r\n";
            expected += "CREATE TABLE [dbo].[TempTablaFormulaPersisted]\r\n(\r\n";
            expected += "\t[ID] [int] IDENTITY (1,1) NOT NULL,\r\n";
            expected += "\t[Campo1] [varchar] (50) COLLATE Modern_Spanish_CI_AS NOT NULL,\r\n";
            expected += "\t[Campo2] AS ([Campo1]+(1)) PERSISTED\r\n) ON [PRIMARY]\r\nGO\r\n\r\n";
            expected += "SET IDENTITY_INSERT [dbo].[TempTablaFormulaPersisted] ON\r\n";
            expected += "INSERT INTO [dbo].[TempTablaFormulaPersisted] ([ID],[Campo1]) SELECT [ID],[Campo1] FROM [dbo].[TablaFormulaPersisted]\r\n";
            expected += "SET IDENTITY_INSERT [dbo].[TempTablaFormulaPersisted] OFF\r\nGO\r\n\r\n";
            expected += "DROP TABLE [dbo].[TablaFormulaPersisted]\r\nGO\r\n";
            expected += "EXEC sp_rename N'[dbo].[TempTablaFormulaPersisted]',N'TablaFormulaPersisted', 'OBJECT'\r\nGO\r\n\r\n";

            expected += "CREATE TABLE [dbo].[TempTablaFormula4]\r\n(\r\n";
            expected += "\t[ID] [int] NOT NULL,\r\n";
            expected += "\t[Campo1] [char] (10) COLLATE Modern_Spanish_CI_AS NOT NULL,\r\n";
            expected += "\t[Campo2] AS ([Campo1]+(1))\r\n) ON [PRIMARY]\r\nGO\r\n\r\n";
            expected += "INSERT INTO [dbo].[TempTablaFormula4] ([ID],[Campo1]) SELECT [ID],[Campo1] FROM [dbo].[TablaFormula4]\r\n";
            expected += "DROP TABLE [dbo].[TablaFormula4]\r\nGO\r\n";
            expected += "EXEC sp_rename N'[dbo].[TempTablaFormula4]',N'TablaFormula4', 'OBJECT'\r\nGO\r\n\r\n";
            

            expected += "CREATE TABLE [dbo].[TempTablaFormula3]\r\n(\r\n";
            expected += "\t[ID] [int] NOT NULL,\r\n";
            expected += "\t[Campo1] [varchar] (50) COLLATE Modern_Spanish_CI_AS NOT NULL,\r\n";
            expected += "\t[Campo2] [varchar] (50) COLLATE Modern_Spanish_CI_AS NOT NULL,\r\n";
            expected += "\t[CampoFormula1] AS (([Campo1]+[Campo2])+'d') PERSISTED,\r\n";
            expected += "\t[CampoFormula2] AS ([ID]+(2))\r\n) ON [PRIMARY]\r\nGO\r\n\r\n";
            expected += "INSERT INTO [dbo].[TempTablaFormula3] ([ID],[Campo1],[Campo2]) SELECT [ID],[Campo1],[Campo2] FROM [dbo].[TablaFormula3]\r\n";
            expected += "DROP TABLE [dbo].[TablaFormula3]\r\nGO\r\n";
            expected += "EXEC sp_rename N'[dbo].[TempTablaFormula3]',N'TablaFormula3', 'OBJECT'\r\nGO\r\n\r\n";

            expected += "ALTER TABLE [dbo].[TablaFormulaPersisted] ADD CONSTRAINT [PK_TablaFormulaPersisted] PRIMARY KEY CLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID] ASC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";

            expected += "ALTER TABLE [dbo].[TablaFormula4] ADD CONSTRAINT [PK_TablaFormula4] PRIMARY KEY CLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID] ASC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";


            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnFormula1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnFormula2;Data Source=(LOCAL)";
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ALTER y ADD COLUMN con Collations."), TestMethod]
        public void TestColumnCollation()
        {
            string expected = "USE TestCollate1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[TablaAddCollection] ADD\r\n";
            expected += "[Campo2] [nchar] (10) COLLATE Greek_CI_AS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaAlterCollation] ALTER COLUMN [Campo1] [varchar] (50) COLLATE Hebrew_CI_AS NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaAlterCollation] ALTER COLUMN [Campo2] [varchar] (50) COLLATE Modern_Spanish_CI_AS NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaAlterCollation] ALTER COLUMN [Campo3] [varchar] (50) COLLATE Modern_Spanish_BIN2 NOT NULL\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption(false);
            filter.OptionFilter.FilterTable = true;

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestCollate1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestCollate2;Data Source=(LOCAL)";
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", ""), expected.Replace(" ", "").Replace("\r\n", ""), "Error");
        }

        [Description("Testea ALTER COLUMN de Tipos de Datos con tamaños variables."), TestMethod]
        public void TestColumnSize()
        {
            string expected = "USE TestOption1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[TablaColumnasSize1] ALTER COLUMN [Campo2] [varchar] (100) COLLATE Modern_Spanish_CI_AS NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaColumnasSize1] ALTER COLUMN [Campo4] [numeric] (18,10) NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaColumnasSize1] ALTER COLUMN [Campo6] [char] (20) COLLATE Modern_Spanish_CI_AS NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaColumnasSize1] ALTER COLUMN [Campo7] [nchar] (20) COLLATE Modern_Spanish_CI_AS NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaColumnasSize1] ALTER COLUMN [Campo8] [nvarchar] (55) COLLATE Modern_Spanish_CI_AS NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaColumnasSize1] ALTER COLUMN [Campo10] [binary] (100) NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaColumnasSize1] ALTER COLUMN [Campo12] [real] NULL\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption(false);
            filter.OptionFilter.FilterTable = true;

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestOption1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestOption2;Data Source=(LOCAL)";
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
            filter.OptionFilter.FilterTrigger = true;

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestOption1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestOption2;Data Source=(LOCAL)";
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", ""), expected.Replace(" ", "").Replace("\r\n", ""), "Error");
        }

        [Description("Testea ENABLE y DISABLE de Triggers."), TestMethod]
        public void TestTriggersDisabled()
        {
            string expected = "USE TestTriggerDisabled1\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ENABLE TRIGGER [TR1]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla2] DISABLE TRIGGER [TR2]\r\nGO\r\n";            

            string result;
            SqlOption filter = new SqlOption(false);
            filter.OptionFilter.FilterTrigger = true;

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestTriggerDisabled1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestTriggerDisabled2;Data Source=(LOCAL)";
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
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestDDLTriger1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestDDLTriger2;Data Source=(LOCAL)";
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD y DROP de User Data Types utilizados en PK."), TestMethod]
        public void TestDataTypePK()
        {
            string expected = "USE TestDataTypePK1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] DROP CONSTRAINT [PK_Tabla1]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [ID] [int] NOT NULL\r\nGO\r\n";
            expected += "DROP TYPE [dbo].[PepeKey]\r\nGO\r\n";
            expected += "CREATE TYPE [dbo].[PepeKey] FROM [int] NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [ID] [PepeKey] NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ADD CONSTRAINT [PK_Tabla1] PRIMARY KEY CLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID] ASC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestDataTypePK1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestDataTypePK2;Data Source=(LOCAL)";
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result, expected, "Error");
        }

        [Description("Testea ADD y DROP de User Data Types utilizados en tablas."), TestMethod]
        public void TestDataTypeUsed()
        {
            string expected = "USE TestDataTypeUseTable1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[TablaChangeRebuild1] DROP CONSTRAINT [PK_TablaChangeRebuild1]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaChangeRebuild1] ALTER COLUMN [ID] [int] NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaChange1] ALTER COLUMN [Campo2] [varchar] (50) NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaChange2] ALTER COLUMN [Campo1] [varchar] (50) NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaChange2] ALTER COLUMN [Campo2] [varchar] (50) NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaChangeRebuild1] ALTER COLUMN [Campo3] [varchar] (50) NOT NULL\r\nGO\r\n";
            expected += "DROP TYPE [dbo].[Primario]\r\nGO\r\n";
            expected += "CREATE TYPE [dbo].[Primario] FROM [int] NULL\r\nGO\r\n";
            expected += "DROP TYPE [dbo].[Tipo1]\r\nGO\r\n";
            expected += "CREATE TYPE [dbo].[Tipo1] FROM [varchar](50) NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaChange2] ALTER COLUMN [Campo1] [Tipo1] NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaChange2] ALTER COLUMN [Campo2] [Tipo1] NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaChangeRebuild1] ALTER COLUMN [ID] [Primario] NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaChangeRebuild1] ALTER COLUMN [Campo3] [Tipo1] NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaChange1] ALTER COLUMN [Campo2] [Tipo1] NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[TablaChangeRebuild1] ADD CONSTRAINT [PK_TablaChangeRebuild1] PRIMARY KEY CLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID] ASC,\r\n";
            expected += "\t\t[Campo2] ASC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestDataTypeUseTable1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestDataTypeUseTable2;Data Source=(LOCAL)";
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result, expected, "Error");
        }

        [Description("Testea ADD y DROP de User Data Types utilizados en tablas con campos Formula."), TestMethod]
        public void TestDataTypeUsedFormula()
        {
            string expected = "USE TestDataTypeFormula1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] DROP COLUMN [CampoFormula]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo1] [varchar] (100) NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo2] [char] (10) NOT NULL\r\nGO\r\n";
            expected += "DROP TYPE [dbo].[Tipo1]\r\nGO\r\n";
            expected += "CREATE TYPE [dbo].[Tipo1] FROM [varchar](100) NOT NULL\r\nGO\r\n";
            expected += "DROP TYPE [dbo].[Tipo2]\r\nGO\r\n";
            expected += "CREATE TYPE [dbo].[Tipo2] FROM [char](10) NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo1] [Tipo1] NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo2] [Tipo2] NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ADD\r\n[CampoFormula] AS ([Campo1]+[Campo2]) PERSISTED\r\nGO\r\n";
            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestDataTypeFormula1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestDataTypeFormula2;Data Source=(LOCAL)";
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD y DROP de User Data Types."), TestMethod]
        public void TestDataTypeSimple()
        {
            string expected = "USE TestDataTypeSimple1\r\nGO\r\n\r\n";
            
            expected += "CREATE TYPE [dbo].[TipoAddNumeric] FROM [numeric] (15,15) NOT NULL\r\nGO\r\n";            
            expected += "DROP TYPE [dbo].[TipoUpdateSize1]\r\nGO\r\n";
            expected += "CREATE TYPE [dbo].[TipoUpdateSize1] FROM [varchar](50) NOT NULL\r\nGO\r\n";
            expected += "CREATE TYPE [dbo].[TipoAdd2NotNull] FROM [text] NOT NULL\r\nGO\r\n";
            expected += "CREATE TYPE [dbo].[TipoAdd1] FROM [char](50) NULL\r\nGO\r\n";
            expected += "DROP TYPE [dbo].[TipoDelete1]\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption(false);
            filter.OptionFilter.FilterTableOption = true;

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestDataTypeSimple1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestDataTypeSimple2;Data Source=(LOCAL)";
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
            expected += "CREATE RULE pattern_rule \r\nAS\r\n";
            expected += "@value LIKE '__-%[0-9]'\r\nGO\r\n";
            expected += "EXEC sp_bindefault N'2', N'TestRemoveDefault'\r\nGO\r\n";                        
            expected += "EXEC sp_bindrule N'pattern_rule', N'TestAddRule','futureonly'\r\nGO\r\n";
                        
            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestDataTypeBind1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestDataTypeBind2;Data Source=(LOCAL)";
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result, expected, "Error");
        }

        [Description("Testea Table Options."), TestMethod]
        public void TestOptions()
        {
            string expected = "USE TestOption1\r\nGO\r\n\r\n";
            expected += "EXEC sp_tableoption TablaOpcion2, 'text in row','off'\r\nGO\r\n";
            expected += "EXEC sp_tableoption TablaOpcion2, 'text in row',500\r\nGO\r\n";
            expected += "EXEC sp_tableoption TablaOpcion3, 'text in row','off'\r\nGO\r\n";
            expected += "EXEC sp_tableoption TablaOpcion3, 'vardecimal storage format','0'\r\nGO\r\n";
            expected += "EXEC sp_tableoption TablaOpcion, 'text in row',500\r\nGO\r\n";
            expected += "EXEC sp_tableoption TablaOpcion, 'large value types out of row',1\r\nGO\r\n";
            expected += "EXEC sp_tableoption TablaOpcion, 'vardecimal storage format','1'\r\nGO\r\n";
            
            string result;
            SqlOption filter = new SqlOption(false);
            filter.OptionFilter.FilterTableOption = true;

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestOption1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestOption2;Data Source=(LOCAL)";
            destino = sql.Process(filter);
            
            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);
            
            result = origen.ToSQLDiff();

            Assert.AreEqual(result,expected,"Error");
        }

        [Description("Testea ALTER COLUMN a NOT NULL de campos Enteros."), TestMethod]
        public void TestColumnNotNullInteger()
        {
            string expected = "USE TestColumnNotNullEntero1\r\nGO\r\n\r\n";
            expected += "UPDATE [dbo].[Tabla1] SET [Campo1] = 0 WHERE [Campo1] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo1] [int] NOT NULL\r\nGO\r\n";
            expected += "UPDATE [dbo].[Tabla1] SET [Campo2] = 0 WHERE [Campo2] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo2] [tinyint] NOT NULL\r\nGO\r\n";
            expected += "UPDATE [dbo].[Tabla1] SET [Campo3] = 0 WHERE [Campo3] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo3] [smallint] NOT NULL\r\nGO\r\n";
            expected += "UPDATE [dbo].[Tabla1] SET [Campo4] = 0 WHERE [Campo4] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo4] [bigint] NOT NULL\r\nGO\r\n";
            expected += "UPDATE [dbo].[Tabla1] SET [Campo5] = 0 WHERE [Campo5] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo5] [bit] NOT NULL\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnNotNullEntero1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnNotNullEntero2;Data Source=(LOCAL)";
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result, expected, "Error");
        }

        [Description("Testea ALTER COLUMN a NOT NULL de campos Real/Float."), TestMethod]
        public void TestColumnNotNullReal()
        {
            string expected = "USE TestColumnNotNullReal1\r\nGO\r\n\r\n";
            expected += "UPDATE [dbo].[Tabla1] SET [Campo1] = 0.0 WHERE [Campo1] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo1] [float] NOT NULL\r\nGO\r\n";
            expected += "UPDATE [dbo].[Tabla1] SET [Campo2] = 0.0 WHERE [Campo2] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo2] [real] NOT NULL\r\nGO\r\n";
            expected += "UPDATE [dbo].[Tabla1] SET [Campo3] = 0.0 WHERE [Campo3] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo3] [money] NOT NULL\r\nGO\r\n";
            expected += "UPDATE [dbo].[Tabla1] SET [Campo4] = 0.0 WHERE [Campo4] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo4] [smallmoney] NOT NULL\r\nGO\r\n";
            expected += "UPDATE [dbo].[Tabla1] SET [Campo5] = 0.0 WHERE [Campo5] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo5] [numeric] (18,0) NOT NULL\r\nGO\r\n";
            expected += "UPDATE [dbo].[Tabla1] SET [Campo6] = 0.0 WHERE [Campo6] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo6] [decimal] (18,0) NOT NULL\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnNotNullReal1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnNotNullReal2;Data Source=(LOCAL)";
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result, expected, "Error");
        }

        [Description("Testea ALTER COLUMN a NOT NULL de campos Text."), TestMethod]
        public void TestColumnNotNullText()
        {
            string expected = "USE TestColumnNotNullText1\r\nGO\r\n\r\n";
            expected += "UPDATE [dbo].[Tabla1] SET [Campo1] = '' WHERE [Campo1] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo1] [char] (10) COLLATE Modern_Spanish_CI_AS NOT NULL\r\nGO\r\n";
            expected += "UPDATE [dbo].[Tabla1] SET [Campo2] = '' WHERE [Campo2] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo2] [varchar] (50) COLLATE Modern_Spanish_CI_AS NOT NULL\r\nGO\r\n";
            expected += "UPDATE [dbo].[Tabla1] SET [Campo3] = '' WHERE [Campo3] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo3] [varchar] (max) COLLATE Modern_Spanish_CI_AS NOT NULL\r\nGO\r\n";
            expected += "UPDATE [dbo].[Tabla1] SET [Campo4] = '' WHERE [Campo4] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo4] [text] COLLATE Modern_Spanish_CI_AS NOT NULL\r\nGO\r\n";
            expected += "UPDATE [dbo].[Tabla1] SET [Campo5] = N'' WHERE [Campo5] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo5] [nchar] (10) COLLATE Modern_Spanish_CI_AS NOT NULL\r\nGO\r\n";
            expected += "UPDATE [dbo].[Tabla1] SET [Campo6] = N'' WHERE [Campo6] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo6] [nvarchar] (50) COLLATE Modern_Spanish_CI_AS NOT NULL\r\nGO\r\n";
            expected += "UPDATE [dbo].[Tabla1] SET [Campo7] = N'' WHERE [Campo7] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo7] [nvarchar] (max) COLLATE Modern_Spanish_CI_AS NOT NULL\r\nGO\r\n";
            expected += "UPDATE [dbo].[Tabla1] SET [Campo8] = N'' WHERE [Campo8] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo8] [ntext] COLLATE Modern_Spanish_CI_AS NOT NULL\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnNotNullText1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnNotNullText2;Data Source=(LOCAL)";
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result, expected, "Error");
        }

        [Description("Testea ALTER COLUMN a NOT NULL de campos Date."), TestMethod]
        public void TestColumnNotNullDate()
        {
            string expected = "USE TestColumnNotNullDate1\r\nGO\r\n\r\n";
            expected += "UPDATE [dbo].[Tabla1] SET [Campo1] = getdate() WHERE [Campo1] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo1] [datetime] NOT NULL\r\nGO\r\n";
            expected += "UPDATE [dbo].[Tabla1] SET [Campo2] = getdate() WHERE [Campo2] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo2] [smalldatetime] NOT NULL\r\nGO\r\n";


            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnNotNullDate1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestColumnNotNullDate2;Data Source=(LOCAL)";
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result, expected, "Error");
        }

        [Description("Testea ALTER COLUMN y ADD y DROP de una Default Constraint."), TestMethod]
        public void TestConstraintDefaultAlterColumn()
        {
            string expected = "USE TestConstraintDefaultAlterColumn1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] DROP CONSTRAINT [DF_Tabla1_Campo2]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] DROP CONSTRAINT [DF_Tabla1_CampoD]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [CampoD] [char] (50) COLLATE Modern_Spanish_CI_AS NOT NULL\r\nGO\r\n";
            expected += "UPDATE [dbo].[Tabla1] SET [Campo2] = (N'Pepa') WHERE [Campo2] IS NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [Campo2] [nchar] (10) COLLATE Modern_Spanish_CI_AS NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ADD CONSTRAINT [DF_Tabla1_CampoD] DEFAULT ('Holas') FOR [CampoD]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ADD CONSTRAINT [DF_Tabla1_Campo2] DEFAULT (N'Pepa') FOR [Campo2]\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintDefaultAlterColumn1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintDefaultAlterColumn2;Data Source=(LOCAL)";
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result, expected, "Error");
        }

        [Description("Testea ALTER COLUMN de una columna Computed con FK Constraint."), TestMethod]
        public void TestConstraintFKType()
        {
            string expected = "USE TestConstraintFKType1\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[Tabla2] DROP CONSTRAINT [FK_Tabla2_Tabla2]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla2] DROP CONSTRAINT [FK_Tabla2_Tabla1]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] DROP CONSTRAINT [IX_Tabla1]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] DROP CONSTRAINT [PK_Tabla1]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] DROP COLUMN [Campo2]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla2] ALTER COLUMN [Tabla1IdFormula] [smallint] NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ALTER COLUMN [ID] [smallint] NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla2] ALTER COLUMN [Tabla1Id] [smallint] NOT NULL\r\nGO\r\n";
            expected += "DROP TYPE [dbo].[MyInt]\r\nGO\r\n";
            expected += "CREATE TYPE [dbo].[MyInt] FROM [smallint] NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla2] ALTER COLUMN [Tabla1Id] [MyInt] NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla2] ALTER COLUMN [Tabla1IdFormula] [MyInt] NOT NULL\r\nGO\r\n";
            expected += "CREATE TABLE [dbo].[TempTabla1]\r\n(\r\n";
            expected += "\t[ID] [MyInt] NOT NULL,\r\n";
            expected += "\t[Campo1] [smallint] NOT NULL,\r\n";
            expected += "\t[Campo2] AS ([ID]+[Campo1]) PERSISTED\r\n";
            expected += ") ON [PRIMARY]\r\nGO\r\n\r\n";
            expected += "INSERT INTO [dbo].[TempTabla1] ([ID],[Campo1]) SELECT [ID],[Campo1] FROM [dbo].[Tabla1]\r\n";
            expected += "DROP TABLE [dbo].[Tabla1]\r\nGO\r\n";
            expected += "EXEC sp_rename N'[dbo].[TempTabla1]',N'Tabla1', 'OBJECT'\r\nGO\r\n\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ADD CONSTRAINT [IX_Tabla1] UNIQUE NONCLUSTERED\r\n\t(\r\n";
            expected += "\t\t[Campo2] ASC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla1] ADD CONSTRAINT [PK_Tabla1] PRIMARY KEY CLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID] ASC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla2] ADD CONSTRAINT [FK_Tabla2_Tabla2] FOREIGN KEY\r\n\t(\r\n";
            expected += "\t\t[Tabla1IdFormula]\r\n";
            expected += "\t)\r\n\tREFERENCES [dbo].[Tabla1]\r\n\t(\r\n";
            expected += "\t\t[Campo2]\r\n\t)\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla2] ADD CONSTRAINT [FK_Tabla2_Tabla1] FOREIGN KEY\r\n\t(\r\n";
            expected += "\t\t[Tabla1Id]\r\n";
            expected += "\t)\r\n\tREFERENCES [dbo].[Tabla1]\r\n\t(\r\n";
            expected += "\t\t[ID]\r\n\t)\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintFKType1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintFKType2;Data Source=(LOCAL)";
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result, expected, "Error");
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
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintFKAlter1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintFKAlter2;Data Source=(LOCAL)";
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result, expected, "Error");
        }

        [Description("Testea ADD/DROP/ALTER de una tabla con nombres largos."), TestMethod]
        public void TestNombreLargo()
        {
            string expected = "USE [Test Nombre Largo1]\r\nGO\r\n\r\n";
            expected += "CREATE RULE dbo.[Mi Nueva Rule]\r\nAS\r\n\t@value > 1\r\nGO\r\n";
            expected += "DROP TRIGGER [dbo].[Lindo Trig]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla Test 1] DROP CONSTRAINT [CK_Tabla Test 1]\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla Test 1] DROP CONSTRAINT [DF_Tabla Test 1_Campo Descripcion]\r\nGO\r\n";
            expected += "DROP INDEX [IX_Tabla Test 1] ON [dbo].[Tabla Test 1]\r\nGO\r\n";
            expected += "CREATE TYPE [dbo].[Mi Nuevo Tipo] FROM [bigint] NOT NULL\r\nGO\r\n";
            expected += "EXEC sp_bindrule N'Mi Nueva Rule', N'Mi Nuevo Tipo','futureonly'\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla Test 1] ALTER COLUMN [Campo Indice] [Mi Nuevo Tipo] NOT NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla Test 1] ADD \r\n[Parent Id] [int] NULL\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla Test 1] ADD CONSTRAINT [CK_Tabla Test 1] CHECK  (([ID Tabla 1]>(0)))\r\nGO\r\n";
            expected += "ALTER TABLE [dbo].[Tabla Test 1] ADD CONSTRAINT [DF_Tabla Test 1_Campo Descripcion] DEFAULT ('Hola Mundo!!!!!') FOR [Campo Descripcion]\r\nGO\r\n";            
            expected += "ALTER TABLE [dbo].[Tabla Test 1] ADD CONSTRAINT [FK_Tabla Test 1_Tabla Test 1] FOREIGN KEY\r\n\t(\r\n\t\t[Parent Id]\r\n\t)\r\n";
            expected += "\tREFERENCES [dbo].[Tabla Test 1]\r\n\t(\r\n\t\t[ID Tabla 1]\r\n\t)\r\nGO\r\n";
            expected += "CREATE UNIQUE NONCLUSTERED INDEX [ID Test 2] ON [dbo].[Tabla Test 1]\r\n(\r\n";
            expected += "\t[ID Tabla 1] ASC,\r\n";
            expected += "\t[Parent Id] ASC\r\n";
            expected += ") WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\nGO\r\n";

            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=Test Nombre Largo1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=Test Nombre Largo2;Data Source=(LOCAL)";
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }

        [Description("Testea ADD/DROP de una tabla."), TestMethod]
        public void TestTableAddDrop()
        {
            string expected = "USE TestTableAdd1\r\nGO\r\n\r\n";
            expected += "DROP TABLE [dbo].[TablaDelete]\r\nGO\r\n";
            expected += "CREATE TABLE [dbo].[TablaAdd]\r\n(\r\n";
            expected += "\t[ID] [int] IDENTITY (2,2) NOT NULL,\r\n";
            expected += "\t[Campo1] [varchar] (50) COLLATE Modern_Spanish_CI_AS NOT NULL CONSTRAINT [DF_TablaAdd_Campo1] DEFAULT ('Hola'),\r\n";
            expected += "\t[ParentId] [int] NOT NULL,\r\n";
            expected += "\t[CampoF] AS ([ID]*(0.2)),\r\n";
            expected += "\tCONSTRAINT [PK_TablaAdd] PRIMARY KEY CLUSTERED\r\n\t(\r\n";
            expected += "\t\t[ID] ASC\r\n";
            expected += "\t) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],\r\n";
            expected += "\tCONSTRAINT [FK_TablaAdd_TablaAdd1] FOREIGN KEY\r\n\t(\r\n";
            expected += "\t\t[ParentId]\r\n";
            expected += "\t)\r\n";
            expected += "\tREFERENCES [dbo].[TablaAdd]\r\n\t(\r\n";
            expected += "\t\t[ID]\r\n";
            expected += "\t)\r\n";
            expected += ") ON [PRIMARY]\r\nGO\r\n";


            string result;
            SqlOption filter = new SqlOption(true);

            DBDiff.Schema.SQLServer.Model.Database origen;
            DBDiff.Schema.SQLServer.Model.Database destino;

            DBDiff.Schema.SQLServer.Generate sql = new DBDiff.Schema.SQLServer.Generate();
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestTableAdd1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestTableAdd2;Data Source=(LOCAL)";
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
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestIndexSimple1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestIndexSimple2;Data Source=(LOCAL)";
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
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestIndexEnabled1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestIndexEnabled2;Data Source=(LOCAL)";
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
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestIndexUserType1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestIndexUserType2;Data Source=(LOCAL)";
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
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestIndexXML1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestIndexXML2;Data Source=(LOCAL)";
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
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestView1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestView2;Data Source=(LOCAL)";
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
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestViewComun1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestViewComun2;Data Source=(LOCAL)";
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
            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintFKRemoveTable1;Data Source=(LOCAL)";
            origen = sql.Process(filter);

            sql.ConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TestConstraintFKRemoveTable2;Data Source=(LOCAL)";
            destino = sql.Process(filter);

            origen = DBDiff.Schema.SQLServer.Generate.Compare(origen, destino);

            result = origen.ToSQLDiff();

            Assert.AreEqual(result.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), expected.Replace(" ", "").Replace("\r\n", "").Replace("\t", ""), "Error");
        }
    }
}
