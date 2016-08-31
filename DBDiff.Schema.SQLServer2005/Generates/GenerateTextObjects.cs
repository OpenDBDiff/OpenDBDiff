using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using DBDiff.Schema.Events;
using DBDiff.Schema.SQLServer.Generates.Generates.Util;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.SQLServer.Generates.Options;

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
            sql += "SELECT O.name, O.type, M.object_id, OBJECT_DEFINITION(M.object_id) AS Text FROM sys.sql_modules M ";
            sql += "INNER JOIN sys.objects O ON O.object_id = M.object_id ";
            sql += "WHERE ";
            if (options.Ignore.FilterStoredProcedure)
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
                if ((database.Options.Ignore.FilterStoredProcedure) || (database.Options.Ignore.FilterView) || (database.Options.Ignore.FilterFunction) || (database.Options.Ignore.FilterTrigger))
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
                                    code = null;
                                    root.RaiseOnReadingOne(reader["name"]);
                                    string type = reader["Type"].ToString().Trim();
                                    string name = reader["name"].ToString();
                                    string definition = reader["Text"].ToString();
                                    int id = (int)reader["object_id"];
                                    if (type.Equals("V"))
                                        code = (ICode)database.Views.Find(id);

                                    if (type.Equals("TR"))
                                        code = (ICode)database.Find(id);

                                    if (type.Equals("P"))
                                    {
                                        var procedure = database.Procedures.Find(id);
                                        if (procedure != null)
                                            ((ICode)procedure).Text = GetObjectDefinition(type, name, definition);
                                    }

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

        private string GetObjectDefinition(string type, string name, string definition)
        {
            string rv = definition;

            string sqlDelimiters = @"(\r|\n|\s)+?";
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline;
            Regex re = new Regex(@"CREATE" + sqlDelimiters + @"PROC(EDURE)?" + sqlDelimiters + @"(\w+\.|\[\w+\]\.)?\[?(?<spname>\w+)\]?" + sqlDelimiters, options);
            switch (type)
            {
                case "P":
                    Match match = re.Match(definition);
                    if (match != null && match.Success)
                    {
                        // Try to replace the name saved in the definition when the object was created by the one used for the object in sys.object
                        string oldName = match.Groups["spname"].Value;
                        //if (String.IsNullOrEmpty(oldName)) System.Diagnostics.Debugger.Break();
                        if (String.Compare(oldName, name) != 0)
                        {
                            rv = rv.Replace(oldName, name);
                        }
                    }
                    break;
                default:
                    //TODO : Add the logic used for other objects than procedures
                    break;
            }

            return rv;
        }
    }
}
