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
        private Indexes indexes;

        public View(ISchemaBase parent)
            : base(parent, Enums.ObjectType.View)
        {
            indexes = new Indexes(this);
        }

        /// <summary>
        /// Clona el objeto en una nueva instancia.
        /// </summary>
        public View Clone(ISchemaBase parent)
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
            item.Indexes = this.Indexes.Clone(parent);
            return item;
        }

        public Indexes Indexes
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

        public override SQLScript Create()
        {            
            int iCount = DependenciesCount;
            Enums.ScripActionType action = Enums.ScripActionType.AddView;
            if (!GetWasInsertInDiffList(action))
            {
                SetWasInsertInDiffList(action);
                return new SQLScript(this.ToSqlAdd() + indexes.ToSQL(), iCount * -1, action);
            }
            else
                return null;

        }

        public override SQLScript Drop()
        {
            int iCount = DependenciesCount;
            Enums.ScripActionType action = Enums.ScripActionType.DropView;
            if (!GetWasInsertInDiffList(action))
            {
                SetWasInsertInDiffList(action);
                return new SQLScript(this.ToSqlDrop(), iCount, action);
            }
            else
                return null;
        }

        /// <summary>
        /// Compara dos store procedures y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(View origen, View destino)
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
            list.AddRange(indexes.ToSQLDiff());
            return list;
        }
    }
}
