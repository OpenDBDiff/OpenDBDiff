using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace DBDiff.Schema.SQLServer.Options
{
    public class SqlOptionFilter
    {
        public enum ObjectType
        {
            All = 0,
            Assembly = 1,
            Constraint = 2,
            [Description("DDL Trigger")]
            DDLTriger = 3,
            Default = 4,
            Function = 5,
            Index = 6,
            Role = 7,
            Rule = 8,
            Schema = 9,
            [Description("Store Procedure")]
            StoreProcedure = 10,
            Synonym = 11,
            Table = 12,
            Trigger = 13,
            User = 14,
            [Description("User Data Type")]
            UserDataType = 15,
            View = 16,
            XMLSchema = 17
        }

        private Collection<SqlOptionFilterItem> items;

        public SqlOptionFilter()
        {
            items = new Collection<SqlOptionFilterItem>();
            Items.Add(new SqlOptionFilterItem(ObjectType.Table,"dbo.dtproperties"));
        }

        public Collection<SqlOptionFilterItem> Items
        {
            get { return items; }
        }

    }
}
