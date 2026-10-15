using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Data.SqlClient;

namespace OpenDBDiff.SqlServer.Driver
{
    /// <summary>
    /// Why a connection test failed, in the categories section 6.1.1 asks the panel to report.
    /// </summary>
    public enum ConnectionErrorCategory
    {
        None,
        Network,
        Authentication,
        Permission,
        NotFound,
        Unknown
    }

    /// <summary>The result of testing one connection, as the connection panel shows it (section 6.1.1).</summary>
    public sealed class ConnectionTestResult
    {
        public ConnectionTestResult(bool succeeded, string serverProduct, long elapsedMs, ConnectionErrorCategory category, string errorMessage, int? errorNumber)
        {
            Succeeded = succeeded;
            ServerProduct = serverProduct;
            ElapsedMs = elapsedMs;
            Category = category;
            ErrorMessage = errorMessage;
            ErrorNumber = errorNumber;
        }

        public bool Succeeded { get; }

        /// <summary>The product name, e.g. "SQL Server 2019"; null when the test failed.</summary>
        public string ServerProduct { get; }

        /// <summary>How long the round trip took, which is what section 6.1.1's "42 ms" reports.</summary>
        public long ElapsedMs { get; }

        public ConnectionErrorCategory Category { get; }

        public string ErrorMessage { get; }

        public int? ErrorNumber { get; }

        /// <summary>What the panel prints next to the button.</summary>
        public string Message
        {
            get
            {
                return Succeeded
                    ? SqlConnectionTest.Describe(ServerProduct, ElapsedMs)
                    : SqlConnectionTest.DescribeError(Category, ErrorMessage);
            }
        }
    }

    /// <summary>
    /// The "Test connection" button of section 6.1.1, which belongs here rather than in the WPF project
    /// so it can be reasoned about and tested without a UI.
    /// </summary>
    /// <remarks>
    /// The pure parts - product naming, error categorisation and the result line - are static so the
    /// tests can pin them without a server; <see cref="Run"/> is the only part that opens a connection.
    /// </remarks>
    public static class SqlConnectionTest
    {
        /// <summary>Section 6.1.1's result line, e.g. "Connected · SQL Server 2019 · 42 ms".</summary>
        public static string Describe(string serverProduct, long elapsedMs)
        {
            var product = string.IsNullOrEmpty(serverProduct) ? "SQL Server" : serverProduct;

            return string.Format(CultureInfo.InvariantCulture, "Connected \u00b7 {0} \u00b7 {1} ms", product, elapsedMs);
        }

        /// <summary>The failure line, with the category in brackets as section 6.1.1 asks for.</summary>
        public static string DescribeError(ConnectionErrorCategory category, string message)
        {
            var text = string.IsNullOrWhiteSpace(message) ? "The connection failed." : message.Trim();

            return string.Format(CultureInfo.InvariantCulture, "{0} ({1})", text, CategoryName(category));
        }

        public static string CategoryName(ConnectionErrorCategory category)
        {
            switch (category)
            {
                case ConnectionErrorCategory.Network: return "network";
                case ConnectionErrorCategory.Authentication: return "auth";
                case ConnectionErrorCategory.Permission: return "permission";
                case ConnectionErrorCategory.NotFound: return "not found";
                default: return "error";
            }
        }

        /// <summary>
        /// Maps the server's product version to the name the panel shows. The mapping is the marketing
        /// name of each major version; an unknown version keeps its own number rather than pretending.
        /// </summary>
        public static string ProductName(string productVersion)
        {
            if (string.IsNullOrWhiteSpace(productVersion))
            {
                return "SQL Server";
            }

            var major = Major(productVersion);

            if (major <= 0)
            {
                // A version we cannot read is reported as SQL Server rather than as "version 0".
                return "SQL Server";
            }

            switch (major)
            {
                case 17: return "SQL Server 2025";
                case 16: return "SQL Server 2022";
                case 15: return "SQL Server 2019";
                case 14: return "SQL Server 2017";
                case 13: return "SQL Server 2016";
                case 12: return "SQL Server 2014";
                case 11: return "SQL Server 2012";
                case 10: return "SQL Server 2008";
                default: return "SQL Server " + major.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Section 6.1.1 lists the categories the panel can report: network, auth, permission, not
        /// found. Matching is by error number first and message second, so a localised server message
        /// still lands in the right bucket when the number is one we know.
        /// </summary>
        public static ConnectionErrorCategory Categorize(int? errorNumber, string message)
        {
            if (errorNumber.HasValue)
            {
                switch (errorNumber.Value)
                {
                    case 18456:
                    case 18452:
                        return ConnectionErrorCategory.Authentication;

                    case 4060:
                    case 911:
                        return ConnectionErrorCategory.NotFound;

                    case 229:
                    case 230:
                    case 262:
                        return ConnectionErrorCategory.Permission;

                    case 53:
                    case 64:
                    case 121:
                    case 233:
                    case 10054:
                    case 10060:
                    case 10061:
                    case 11001:
                    case 40613:
                    case -1:
                    case -2:
                        return ConnectionErrorCategory.Network;
                }
            }

            var text = message ?? string.Empty;

            if (Contains(text, "login failed") || Contains(text, "authentication"))
            {
                return ConnectionErrorCategory.Authentication;
            }

            if (Contains(text, "cannot open database") || Contains(text, "database") && Contains(text, "not found"))
            {
                return ConnectionErrorCategory.NotFound;
            }

            if (Contains(text, "permission"))
            {
                return ConnectionErrorCategory.Permission;
            }

            if (Contains(text, "network") || Contains(text, "server was not found") || Contains(text, "timeout")
                || Contains(text, "instance-specific"))
            {
                return ConnectionErrorCategory.Network;
            }

            return ConnectionErrorCategory.Unknown;
        }

        /// <summary>
        /// Opens the connection, reads the product version and times the round trip. The server name is
        /// deliberately not put in the result: the panel already shows it.
        /// </summary>
        public static ConnectionTestResult Run(string connectionString)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string productVersion = null;

                    using (var command = new SqlCommand("SELECT CAST(SERVERPROPERTY('ProductVersion') AS nvarchar(128))", connection))
                    {
                        productVersion = command.ExecuteScalar() as string;
                    }

                    stopwatch.Stop();

                    return new ConnectionTestResult(true, ProductName(productVersion), stopwatch.ElapsedMilliseconds,
                        ConnectionErrorCategory.None, null, null);
                }
            }
            catch (SqlException ex)
            {
                stopwatch.Stop();

                var number = ex.Number;
                var category = Categorize(number, ex.Message);

                return new ConnectionTestResult(false, null, stopwatch.ElapsedMilliseconds, category, ex.Message, number);
            }
            catch (InvalidOperationException ex)
            {
                // A malformed connection string reaches here rather than as a SqlException.
                stopwatch.Stop();

                return new ConnectionTestResult(false, null, stopwatch.ElapsedMilliseconds,
                    ConnectionErrorCategory.Unknown, ex.Message, null);
            }
        }

        private static int Major(string productVersion)
        {
            var text = productVersion.Trim();
            var end = text.IndexOfAny(new[] { '.', ' ', '-' });

            if (end > 0)
            {
                text = text.Substring(0, end);
            }

            int major;

            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out major) && major > 0
                ? major
                : 0;
        }

        private static bool Contains(string text, string needle)
        {
            return text.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
