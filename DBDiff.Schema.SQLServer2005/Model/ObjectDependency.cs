namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class ObjectDependency
    {
        private string columnName;
        private string name;
        private Enums.ObjectType type;

        public ObjectDependency(string name, string Column, Enums.ObjectType type)
        {
            this.name = name;
            this.columnName = Column;
            this.type = type;
        }

        public ObjectDependency(string name, string Column)
        {
            this.name = name;
            this.columnName = Column;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string ColumnName
        {
            get { return columnName; }
            set { columnName = value; }
        }

        public Enums.ObjectType Type
        {
            get { return type; }
            set { type = value; }
        }

        public bool IsCodeType
        {
            get { return ((type == Enums.ObjectType.StoreProcedure) || (type == Enums.ObjectType.Trigger) || (type == Enums.ObjectType.View) || (type == Enums.ObjectType.Function)); }

        }
    }
}
