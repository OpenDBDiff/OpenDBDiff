using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using DBDiff.Schema.Errors;
using DBDiff.Schema.Misc;
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
        private List<MessageLog> messages;

        public Generate()
        {
            messages = new List<MessageLog>();
        }

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
                /*Thread t1 = new Thread(delegate()
                {
                    try
                    {*/
                        GenerateRules.Fill(databaseSchema, connectionString);
                        GenerateTables.Fill(databaseSchema, connectionString, messages);
                        GenerateUserDataTypes.Fill(databaseSchema, connectionString);
                        GenerateXMLSchemas.Fill(databaseSchema, connectionString);
                        GenerateSchemas.Fill(databaseSchema, connectionString);
                        GenerateUsers.Fill(databaseSchema, connectionString);
                    /*}
                    catch (Exception ex)
                    {
                        error = ex.StackTrace;
                    }
                });
                Thread t2 = new Thread(delegate()
                {
                    try
                    {*/
                        GenerateFileGroups.Fill(databaseSchema, connectionString);                        
                        GenerateDDLTriggers.Fill(databaseSchema, connectionString);
                        GenerateSynonyms.Fill(databaseSchema, connectionString);
                        GenerateAssemblies.Fill(databaseSchema, connectionString);
                    /*}
                    catch (Exception ex)
                    {
                        error = ex.StackTrace;
                    }
                });
                Thread t3 = new Thread(delegate()
                {
                    try
                    {*/
                        GenerateStoreProcedures.Fill(databaseSchema, connectionString);
                        GenerateViews.Fill(databaseSchema, connectionString);
                        GenerateFunctions.Fill(databaseSchema, connectionString);
                    /*}
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
                t3.Join();*/
                if (String.IsNullOrEmpty(error))
                {
                    /*Las propiedades extendidas deben ir despues de haber capturado el resto de los objetos de la base*/
                    GenerateExtendedProperties.Fill(databaseSchema, connectionString);
                    databaseSchema.BuildDependency();
                    return databaseSchema;
                }
                else
                    throw new SchemaException(error);
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
