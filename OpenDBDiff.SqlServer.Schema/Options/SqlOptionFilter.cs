using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenDBDiff.SqlServer.Schema.Options
{
    public class SqlOptionFilter : IOptionFilter
    {
        public SqlOptionFilter()
        {
            Items = new List<SqlOptionFilterItem>
            {
                new SqlOptionFilterItem(ObjectType.Table, "dbo.dtproperties"),
                new SqlOptionFilterItem(ObjectType.Assembly, "Microsoft.SqlServer.Types"),
                new SqlOptionFilterItem(ObjectType.Schema, "db_accessadmin"),
                new SqlOptionFilterItem(ObjectType.Schema, "db_backupoperator"),
                new SqlOptionFilterItem(ObjectType.Schema, "db_datareader"),
                new SqlOptionFilterItem(ObjectType.Schema, "db_datawriter"),
                new SqlOptionFilterItem(ObjectType.Schema, "db_ddladmin"),
                new SqlOptionFilterItem(ObjectType.Schema, "db_denydatareader"),
                new SqlOptionFilterItem(ObjectType.Schema, "db_denydatawriter"),
                new SqlOptionFilterItem(ObjectType.Schema, "db_owner"),
                new SqlOptionFilterItem(ObjectType.Schema, "db_securityadmin"),
                //new SqlOptionFilterItem(ObjectType.Schema, "dbo"),
                new SqlOptionFilterItem(ObjectType.Schema, "guest"),
                new SqlOptionFilterItem(ObjectType.Schema, "INFORMATION_SCHEMA"),
                new SqlOptionFilterItem(ObjectType.Schema, "sys"),
                new SqlOptionFilterItem(ObjectType.Table, "sysdiagrams"),
                new SqlOptionFilterItem(ObjectType.Function, "fn_diagramobjects"),
                new SqlOptionFilterItem(ObjectType.StoredProcedure, "sp_alterdiagram"),
                new SqlOptionFilterItem(ObjectType.StoredProcedure, "sp_creatediagram"),
                new SqlOptionFilterItem(ObjectType.StoredProcedure, "sp_dropdiagram"),
                new SqlOptionFilterItem(ObjectType.StoredProcedure, "sp_helpdiagramdefinition"),
                new SqlOptionFilterItem(ObjectType.StoredProcedure, "sp_helpdiagrams"),
                new SqlOptionFilterItem(ObjectType.StoredProcedure, "sp_renamediagram"),
                new SqlOptionFilterItem(ObjectType.StoredProcedure, "sp_upgraddiagrams"),
            };
        }

        public SqlOptionFilter(IOptionFilter optionFilter)
        {
            Items = new List<SqlOptionFilterItem>();
            var options = optionFilter.GetOptions();
            foreach (var pair in options)
            {
                Items.Add(
                    new SqlOptionFilterItem(
                        objectType: (ObjectType)Enum.Parse(typeof(ObjectType), pair.Value, true),
                        filterPattern: pair.Key
                    )
                );
            }
        }

        public IList<SqlOptionFilterItem> Items { get; private set; }

        public IDictionary<string, string> GetOptions()
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            for (int i = 0; i < Items.Count; i++)
            {
                values.Add(Items[i].FilterPattern, Items[i].ObjectType.ToString());
            }
            return values;
        }

        public bool IsItemIncluded(ISchemaBase item)
        {
            return !Items.Any(i => i.IsMatch(item));
        }
    }
}
