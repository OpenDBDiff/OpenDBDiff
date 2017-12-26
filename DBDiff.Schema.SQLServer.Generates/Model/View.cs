using System;
using DBDiff.Schema.Attributes;
using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Generates.Model.Util;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class View : Code
    {
        public View(ISchemaBase parent)
            : base(parent, Enums.ObjectType.View, Enums.ScripActionType.AddView, Enums.ScripActionType.DropView)
        {
            Indexes = new SchemaList<Index, View>(this, ((Database)parent).AllObjects);
            Triggers = new SchemaList<Trigger, View>(this, ((Database)parent).AllObjects);
            CLRTriggers = new SchemaList<CLRTrigger, View>(this, ((Database)parent).AllObjects);
        }

        /// <summary>
        /// Clona el objeto en una nueva instancia.
        /// </summary>
        public override ISchemaBase Clone(ISchemaBase parent)
        {
            View item = new View(parent);
            item.Text = this.Text;
            item.Status = this.Status;
            item.Name = this.Name;
            item.Id = this.Id;
            item.Owner = this.Owner;
            item.IsSchemaBinding = this.IsSchemaBinding;
            item.DependenciesIn = this.DependenciesIn;
            item.DependenciesOut = this.DependenciesOut;
            item.Indexes = this.Indexes.Clone(item);
            item.Triggers = this.Triggers.Clone(item);
            return item;
        }

        [ShowItem("CLR Triggers")]
        public SchemaList<CLRTrigger, View> CLRTriggers { get; set; }

        [ShowItem("Triggers")]
        public SchemaList<Trigger, View> Triggers { get; set; }

        [ShowItem("Indexes", "Index")]
        public SchemaList<Index, View> Indexes { get; set; }

        public override Boolean IsCodeType
        {
            get { return true; }
        }

        public override string ToSqlAdd()
        {
            string sql = ToSql();
            this.Indexes.ForEach(item =>
                {
                    if (item.Status != Enums.ObjectStatusType.DropStatus)
                    {
                        item.SetWasInsertInDiffList(Enums.ScripActionType.AddIndex);
                        sql += item.ToSql();
                    }
                }
            );
            this.Triggers.ForEach(item =>
                {
                    if (item.Status != Enums.ObjectStatusType.DropStatus)
                    {
                        item.SetWasInsertInDiffList(Enums.ScripActionType.AddTrigger);
                        sql += item.ToSql();
                    }
                }
            );

            sql += this.ExtendedProperties.ToSql();
            return sql;
        }

        public string ToSQLAlter()
        {
            return ToSQLAlter(false);
        }

        public string ToSQLAlter(Boolean quitSchemaBinding)
        {
            return FormatCode.FormatAlter("VIEW", ToSql(), this, quitSchemaBinding);
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
            {
                if (this.HasState(Enums.ObjectStatusType.RebuildDependenciesStatus))
                    list.AddRange(RebuildDependencys());
                if (this.HasState(Enums.ObjectStatusType.RebuildStatus))
                {
                    list.Add(Drop());
                    list.Add(Create());
                }
                if (this.HasState(Enums.ObjectStatusType.AlterBodyStatus))
                {
                    int iCount = DependenciesCount;
                    list.Add(ToSQLAlter(), iCount, Enums.ScripActionType.AlterView);
                }
                if (!this.GetWasInsertInDiffList(Enums.ScripActionType.DropFunction) && (!this.GetWasInsertInDiffList(Enums.ScripActionType.AddFunction)))
                    list.AddRange(Indexes.ToSqlDiff());

                list.AddRange(Triggers.ToSqlDiff());
            }
            return list;
        }
    }
}
