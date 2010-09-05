using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Model.Util;

namespace DBDiff.Schema.SQLServer.Model
{
    public class StoreProcedure : Code 
    {
        public StoreProcedure(ISchemaBase parent)
            : base(parent, Enums.ObjectType.StoreProcedure, Enums.ScripActionType.AddStoreProcedure, Enums.ScripActionType.DropStoreProcedure)
        {

        }

        /// <summary>
        /// Clona el objeto en una nueva instancia.
        /// </summary>
        public StoreProcedure Clone(ISchemaBase parent)
        {
            StoreProcedure item = new StoreProcedure(parent);
            item.Text = this.Text;
            item.Status = this.Status;
            item.Name = this.Name;
            item.Id = this.Id;
            item.Owner = this.Owner;
            item.Guid = this.Guid;
            return item;
        }

        public override Boolean IsCodeType
        {
            get { return true; }
        }

        public override string ToSql()
        {
            if (String.IsNullOrEmpty(sql))
                sql = FormatCode.FormatCreate("PROCEDURE;PROC", Text, this);
            return sql;
        }

        public string ToSQLAlter()
        {
            return FormatCode.FormatAlter("PROCEDURE;PROC", ToSql(), this, false);
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList list = new SQLScriptList();

            if (this.HasState(Enums.ObjectStatusType.DropStatus))
                list.Add(Drop());
            if (this.HasState(Enums.ObjectStatusType.CreateStatus))
                list.Add(Create());
            if (this.Status == Enums.ObjectStatusType.AlterStatus)
                list.Add(ToSQLAlter(), 0, Enums.ScripActionType.AlterProcedure);
            return list;
        }
    }
}
