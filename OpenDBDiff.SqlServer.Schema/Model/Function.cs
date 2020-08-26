using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model.Util;
using System;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    public class Function : Code
    {
        public Function(ISchemaBase parent)
            : base(parent, ObjectType.Function, ScriptAction.AddFunction, ScriptAction.DropFunction)
        {

        }

        /// <summary>
        /// Clona el objeto en una nueva instancia.
        /// </summary>
        public override ISchemaBase Clone(ISchemaBase parent)
        {
            Function item = new Function(parent);
            item.Text = this.Text;
            item.Status = this.Status;
            item.Name = this.Name;
            item.Id = this.Id;
            item.Owner = this.Owner;
            item.Guid = this.Guid;
            item.IsSchemaBinding = this.IsSchemaBinding;
            this.DependenciesIn.ForEach(dep => item.DependenciesIn.Add(dep));
            this.DependenciesOut.ForEach(dep => item.DependenciesOut.Add(dep));
            return item;
        }

        public override Boolean IsCodeType
        {
            get { return true; }
        }

        public string ToSQLAlter()
        {
            return ToSQLAlter(false);
        }

        public string ToSQLAlter(Boolean quitSchemaBinding)
        {
            return FormatCode.FormatAlter("FUNCTION", ToSql(), this, quitSchemaBinding);
        }

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
            {
                if (this.HasState(ObjectStatus.RebuildDependencies))
                    list.AddRange(RebuildDependencies());

                if (!this.GetWasInsertInDiffList(ScriptAction.DropFunction))
                {
                    if (this.HasState(ObjectStatus.Rebuild))
                    {
                        list.Add(Drop());
                        list.Add(Create());
                    }
                    if (this.HasState(ObjectStatus.AlterBody))
                    {
                        int iCount = DependenciesCount;
                        list.Add(ToSQLAlter(), iCount, ScriptAction.AlterFunction);
                    }
                }
            }
            return list;
        }
    }
}
