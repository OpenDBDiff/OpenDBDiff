using System;
using System.Data.SqlClient;
using System.Globalization;
using DBDiff.Schema.Misc;
using DBDiff.Schema.SQLServer.Generates.Generates.SQLCommands;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.SQLServer.Generates.Options;
#if DEBUG
using System.Runtime.InteropServices;
#endif

namespace DBDiff.Schema.SQLServer.Generates.Generates
{
    public class GenerateDatabase
    {
        private string connectioString;
        private SqlOption objectFilter;

        public bool UseDefaultVersionOnVersionParseError { get; private set; }

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="connectioString">Connection string de la base</param>
        public GenerateDatabase(string connectioString, SqlOption filter)
        {
            this.connectioString = connectioString;
            this.objectFilter = filter;
        }

        public DatabaseInfo Get(Database database)
        {
            DatabaseInfo item = new DatabaseInfo();
            using (SqlConnection conn = new SqlConnection(connectioString))
            {
                using (SqlCommand command = new SqlCommand(DatabaseSQLCommand.GetVersion(database), conn))
                {
                    conn.Open();

                    item.Server = conn.DataSource;
                    item.Database = conn.Database;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string versionValue = reader["Version"] as string;
                            try
                            {
                                // used to use the decimal as well when Azure was 10.25
                                var version = new Version(versionValue);
                                item.VersionNumber = float.Parse(String.Format("{0}.{1}", version.Major, version.Minor), CultureInfo.InvariantCulture);

                                int? edition = null;
                                if (reader.FieldCount > 1 && !reader.IsDBNull(1))
                                {
                                    int validEdition;
                                    string editionValue = reader[1].ToString();
                                    if (!String.IsNullOrEmpty(editionValue) && int.TryParse(editionValue, out validEdition))
                                    {
                                        edition = validEdition;
                                    }
                                }

                                item.SetEdition(edition);
                            }
                            catch (Exception notAGoodIdeaToCatchAllErrors)
                            {
                                var exception = new SchemaException(
                                    String.Format("Error parsing ProductVersion. ({0})", versionValue ?? "[null]")
                                    , notAGoodIdeaToCatchAllErrors);

                                if (!UseDefaultVersionOnVersionParseError)
                                {
                                    throw exception;
                                }
                            }
                        }
                    }
                }

                using (SqlCommand command = new SqlCommand(DatabaseSQLCommand.Get(item.Version, database), conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            item.Collation = reader["Collation"].ToString();
                            item.HasFullTextEnabled = ((int)reader["IsFulltextEnabled"]) == 1;
                        }
                    }
                }

            }

            return item;
        }
    }
}
