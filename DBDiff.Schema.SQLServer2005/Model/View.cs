using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DBDiff.Schema.Model;
using DBDiff.Schema.SQLServer.Model.Util;

namespace DBDiff.Schema.SQLServer.Model
{
    public class View : Code 
    {
        private SchemaList<Index, View> indexes;
        private SchemaList<Trigger, View> triggers;

        public View(ISchemaBase parent)
            : base(parent, Enums.ObjectType.View, Enums.ScripActionType.AddView, Enums.ScripActionType.DropView)
        {
            indexes = new SchemaList<Index, View>(this, ((Database)parent).AllObjects);
            triggers = new SchemaList<Trigger, View>(this, ((Database)parent).AllObjects);
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
            item.DependenciesIn  = this.DependenciesIn;
            item.DependenciesOut = this.DependenciesOut;
            item.Indexes = this.Indexes.Clone(item);
            item.Triggers = this.Triggers.Clone(item);
            return item;
        }

        public SchemaList<Trigger, View> Triggers
        {
            get { return triggers; }
            set { triggers = value; }
        }

        public SchemaList<Index, View> Indexes
        {
            get { return indexes; }
            set { indexes = value; }
        }

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
                        sql += item.ToSql();
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
                    list.AddRange(indexes.ToSqlDiff());
            }
            return list;
        }
    }
}
