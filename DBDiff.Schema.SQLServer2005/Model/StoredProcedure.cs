using System;
using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model.Util;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class StoredProcedure : Code
    {
        public StoredProcedure(ISchemaBase parent)
            : base(parent, Enums.ObjectType.StoredProcedure, Enums.ScripActionType.AddStoredProcedure, Enums.ScripActionType.DropStoredProcedure)
        {

        }

        /// <summary>
        /// Clona el objeto en una nueva instancia.
        /// </summary>
        public override ISchemaBase Clone(ISchemaBase parent)
        {
            StoredProcedure item = new StoredProcedure(parent);
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
            //if (String.IsNullOrEmpty(sql))
            sql = FormatCode.FormatCreate("PROC(EDURE)?", Text, this);
            return sql;
        }

        public string ToSQLAlter()
        {
            return FormatCode.FormatAlter("PROC(EDURE)?", ToSql(), this, false);
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList list = new SQLScriptList();
            if (this.Status != Enums.ObjectStatusType.OriginalStatus)
                RootParent.ActionMessage.Add(this);

            if (this.HasState(Enums.ObjectStatusType.DropStatus))
                list.Add(Drop());
            if (this.HasState(Enums.ObjectStatusType.CreateStatus))
                list.Add(Create());
            if (this.HasState(Enums.ObjectStatusType.AlterStatus))
                list.Add(ToSQLAlter(), 0, Enums.ScripActionType.AlterProcedure);
            if (this.HasState(Enums.ObjectStatusType.AlterWhitespaceStatus))
                list.Add(ToSQLAlter(), 0, Enums.ScripActionType.AlterProcedure);
            return list;
        }
    }
}
