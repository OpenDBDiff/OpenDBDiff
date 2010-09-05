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

        public View(ISchemaBase parent)
            : base(parent, Enums.ObjectType.View, Enums.ScripActionType.AddView, Enums.ScripActionType.DropView)
        {
            indexes = new SchemaList<Index, View>(this, ((Database)parent).AllObjects);
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
            item.Guid = this.Guid;
            item.IsSchemaBinding = this.IsSchemaBinding;
            item.DependenciesIn  = this.DependenciesIn;
            item.DependenciesOut = this.DependenciesOut;
            item.Indexes = this.Indexes.Clone(item);
            return item;
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

            if (this.Status == Enums.ObjectStatusType.AlterStatus)
            {
                if (!HasTableDependencyToRebuild())
                    list.Add(ToSQLAlter(), 0, Enums.ScripActionType.AlterView);
                else
                {
                    if (((Database)Parent).Options.Script.AlterObjectOnSchemaBinding)
                    {
                        list.Add(ToSQLAlter(true), 0, Enums.ScripActionType.DropView);
                        list.Add(ToSQLAlter(), 0, Enums.ScripActionType.AddView);
                    }
                    else
                    {
                        list.Add(Drop());
                        list.Add(Create());
                    }
                }
            }
            list.AddRange(indexes.ToSqlDiff());
            return list;
        }
    }
}
