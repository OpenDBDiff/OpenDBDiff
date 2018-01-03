using OpenDBDiff.Front;
using System;
using OpenDBDiff.Schema.Model;
using OpenDBDiff.Schema.SQLServer.Generates.Generates;

namespace OpenDBDiff.Schema.SQLServer.Generates.Front
{
    public class SQLServerComparer : IDatabaseComparer
    {
        public IDatabase Compare(IDatabase source, IDatabase destination)
        {
            if (source is Model.Database && destination is Model.Database)
            {
                return Generate.Compare(destination as Model.Database, source as Model.Database);
            }
            else if (!(source is Model.Database))
            {
                throw new NotSupportedException("Source Database type not supported: " + source.GetType());
            }
            else
            {
                throw new NotSupportedException("Destination Database type not supported: " + source.GetType());
            }
        }
    }
}