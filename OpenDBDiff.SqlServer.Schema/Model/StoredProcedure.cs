using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model.Util;
using System;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    public class StoredProcedure : Code
    {
        public StoredProcedure(ISchemaBase parent)
            : base(parent, ObjectType.StoredProcedure, ScriptAction.AddStoredProcedure, ScriptAction.DropStoredProcedure)
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
        public override SQLScriptList ToSqlDiff(System.Collections.Generic.ICollection<ISchemaBase> schemas)
        {
            SQLScriptList list = new SQLScriptList();
            if (this.Status != ObjectStatus.Original)
                RootParent.ActionMessage.Add(this);

            if (this.HasState(ObjectStatus.Drop))
                list.Add(Drop());
            if (this.HasState(ObjectStatus.Create))
                list.Add(Create());
            if (this.HasState(ObjectStatus.Alter))
                list.Add(ToSQLAlter(), 0, ScriptAction.AlterProcedure);
            if (this.HasState(ObjectStatus.AlterWhitespace))
                list.Add(ToSQLAlter(), 0, ScriptAction.AlterProcedure);
            return list;
        }
    }
}
