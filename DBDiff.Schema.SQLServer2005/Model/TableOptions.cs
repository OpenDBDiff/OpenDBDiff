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
            prop.Status = Enums.ObjectStatusType.OriginalStatus;
            base.Add(prop);
        }

        public string ToSQL()
        {
            StringBuilder sql = new StringBuilder();
            this.ForEach(item => sql.Append(item.ToSql() + "\r\n"));
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
                if (this[index].Status == Enums.ObjectStatusType.DropStatus)
                    sqlDrop += this[index].ToSqlDrop();
                if (this[index].Status == Enums.ObjectStatusType.CreateStatus)
                    sqlAdd += this[index].ToSql();
                if (this[index].Status == Enums.ObjectStatusType.AlterStatus)
                    sqlAlter += this[index].ToSqlDrop() + this[index].ToSql();
            }
            if (!String.IsNullOrEmpty(sqlAlter + sqlDrop + sqlAdd))
                list.Add(sqlAlter + sqlDrop + sqlAdd, ((Table)Parent).DependenciesCount, Enums.ScripActionType.AlterOptions);
            return list;
        }
    }
}
