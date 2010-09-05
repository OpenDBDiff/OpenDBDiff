using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer2000.Model
{
    public class Table:SchemaBase,IComparable<Table>
    {
        private Columns columns;
        private Constraints constraints;
        private TableOptions options;
        private TableTriggers triggers;
        private Table originalTable;
        private int dependenciesCount;

        public Table(Database parent)
            : base("[", "]", StatusEnum.ObjectTypeEnum.Table)
        {
            columns = new Columns(this);
            constraints = new Constraints(this);
            options = new TableOptions(this);
            triggers = new TableTriggers(this);
            this.Status = StatusEnum.ObjectStatusType.OriginalStatus;            
            this.Parent = parent;
        }

        /// <summary>
        /// Clona el objeto Table en una nueva instancia.
        /// </summary>
        public Table Clone(Database objectParent)
        {
            Table table = new Table(objectParent);
            table.Owner = this.Owner;
            table.Name = this.Name;
            table.Id = this.Id;
            table.Status = StatusEnum.ObjectStatusType.OriginalStatus;
            table.DependenciesCount = this.DependenciesCount;
            table.Columns = this.Columns.Clone(table);
            table.Options = this.Options.Clone();
            table.Triggers = this.Triggers.Clone(table);
            return table;
        }

        /*public Table Clone()
        {
            return Clone(this);
        }*/

        public TableTriggers Triggers
        {
            get { return triggers; }
            set { triggers = value; }
        }

        public TableOptions Options
        {
            get { return options; }
            set { options = value; }
        }

        /// <summary>
        /// Objeto tabla con la definicion original de la tabla. Se usa cuando se requiere regenerar la tabla.
        /// </summary>
        public Table OriginalTable
        {
            get { return originalTable; }
            set { originalTable = value; }
        }

        /// <summary>
        /// Colecion de constraints de la tabla.
        /// </summary>
        public Constraints Constraints
        {
            get { return constraints; }
            set { constraints = value; }
        }

        /// <summary>
        /// Coleccion de campos de la tabla.
        /// </summary>
        public Columns Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        /// <summary>
        /// Convierte el schema de la tabla en XML.
        /// </summary>        
        public string ToXML()
        {
            string xml = "";
            xml += "<TABLE owner=\"" + Owner + "\" name=\"" + Name + "\">\r\n";
            xml += columns.ToXML();
            xml += constraints.ToXML();
            xml += triggers.ToXML();
            xml += "</TABLE>r\n";
            return xml;
        }

        /// <summary>
        /// Devuelve el schema de la tabla en formato SQL.
        /// </summary>
        public string ToSQL()
        {
            string sql = "";
            sql += "CREATE TABLE " + FullName + "\r\n(\r\n";
            sql += columns.ToSQL();
            if (constraints.Count > 0) sql += ",\r\n";
            sql += constraints.ToSQL();
            sql += ")";
            sql += "\r\n";
            sql += "GO\r\n";
            sql += options.ToSQL();
            sql += triggers.ToSQL();
            return sql;

        }

        /// <summary>
        /// Devuelve el schema de diferencias de la tabla en formato SQL.
        /// </summary>
        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == StatusEnum.ObjectStatusType.DropStatus)
            {
                listDiff.Add("DROP TABLE " + FullName + "\r\nGO\r\n", dependenciesCount, StatusEnum.ScripActionType.DropTable);
            }
            if (this.Status == StatusEnum.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSQL(), dependenciesCount, StatusEnum.ScripActionType.AddTable);
            }
            if (this.Status == StatusEnum.ObjectStatusType.AlterStatus)
            {
                listDiff.Add(columns.ToSQLDiff());
                listDiff.Add(constraints.ToSQLDiff());
                listDiff.Add(options.ToSQLDiff());
                listDiff.Add(triggers.ToSQLDiff());
            }
            if (this.Status == StatusEnum.ObjectStatusType.AlterRebuildStatus)
            {
                listDiff.Add(ToSQLRebuild(), dependenciesCount, StatusEnum.ScripActionType.RebuildTable);
                listDiff.Add(columns.ToSQLDiff());
                listDiff.Add(constraints.ToSQLDiff());
                listDiff.Add(options.ToSQLDiff());
                //Como recrea la tabla, solo pone los nuevos triggers, por eso va ToSQL y no ToSQLDiff
                listDiff.Add(triggers.ToSQL(), dependenciesCount, StatusEnum.ScripActionType.AddTrigger); 

            }
            return listDiff;
        }

        private string ToSQLRebuild()
        {
            string sql = "";
            string tempTable = "Temp" + Name;
            string listColumns = "";
            for (int index = 0; index <= OriginalTable.Columns.Count - 1; index++)
            {
                if (!OriginalTable.Columns[index].IsComputed)
                {
                    listColumns += OriginalTable.Columns[index].Name;
                    if (index != OriginalTable.Columns.Count - 1)
                        listColumns += ",";
                }
            }
            sql += ToSQLDropDependencis();
            sql += ToSQLTemp(tempTable);
            sql += "SET IDENTITY_INSERT " + tempTable + " ON\r\n";
            sql += "INSERT INTO " + tempTable + "(" + listColumns + ")" + " SELECT " + listColumns + " FROM " + this.FullName + "\r\n";
            sql += "SET IDENTITY_INSERT " + tempTable + " OFF\r\n";
            sql += "DROP TABLE " + this.FullName + "\r\nGO\r\n";
            sql += "EXEC sp_rename N'" + tempTable + "',N'" + this.Name + "', 'OBJECT'\r\nGO\r\n";
            sql += OriginalTable.Options.ToSQL();
            sql += ToSQLCreateDependencis();
            return sql;
        }

        private string ToSQLTemp(String TableName)
        {
            string sql = "";
            sql += "CREATE TABLE " + TableName + "\r\n(\r\n";
            for (int index = 0; index < this.OriginalTable.Columns.Count; index++)
            {
                if (this.Columns[OriginalTable.Columns[index].Name].Status == StatusEnum.ObjectStatusType.AlterRebuildStatus)
                    sql += this.Columns[OriginalTable.Columns[index].Name].ToSQL(true);
                else
                    sql += this.OriginalTable.Columns[index].ToSQL(true);
                if (index != this.OriginalTable.Columns.Count - 1)
                {
                    sql += ",";
                    sql += "\r\n";
                }
            }
            sql += ")";
            sql += "\r\n";
            sql += "GO\r\n";
            return sql;
        }

        private string ToSQLDropDependencis()
        {
            string sql = "";
            Constraints dependencis = ((Database)Parent).Dependencies.Find(this.Id);
            //Se buscan todas las table constraints.
            for (int index = 0; index < dependencis.Count; index++)
            {
                sql += dependencis[index].ToSQLDrop();
            }
            //Se buscan todas las columns constraints.
            for (int index = 0; index < columns.Count; index++)
            {
                for (int cindex = 0; cindex < columns[index].Constraints.Count; cindex++)
                    sql += columns[index].Constraints[cindex].ToSQLDrop();
            }
            return sql;
        }

        private string ToSQLCreateDependencis()
        {
            string sql = "";
            Constraints dependencis = ((Database)Parent).Dependencies.Find(this.Id);
            //Las constraints de deben recorrer en el orden inverso.
            for (int index = dependencis.Count - 1; index >= 0; index--)
            {
                sql += dependencis[index].ToSQLAdd();
            }
            return sql;
        }

        /// <summary>
        /// Indica la cantidad de Constraints dependientes de otra tabla (FK) que tiene
        /// la tabla.
        /// </summary>
        public int DependenciesCount
        {
            get { return dependenciesCount; }
            set { dependenciesCount = value; }
        }

        /// <summary>
        /// Compara en primer orden por la operacion 
        /// (Primero van los Drops, luego los Create y finalesmente los Alter).
        /// Si la operacion es la misma, ordena por cantidad de tablas dependientes.
        /// </summary>
        public int CompareTo(Table other)
        {
            if (this.Status == other.Status)
                return this.DependenciesCount.CompareTo(other.DependenciesCount);
            else
                return other.Status.CompareTo(this.Status);
        }  
    }
}
