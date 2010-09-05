using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Schema : SQLServerSchemaBase
    {
        public Schema(Database parent):base(StatusEnum.ObjectTypeEnum.Schema)
        {
            this.Parent = parent;            
        }

        public string ToSQL()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("CREATE SCHEMA ");
            sql.Append("[" + this.Name + "] AUTHORIZATION [" + Owner + "]");
            sql.Append("\r\nGO");
            return sql.ToString();
        }

        public override string ToSQLAdd()
        {
            return ToSQL();
        }

        public override string ToSQLDrop()
        {
            return "DROP SCHEMA [" + Name + "]\r\nGO\r\n";
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == StatusEnum.ObjectStatusType.DropStatus)
            {
                listDiff.Add(ToSQLDrop(), 0, StatusEnum.ScripActionType.DropSchema);
            }
            if (this.Status == StatusEnum.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSQL(), 0, StatusEnum.ScripActionType.AddSchema);
            }
            return listDiff;
        }
    }
}
