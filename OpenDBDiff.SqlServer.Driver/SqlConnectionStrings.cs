using System;
using Microsoft.Data.SqlClient;

namespace OpenDBDiff.SqlServer.Driver
{
    /// <summary>
    /// The connection settings a database panel holds, independent of any UI toolkit.
    /// </summary>
    public sealed class SqlConnectionSettings
    {
        public SqlConnectionSettings(string serverName, string databaseName, bool integratedSecurity, string userName, string password)
        {
            ServerName = serverName;
            DatabaseName = databaseName;
            IntegratedSecurity = integratedSecurity;
            UserName = userName;
            Password = password;
        }

        public string ServerName { get; }

        public string DatabaseName { get; }

        public bool IntegratedSecurity { get; }

        public string UserName { get; }

        public string Password { get; }
    }

    /// <summary>
    /// Builds and reads back the connection strings a database panel uses.
    /// </summary>
    /// <remarks>
    /// This is the logic SqlServerConnectFront built inline, lifted somewhere it can be tested without
    /// a server: Windows authentication sets IntegratedSecurity and TrustServerCertificate, SQL
    /// authentication adds UserID and Password, and a connection with no database lands on the login's
    /// default database - which is what the WinForms control connected to when it listed the databases
    /// on a server.
    /// </remarks>
    public static class SqlConnectionStrings
    {
        public const string MasterDatabase = "master";

        public static string ForDefaultDatabase(string serverName, string userName = null, string password = null)
        {
            return ForDatabase(serverName, null, userName, password);
        }

        public static string ForMasterDatabase(string serverName, string userName = null, string password = null)
        {
            return ForDatabase(serverName, MasterDatabase, userName, password);
        }

        public static string ForDatabase(string serverName, string databaseName, string userName = null, string password = null)
        {
            if (serverName == null)
            {
                throw new ArgumentNullException(nameof(serverName));
            }

            var builder = new SqlConnectionStringBuilder
            {
                DataSource = serverName.Trim(),
                IntegratedSecurity = userName == null,
                TrustServerCertificate = true
            };

            // Without a database the connection lands on the login's default database.
            if (!string.IsNullOrEmpty(databaseName))
            {
                builder.InitialCatalog = databaseName.Trim();
            }

            if (userName != null)
            {
                builder.UserID = userName;
                builder.Password = password ?? string.Empty;
            }

            return builder.ConnectionString;
        }

        /// <summary>
        /// Reads a saved connection string back into the settings a panel shows.
        /// </summary>
        public static SqlConnectionSettings Parse(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);

            var integrated = builder.IntegratedSecurity;

            return new SqlConnectionSettings(
                builder.DataSource,
                builder.InitialCatalog,
                integrated,
                integrated ? string.Empty : builder.UserID,
                integrated ? string.Empty : builder.Password);
        }
    }
}
