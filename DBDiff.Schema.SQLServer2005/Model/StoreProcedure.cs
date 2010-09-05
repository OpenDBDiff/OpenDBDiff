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
            : base(parent, Enums.ObjectType.StoreProcedure)
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

        public override SQLScript Create()
        {
            Enums.ScripActionType action = Enums.ScripActionType.AddStoreProcedure;
            if (!GetWasInsertInDiffList(action))
            {
                SetWasInsertInDiffList(action);
                return new SQLScript(this.ToSqlAdd(), 0, action);
            }
            else
                return null;

        }

        public override SQLScript Drop()
        {
            Enums.ScripActionType action = Enums.ScripActionType.DropStoreProcedure;
            if (!GetWasInsertInDiffList(action))
            {
                SetWasInsertInDiffList(action);
                return new SQLScript(this.ToSqlDrop(), 0, action);
            }
            else
                return null;
        }
        /// <summary>
        /// Compara dos store procedures y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(StoreProcedure origen, StoreProcedure destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (!origen.ToSql().Equals(destino.ToSql())) return false;
            return true;
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public SQLScriptList ToSQLDiff()
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
