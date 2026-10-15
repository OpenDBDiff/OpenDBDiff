using System;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Generates;
using OpenDBDiff.SqlServer.Schema.Model;

namespace OpenDBDiff.SqlServer.Driver
{
    /// <summary>
    /// Compares two SQL Server schema snapshots.
    /// </summary>
    public class SqlServerComparer : IDatabaseComparer
    {
        public IDatabase Compare(IDatabase origin, IDatabase destination)
        {
            if (origin is Database && destination is Database)
            {
                return Generate.Compare(origin as Database, destination as Database);
            }

            if (!(origin is Database))
            {
                throw new NotSupportedException("Origin database type not supported: " + origin.GetType());
            }

            throw new NotSupportedException("Destination database type not supported: " + destination.GetType());
        }
    }
}
