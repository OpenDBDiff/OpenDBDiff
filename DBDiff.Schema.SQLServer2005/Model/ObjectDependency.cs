namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class ObjectDependency
    {
        public ObjectDependency(string name, string Column, Enums.ObjectType type)
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

        public Enums.ObjectType Type { get; set; }

        public bool IsCodeType
        {
            get { return ((Type == Enums.ObjectType.StoredProcedure) || (Type == Enums.ObjectType.Trigger) || (Type == Enums.ObjectType.View) || (Type == Enums.ObjectType.Function)); }

        }
    }
}
