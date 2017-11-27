using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public interface ISQLServerSchemaBase
    {
        SchemaList<ExtendedProperty, ISchemaBase> ExtendedProperties { get; }
    }
}
