using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer2000.Model
{
    public class Constraints:List<Constraint> 
    {
        private Hashtable hash = new Hashtable();
        private Table parent;

        public Constraints()
        {
        }
        
        public Constraints(Table parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Indica si el nombre de la tabla existe en la coleccion de tablas del objeto.
        /// </summary>
        /// <param name="table">
        /// Nombre de la tabla a buscar.
        /// </param>
        /// <returns></returns>
        public Boolean Find(string table)
        {
            return hash.ContainsKey(table);
        }

        public Constraint this[string name]
        {
            get { return (Constraint)hash[name]; }
            set
            {
                hash[name] = value;
                for (int index = 0; index < base.Count; index++)
                {
                    if (((Constraint)base[index]).Name.Equals(name))
                    {
                        base[index] = value;
                        break;
                    }
                }
            }
        }

        public Constraint Get(Constraint.ConstraintType type, string columnName)
        {
            for (int index = 0; index < this.Count; index++)
            {
                if (this[index].Type == Constraint.ConstraintType.Default)
                {
                    if (this[index].Columns[0].Name.Equals(columnName))
                        return this[index];
                }
            }
            return null;
        }

        public new void Add(Constraint column)
        {
            if (!hash.ContainsKey(column.Name))
            {
                hash.Add(column.Name, column);
                base.Add(column);
            }
        }

        /// <summary>
        /// Devuelve la tabla perteneciente a la coleccion de campos.
        /// </summary>
        public Table Parent
        {
            get { return parent; }
        }

        public string ToXML()
        {
            string xml = "";
            int index;
            xml += "<CONSTRAINTS>\n";
            for (index = 0; index < this.Count; index++)
            {
                xml += this[index].ToXML() + "\n";
            }
            xml += "</CONSTRAINTS>\n";
            return xml;
        }

        public string ToSQL()
        {
            string sql = "";
            int index;
            for (index = 0; index < this.Count; index++)
            {
                sql += "\t" + this[index].ToSQL();
                if (index != this.Count-1) 
                {
                    sql += ",";                    
                }
                sql += "\r\n";
            }
            return sql;
        }

        public SQLScriptList ToSQLDiff()
        {
            /*string sqlDrop = "";
            string sqlAdd = "";
            string sqlAlter = "";*/
            SQLScriptList list = new SQLScriptList();
            int index;
            for (index = 0; index < this.Count; index++)
            {
                if (this[index].Status == StatusEnum.ObjectStatusType.DropStatus)
                    list.Add(this[index].ToSQLDrop(),parent.DependenciesCount,StatusEnum.ScripActionType.DropConstraint);
                    //sqlDrop += this[index].ToSQLDropConstraint();
                if (this[index].Status == StatusEnum.ObjectStatusType.CreateStatus)
                    list.Add(this[index].ToSQLAdd(), parent.DependenciesCount, StatusEnum.ScripActionType.AddConstraint);
                    //sqlAdd += this[index].ToSQLAddConstraint();
                if (this[index].Status == StatusEnum.ObjectStatusType.AlterStatus)
                {
                    list.Add(this[index].ToSQLDrop(), parent.DependenciesCount, StatusEnum.ScripActionType.DropConstraint);
                    list.Add(this[index].ToSQLAdd(), parent.DependenciesCount, StatusEnum.ScripActionType.AddConstraint);
                    //sqlAlter += this[index].ToSQLDropConstraint() + this[index].ToSQLAddConstraint();
                }
            }
            return list;
            //return sqlAlter+sqlDrop+sqlAdd;
        }
    }
}
