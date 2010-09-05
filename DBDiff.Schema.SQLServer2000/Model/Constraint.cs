using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer2000.Model
{
    public class Constraint:SchemaBase
    {
        public enum ConstraintType
        {
            PrimaryKey = 1,
            ForeignKey = 2,
            Default = 3,
            Unique = 4,
            Check = 5
        }

        private ConstraintType type;
        private ConstraintColumns columns;
        private string relationalTable;
        private int relationalTableId;
        private Boolean clustered;
        private Boolean withNoCheck;
        private Boolean notForReplication;
        private Boolean onUpdateCascade;
        private Boolean onDeleteCascade;

        public Constraint(Table parent)
            : base("[", "]", StatusEnum.ObjectTypeEnum.Constraint)
        {
            this.Parent = parent;
            columns = new ConstraintColumns(this);
            this.Status = StatusEnum.ObjectStatusType.OriginalStatus;
        }      

        /// <summary>
        /// Coleccion de columnas de la constraint.
        /// </summary>
        public ConstraintColumns Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        /// <summary>
        /// Clona el objeto Column en una nueva instancia.
        /// </summary>
        public Constraint Clone(Table parent)
        {
            Constraint col = new Constraint(parent);
            col.Clustered = this.Clustered;
            col.Name = this.Name;
            col.NotForReplication = this.NotForReplication;
            col.RelationalTable = this.RelationalTable;
            col.Status = this.Status;
            col.Type = this.Type;
            col.WithNoCheck = this.WithNoCheck;
            col.OnDeleteCascade = this.OnDeleteCascade;
            col.OnUpdateCascade = this.OnUpdateCascade;
            col.Columns = this.Columns.Clone();
            return col;
        }

        public Boolean OnDeleteCascade
        {
            get { return onDeleteCascade; }
            set { onDeleteCascade = value; }
        }

        public Boolean OnUpdateCascade
        {
            get { return onUpdateCascade; }
            set { onUpdateCascade = value; }
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
        /// Indica si el indice de la constraint es Clustered o no (solo aplica a PrimaryKey y Unique)
        /// </summary>
        public Boolean Clustered
        {
            get { return clustered; }
            set { clustered = value; }
        }
        
        public Boolean WithNoCheck
        {
            get { return withNoCheck; }
            set { withNoCheck = value; }
        }

        /// <summary>
        /// Indica el tipo de constraint (PrimaryKey, ForeignKey, Unique o Default).
        /// </summary>
        public ConstraintType Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// ID de la tabla relacionada a la que hace referencia (solo aplica a FK)
        /// </summary>
        public int RelationalTableId
        {
            get { return relationalTableId; }
            set { relationalTableId = value; }
        }

        /// <summary>
        /// Nombre de la tabla relacionada a la que hace referencia (solo aplica a FK)
        /// </summary>
        public string RelationalTable
        {
            get { return relationalTable; }
            set { relationalTable = value; }
        }       

        /// <summary>
        /// Convierte el schema de la tabla en XML.
        /// </summary>  
        public string ToXML()
        {
            string xml = "";
            if (this.Type == Constraint.ConstraintType.PrimaryKey)
            {
                xml += "<CONSTRAINT name=\"" + Name + "\" type=\"PK\"/>\n";
            }
            if (this.Type == Constraint.ConstraintType.Unique)
            {
                xml += "<CONSTRAINT name=\"" + Name + "\" type=\"UQ\"/>\n";
            }
            if (this.Type == Constraint.ConstraintType.ForeignKey)
            {
                xml += "<CONSTRAINT name=\"" + Name + "\" type=\"FK\" relationalTableId=\"" + relationalTableId.ToString() + "\" relationalTable=\"" + relationalTable + "\">\n";
                xml += "\t<WITHNOCHECK>" + (WithNoCheck ? "1" : "0") + "</WITHNOCHECK>";
                xml += "\t<UPDATECASCADE>" + (OnUpdateCascade ? "1" : "0") + "</UPDATECASCADE>";
                xml += "\t<DELETECASCADE>" + (OnDeleteCascade ? "1" : "0") + "</DELETECASCADE>";
                xml += "</CONSTRAINT>";
            }          
  
            return xml;
        }

        /// <summary>
        /// Compara dos campos y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(Constraint origen, Constraint destino)
        {
            if (origen.NotForReplication != destino.NotForReplication) return false;
            if (origen.Clustered != destino.Clustered) return false;
            if ((origen.RelationalTable == null) && (destino.RelationalTable != null)) return false;
            if (origen.RelationalTable != null)
                if (!origen.RelationalTable.Equals(destino.RelationalTable)) return false;
            if (origen.WithNoCheck != destino.WithNoCheck) return false;            
            if (origen.OnUpdateCascade != destino.OnUpdateCascade) return false;
            if (origen.OnDeleteCascade != destino.OnDeleteCascade) return false;
            if (!ConstraintColumns.Compare(origen.Columns,destino.Columns)) return false;
            return true;
        }

        /// <summary>
        /// Devuelve el schema de la tabla en formato SQL.
        /// </summary>
        public string ToSQL()
        {
            string sql = "";
            if (this.Type == Constraint.ConstraintType.PrimaryKey)
            {
                sql += "CONSTRAINT [" + Name + "] PRIMARY KEY " + (clustered ? "CLUSTERED" : "NONCLUSTERED") + "\r\n\t(\r\n";
		        for (int index=0;index<this.Columns.Count;index++)
                {                    
                    sql += "\t\t[" + Columns[index].Name + "]";
                    if (index != this.Columns.Count-1) sql += ",";
                    sql += "\r\n";
                }
	            sql += "\t)";
            }
            if (this.Type == Constraint.ConstraintType.ForeignKey)
            {
                sql += "CONSTRAINT [" + Name + "] FOREIGN KEY\r\n\t(\r\n";
                for (int index = 0; index < this.Columns.Count; index++)
                {
                    sql += "\t\t[" + Columns[index].Name + "]";
                    if (index != this.Columns.Count - 1) sql += ",";
                    sql += "\r\n";
                }
                sql += "\t)";
                sql += "REFERENCES [" + this.RelationalTable + "]\r\n\t(\r\n";
                for (int index = 0; index < this.Columns.Count; index++)
                {
                    sql += "\t\t[" + Columns[index].ColumnRelationalName + "]";
                    if (index != this.Columns.Count - 1) sql += ",";
                    sql += "\r\n";
                }
                sql += "\t)";
                if (onUpdateCascade) sql += " ON UPDATE CASCADE\r\n";
                if (onDeleteCascade) sql += "\tON DELETE CASCADE";
            }
            if (this.Type == Constraint.ConstraintType.Unique)
            {
                sql += "CONSTRAINT [" + Name + "] UNIQUE " + (clustered ? "CLUSTERED" : "NONCLUSTERED") + "\r\n\t(\r\n";
		        for (int index=0;index<this.Columns.Count;index++)
                {                    
                    sql += "\t\t[" + Columns[index].Name + "]";
                    if (index != this.Columns.Count-1) sql += ",";
                    sql += "\r\n";
                }
	            sql += "\t)";
            }
            
            return sql;
        }

        public override string ToSQLAdd()
        {
            return "ALTER TABLE " + ((Table)Parent).FullName + (WithNoCheck ? " WITH NOCHECK" : "") + " ADD " + ToSQL() + "\r\nGO\r\n";
        }

        public override string ToSQLDrop()
        {
            return "ALTER TABLE " + ((Table)Parent).FullName + " DROP CONSTRAINT [" + Name + "]\r\nGO\r\n";
        }
    }
}
