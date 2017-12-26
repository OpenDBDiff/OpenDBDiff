using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DBDiff.Schema.Errors;
using DBDiff.Schema.Events;
using DBDiff.Schema.SQLServer.Generates.Generates.SQLCommands;
using DBDiff.Schema.SQLServer.Generates.Generates.Util;
using DBDiff.Schema.SQLServer.Generates.Model;

namespace DBDiff.Schema.SQLServer.Generates.Generates
{
    public class GenerateViews
    {
        private Generate root;

        public GenerateViews(Generate root)
        {
            this.root = root;
        }

        public void Fill(Database database, string connectionString, List<MessageLog> messages)
        {
            try
            {
                root.RaiseOnReading(new ProgressEventArgs("Reading views...", Constants.READING_VIEWS));
                if (database.Options.Ignore.FilterView)
                {
                    FillView(database, connectionString);
                }
            }
            catch (Exception ex)
            {
                messages.Add(new MessageLog(ex.Message, ex.StackTrace, MessageLog.LogType.Error));
            }
        }

        private void FillView(Database database, string connectionString)
        {
            int lastViewId = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(ViewSQLCommand.GetView(database.Info.Version), conn))
                {
                    conn.Open();
                    command.CommandTimeout = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        View item = null;
                        while (reader.Read())
                        {
                            root.RaiseOnReadingOne(reader["name"]);
                            if (lastViewId != (int)reader["object_id"])
                            {
                                item = new View(database);
                                item.Id = (int)reader["object_id"];
                                item.Name = reader["name"].ToString();
                                item.Owner = reader["owner"].ToString();
                                item.IsSchemaBinding = reader["IsSchemaBound"].ToString().Equals("1");
                                database.Views.Add(item);
                                lastViewId = item.Id;
                            }
                            if (item.IsSchemaBinding)
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("referenced_major_id")))
                                    database.Dependencies.Add(database, (int)reader["referenced_major_id"], item);
                                if (!String.IsNullOrEmpty(reader["TableName"].ToString()))
                                    item.DependenciesIn.Add(reader["TableName"].ToString());
                                if (!String.IsNullOrEmpty(reader["DependOut"].ToString()))
                                    item.DependenciesOut.Add(reader["DependOut"].ToString());
                            }
                        }
                    }
                }
            }
        }
    }
}
