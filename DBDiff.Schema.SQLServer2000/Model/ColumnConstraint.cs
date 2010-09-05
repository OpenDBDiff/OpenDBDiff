using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer2000.Model
{
    /// <summary>
    /// Clase de constraints de Columnas (Default Constraint y Check Constraint)
    /// </summary>
    public class ColumnConstraint:SchemaBase
    {
        private Constraint.ConstraintType type;
        private string value;
        private Boolean notForReplication;

        public ColumnConstraint(Column parent)
            : base("[", "]", StatusEnum.ObjectTypeEnum.Constraint)
        {
            this.Parent = parent;
            this.Status = StatusEnum.ObjectStatusType.OriginalStatus;
        }

        /// <summary>
        /// Clona el objeto ColumnConstraint en una nueva instancia.
        /// </summary>
        public ColumnConstraint Clone(Column parent)
        {
            ColumnConstraint ccons = new ColumnConstraint(parent);
            ccons.Name = this.Name;
            ccons.Type = this.Type;
            ccons.Value = this.Value;
            ccons.Status = this.Status;
            return ccons;
        }

        /// <summary>
        /// Indica si la constraint va a ser usada en replicacion.
        /// </summary>
        public Boolean NotForReplication
        {
            get { return notForReplication; }
            set { notForReplication = value; }
        }

        /// <summary>
        /// Valor de la constraint.
        /// </summary>
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Indica el tipo de constraint (Default o Check constraint).
        /// </summary>
        public Constraint.ConstraintType Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Convierte el schema de la constraint en XML.
        /// </summary>  
        public string ToXML()
        {
            string xml = "";
            if (this.Type == Constraint.ConstraintType.Default)
            {
                xml += "<COLUMNCONSTRAINT name=\"" + Name + "\" type=\"DF\" value=\"" + value + "\"/>\n";
            }
            if (this.Type == Constraint.ConstraintType.Check)
            {
                xml += "<COLUMNCONSTRAINT name=\"" + Name + "\" type=\"C\" value=\"" + value + "\" notForReplication=\"" + (NotForReplication?"1":"0") + "\"/>\n";
            }
            return xml;
        }

        /// <summary>
        /// Compara dos campos y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(ColumnConstraint origen, ColumnConstraint destino)
        {
            if (origen.NotForReplication != destino.NotForReplication) return false;
            if (!origen.Value.Equals(destino.Value)) return false;
            return true;
        }

        /// <summary>
        /// Devuelve el schema de la constraint en formato SQL.
        /// </summary>
        public string ToSQL()
        {
            string sql = "";
            if (this.Type == Constraint.ConstraintType.Default)
                sql = " CONSTRAINT [" + Name + "] DEFAULT " + value;
            if (this.Type == Constraint.ConstraintType.Check)
                sql = " CONSTRAINT [" + Name + "] CHECK " + (NotForReplication?"NOT FOR REPLICATION":"") + " (" + value + ")";
            return sql;
        }

        public override string ToSQLAdd()
        {
            if (this.Type == Constraint.ConstraintType.Default)
                return "ALTER TABLE " + ((Table)Parent.Parent).FullName + " ADD" + ToSQL() + " FOR " + Parent.Name + "\r\nGO\r\n";
            if (this.Type == Constraint.ConstraintType.Check)
                return "ALTER TABLE " + ((Table)Parent.Parent).FullName + " ADD" + ToSQL() + "\r\nGO\r\n";
            return "";
        }

        public override string ToSQLDrop()
        {
            return "ALTER TABLE " + ((Table)Parent.Parent).FullName + " DROP CONSTRAINT [" + Name + "]\r\nGO\r\n";
        }
    }
}
