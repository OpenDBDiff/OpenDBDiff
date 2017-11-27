using System;
using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model.Util;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class Function : Code
    {
        public Function(ISchemaBase parent)
            : base(parent, Enums.ObjectType.Function, Enums.ScripActionType.AddFunction, Enums.ScripActionType.DropFunction)
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
            {
                if (this.HasState(Enums.ObjectStatusType.RebuildDependenciesStatus))
                    list.AddRange(RebuildDependencys());

                if (!this.GetWasInsertInDiffList(Enums.ScripActionType.DropFunction))
                {
                    if (this.HasState(Enums.ObjectStatusType.RebuildStatus))
                    {
                        list.Add(Drop());
                        list.Add(Create());
                    }
                    if (this.HasState(Enums.ObjectStatusType.AlterBodyStatus))
                    {
                        int iCount = DependenciesCount;
                        list.Add(ToSQLAlter(), iCount, Enums.ScripActionType.AlterFunction);
                    }
                }
            }
            return list;
        }
    }
}
