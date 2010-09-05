using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.MySQL.Model
{
    public class Table : MySQLSchemaBase, IComparable<Table>
    {
        private string engine;
        private string createOptions;
        private int autoIncrement;
        private Boolean checkSum;
        private string collation;
        private string comments;
        private Columns columns;
        private TableTriggers triggers;
        private Constraints constraints;
        private int dependenciesCount;

        public Table(Database parent):base(StatusEnum.ObjectTypeEnum.Table)
        {
            this.Parent = parent;
            columns = new Columns(this);
            triggers = new TableTriggers(this);
            constraints = new Constraints(this);
        }

        /// <summary>
        /// Colecion de constraints de la tabla.
        /// </summary>
        public Constraints Constraints
        {
            get { return constraints; }
            set { constraints = value; }
        }

        public TableTriggers Triggers
        {
            get { return triggers; }
            set { triggers = value; }
        }

        public Columns Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        public string CreateOptions
        {
            get { return createOptions; }
            set { createOptions = value; }
        }

        public string Engine
        {
            get { return engine; }
            set { engine = value; }
        }

        public int AutoIncrement
        {
            get { return autoIncrement; }
            set { autoIncrement = value; }
        }

        public Boolean CheckSum
        {
            get { return checkSum; }
            set { checkSum = value; }
        }

        public string Collation
        {
            get { return collation; }
            set { collation = value; }
        }

        public string Comments
        {
            get { return comments; }
            set { comments = value; }
        }

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
            if (other == null) throw new ArgumentNullException("other");
            if (this.Status == other.Status)
                return this.DependenciesCount.CompareTo(other.DependenciesCount);
            else
                return other.Status.CompareTo(this.Status);
        }

        /// <summary>
        /// Devuelve el schema de diferencias de la tabla en formato SQL.
        /// </summary>
        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == StatusEnum.ObjectStatusType.DropStatus)
            {
                listDiff.Add(ToSQLDrop(), dependenciesCount, StatusEnum.ScripActionType.DropTable);
            }
            if (this.Status == StatusEnum.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSQL(), dependenciesCount, StatusEnum.ScripActionType.AddTable);
            }
            if (this.Status == StatusEnum.ObjectStatusType.AlterStatus)
            {
                listDiff.Add(columns.ToSQLDiff());
                listDiff.Add(constraints.ToSQLDiff());
                //listDiff.Add(indexes.ToSQLDiff());
                listDiff.Add(triggers.ToSQLDiff());
            }
            if (this.Status == StatusEnum.ObjectStatusType.AlterRebuildStatus)
            {
                /*listDiff.Add(ToSQLRebuild(), dependenciesCount, StatusEnum.ScripActionType.RebuildTable);
                listDiff.Add(columns.ToSQLDiff());
                listDiff.Add(constraints.ToSQLDiff());
                listDiff.Add(indexes.ToSQLDiff());
                listDiff.Add(options.ToSQLDiff());
                //Como recrea la tabla, solo pone los nuevos triggers, por eso va ToSQL y no ToSQLDiff
                listDiff.Add(triggers.ToSQL(), dependenciesCount, StatusEnum.ScripActionType.AddTrigger);
                */
            }
            return listDiff;
        }

        public string ToSQL()
        {
            string sql;
            sql = "CREATE TABLE " + FullName + "(\r\n";
            sql += Columns.ToSQL();
            sql += Constraints.ToSQL();
            sql += ") ";
            sql += "ENGINE=" + Engine + " ";
            sql += "DEFAULT CHARSET=" + Collation + " ";
            if (!String.IsNullOrEmpty(Comments)) sql += "COMMENT='" + Comments + "' ";
            if (!String.IsNullOrEmpty(CreateOptions)) sql += " " + CreateOptions + " ";
            sql = sql.Trim() + ";\r\n";
            return sql;
        }

        public override string ToSQLDrop()
        {
            return "DROP TABLE " + FullName + ";\r\n\r\n";
        }

        /// <summary>
        /// Compara dos campos y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(Table origen, Table destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (!origen.Collation.Equals(destino.Collation)) return false;
            if (!origen.Comments.Equals(destino.Comments)) return false;
            if (!origen.CreateOptions.Equals(destino.CreateOptions)) return false;
            if (!origen.Engine.Equals(destino.Engine)) return false;
            if (origen.AutoIncrement != destino.AutoIncrement) return false;
            if (origen.CheckSum != destino.CheckSum) return false;
            return true;
        }
    }
}
