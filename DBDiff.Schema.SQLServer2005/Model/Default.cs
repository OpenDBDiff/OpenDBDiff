using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Default:SQLServerSchemaBase 
    {
        private string value;

        public Default(ISchemaBase parent):base(StatusEnum.ObjectTypeEnum.Default)
        {
            this.Parent = parent;            
        }

        public Default Clone(ISchemaBase parent)
        {
            Default item = new Default(parent);
            item.Id = this.Id;
            item.Name = this.Name;
            item.Owner = this.Owner;
            item.Value = this.Value;
            return item;
        }
        
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public string ToSQLAddBind()
        {
            string sql = "";
            sql += "EXEC sp_bindefault N'" + Name + "', N'" + this.Parent.Name + "'\r\nGO\r\n";
            return sql;
        }

        public string ToSQLAddUnBind()
        {
            string sql = "";
            sql += "EXEC sp_unbindefault @objname=N'" + this.Parent.Name + "'\r\nGO\r\n";
            return sql;
        }

        public override string ToSQLAdd()
        {
            return "";
        }

        public override string ToSQLDrop()
        {
            return "";
        }
    }
}
