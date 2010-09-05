using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using DBDiff.Schema.SQLServer2000.Model;
using DBDiff.Schema.SQLServer2000;

namespace TestDBDiff
{
    /// <summary>
    ///This is a test class for DBDiff.DBLibrary.SQLServer2000.Generate and is intended
    ///to contain all DBDiff.DBLibrary.SQLServer2000.Generate Unit Tests
    ///</summary>
    [TestClass()]
    public class GenerateTest
    {        
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Compare (Database, Database)
        ///</summary>
        [TestMethod()]
        public void CompareTest()
        {
            Database databaseOriginalSchema = null; // TODO: Initialize to an appropriate value

            Database databaseCompareSchema = null; // TODO: Initialize to an appropriate value

            Database expected = null;
            Database actual;

            actual = DBDiff.Schema.SQLServer2000.Generate.Compare(databaseOriginalSchema, databaseCompareSchema);

            Assert.AreEqual(expected, actual, "DBDiff.DBLibrary.SQLServer2000.Generate.Compare did not return the expected value" +
                    ".");
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///Prueba la funcion Process (La que genera el objeto schema Database)
        ///</summary>
        [TestMethod()]
        public void ProcessTest()
        {            
            Database actualDatabase;
            Generate target = new Generate();
            Database expected = null;

            target.ConnectioString = @"Persist Security Info=True;User ID=sa;Initial Catalog=Test1;Data Source=(LOCAL)";
            actualDatabase = target.Process();

            Assert.AreNotEqual(expected, actualDatabase, "DBDiff.DBLibrary.SQLServer2000.Generate.Process did not return the expected value.");
        }

        /// <summary>
        ///A test for ToXML ()
        ///</summary>
        [TestMethod()]
        public void ToXMLTest()
        {
            Database actualDatabase;
            Generate target = new Generate();
            string expected = null;
            string actual = "";

            target.ConnectioString = @"Persist Security Info=True;User ID=sa;Initial Catalog=Test2;Data Source=(LOCAL)";
            actualDatabase = target.Process();
            actual = actualDatabase.ToXML();

            Assert.AreNotEqual(expected, actual, "DBDiff.DBLibrary.SQLServer2000.Schema.Database.ToXML did not return the expected value.");
        }

    }


}
