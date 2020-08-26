using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Attributes;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Model.Util;
using System;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    public class View : Code
    {
        public View(ISchemaBase parent)
            : base(parent, ObjectType.View, ScriptAction.AddView, ScriptAction.DropView)
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

        [SchemaNode("CLR Triggers")]
        public SchemaList<CLRTrigger, View> CLRTriggers { get; set; }

        [SchemaNode("Triggers")]
        public SchemaList<Trigger, View> Triggers { get; set; }

        [SchemaNode("Indexes", "Index")]
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
                    if (item.Status != ObjectStatus.Drop)
                    {
                        item.SetWasInsertInDiffList(ScriptAction.AddIndex);
                        sql += item.ToSql();
                    }
                }
            );
            this.Triggers.ForEach(item =>
                {
                    if (item.Status != ObjectStatus.Drop)
                    {
                        item.SetWasInsertInDiffList(ScriptAction.AddTrigger);
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
                if (this.HasState(ObjectStatus.Rebuild))
                {
                    list.Add(Drop());
                    list.Add(Create());
                }
                if (this.HasState(ObjectStatus.AlterBody))
                {
                    int iCount = DependenciesCount;
                    list.Add(ToSQLAlter(), iCount, ScriptAction.AlterView);
                }
                if (!this.GetWasInsertInDiffList(ScriptAction.DropFunction) && (!this.GetWasInsertInDiffList(ScriptAction.AddFunction)))
                    list.AddRange(Indexes.ToSqlDiff());

                list.AddRange(Triggers.ToSqlDiff());
            }
            return list;
        }
    }
}
