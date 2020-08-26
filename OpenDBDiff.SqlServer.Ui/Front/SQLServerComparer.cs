using OpenDBDiff.Schema.Model;
using OpenDBDiff.Schema.SQLServer.Generates.Generates;
using OpenDBDiff.Abstractions.Ui;
using System;

namespace OpenDBDiff.SqlServer.Ui
{
    public class SQLServerComparer : IDatabaseComparer
    {
        public IDatabase Compare(IDatabase origin, IDatabase destination)
        {
            if (origin is Schema.SQLServer.Generates.Model.Database && destination is Schema.SQLServer.Generates.Model.Database)
            {
                return Generate.Compare(origin as Schema.SQLServer.Generates.Model.Database, destination as Schema.SQLServer.Generates.Model.Database);
            }
            else if (!(origin is Schema.SQLServer.Generates.Model.Database))
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
