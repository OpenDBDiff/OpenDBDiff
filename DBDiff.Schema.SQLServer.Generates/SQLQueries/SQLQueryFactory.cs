using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBDiff.Schema.SQLServer.Generates.Generates.SQLQueries
{
    public static class SQLQueryFactory
    {
        private static Dictionary<string, string> queries = new Dictionary<string, string>();

        public static string Get(string queryFullName, Model.DatabaseInfo.VersionTypeEnum version)
        {
            return Get($"{queryFullName}.{version}");
        }
        public static string Get(string queryFullName)
        {
            if (queries.ContainsKey(queryFullName))
            {
                return queries[queryFullName];
            }
            else
            {
                string query = FetchQuery(queryFullName);
                queries.Add(queryFullName, query);
                return query;
            }
        }

        private static string FetchQuery(string queryFullName)
        {
            string resourceName = queryFullName + ".sql";
            using (System.IO.Stream stream = typeof(SQLQueryFactory).Assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) throw new InvalidOperationException("The Query " + queryFullName + " cannot be found");
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
