using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.MySQL.Model
{
    public class Constraint : MySQLSchemaBase
    {
        public enum ConstraintType
        {
            None = 0,
            PrimaryKey = 1,
            ForeignKey = 2,
            Default = 3,
            Unique = 4,
            Check = 5
        }

        private string typeText;
        private ConstraintColumns columns;

        public Constraint(Table parent):base(StatusEnum.ObjectTypeEnum.Constraint)
        {
            this.Parent = parent;
            this.Columns = new ConstraintColumns(this);
        }

        public Constraint Clone(Table parentObject)
        {
            Constraint item = new Constraint(parentObject);
            item.TypeText = this.TypeText;
            item.Owner = this.Owner;
            item.Name = this.Name;
            item.Id = this.Id;
            item.Columns = this.Columns.Clone(this);
            return item;
        }

        /// <summary>
        /// Coleccion de columnas de la constraint.
        /// </summary>
        public ConstraintColumns Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        public ConstraintType Type
        {
            get 
            {
                if (TypeText.Equals("PRIMARY KEY")) return Constraint.ConstraintType.PrimaryKey;
                if (TypeText.Equals("UNIQUE")) return Constraint.ConstraintType.Unique;
                if (TypeText.Equals("CHECK")) return Constraint.ConstraintType.Check;
                if (TypeText.Equals("FOREIGN KEY")) return Constraint.ConstraintType.ForeignKey;
                return ConstraintType.None;
            }
        }

        public string TypeText
        {
            get { return typeText; }
            set { typeText = value; }
        }

        /// <summary>
        /// Devuelve el schema de la tabla en formato SQL.
        /// </summary>
        public string ToSQL()
        {
            StringBuilder sql = new StringBuilder();
            if (this.Type == ConstraintType.PrimaryKey)
            {
                sql.Append("PRIMARY KEY (");
                for (int index=0;index<this.Columns.Count;index++)
                {
                    if (index != this.Columns.Count - 1)
                        sql.Append(this.Columns[index].Name + ",");
                    else
                        sql.Append(this.Columns[index].Name);
                }
                sql.Append(")");
                return sql.ToString();
            }
            if (this.Type == Constraint.ConstraintType.ForeignKey)
            {
                sql.Append("CONSTRAINT [" + Name + "] FOREIGN KEY (");
                for (int index = 0; index < this.Columns.Count; index++)
                {
                    if (index != this.Columns.Count - 1)
                        sql.Append(this.Columns[index].Name + ",");
                    else
                        sql.Append(this.Columns[index].Name);
                }
                sql.Append(") REFERENCES " + this.Columns[0].ReferencedTableName + "(");
                for (int index = 0; index < this.Columns.Count; index++)
                {
                    if (index != this.Columns.Count - 1)
                        sql.Append(this.Columns[index].ReferencedColumnName + ",");
                    else
                        sql.Append(this.Columns[index].ReferencedColumnName);
                }
                sql.Append(")");
                return sql.ToString();
            }
            return "";
        }

        public override string ToSQLAdd()
        {
            return "ALTER TABLE " + ((Table)Parent).FullName + " ADD " + ToSQL() + ";\r\n\r\n";
        }

        public override string ToSQLDrop()
        {
            return "ALTER TABLE " + ((Table)Parent).FullName + " DROP " + TypeText + " [" + Name + "];\r\n\r\n";
        }

        /// <summary>
        /// Compara dos campos y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(Constraint origen, Constraint destino)
        {
            if (!ConstraintColumns.Compare(origen.Columns, destino.Columns)) return false;
            return true;
        }
    }
}
