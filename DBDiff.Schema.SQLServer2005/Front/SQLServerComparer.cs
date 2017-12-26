using DBDiff.Front;
using System;
using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Generates;

namespace DBDiff.Schema.SQLServer.Generates.Front
{
    public class SQLServerComparer : IDatabaseComparer
    {
        public IDatabase Compare(IDatabase source, IDatabase destination)
        {
            if (source is Model.Database && destination is Model.Database)
            {
                return Generate.Compare(source as Model.Database, destination as Model.Database);
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