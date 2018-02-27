using OpenDBDiff.Schema.Model;

namespace OpenDBDiff.Schema.SQLServer.Generates.Model
{
    public abstract class SQLServerSchemaBase : SchemaBase, ISQLServerSchemaBase
    {
        protected SQLServerSchemaBase(ISchemaBase parent, ObjectType objectType) : base("[", "]", objectType)
        {
            this.Parent = parent;
            ExtendedProperties = new SchemaList<ExtendedProperty, ISchemaBase>(parent);
        }

        public SchemaList<ExtendedProperty, ISchemaBase> ExtendedProperties { get; private set; }
    }
}
