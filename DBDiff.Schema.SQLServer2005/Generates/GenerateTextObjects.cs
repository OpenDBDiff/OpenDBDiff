using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.SQLServer.Generates.Options;
using System.Data.SqlClient;
using DBDiff.Schema.Events;
using DBDiff.Schema.SQLServer.Generates.Generates.Util;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Generates
{
    public class GenerateTextObjects
    {
        private Generate root;

        public GenerateTextObjects(Generate root)
        {
            this.root = root;
        }

        private static string GetSQL(SqlOption options)
        {
            string filter = "";
            string sql = "";
            sql += "SELECT O.type, M.object_id, OBJECT_DEFINITION(M.object_id) AS Text FROM sys.sql_modules M ";
            sql += "INNER JOIN sys.objects O ON O.object_id = M.object_id ";
            sql += "WHERE ";
            if (options.Ignore.FilterStoreProcedure)
                filter += "O.type = 'P' OR ";
            if (options.Ignore.FilterView)
                filter += "O.type = 'V' OR ";
            if (options.Ignore.FilterTrigger)
                filter += "O.type = 'TR' OR ";
            if (options.Ignore.FilterFunction)
                filter += "O.type IN ('IF','FN','TF') OR ";
            filter = filter.Substring(0, filter.Length - 4);
            return sql + filter;
        }

        public void Fill(Database database, string connectionString)
        {
            ICode code = null;
            try
            {
                if ((database.Options.Ignore.FilterStoreProcedure) || (database.Options.Ignore.FilterView) || (database.Options.Ignore.FilterFunction) || (database.Options.Ignore.FilterTrigger))
                {
                    root.RaiseOnReading(new ProgressEventArgs("Reading Text Objects...", Constants.READING_TEXTOBJECTS));
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand(GetSQL(database.Options), conn))
                        {
                            conn.Open();
                            command.CommandTimeout = 0;
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string type = reader["Type"].ToString().Trim();
                                    int id = (int)reader["object_id"];
                                    if (type.Equals("V"))
                                        code = (ICode)database.Views.Find(id);

                                    if (type.Equals("TR"))
                                        code = (ICode)database.Find(id);

                                    if (type.Equals("P"))
                                        code = (ICode)database.Procedures.Find(id);

                                    if (type.Equals("IF") || type.Equals("FN") || type.Equals("TF"))
                                        code = (ICode)database.Functions.Find(id);

                                    if (code != null)
                                        code.Text = reader["Text"].ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
