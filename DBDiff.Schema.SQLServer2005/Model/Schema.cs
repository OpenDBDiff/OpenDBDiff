using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Schema : SQLServerSchemaBase
    {
        public Schema(Database parent):base(Enums.ObjectType.Schema)
        {
            this.Parent = parent;            
        }

        public override string ToSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("CREATE SCHEMA ");
            sql.Append("[" + this.Name + "] AUTHORIZATION [" + Owner + "]");
            sql.Append("\r\nGO\r\n");
            return sql.ToString();
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        public override string ToSqlDrop()
        {
            return "DROP SCHEMA [" + Name + "]\r\nGO\r\n";
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == Enums.ObjectStatusType.DropStatus)
            {
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropSchema);
            }
            if (this.Status == Enums.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSql(), 0, Enums.ScripActionType.AddSchema);
            }
            return listDiff;
        }
    }
}
