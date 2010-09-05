using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer2000.Model
{
    public class Column : SchemaBase
    {
        private string collation;
        private Boolean identity;
        private Boolean identityForReplication;
        private Boolean isComputed;
        private Boolean isRowGuid;
        private Boolean hasComputedDependencies;
        private Boolean hasIndexDependencies;
        private string computedFormula;
        private string type;
        private string originalType;
        private int size;
        private int identitySeed;
        private int identityIncrement;
        private Boolean nullable;
        private int precision;
        private int scale;
        private ColumnConstraints constraints;

        /// Clona el objeto en una nueva instancia.
        public Column(Table parent)
            : base("[", "]", StatusEnum.ObjectTypeEnum.Column)
        {
            this.Status = StatusEnum.ObjectStatusType.OriginalStatus;
            this.Parent = parent;            
            computedFormula = "";
            this.constraints = new ColumnConstraints(this);
        }

        /// <summary>
        /// Clona el objeto Column en una nueva instancia.
        /// </summary>
        public Column Clone(Table parent)
        {
            Column col = new Column(parent);
            col.ComputedFormula = this.computedFormula;
            col.Id = this.Id;
            col.Identity = this.Identity;
            col.IdentityForReplication = this.IdentityForReplication;
            col.IdentityIncrement = this.IdentityIncrement;
            col.IdentitySeed = this.IdentitySeed;
            col.IsComputed = this.IsComputed;
            col.IsRowGuid = this.IsRowGuid;
            col.HasComputedDependencies = this.HasComputedDependencies;
            col.HasIndexDependencies = this.HasIndexDependencies;
            col.Name = this.Name;
            col.Nullable = this.Nullable;
            col.Precision = this.Precision;
            col.Scale = this.Scale;
            col.Collation = this.Collation;
            col.Size = this.Size;
            col.Status = this.Status;
            col.Type = this.Type;
            col.OriginalType = this.OriginalType;
            col.Constraints = this.Constraints.Clone(this);
            return col;
        }

        public ColumnConstraints Constraints
        {
            get { return constraints; }
            set { constraints = value; }
        }

        /// <summary>
        /// Cantidad de digitos que permite el campo (solo para campos Numeric).
        /// </summary>
        public int Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// Indica si la columna es usada en un indice.
        /// </summary>
        public Boolean HasIndexDependencies
        {
            get { return hasIndexDependencies; }
            set { hasIndexDependencies = value; }
        }

        /// <summary>
        /// Indica si la columna es usada en otra columna de formula dentro de la misma tabla.
        /// </summary>
        public Boolean HasComputedDependencies
        {
            get { return hasComputedDependencies; }
            set { hasComputedDependencies = value; }
        }

        /// <summary>
        /// Cantidad de decimales que permite el campo (solo para campos Numeric).
        /// </summary>
        public int Precision
        {
            get { return precision; }
            set { precision = value; }
        }

        
        public string Collation
        {
            get { return collation; }
            set { collation = value; }
        }

        /// <summary>
        /// Indica si el campos permite valores NULL o no.
        /// </summary>
        public Boolean Nullable
        {
            get { return nullable; }
            set { nullable = value; }
        }

        /// <summary>
        /// Indica en bytes, el tamaño del campo.
        /// </summary>
        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary>
        /// Nombre del tipo de dato del campo.
        /// </summary>
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Nombre del tipo de dato original del (no tiene en cuenta si es un User Defined Data Type).
        /// </summary>
        public string OriginalType
        {
            get { return originalType; }
            set { originalType = value; }
        }

        /// <summary>
        /// Formula del campo (solo aplica si el campo es IsComputed)
        /// </summary>
        public string ComputedFormula
        {
            get { return computedFormula; }
            set { computedFormula = value; }
        }

        /// <summary>
        /// Indica si el campo es de tipo Computed (formula).
        /// </summary>
        public Boolean IsComputed
        {
            get { return isComputed; }
            set { isComputed = value; }
        }

        /// <summary>
        /// Indica si el campos es Indentity for replication
        /// </summary>
        public Boolean IdentityForReplication
        {
            get { return identityForReplication; }
            set { identityForReplication = value; }
        }

        /// <summary>
        /// Indica si es campo es Identity o no.
        /// </summary>
        public Boolean Identity
        {
            get { return identity; }
            set { identity = value; }
        }

        public int IdentityIncrement
        {
            get { return identityIncrement; }
            set { identityIncrement = value; }
        }

        public int IdentitySeed
        {
            get { return identitySeed; }
            set { identitySeed = value; }
        }

        /// <summary>
        /// Indica si el campo es Row Guid
        /// </summary>
        public Boolean IsRowGuid
        {
            get { return isRowGuid; }
            set { isRowGuid = value; }
        }

        /// <summary>
        /// Convierte el schema de la tabla en XML.
        /// </summary> 
        public string ToXML()
        {
            /*string xml = "";
            xml += "<COLUMN name=\"" + Name + "\" HasIndexDependencies=\"" + (hasIndexDependencies ? "1" : "0") + "\" HasComputedDependencies=\"" + (hasComputedDependencies ? "1" : "0") + "\" IsRowGuid=\"" + (isRowGuid ? "1" : "0") + "\" IsComputed=\"" + (isComputed ? "1" : "0") + "\" computedFormula=\"" + computedFormula + "\">\n";
            xml += "<TYPE>" + type + "</TYPE>";
            xml += "<ORIGINALTYPE>" + OriginalType + "</ORIGINALTYPE>";
            xml += "<SIZE>" + size.ToString() + "</SIZE>";
            xml += "<NULLABLE>" + (nullable ? "1":"0") + "</NULLABLE>";
            xml += "<PREC>" + precision.ToString() + "</PREC>";
            xml += "<SCALE>" + scale.ToString() + "</SCALE>";
            if (this.identity)
                xml += "<IDENTITY seed=\"" + identitySeed.ToString() + "\",increment=\"" + identityIncrement.ToString() + "\"/>";
            if (this.identityForReplication)
                xml += "<IDENTITYNOTFORREPLICATION seed=\"" + identitySeed.ToString() + "\",increment=\"" + identityIncrement.ToString() + "\"/>";
            xml += constraints.ToXML();
            xml += "</COLUMN>\n";
            return xml;*/
            XmlSerializer serial = new XmlSerializer(this.GetType());
            return serial.ToString();
        }

        public Constraints DependenciesConstraint(string columnName)
        {
            Table ParenTable = (Table)Parent;
            Constraints cons = new Constraints(ParenTable);
            for (int index = 0; index < ParenTable.Constraints.Count; index++)
            {
                if (ParenTable.Constraints[index].Columns.Find(columnName))
                {
                    cons.Add(ParenTable.Constraints[index]);
                }
            }
            return cons;
        }

        /// <summary>
        /// Devuelve el schema de la columna en formato SQL.
        /// </summary>
        public string ToSQL(Boolean sqlConstraint)
        {
            string sql = "";
            sql += "\t[" + Name + "] ";
            if (!IsComputed)
            {
                sql += "[" + Type + "]";
                if (Type.Equals("varbinary") || Type.Equals("varchar") || Type.Equals("char") || Type.Equals("nchar") || Type.Equals("nvarchar")) sql += " (" + Size.ToString() + ")";
                if (Type.Equals("numeric") || Type.Equals("decimal")) sql += " (" + Precision.ToString() + "," + Scale.ToString() + ")";
                if (!String.IsNullOrEmpty(Collation)) sql += " COLLATE " + Collation;
                if (Identity) sql += " IDENTITY (" + IdentitySeed.ToString() + "," + IdentityIncrement.ToString() + ")";
                if (IdentityForReplication) sql += " NOT FOR REPLICATION";
                if (Nullable)
                    sql += " NULL";
                else
                    sql += " NOT NULL";
                if (IsRowGuid) sql += " ROWGUIDCOL";
            }
            else
            {
                sql += "AS " + computedFormula;
            }
            if ((sqlConstraint) && (constraints.Count > 0)) sql += " " + constraints.ToSQL();
            return sql;
        }

        /// <return>
        /// Compara dos campos y devuelve true si son iguales, false en caso contrario.
        /// </summary>
        /// <param name="destino">
        /// Objeto Column con el que se desea comparar.
        /// </param>
        public StatusEnum.ObjectStatusType CompareStatus(Column destino)
        {
            StatusEnum.ObjectStatusType status = StatusEnum.ObjectStatusType.OriginalStatus;
            if (!Compare(this,destino))
            {
                if (destino.Identity == this.Identity)
                {

                }
            }
            return status;
        }

        /// <summary>
        /// Compara solo las propiedades de dos campos relacionadas con los Identity. Si existen
        /// diferencias, devuelve falso, caso contrario, true.
        /// </summary>
        public static Boolean CompareIdentity(Column origen, Column destino)
        {
            if (origen.Identity != destino.identity) return false;
            if (origen.IdentityForReplication != destino.identityForReplication) return false;
            if (origen.IdentityIncrement != destino.IdentityIncrement) return false;
            if (origen.IdentitySeed != destino.IdentitySeed) return false;
            return true;
        }

        /// <summary>
        /// Compara dos campos y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(Column origen, Column destino)
        {
            if (!origen.ComputedFormula.Equals(destino.computedFormula)) return false;
            if (origen.IsComputed != destino.isComputed) return false;
            if (!origen.IsComputed)
            {
                if (origen.Nullable != destino.nullable) return false;
                if (origen.Precision != destino.precision) return false;
                if (origen.Scale != destino.scale) return false;
                if (!origen.Collation.Equals(destino.Collation)) return false;
                //Si el tamaño de un campo Text cambia, entonces por la opcion TextInRowLimit.
                if ((origen.Size != destino.size) && (origen.Type.Equals(destino.Type)) && (!origen.Type.Equals("text"))) return false;
                if (!origen.Type.Equals(destino.type)) return false;
            }
            return CompareIdentity(origen,destino);
        }

        public override string ToSQLAdd()
        {
            return ToSQL(true);
        }
    }
}
