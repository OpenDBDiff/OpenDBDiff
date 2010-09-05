using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class TableOptions:FindBaseList<TableOption,Table> 
    {
        public TableOptions(Table parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Clona el objeto Columns en una nueva instancia.
        /// </summary>
        public TableOptions Clone(Table parent)
        {
            TableOptions options = new TableOptions(parent);
            for (int index = 0; index < this.Count; index++)
            {
                options.Add(this[index].Clone(parent));
            }
            return options;
        }

        /// <summary>
        /// Agrega un objeto columna a la coleccion de columnas.
        /// </summary>
        public void Add(string name, string value)
        {
            TableOption prop = new TableOption(Parent);
            prop.Name = name;
            prop.Value = value;
            prop.Status = StatusEnum.ObjectStatusType.OriginalStatus;
            base.Add(prop);
        }

        public string ToSQL()
        {
            StringBuilder sql = new StringBuilder();
            this.ForEach(item => sql.Append(item.ToSQL() + "\r\n"));
            return sql.ToString();
        }


        public SQLScriptList ToSQLDiff()
        {
            string sqlDrop = "";
            string sqlAdd = "";
            string sqlAlter = "";
            SQLScriptList list = new SQLScriptList();
            int index;
            for (index = 0; index < this.Count; index++)
            {
                if (this[index].Status == StatusEnum.ObjectStatusType.DropStatus)
                    sqlDrop += this[index].ToSQLDrop();
                if (this[index].Status == StatusEnum.ObjectStatusType.CreateStatus)
                    sqlAdd += this[index].ToSQL();
                if (this[index].Status == StatusEnum.ObjectStatusType.AlterStatus)
                    sqlAlter += this[index].ToSQLDrop() + this[index].ToSQL();
            }
            if (!String.IsNullOrEmpty(sqlAlter + sqlDrop + sqlAdd))
                list.Add(sqlAlter + sqlDrop + sqlAdd, ((Table)Parent).DependenciesCount, StatusEnum.ScripActionType.AlterOptions);
            return list;
        }
    }
}
