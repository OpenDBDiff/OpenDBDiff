using System;
using System.Data.SqlClient;
using System.Threading;
using DBDiff.Schema.Events;
using DBDiff.Schema.SQLServer.Compare;
using DBDiff.Schema.SQLServer.Model;
using DBDiff.Schema.SQLServer.Generates;
using DBDiff.Schema.SQLServer.Options;

namespace DBDiff.Schema.SQLServer
{
    public class Generate
    {
        public enum VersionTypeEnum
        {
            None = 0,
            SQL2000 = 1,
            SQL2005 = 2,
            SQL2008 = 3
        }

        public event Progress.ProgressHandler OnTableProgress;
        private string connectionString;

        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        public string Name
        {
            get
            {
                string name = "";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    name = conn.Database;
                }
                return name;
            }
        }

        /// <summary>
        /// Genera el schema de la base de datos seleccionada y devuelve un objeto Database.
        /// </summary>
        public Database Process(SqlOption filters)
        {
            string error = "";
            try
            {
                Database databaseSchema = new Database();

                //tables.OnTableProgress += new Progress.ProgressHandler(tables_OnTableProgress);
                databaseSchema.Options = filters;
                databaseSchema.Name = this.Name;
                databaseSchema.Info = (new GenerateDatabase(connectionString, filters)).Get();
                Thread t1 = new Thread(delegate()
                {
                    try
                    {
                        databaseSchema.Tables = (new GenerateTables(connectionString, filters)).Get(databaseSchema);
                        databaseSchema.UserTypes = (new GenerateUserDataTypes(connectionString, filters)).Get(databaseSchema);
                        databaseSchema.XmlSchemas = (new GenerateXMLSchemas(connectionString, filters)).Get(databaseSchema);
                        databaseSchema.Schemas = (new GenerateSchemas(connectionString, filters)).Get(databaseSchema);
                    }
                    catch (Exception ex)
                    {
                        error = ex.StackTrace;
                    }
                });
                Thread t2 = new Thread(delegate()
                {
                    try
                    {
                        databaseSchema.FileGroups = (new GenerateFileGroups(connectionString, filters)).Get(databaseSchema);
                        databaseSchema.Rules = (new GenerateRules(connectionString, filters)).Get(databaseSchema);
                        databaseSchema.DDLTriggers = (new GenerateDDLTriggers(connectionString, filters)).Get(databaseSchema);
                        databaseSchema.Synonyms = (new GenerateSynonyms(connectionString, filters)).Get(databaseSchema);
                        databaseSchema.Assemblies = (new GenerateAssemblies(connectionString, filters)).Get(databaseSchema);
                    }
                    catch (Exception ex)
                    {
                        error = ex.StackTrace;
                    }
                });
                Thread t3 = new Thread(delegate()
                {
                    try
                    {
                        databaseSchema.Procedures = (new GenerateStoreProcedures(connectionString, filters)).Get(databaseSchema);
                        databaseSchema.Views = (new GenerateViews(connectionString, filters)).Get(databaseSchema);
                    }
                    catch (Exception ex)
                    {
                        error = ex.StackTrace;
                    }
                });
                t1.Start();
                t2.Start();
                t3.Start();
                t1.Join();
                t2.Join();
                t3.Join();
                if (String.IsNullOrEmpty(error))
                    return databaseSchema;
                else
                    throw new Exception(error);
            }
            catch
            {
                throw;
            }
        }

        private void tables_OnTableProgress(object sender, ProgressEventArgs e)
        {
            if (OnTableProgress != null)
                OnTableProgress(sender, e);
        }

        /// <summary>
        /// Compara el schema de la base con otro y devuelve el script SQL con las diferencias.
        /// </summary>
        /// <param name="xmlCompareSchema"></param>
        /// <returns></returns>
        public static Database Compare(Database databaseOriginalSchema, Database databaseCompareSchema)
        {
            Database merge;
            merge = CompareDatabase.GenerateDiferences(databaseOriginalSchema, databaseCompareSchema);
            return merge;
        }
    }
}
