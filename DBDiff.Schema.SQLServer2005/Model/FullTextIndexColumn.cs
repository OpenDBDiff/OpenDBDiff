using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class FullTextIndexColumn
    {
        private string columnName;
        private string language;

        public string Language
        {
            get { return language; }
            set { language = value; }
        }

        public string ColumnName
        {
            get { return columnName; }
            set { columnName = value; }
        }

    }
}
