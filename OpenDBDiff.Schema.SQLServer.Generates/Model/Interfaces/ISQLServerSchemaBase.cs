using OpenDBDiff.Schema.Model;

namespace OpenDBDiff.Schema.SQLServer.Generates.Model
{
    public interface ISQLServerSchemaBase
    {
        SchemaList<ExtendedProperty, ISchemaBase> ExtendedProperties { get; }
    }
}
