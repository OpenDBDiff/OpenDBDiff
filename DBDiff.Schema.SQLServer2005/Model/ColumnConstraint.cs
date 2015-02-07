using System;

namespace DBDiff.Schema.SQLServer.Generates.Model
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
            : base(parent, Enums.ObjectType.Constraint)
        {
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
            ccons.Owner = this.Owner;
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
            if ((!origen.Definition.Equals(destino.Definition)) && (!origen.Definition.Equals("(" + destino.Definition + ")"))) return false;
            return true;
        }

        public override SQLScript Create()
        {
            Enums.ScripActionType action = Enums.ScripActionType.AddConstraint;
            if (!GetWasInsertInDiffList(action))
            {
                SetWasInsertInDiffList(action);
                return new SQLScript(this.ToSqlAdd(), 0, action);
            }
            else
                return null;

        }

        public override SQLScript Drop()
        {
            Enums.ScripActionType action = Enums.ScripActionType.DropConstraint;
            if (!GetWasInsertInDiffList(action))
            {
                SetWasInsertInDiffList(action);
                return new SQLScript(this.ToSqlDrop(), 0, action);
            }
            else
                return null;
        }

        public Boolean CanCreate
        {
            get
            {
                Enums.ObjectStatusType tableStatus = this.Parent.Parent.Status;
                Enums.ObjectStatusType columnStatus = this.Parent.Status;
                return ((columnStatus != Enums.ObjectStatusType.DropStatus) && (((tableStatus == Enums.ObjectStatusType.AlterStatus) || (tableStatus == Enums.ObjectStatusType.OriginalStatus) || (tableStatus == Enums.ObjectStatusType.RebuildDependenciesStatus)) && (this.Status == Enums.ObjectStatusType.OriginalStatus)));
            }
        }

        /// <summary>
        /// Devuelve el schema de la constraint en formato SQL.
        /// </summary>
        public override string ToSql()
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
        public override string ToSqlAdd()
        {
            if (this.Type == Constraint.ConstraintType.Default)
                return "ALTER TABLE " + ((Table)Parent.Parent).FullName + " ADD" + ToSql() + " FOR [" + Parent.Name + "]\r\nGO\r\n";
            if (this.Type == Constraint.ConstraintType.Check)
                return "ALTER TABLE " + ((Table)Parent.Parent).FullName + " ADD" + ToSql() + "\r\nGO\r\n";
            return "";
        }

        /// <summary>
        /// Toes the SQL drop.
        /// </summary>
        /// <returns></returns>
        public override string ToSqlDrop()
        {
            return "ALTER TABLE " + ((Table)Parent.Parent).FullName + " DROP CONSTRAINT [" + Name + "]\r\nGO\r\n";
        }

        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList list = new SQLScriptList();
            if (this.HasState(Enums.ObjectStatusType.DropStatus))
                list.Add(Drop());
            if (this.HasState(Enums.ObjectStatusType.CreateStatus))
                list.Add(Create()); 
            
            if (this.Status == Enums.ObjectStatusType.AlterStatus)
            {
                list.Add(Drop());
                list.Add(Create()); 
            }
            return list;
        }
    }
}
