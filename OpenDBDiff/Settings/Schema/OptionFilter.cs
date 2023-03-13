using OpenDBDiff.SqlServer.Schema.Options;
using System.Collections.Generic;

namespace OpenDBDiff.Settings.Schema
{
    public class OptionFilter
    {
        public IList<SqlOptionFilterItem> Items { get; set; }
    }
}
