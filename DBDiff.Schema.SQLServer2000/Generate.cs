using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using DBDiff.Schema.Events;
using DBDiff.Schema.SQLServer2000.Model;
using DBDiff.Schema.SQLServer2000.Compare;

namespace DBDiff.Schema.SQLServer2000
{    
    public class Generate
    {
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
        public Database Process()
        {            
            Database databaseSchema = new Database();
            GenerateTables tables = new GenerateTables(connectioString);
            GenerateUserDataTypes types = new GenerateUserDataTypes(connectioString);

            tables.OnTableProgress += new Progress.ProgressHandler(tables_OnTableProgress);
            databaseSchema.Tables = tables.Get(databaseSchema);
            databaseSchema.UserTypes = types.Get(databaseSchema);

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
            Database merge;
            CompareDatabase ftables = new CompareDatabase();
            merge = ftables.GenerateDiferences(databaseOriginalSchema, databaseCompareSchema);
            return merge;
        }
    }
}
