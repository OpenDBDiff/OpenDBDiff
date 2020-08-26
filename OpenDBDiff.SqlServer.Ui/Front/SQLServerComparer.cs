using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.Abstractions.Ui;
using OpenDBDiff.SqlServer.Schema.Generates;
using OpenDBDiff.SqlServer.Schema.Model;
using System;

namespace OpenDBDiff.SqlServer.Ui
{
    public class SQLServerComparer : IDatabaseComparer
    {
        public IDatabase Compare(IDatabase origin, IDatabase destination)
        {
            if (origin is Database && destination is Database)
            {
                return Generate.Compare(origin as Database, destination as Database);
            }
            else if (!(origin is Database))
            {
                throw new NotSupportedException("Origin database type not supported: " + origin.GetType());
            }
            else
            {
                throw new NotSupportedException("Destination database type not supported: " + destination.GetType());
            }
        }
    }
}
