using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Tables : FindBaseList<Table,Database> 
    {
        private string sqlScript;

        public Tables(Database parent)
            : base(parent)
        {
        }

        public string ToSQL()
        {
            if (sqlScript == null)
            {
                StringBuilder sql = new StringBuilder();
                this.ForEach(item => sql.Append(item.ToSQL()));
                sqlScript = sql.ToString();
            }
            return sqlScript;
        }

        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();
            this.ForEach(item => listDiff.Add(item.ToSQLDiff()));

            return listDiff;
        }

        public new void Sort()
        {
            //BuildDependenciesTree();
            base.Sort(); //Ordena las tablas en funcion de su dependencias
        }

        /// <summary>
        /// Reordena la tabla en funcion de sus dependencias.
        /// </summary>
        
    }
}
