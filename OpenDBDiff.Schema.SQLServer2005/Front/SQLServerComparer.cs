using OpenDBDiff.Schema.Model;
using OpenDBDiff.Schema.SQLServer.Generates.Generates;
using OpenDBDiff.Abstractions.Ui;
using System;

namespace OpenDBDiff.Schema.SQLServer.Generates.Front
{
    public class SQLServerComparer : IDatabaseComparer
    {
        public IDatabase Compare(IDatabase origin, IDatabase destination)
        {
            if (origin is Model.Database && destination is Model.Database)
            {
                return Generate.Compare(origin as Model.Database, destination as Model.Database);
            }
            else if (!(origin is Model.Database))
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
