using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.SqlServer.Driver;

namespace OpenDBDiff.Tests.Driver
{
    /// <summary>
    /// The connection strings a database panel builds, including the one used to list the databases on
    /// a server - the case that made a WPF panel show an empty list.
    /// </summary>
    [TestClass]
    public class SqlConnectionStringsTests
    {
        [TestMethod]
        public void WindowsAuthentication_IsIntegratedAndTrustsTheServerCertificate()
        {
            var connectionString = SqlConnectionStrings.ForDatabase("SRV012025", "VivatDI");

            var settings = SqlConnectionStrings.Parse(connectionString);

            Assert.IsTrue(settings.IntegratedSecurity);
            Assert.AreEqual("SRV012025", settings.ServerName);
            Assert.AreEqual("VivatDI", settings.DatabaseName);
            Assert.AreEqual(string.Empty, settings.UserName);
        }

        [TestMethod]
        public void SqlAuthentication_CarriesTheCredentials()
        {
            var connectionString = SqlConnectionStrings.ForDatabase("SRV012025", "VivatDI", "sa", "secret");

            var settings = SqlConnectionStrings.Parse(connectionString);

            Assert.IsFalse(settings.IntegratedSecurity);
            Assert.AreEqual("sa", settings.UserName);
            Assert.AreEqual("secret", settings.Password);
        }

        /// <summary>
        /// The list of databases is read over a connection with no InitialCatalog: naming a database
        /// that happens to be offline is what makes that enumeration fail before it starts.
        /// </summary>
        [TestMethod]
        public void ForDefaultDatabase_HasNoInitialCatalog()
        {
            var connectionString = SqlConnectionStrings.ForDefaultDatabase("SRV012025");

            var settings = SqlConnectionStrings.Parse(connectionString);

            Assert.AreEqual(string.Empty, settings.DatabaseName);
            Assert.IsTrue(settings.IntegratedSecurity);
        }

        [TestMethod]
        public void ForMasterDatabase_NamesMaster()
        {
            var connectionString = SqlConnectionStrings.ForMasterDatabase("SRV012025");

            Assert.AreEqual("master", SqlConnectionStrings.Parse(connectionString).DatabaseName);
        }

        [TestMethod]
        public void ServerNamesAreTrimmed()
        {
            var settings = SqlConnectionStrings.Parse(SqlConnectionStrings.ForDatabase("  SRV012025  ", "  VivatDI  "));

            Assert.AreEqual("SRV012025", settings.ServerName);
            Assert.AreEqual("VivatDI", settings.DatabaseName);
        }

        [TestMethod]
        public void AnEmptyConnectionStringIsNotMistakenForAServer()
        {
            var settings = SqlConnectionStrings.Parse(string.Empty);

            Assert.AreEqual(string.Empty, settings.ServerName);
            Assert.AreEqual(string.Empty, settings.DatabaseName);
        }
    }
}
