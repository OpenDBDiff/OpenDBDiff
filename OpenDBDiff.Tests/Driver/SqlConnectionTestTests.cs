using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDBDiff.SqlServer.Driver;

namespace OpenDBDiff.Tests.Driver
{
    /// <summary>
    /// The "Test connection" result of section 6.1.1: the product name, the elapsed-time line and the
    /// error categories the panel reports.
    /// </summary>
    [TestClass]
    public class SqlConnectionTestTests
    {
        [TestMethod]
        public void TheSuccessLineMatchesTheFormInTheSpec()
        {
            // Section 6.1.1: "Connected · SQL Server 2019 · 42 ms".
            Assert.AreEqual("Connected \u00b7 SQL Server 2019 \u00b7 42 ms", SqlConnectionTest.Describe("SQL Server 2019", 42));
        }

        [TestMethod]
        public void TheProductVersionMapsToTheMarketingName()
        {
            Assert.AreEqual("SQL Server 2019", SqlConnectionTest.ProductName("15.0.2000.5"));
            Assert.AreEqual("SQL Server 2022", SqlConnectionTest.ProductName("16.0.1000.6"));
            Assert.AreEqual("SQL Server 2017", SqlConnectionTest.ProductName("14.0.1000.169"));
            Assert.AreEqual("SQL Server 2016", SqlConnectionTest.ProductName("13.0.5026.0"));
            Assert.AreEqual("SQL Server 2012", SqlConnectionTest.ProductName("11.0.2100.60"));
        }

        [TestMethod]
        public void AnUnknownOrMissingVersionStillProducesSomethingShowable()
        {
            Assert.AreEqual("SQL Server", SqlConnectionTest.ProductName(null));
            Assert.AreEqual("SQL Server", SqlConnectionTest.ProductName("  "));
            Assert.AreEqual("SQL Server 99", SqlConnectionTest.ProductName("99.1.2"));
            Assert.AreEqual("SQL Server", SqlConnectionTest.ProductName("not-a-version"));
        }

        [TestMethod]
        public void ErrorNumbersLandInTheCategoriesTheSpecNames()
        {
            Assert.AreEqual(ConnectionErrorCategory.Authentication, SqlConnectionTest.Categorize(18456, "Login failed for user 'sa'."));
            Assert.AreEqual(ConnectionErrorCategory.NotFound, SqlConnectionTest.Categorize(4060, "Cannot open database \"VivaLDI\" requested by the login."));
            Assert.AreEqual(ConnectionErrorCategory.Permission, SqlConnectionTest.Categorize(229, "The SELECT permission was denied."));
            Assert.AreEqual(ConnectionErrorCategory.Network, SqlConnectionTest.Categorize(53, "A network-related error occurred."));
            Assert.AreEqual(ConnectionErrorCategory.Network, SqlConnectionTest.Categorize(10060, "timeout"));
        }

        [TestMethod]
        public void AMessageWithNoNumberIsStillCategorised()
        {
            Assert.AreEqual(ConnectionErrorCategory.Authentication, SqlConnectionTest.Categorize(null, "Login failed for user 'sa'."));
            Assert.AreEqual(ConnectionErrorCategory.Network, SqlConnectionTest.Categorize(null, "A network-related or instance-specific error occurred."));
            Assert.AreEqual(ConnectionErrorCategory.NotFound, SqlConnectionTest.Categorize(null, "Cannot open database requested by the login."));
            Assert.AreEqual(ConnectionErrorCategory.Permission, SqlConnectionTest.Categorize(null, "The server principal has no permission."));
            Assert.AreEqual(ConnectionErrorCategory.Unknown, SqlConnectionTest.Categorize(null, "Something else entirely"));
        }

        [TestMethod]
        public void TheFailureLineNamesTheCategory()
        {
            Assert.AreEqual("Login failed for user 'sa'. (auth)",
                SqlConnectionTest.DescribeError(ConnectionErrorCategory.Authentication, "Login failed for user 'sa'."));

            Assert.AreEqual("The connection failed. (network)",
                SqlConnectionTest.DescribeError(ConnectionErrorCategory.Network, null));
        }

        [TestMethod]
        public void AResultPicksTheLineForItsOutcome()
        {
            var ok = new ConnectionTestResult(true, "SQL Server 2019", 42, ConnectionErrorCategory.None, null, null);
            Assert.AreEqual("Connected \u00b7 SQL Server 2019 \u00b7 42 ms", ok.Message);

            var failed = new ConnectionTestResult(false, null, 12, ConnectionErrorCategory.Permission, "permission denied", 229);
            Assert.AreEqual("permission denied (permission)", failed.Message);
            Assert.AreEqual(229, failed.ErrorNumber.Value);
        }
    }
}
