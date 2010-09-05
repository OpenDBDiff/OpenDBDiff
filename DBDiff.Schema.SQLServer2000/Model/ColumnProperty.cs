using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer2000.Model
{
    public class ColumnProperty
    {
        public enum ColumnPropertyType
        {
            TextInRowType = 1,
            DescriptionType = 2,
            FullTextType = 3
        }

        private ColumnPropertyType type;
        private string value;
        private StatusEnum.ObjectStatusType status;

        public StatusEnum.ObjectStatusType Status
        {
            get { return status; }
            set { status = value; }
        }

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public ColumnPropertyType Type
        {
            get { return type; }
            set { type = value; }
        }

    }
}
