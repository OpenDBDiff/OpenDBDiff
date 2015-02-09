namespace DBDiff.Schema.SQLServer.Generates.Options
{
    public class SqlOptionFilterItem
    {
        public SqlOptionFilterItem(Enums.ObjectType type, string value)
        {
            this.Filter = value;
            this.Type = type;
        }

        public Enums.ObjectType Type { get; set; }

        public string Filter { get; set; }
    }
}
