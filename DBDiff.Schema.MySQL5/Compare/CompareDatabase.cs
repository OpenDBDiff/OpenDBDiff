using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.MySQL.Model;

namespace DBDiff.Schema.MySQL.Compare
{
    internal static class CompareDatabase
    {        
        public static Database GenerateDiferences(Database Origen, Database Destino)
        {
            Database data = Origen;
            data.Tables = CompareTables.GenerateDiferences(Origen.Tables, Destino.Tables);
            return data;
        }
    }
}
