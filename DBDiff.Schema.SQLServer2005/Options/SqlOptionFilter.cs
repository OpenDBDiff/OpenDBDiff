using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using DBDiff.Schema;

namespace DBDiff.Schema.SQLServer.Generates.Options
{
    public class SqlOptionFilter
    {
        private Collection<SqlOptionFilterItem> items;

        public SqlOptionFilter()
        {
            items = new Collection<SqlOptionFilterItem>();
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Table,"dbo.dtproperties"));
        }

        public Collection<SqlOptionFilterItem> Items
        {
            get { return items; }
        }

    }
}
