using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    /// <summary>
    /// Clase de constraints de Columnas (Default Constraint y Check Constraint)
    /// </summary>
    public class ColumnConstraint : SQLServerSchemaBase
    {
        private Constraint.ConstraintType type;
        private string definition;
        private Boolean notForReplication;
        private Boolean disabled;
        private Boolean withNoCheck;

        public ColumnConstraint(Column parent)
            : base(StatusEnum.ObjectTypeEnum.Constraint)
        {
            this.Parent = parent;
        }

        /// <summary>
        /// Clona el objeto ColumnConstraint en una nueva instancia.
        /// </summary>
        public ColumnConstraint Clone(Column parent)
        {
            ColumnConstraint ccons = new ColumnConstraint(parent);
            ccons.Name = this.Name;
            ccons.Type = this.Type;
            ccons.Definition = this.Definition;
            ccons.Status = this.Status;
            ccons.Disabled = this.Disabled;
            return ccons;
        }

        /// <summary>
        /// Indica si la constraint esta deshabilitada.
        /// </summary>
        public Boolean Disabled
        {
            get { return disabled; }
            set { disabled = value; }
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
        /// Gets or sets a value indicating whether [with no check].
        /// </summary>
        /// <value><c>true</c> if [with no check]; otherwise, <c>false</c>.</value>
        public Boolean WithNoCheck
        {
            get { return withNoCheck; }
            set { withNoCheck = value; }
        }

        /// <summary>
        /// Valor de la constraint.
        /// </summary>
        public string Definition
        {
            get { return definition; }
            set { definition = value; }
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
                xml += "<COLUMNCONSTRAINT name=\"" + Name + "\" type=\"DF\" value=\"" + definition + "\"/>\n";
            }
            if (this.Type == Constraint.ConstraintType.Check)
            {
                xml += "<COLUMNCONSTRAINT name=\"" + Name + "\" type=\"C\" value=\"" + definition + "\" notForReplication=\"" + (NotForReplication?"1":"0") + "\"/>\n";
            }
            return xml;
        }

        /// <summary>
        /// Compara dos campos y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(ColumnConstraint origen, ColumnConstraint destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (origen.NotForReplication != destino.NotForReplication) return false;
            if (origen.Disabled != destino.Disabled) return false;
            if (!origen.Definition.Equals(destino.Definition)) return false;
            return true;
        }

        /// <summary>
        /// Devuelve el schema de la constraint en formato SQL.
        /// </summary>
        public string ToSQL()
        {
            string sql = "";
            if (this.Type == Constraint.ConstraintType.Default)
                sql = " CONSTRAINT [" + Name + "] DEFAULT " + definition;
            return sql;
        }

        /// <summary>
        /// Toes the SQL add.
        /// </summary>
        /// <returns></returns>
        public override string ToSQLAdd()
        {
            if (this.Type == Constraint.ConstraintType.Default)
                return "ALTER TABLE " + ((Table)Parent.Parent).FullName + " ADD" + ToSQL() + " FOR [" + Parent.Name + "]\r\nGO\r\n";
            if (this.Type == Constraint.ConstraintType.Check)
                return "ALTER TABLE " + ((Table)Parent.Parent).FullName + " ADD" + ToSQL() + "\r\nGO\r\n";
            return "";
        }

        /// <summary>
        /// Toes the SQL drop.
        /// </summary>
        /// <returns></returns>
        public override string ToSQLDrop()
        {
            return "ALTER TABLE " + ((Table)Parent.Parent).FullName + " DROP CONSTRAINT [" + Name + "]\r\nGO\r\n";
        }

        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList list = new SQLScriptList();
            if (this.Status == StatusEnum.ObjectStatusType.DropStatus)
                list.Add(this.ToSQLDrop(), 0, StatusEnum.ScripActionType.DropConstraint);
            if (this.Status == StatusEnum.ObjectStatusType.CreateStatus)
                list.Add(this.ToSQLAdd(), 0, StatusEnum.ScripActionType.AddConstraint);
            if (this.Status == StatusEnum.ObjectStatusType.AlterStatus)
            {
                list.Add(this.ToSQLDrop(), 0, StatusEnum.ScripActionType.DropConstraint);
                list.Add(this.ToSQLAdd(), 0, StatusEnum.ScripActionType.AddConstraint);
            }
            return list;
        }
    }
}
