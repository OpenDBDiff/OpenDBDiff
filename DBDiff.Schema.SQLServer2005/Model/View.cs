using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class View : SQLServerSchemaBase 
    {
        private string text;
        private Boolean isSchemaBinding;
        private Dictionary<int, string> dependencies;
        private Indexes indexes;

        public View(ISchemaBase parent) : base(StatusEnum.ObjectTypeEnum.View)
        {
            this.Parent = parent;
            dependencies = new Dictionary<int, string>();
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
            item.Dependencies = this.Dependencies;
            item.Indexes = this.Indexes.Clone(parent);
            return item;
        }

        public Dictionary<int, string> Dependencies
        {
            get { return dependencies; }
            set { dependencies = value; }
        }

        public Indexes Indexes
        {
            get { return indexes; }
            set { indexes = value; }
        }

        public Boolean IsSchemaBinding
        {
            get { return isSchemaBinding; }
            set { isSchemaBinding = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public string ToSQL()
        {
            return text + "GO\r\n";
        }

        public override string ToSQLAdd()
        {
            return ToSQL();
        }

        public override string ToSQLDrop()
        {
            return "DROP VIEW " + FullName + "\r\nGO\r\n";
        }

        /// <summary>
        /// Compara dos store procedures y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(View origen, View destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (!origen.Text.Equals(destino.Text)) return false;
            return true;
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == StatusEnum.ObjectStatusType.DropStatus)
            {
                listDiff.Add(ToSQLDrop(), 0, StatusEnum.ScripActionType.DropView);
            }
            if (this.HasState(StatusEnum.ObjectStatusType.AlterStatus))
            {
                listDiff.Add(this.ToSQLDrop(), 0, StatusEnum.ScripActionType.DropView);
                listDiff.Add(this.ToSQL(), 0, StatusEnum.ScripActionType.AddView);
            }
            if (this.Status == StatusEnum.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSQL(), 0, StatusEnum.ScripActionType.AddView);
            }
            return listDiff;
        }
    }
}
