using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.MySQL.Model
{
    public abstract class MySQLSchemaBase:SchemaBase
    {
        public MySQLSchemaBase(StatusEnum.ObjectTypeEnum objectType)
            : base("`", "`", objectType)
        {
            this.Owner = "";
        }

        public override string ToSQLDrop()
        {
            return "";
        }

        public override string ToSQLAdd()
        {
            return "";
        }
    }
}
