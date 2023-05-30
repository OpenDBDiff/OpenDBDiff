using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;

namespace OpenDBDiff.SqlServer.Schema.SQLQueries
{
    public static class SQLQueryFactory
    {
        private static Dictionary<string, string> queries = new Dictionary<string, string>();

        public static string Get(string queryFullName, Model.DatabaseInfo.SQLServerVersion version)
        {
            return Get($"{queryFullName}.{version}");
        }

        public static string Get(string queryClass)
        {
            var ns = typeof(SQLQueryFactory).Namespace;
            var qualifiedQueryClass = string.Concat(ns, '.', queryClass);

            if (queries.ContainsKey(qualifiedQueryClass))
            {
                return queries[qualifiedQueryClass];
            }
            else
            {
                string query = FetchQuery(qualifiedQueryClass);
                queries.Add(qualifiedQueryClass, query);
                return query;
            }
        }

        private static string FetchQuery(string queryFullName)
        {
            // New SDK-style .csproj format: each <Resource /> embedded within OpenDBDiff.SqlServer.Schema.g.resources
            var assembly = typeof(SQLQueryFactory).Assembly;
            string fileName = $"/{queryFullName.Substring(typeof(SQLQueryFactory).Namespace.Length + 1)}.sql";
            using (var sdkStyleResources = assembly.GetManifestResourceStream(assembly.GetManifestResourceNames().First()))
            using (var resourceReader = new ResourceReader(sdkStyleResources))
            {
                var enumerator = resourceReader.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Key.ToString().EndsWith(fileName, StringComparison.OrdinalIgnoreCase)) // "sqlqueries/*.sql"
                    {
                        using (var stringReader = new StreamReader(enumerator.Value as Stream)) // UnmanagedMemoryStream (PinnedBufferMemoryStream)
                        {
                            return stringReader.ReadToEnd();
                        }
                    }
                }
            }

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
