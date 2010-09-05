using System;
using DBDiff.Schema.Model;
namespace DBDiff.Schema.SQLServer.Model
{
    public interface ISQLServerSchemaBase
    {
        SchemaList<ExtendedProperty, ISchemaBase> ExtendedProperties { get; }
    }
}
