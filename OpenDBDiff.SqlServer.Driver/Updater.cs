using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;

namespace OpenDBDiff.SqlServer.Driver
{
    /// <summary>
    /// Runs the update scripts against the destination database: creating an object, adding one of
    /// another type, altering, rebuilding, and pushing table data edits back.
    /// </summary>
    /// <remarks>
    /// Moved here from the WinForms project (OpenDBDiff/Updater.cs) so both front ends share it. The
    /// SQL is unchanged; the MessageBox calls became <see cref="IUpdateConfirmation"/>, which the
    /// caller supplies.
    /// </remarks>
    public static class Updater
    {
        public static string createNew(ISchemaBase target, string connectionString, IUpdateConfirmation confirmation)
        {
            return execute(target.ToSql(), connectionString);
        }

        public static string addNew(ISchemaBase target, string connectionString, IUpdateConfirmation confirmation)
        {
            return execute(target.ToSqlAdd(), connectionString);
        }

        /// <summary>
        /// Splits a script into batches on lines that contain only GO, like SSMS and sqlcmd do.
        /// A plain split on "GO" would also cut words such as GOTO or CATEGORY.
        /// </summary>
        private static List<string> splitBatches(string script)
        {
            var batches = new List<string>();

            foreach (string batch in Regex.Split(script, @"^[ \t]*GO[ \t]*\r?$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(batch))
                {
                    batches.Add(batch);
                }
            }

            return batches;
        }

        private static string execute(string script, string connectionString)
        {
            string result = string.Empty;

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    foreach (string batch in splitBatches(script))
                    {
                        var command = new SqlCommand(batch, connection);
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
                catch (Exception e)
                {
                    result = e.Message + "\n";
                }
            }

            return result;
        }

        public static DataTable getData(ISchemaBase selected, string connectionString, IUpdateConfirmation confirmation)
        {
            var data = new DataTable();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var command = new SqlCommand
                    {
                        Connection = connection,
                        CommandText = "SELECT * FROM " + selected.FullName
                    };

                    connection.Open();

                    using (var reader = command.ExecuteReader(CommandBehavior.KeyInfo))
                    {
                        data.Load(reader);
                    }

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                confirmation.ReportError(e.Message);
            }

            return data;
        }

        public static string alter(ISchemaBase target, string connectionString, IUpdateConfirmation confirmation)
        {
            var db = target.RootParent as IDatabase;

            using (var connection = new SqlConnection(connectionString))
            {
                if (db != null && !confirmation.ConfirmAlter(target.ObjectType, target.Name, connection.DataSource, connection.Database))
                {
                    return "Cancelled.";
                }

                var sb = new StringBuilder();
                var sqlDiff = target.ToSqlDiff(new List<ISchemaBase>());
                var scripts = splitBatches(sqlDiff.ToSQL());

                foreach (string sql in scripts)
                {
                    var script = sql;

                    if (target.ObjectType == ObjectType.StoredProcedure)
                    {
                        script = sql.Replace("CREATE PROCEDURE", "ALTER PROCEDURE");
                    }

                    var command = new SqlCommand(script, connection);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    catch (Exception e)
                    {
                        sb.AppendLine($"{target.Name}: {e.Message}");
                        connection.Close();
                    }
                }

                return sb.ToString();
            }
        }

        public static string rebuild(ISchemaBase target, string connectionString, IUpdateConfirmation confirmation)
        {
            var sqlDiff = target.ToSqlDiff(new List<ISchemaBase>());
            var scripts = splitBatches(sqlDiff.ToSQL());
            var result = string.Empty;
            var script = scripts[0];

            if (target.ObjectType == ObjectType.Table)
            {
                script = script.Replace("CREATE TABLE", "ALTER TABLE");
            }

            confirmation.InformScript(script);
            return result;
        }

        public static bool CommitTable(DataTable table, string tableFullName, string connectionString, IUpdateConfirmation confirmation)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("SELECT * FROM " + tableFullName, connection);
                var adapter = new SqlDataAdapter(command);

                try
                {
                    using (var builder = new SqlCommandBuilder(adapter))
                    {
                        connection.Open();
                        adapter.Update(table);
                        connection.Close();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    confirmation.ReportError(e.Message);
                    return false;
                }
            }
        }
    }
}
