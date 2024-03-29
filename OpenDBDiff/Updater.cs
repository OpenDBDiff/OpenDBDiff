using Microsoft.Data.SqlClient;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace OpenDBDiff
{
    /// <summary>
    /// Handles the SQL update queries
    /// </summary>
    static class Updater
    {
        public static string createNew(ISchemaBase target, string connectionString)
        {
            string script = target.ToSql();
            script = script.Replace("GO", "");
            //script = script.Replace("\r", "");
            //script = script.Replace("\t", "");
            //script = script.Replace("\n", "");
            string result = string.Empty;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(script, connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception e)
                {
                    result = e.Message + "\n";
                }
            }
            return result;
        }

        public static string addNew(ISchemaBase target, string connectionString)
        {
            string result = string.Empty;
            string script = target.ToSqlAdd();
            script = script.Replace("GO", "");
            //script = script.Replace("\r", "");
            //script = script.Replace("\t", "");
            //script = script.Replace("\n", "");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(script, connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception e)
                {
                    result = e.Message + "\n";
                }
            }
            return result;
        }

        public static DataTable getData(ISchemaBase selected, string connectionString)
        {
            DataTable data = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = "SELECT * FROM " + selected.FullName;

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader(CommandBehavior.KeyInfo);
                    data.Load(reader);
                    //data.Load(command.ExecuteReader());

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return data;
        }

        public static string alter(ISchemaBase target, string connectionString)
        {
            var db = target.RootParent as IDatabase;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                if (db != null && DialogResult.Yes != MessageBox.Show(String.Format("Alter {0} {1} in {2}..{3}?\n(WARNING: No automatic backup is made!)",
                        target.ObjectType,
                        target.Name,
                        connection.DataSource,
                        connection.Database), "ALTER Destination?", MessageBoxButtons.YesNo, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button2))
                {
                    return "Cancelled.";
                }

                var sb = new StringBuilder();
                SQLScriptList SqlDiff = target.ToSqlDiff(new List<ISchemaBase>());
                string[] splitOn = { "GO" };
                string[] tempList = SqlDiff.ToSQL().Split(splitOn, StringSplitOptions.RemoveEmptyEntries);
                List<string> scripts = new List<string>(tempList);

                foreach (string sql in scripts)
                {
                    string script = sql;
                    //script = script.Replace("\r", "");
                    //script = script.Replace("\t", "");
                    //script = script.Replace("\n", " ");
                    if (target.ObjectType == ObjectType.StoredProcedure)
                    {
                        script = sql.Replace("CREATE PROCEDURE", "ALTER PROCEDURE");
                    }
                    SqlCommand command = new SqlCommand(script, connection);
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

        public static string rebuild(ISchemaBase target, string connectionString)
        {
            SQLScriptList SqlDiff = target.ToSqlDiff(new List<ISchemaBase>());
            string[] splitOn = { "GO" };
            string[] tempList = SqlDiff.ToSQL().Split(splitOn, StringSplitOptions.RemoveEmptyEntries);
            List<string> scripts = new List<string>(tempList);
            string result = string.Empty;
            string script = scripts[0];
            if (target.ObjectType == ObjectType.Table)
            {
                script = script.Replace("CREATE TABLE", "ALTER TABLE");
            }
            MessageBox.Show(script);
            return result;
        }

        public static bool CommitTable(DataTable table, string tableFullName, string ConnectionString)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM " + tableFullName, connection);
                SqlDataAdapter da = new SqlDataAdapter(command);
                try
                {
                    using (SqlCommandBuilder builder = new SqlCommandBuilder(da))
                    {
                        connection.Open();
                        da.Update(table);
                        connection.Close();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }
        }
    }
}
