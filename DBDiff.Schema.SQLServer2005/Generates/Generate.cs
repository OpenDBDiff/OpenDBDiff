using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DBDiff.Schema.Errors;
using DBDiff.Schema.Events;
using DBDiff.Schema.Misc;
using DBDiff.Schema.SQLServer.Generates.Compare;
using DBDiff.Schema.SQLServer.Generates.Generates.Util;
using DBDiff.Schema.SQLServer.Generates.Model;
using DBDiff.Schema.SQLServer.Generates.Options;

namespace DBDiff.Schema.SQLServer.Generates.Generates
{
    public class Generate
    {
        private readonly List<MessageLog> messages;
        private SqlOption options;
        private ProgressEventArgs currentlyReading;

        public Generate()
        {
            messages = new List<MessageLog>();
            OnReading += Generate_OnReading;
        }

        public static int MaxValue
        {
            get { return Constants.READING_MAX; }
        }

        public string ConnectionString { get; set; }

        private string Name
        {
            get
            {
                string name;
                using (var conn = new SqlConnection(ConnectionString))
                {
                    name = conn.Database;
                }
                return name;
            }
        }

        public SqlOption Options
        {
            set { options = value; }
        }

        private event ProgressEventHandler.ProgressHandler OnReading;
        public event ProgressEventHandler.ProgressHandler OnProgress;
        public event ProgressEventHandler.ProgressHandler OnFinish;

        private void Generate_OnReading(ProgressEventArgs e)
        {
            if (OnProgress != null) OnProgress(e);
        }

        public void RaiseOnReading(ProgressEventArgs e)
        {
            this.currentlyReading = e;
            if (OnReading != null) OnReading(e);
        }

        public void RaiseOnReadingOne(object name)
        {
            if (name != null && this.OnReading != null && this.currentlyReading != null)
            {
                var eOne = new ProgressEventArgs(this.currentlyReading.Message, this.currentlyReading.Progress);
                eOne.Message = eOne.Message.Replace("...", String.Format(": [{0}]", name));
                this.OnReading(eOne);
            }
        }

        /// <summary>
        /// Genera el schema de la base de datos seleccionada y devuelve un objeto Database.
        /// </summary>
        public Database Process()
        {
            string error = "";
            var databaseSchema = new Database();

            //tables.OnTableProgress += new Progress.ProgressHandler(tables_OnTableProgress);
            databaseSchema.Options = options;
            databaseSchema.Name = Name;
            databaseSchema.Info = (new GenerateDatabase(ConnectionString, options)).Get(databaseSchema);
            /*Thread t1 = new Thread(delegate()
                {
                    try
                    {*/
            (new GenerateRules(this)).Fill(databaseSchema, ConnectionString);
            (new GenerateTables(this)).Fill(databaseSchema, ConnectionString, messages);
            (new GenerateViews(this)).Fill(databaseSchema, ConnectionString, messages);

            if (options.Ignore.FilterIndex)
            {
                (new GenerateIndex(this)).Fill(databaseSchema, ConnectionString);
                (new GenerateFullTextIndex(this)).Fill(databaseSchema, ConnectionString);
            }
            (new GenerateUserDataTypes(this)).Fill(databaseSchema, ConnectionString, messages);
            (new GenerateXMLSchemas(this)).Fill(databaseSchema, ConnectionString);
            (new GenerateSchemas(this)).Fill(databaseSchema, ConnectionString);
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

            //not supported in azure yet
            if (databaseSchema.Info.Version != DatabaseInfo.VersionTypeEnum.SQLServerAzure10)
            {
                (new GeneratePartitionFunctions(this)).Fill(databaseSchema, ConnectionString);
                (new GeneratePartitionScheme(this)).Fill(databaseSchema, ConnectionString);
                (new GenerateFileGroups(this)).Fill(databaseSchema, ConnectionString);
            }

            (new GenerateDDLTriggers(this)).Fill(databaseSchema, ConnectionString);
            (new GenerateSynonyms(this)).Fill(databaseSchema, ConnectionString);

            //not supported in azure yet
            if (databaseSchema.Info.Version != DatabaseInfo.VersionTypeEnum.SQLServerAzure10)
            {
                (new GenerateAssemblies(this)).Fill(databaseSchema, ConnectionString);
                (new GenerateFullText(this)).Fill(databaseSchema, ConnectionString);
            }
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
            (new GenerateStoredProcedures(this)).Fill(databaseSchema, ConnectionString);
            (new GenerateFunctions(this)).Fill(databaseSchema, ConnectionString);
            (new GenerateTriggers(this)).Fill(databaseSchema, ConnectionString, messages);
            (new GenerateTextObjects(this)).Fill(databaseSchema, ConnectionString);
            (new GenerateUsers(this)).Fill(databaseSchema, ConnectionString);
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
                (new GenerateExtendedProperties(this)).Fill(databaseSchema, ConnectionString, messages);
                databaseSchema.BuildDependency();
                return databaseSchema;
            }
            else
                throw new SchemaException(error);
        }

        private void tables_OnTableProgress(object sender, ProgressEventArgs e)
        {
            ProgressEventHandler.RaiseOnChange(e);
        }

        // TODO: Static because Compare method is static; static events are not my favorite
        public static event ProgressEventHandler.ProgressHandler OnCompareProgress;

        internal static void RaiseOnCompareProgress(string formatString, params object[] formatParams)
        {
            if (OnCompareProgress != null)
            {
                OnCompareProgress(new ProgressEventArgs(String.Format(formatString, formatParams), -1));
            }
        }

        public static Database Compare(Database databaseOriginalSchema, Database databaseCompareSchema)
        {
            Database merge = CompareDatabase.GenerateDifferences(databaseOriginalSchema, databaseCompareSchema);
            return merge;
        }
    }
}
