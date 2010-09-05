using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using DBDiff.Schema.Events;
using DBDiff.Schema.Sybase.Model;
using DBDiff.Schema.Sybase.Options;
using DBDiff.Schema.Sybase.Generates;

namespace DBDiff.Schema.Sybase
{
    public class Generate
    {
        public enum VersionTypeEnum
        {
            None = 0,
            v125 = 1,
            v150 = 2,
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
        public Database Process(AseOption filters)
        {            
            Database databaseSchema = new Database();
            GenerateTables tables = new GenerateTables(connectioString, filters);
            //GenerateUserDataTypes types = new GenerateUserDataTypes(connectioString, filters);
            //GenerateStoreProcedures procedures = new GenerateStoreProcedures(connectioString, filters);

            tables.OnTableProgress += new Progress.ProgressHandler(tables_OnTableProgress);
            databaseSchema.Tables = tables.Get(databaseSchema);
            //databaseSchema.UserTypes = types.Get(databaseSchema);
            //databaseSchema.Procedures = procedures.Get(databaseSchema);

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
            //merge = CompareDatabase.GenerateDiferences(databaseOriginalSchema, databaseCompareSchema);
            return merge;
        }
    }
}
