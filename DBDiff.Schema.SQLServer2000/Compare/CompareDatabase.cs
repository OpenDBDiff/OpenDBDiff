using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.SQLServer2000.Model;

namespace DBDiff.Schema.SQLServer2000.Compare
{
    internal class CompareDatabase
    {
        
        public Database GenerateDiferences(Database Origen, Database Destino)
        {
            Database data = Origen;
            data.Tables = (new CompareTables()).GenerateDiferences(Origen.Tables, Destino.Tables);
            data.UserTypes = (new CompareUserDataTypes()).GenerateDiferences(Origen.UserTypes, Destino.UserTypes);
            //sql += CreateTables(Origen.Tables, Destino.Tables);

            return data;
        }
    }
}
