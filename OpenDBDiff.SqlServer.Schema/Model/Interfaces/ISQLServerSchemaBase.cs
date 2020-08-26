using OpenDBDiff.Abstractions.Schema.Model;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    public interface ISQLServerSchemaBase
    {
        SchemaList<ExtendedProperty, ISchemaBase> ExtendedProperties { get; }
    }
}
