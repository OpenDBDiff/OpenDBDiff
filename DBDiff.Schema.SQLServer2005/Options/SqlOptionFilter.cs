using System.Collections.ObjectModel;

namespace DBDiff.Schema.SQLServer.Generates.Options
{
    public class SqlOptionFilter
    {
        public SqlOptionFilter()
        {
            Items = new Collection<SqlOptionFilterItem>();
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Table, "dbo.dtproperties"));
            /* // TODO: Filter by name doesn't seem to be implemented?
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Assembly, "Microsoft.SqlServer.Types"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "db_accessadmin"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "db_backupoperator"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "db_datareader"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "db_datawriter"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "db_ddladmin"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "db_denydatareader"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "db_denydatawriter"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "db_owner"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "db_securityadmin"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "dbo"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "guest"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "INFORMATION_SCHEMA"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "sys"));
            //*/
        }

        public Collection<SqlOptionFilterItem> Items { get; private set; }
    }
}
