using OpenDBDiff.Abstractions.Schema;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    public class ObjectDependency
    {
        public ObjectDependency(string name, string Column, ObjectType type)
        {
            this.Name = name;
            this.ColumnName = Column;
            this.Type = type;
        }

        public ObjectDependency(string name, string Column)
        {
            this.Name = name;
            this.ColumnName = Column;
        }

        public string Name { get; set; }

        public string ColumnName { get; set; }

        public ObjectType Type { get; set; }

        public bool IsCodeType
        {
            get { return ((Type == ObjectType.StoredProcedure) || (Type == ObjectType.Trigger) || (Type == ObjectType.View) || (Type == ObjectType.Function)); }

        }
    }
}
