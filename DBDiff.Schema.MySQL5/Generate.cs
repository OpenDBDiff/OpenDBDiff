using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using DBDiff.Schema.Events;
using DBDiff.Schema.MySQL.Options;
using DBDiff.Schema.MySQL.Compare;
using DBDiff.Schema.MySQL.Model;
using DBDiff.Schema.MySQL.Generates;

namespace DBDiff.Schema.MySQL
{
    public class Generate
    {
        public enum VersionTypeEnum
        {
            None = 0,
            MySQL50 = 1,
            MySQL51 = 2,
            MySQL60 = 3
        }

        public event Progress.ProgressHandler OnTableProgress;
        private string connectioString;

        public string ConnectioString
        {
            get { return connectioString; }
            set { connectioString = value; }
        }

        /// <summary>
        /// Genera el schema de la base de datos seleccionada y devuelve un objeto Database.
        /// </summary>
        public Database Process(MySqlOption filters)
        {            
            Database databaseSchema = new Database();
            GenerateTables tables = new GenerateTables(connectioString, filters);

            tables.OnTableProgress += new Progress.ProgressHandler(tables_OnTableProgress);
            databaseSchema.Tables = tables.Get(databaseSchema);

            return databaseSchema;
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
            Database merge = null;
            merge = CompareDatabase.GenerateDiferences(databaseOriginalSchema, databaseCompareSchema);
            return merge;
        }
    }
}
