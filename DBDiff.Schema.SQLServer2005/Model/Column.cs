using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Xml.Serialization;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class Column : SQLServerSchemaBase, IComparable<Column>
    {
        private string collation;
        private Boolean identity;
        private Boolean identityForReplication;
        private Boolean isComputed;
        private Boolean isRowGuid;
        private Boolean isSparse;
        private Boolean isFileStream;
        private string computedFormula;
        private string type;
        private int size;
        private int identitySeed;
        private int identityIncrement;
        private Boolean nullable;
        private int precision;
        private int scale;
        private Boolean isPersisted;
        private Boolean hasComputedDependencies;
        private Boolean hasIndexDependencies;
        private string xmlSchema;
        private Boolean isXmlDocument;
        private Boolean isUserDefinedType;
        private int position;
        private int dataUserTypeId;
        private Default _default;
        private Rule rule;
        private ColumnConstraint defaultConstraint;

        public Column(ISchemaBase parent)
            : base(parent, Enums.ObjectType.Column)
        {
            computedFormula = "";
            collation = "";
            this._default = new Default(this);
            this.rule = new Rule(this);
            this.DefaultConstraint = null;
        }

        /// <summary>
        /// Clona el objeto Column en una nueva instancia.
        /// </summary>
        public Column Clone(ISchemaBase parent)
        {
            Column col;
            if (parent == null) 
                col = new Column(this.Parent);
            else
                col = new Column(parent);
            col.ComputedFormula = this.computedFormula;
            col.DataUserTypeId = this.DataUserTypeId;
            col.Id = this.Id;
            col.Guid = this.Guid;
            col.Owner = this.Owner;
            col.IdentityIncrement = this.IdentityIncrement;
            col.IdentitySeed = this.IdentitySeed;
            col.IsIdentity = this.IsIdentity;
            col.IsIdentityForReplication = this.IsIdentityForReplication;
            col.IsComputed = this.IsComputed;
            col.IsRowGuid = this.IsRowGuid;
            col.IsPersisted = this.IsPersisted;
            col.IsFileStream = this.IsFileStream;
            col.IsSparse = this.IsSparse;
            col.IsXmlDocument = this.IsXmlDocument;
            col.IsUserDefinedType = this.IsUserDefinedType;
            col.HasComputedDependencies = this.HasComputedDependencies;
            col.HasIndexDependencies = this.HasIndexDependencies;
            col.Name = this.Name;
            col.IsNullable = this.IsNullable;
            col.Position = this.Position;
            col.Precision = this.Precision;            
            col.Scale = this.Scale;
            col.Collation = this.Collation;
            col.Size = this.Size;
            col.Status = this.Status;
            col.Type = this.Type;
            col.XmlSchema = this.XmlSchema;
            col.Default = this.Default.Clone(this);
            col.Rule = this.Rule.Clone(this);
            if (this.DefaultConstraint != null)
                col.DefaultConstraint = this.DefaultConstraint.Clone(this);
            return col;
        }

        public ColumnConstraint DefaultConstraint
        {
            get { return defaultConstraint; }
            set { defaultConstraint = value; }
        }

        public Rule Rule
        {
            get { return rule; }
            set { rule = value; }
        }

        public Default Default
        {
            get { return _default; }
            set { _default = value; }
        }

        public Boolean IsFileStream
        {
            get { return isFileStream; }
            set { isFileStream = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is XML document.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is XML document; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsXmlDocument
        {
            get { return isXmlDocument; }
            set { isXmlDocument = value; }
        }

        /// <summary>
        /// Gets or sets the XML schema.
        /// </summary>
        /// <value>The XML schema.</value>
        public string XmlSchema
        {
            get { return xmlSchema; }
            set { xmlSchema = value; }
        }

        public Boolean IsSparse
        {
            get { return isSparse; }
            set { isSparse = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is user defined type.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is user defined type; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsUserDefinedType
        {
            get { return isUserDefinedType; }
            set { isUserDefinedType = value; }
        }
        
        public int DataUserTypeId
        {
            get { return dataUserTypeId; }
            set { dataUserTypeId = value; }
        }

        /// <summary>
        /// Gets or sets the column position.
        /// </summary>
        /// <value>The position.</value>
        public int Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Gets or sets the scale (only in numeric or decimal datatypes).
        /// </summary>
        /// <value>The scale.</value>
        public int Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// Gets or sets the precision (only in numeric or decimal datatypes).
        /// </summary>
        /// <value>The precision.</value>
        public int Precision
        {
            get { return precision; }
            set { precision = value; }
        }

        /// <summary>
        /// Gets or sets the collation (only in text datatypes).
        /// </summary>
        /// <value>The collation.</value>
        public string Collation
        {
            get { return collation; }
            set { collation = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Column"/> is nullable.
        /// </summary>
        /// <value><c>true</c> if nullable; otherwise, <c>false</c>.</value>
        public Boolean IsNullable
        {
            get { return nullable; }
            set { nullable = value; }
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is persisted (only in Computed columns).
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is persisted; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsPersisted
        {
            get { return isPersisted; }
            set { isPersisted = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has index dependencies.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has index dependencies; otherwise, <c>false</c>.
        /// </value>
        public Boolean HasIndexDependencies
        {
            get { return hasIndexDependencies; }
            set { hasIndexDependencies = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has computed dependencies.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has computed dependencies; otherwise, <c>false</c>.
        /// </value>
        public Boolean HasComputedDependencies
        {
            get { return hasComputedDependencies; }
            set { hasComputedDependencies = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has to rebuild only constraint.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has to rebuild only constraint; otherwise, <c>false</c>.
        /// </value>
        public Boolean HasToRebuildOnlyConstraint
        {
            get
            {
                return (HasIndexDependencies && !HasComputedDependencies && !IsComputed);
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance has to rebuild.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has to rebuild; otherwise, <c>false</c>.
        /// </value>
        public Boolean HasToRebuild(int newPosition, string newType, bool isFileStream)
        {
            if (newType.Equals("text") && (!this.IsText)) return true;
            if (newType.Equals("ntext") && (!this.IsText)) return true;
            if (newType.Equals("image") && (!this.IsBinary)) return true;
            if (isFileStream != this.isFileStream) return true;
            return ((Position != newPosition) || HasComputedDependencies || HasIndexDependencies || IsComputed || type.ToLower().Equals("timestamp"));
        }

        /// <summary>
        /// Gets or sets the computed formula (only in Computed columns).
        /// </summary>
        /// <value>The computed formula.</value>
        public string ComputedFormula
        {
            get { return computedFormula; }
            set { computedFormula = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is computed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is computed; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsComputed
        {
            get { return isComputed; }
            set { isComputed = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this column is BLOB.
        /// </summary>
        /// <value><c>true</c> if this column is BLOB; otherwise, <c>false</c>.</value>
        public Boolean IsBLOB
        {
            get 
            {
                return type.Equals("varchar(MAX)") || type.Equals("nvarchar(MAX)") || type.Equals("varbinary(MAX)") || type.Equals("text") || type.Equals("image") || type.Equals("ntext") || type.Equals("xml");
            }
        }

        public Boolean IsText
        {
            get
            {
                return type.Equals("varchar(MAX)") || type.Equals("nvarchar(MAX)") || type.Equals("ntext") || type.Equals("text") || type.Equals("nvarchar") || type.Equals("varchar") || type.Equals("xml") || type.Equals("char") || type.Equals("nchar");
            }
        }

        public Boolean IsBinary
        {
            get
            {
                return type.Equals("varbinary") || type.Equals("varbinary(MAX)") || type.Equals("image") || type.Equals("binary");
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this field is identity for replication.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this field is identity for replication; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsIdentityForReplication
        {
            get { return identityForReplication; }
            set { identityForReplication = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this field is identity.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this field is identity; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsIdentity
        {
            get { return identity; }
            set { identity = value; }
        }

        /// <summary>
        /// Gets or sets the identity increment (only if the field is Identity).
        /// </summary>
        /// <value>The identity increment.</value>
        public int IdentityIncrement
        {
            get { return identityIncrement; }
            set { identityIncrement = value; }
        }

        /// <summary>
        /// Gets or sets the identity seed (only if the field is Identity).
        /// </summary>
        /// <value>The identity seed.</value>
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
        /// Nombre completo del objeto, incluyendo el owner.
        /// </summary>
        public override string FullName
        {
            get
            {
                return Parent.FullName + ".[" + Name + "]";
            }
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

        public Boolean HasToForceValue
        {
            get
            {
                return (this.HasState(Enums.ObjectStatusType.UpdateStatus)) || ((!this.IsNullable) && (this.Status == Enums.ObjectStatusType.CreateStatus));
            }
        }

        /// <summary>
        /// Gets the default force value.
        /// </summary>
        /// <value>The default force value.</value>
        public string DefaultForceValue
        {
            get
            {
                string tl = this.Type;
                if (this.IsUserDefinedType)
                    tl = ((Database)this.Parent.Parent).UserTypes[type].Type.ToLower();

                if ((((Database)Parent.Parent).Options.Defaults.UseDefaultValueIfExists) && (this.DefaultConstraint != null))
                {
                    return this.DefaultConstraint.Definition;
                }
                else
                {
                    if (tl.Equals("time")) return ((Database)Parent.Parent).Options.Defaults.DefaultTime;
                    if (tl.Equals("int") || tl.Equals("bit") || tl.Equals("smallint") || tl.Equals("bigint") || tl.Equals("tinyint")) return ((Database)Parent.Parent).Options.Defaults.DefaultIntegerValue;
                    if (tl.Equals("text") || tl.Equals("char") || tl.Equals("varchar") || tl.Equals("varchar(max)")) return ((Database)Parent.Parent).Options.Defaults.DefaultTextValue;
                    if (tl.Equals("ntext") || tl.Equals("nchar") || tl.Equals("nvarchar") || tl.Equals("nvarchar(max)")) return ((Database)Parent.Parent).Options.Defaults.DefaultNTextValue;
                    if (tl.Equals("date") || tl.Equals("datetimeoffset") || tl.Equals("datetime2") || tl.Equals("datetime") || tl.Equals("smalldatetime")) return ((Database)Parent.Parent).Options.Defaults.DefaultDateValue;
                    if (tl.Equals("numeric") || tl.Equals("decimal") || tl.Equals("float") || tl.Equals("money") || tl.Equals("smallmoney") || tl.Equals("real")) return ((Database)Parent.Parent).Options.Defaults.DefaultRealValue;
                    if (tl.Equals("sql_variant")) return ((Database)Parent.Parent).Options.Defaults.DefaultVariantValue;
                    if (tl.Equals("uniqueidentifier")) return ((Database)Parent.Parent).Options.Defaults.DefaultUniqueValue;
                    if (tl.Equals("image") || tl.Equals("binary") || tl.Equals("varbinary")) return ((Database)Parent.Parent).Options.Defaults.DefaultBlobValue;
                }
                return "";
            }
        }


        /// <summary>
        /// Toes the SQL drop.
        /// </summary>
        /// <returns></returns>
        public override string ToSqlDrop()
        {
            string sql = "ALTER TABLE " + Parent.FullName + " DROP COLUMN [" + Name + "]\r\nGO\r\n";
            return sql;
        }

        /// <summary>
        /// Toes the SQL add.
        /// </summary>
        /// <returns></returns>
        public override string ToSqlAdd()
        {
            return "ALTER TABLE " + Parent.FullName + " ADD " + ToSql(false) + "\r\nGO\r\n";
        }

        public override string ToSql()
        {
            return ToSql(true);
        }

        public string ToSQLRedefine(string type, int size, string xmlSchema)
        {
            string originalType = "";
            int originalSize = 0;
            string originalXMLSchema = "";

            string sql;

            if (type != null)
            {
                originalType = this.Type;
                this.Type = type;
            }
            if (size != 0)
            {
                originalSize = this.Size;
                this.Size = size;
            }
            if (xmlSchema != null)
            {
                originalXMLSchema = this.XmlSchema;
                this.XmlSchema = xmlSchema;

            }
            sql = this.ToSql(false);

            if (type != null)
                this.Type = originalType;
            if (size != 0)
                this.Size = originalSize;
            if (xmlSchema != null)
                this.XmlSchema = originalXMLSchema;
            return sql;
        }
        /// <summary>
        /// Devuelve el schema de la columna en formato SQL.
        /// </summary>
        public string ToSql(Boolean sqlConstraint)
        {
            string sql = "";
            sql += "[" + Name + "] ";
            if (!IsComputed)
            {
                if (this.IsUserDefinedType)
                    sql += Type;
                else
                    sql += "[" + Type + "]";
                if (Type.Equals("binary") || Type.Equals("varbinary") || Type.Equals("varchar") || Type.Equals("char") || Type.Equals("nchar") || Type.Equals("nvarchar"))
                {
                    if (Size == -1)
                        sql += " (max)";
                    else
                    {
                        if (Type.Equals("nchar") || Type.Equals("nvarchar"))
                            sql += " (" + (Size / 2).ToString(CultureInfo.InvariantCulture) + ")";
                        else
                            sql += " (" + Size.ToString(CultureInfo.InvariantCulture) + ")";
                    }
                }
                if (Type.Equals("xml"))
                {
                    if (!String.IsNullOrEmpty(XmlSchema))
                    {
                        if (IsXmlDocument)
                            sql += "(DOCUMENT " + xmlSchema + ")";
                        else
                            sql += "(CONTENT " + xmlSchema + ")";
                    }
                }
                if (Type.Equals("numeric") || Type.Equals("decimal")) sql += " (" + Precision.ToString(CultureInfo.InvariantCulture) + "," + Scale.ToString(CultureInfo.InvariantCulture) + ")";
                if (((Database)Parent.Parent).Info.Version == DatabaseInfo.VersionTypeEnum.SQLServer2008)
                {
                    if (Type.Equals("datetime2") || Type.Equals("datetimeoffset") || Type.Equals("time")) sql += "(" + Scale.ToString(CultureInfo.InvariantCulture) + ")";
                }
                if ((!String.IsNullOrEmpty(Collation)) && (!IsUserDefinedType)) sql += " COLLATE " + Collation;
                if (IsIdentity) sql += " IDENTITY (" + IdentitySeed.ToString(CultureInfo.InvariantCulture) + "," + IdentityIncrement.ToString(CultureInfo.InvariantCulture) + ")";
                if (IsIdentityForReplication) sql += " NOT FOR REPLICATION";
                if (IsSparse) sql += " SPARSE";
                if (IsFileStream) sql += " FILESTREAM";
                if (IsNullable)
                    sql += " NULL";
                else
                    sql += " NOT NULL";
                if (IsRowGuid) sql += " ROWGUIDCOL";
            }
            else
            {
                sql += "AS " + computedFormula;
                if (IsPersisted) sql += " PERSISTED";
            }
            if ((sqlConstraint) && (DefaultConstraint != null))
            {
                if (DefaultConstraint.Status != Enums.ObjectStatusType.DropStatus)
                    sql += " " + DefaultConstraint.ToSql().Replace("\t", "").Trim();
            }
            return sql;
        }

        public SQLScriptList RebuildDependencies()
        {
            SQLScriptList list = new SQLScriptList();
            list.AddRange(RebuildConstraint());
            list.AddRange(RebuildIndex());
            list.AddRange(RebuildFullTextIndex());
            return list;
        }

        private SQLScriptList RebuildFullTextIndex()
        {
            return RebuildFullTextIndex(null);
        }

        private SQLScriptList RebuildFullTextIndex(string index)
        {
            bool it;
            SQLScriptList list = new SQLScriptList();
            ((Table)Parent).FullTextIndex.ForEach(item =>
            {
                if (index == null)
                    it = item.Columns.Exists(col => { return col.ColumnName.Equals(this.Name); });
                else
                    it = item.Index.Equals(index);
                if (it)
                {
                    if (item.Status != Enums.ObjectStatusType.CreateStatus) list.Add(item.Drop());
                    if (item.Status != Enums.ObjectStatusType.DropStatus) list.Add(item.Create());
                }
            }
            );
            return list;
        }

        private SQLScriptList RebuildConstraint()
        {
            SQLScriptList list = new SQLScriptList();
            ((Table)Parent).Constraints.ForEach(item =>
            {
                ConstraintColumn ic = item.Columns.Find(this.Id);
                if (ic != null)
                {
                    if (item.Status != Enums.ObjectStatusType.CreateStatus) list.Add(item.Drop());
                    if (item.Status != Enums.ObjectStatusType.DropStatus) list.Add(item.Create());
                    list.AddRange(RebuildFullTextIndex(item.Name));
                }
            });
            return list;
        }

        private SQLScriptList RebuildIndex()
        {
            SQLScriptList list = new SQLScriptList();
            if (HasIndexDependencies)
            {                
                ((Table)Parent).Indexes.ForEach(item =>
                    {
                        IndexColumn ic = item.Columns.Find(this.Id);
                        if (ic != null)
                        {
                            if (item.Status != Enums.ObjectStatusType.CreateStatus) list.Add(item.Drop());
                            if (item.Status != Enums.ObjectStatusType.DropStatus) list.Add(item.Create());
                            list.AddRange(RebuildFullTextIndex(item.Name));
                        }
                    });
            }
            return list;
        }

        public SQLScriptList RebuildConstraint(Boolean Check)
        {
            SQLScriptList list = new SQLScriptList();
            if (DefaultConstraint != null)
            {
                if ((!Check) || (DefaultConstraint.CanCreate)) list.Add(DefaultConstraint.Create());
                list.Add(DefaultConstraint.Drop());
            }
            return list;
        }

        public SQLScriptList RebuildSchemaBindingDependencies()
        {
            SQLScriptList list = new SQLScriptList();
            List<ISchemaBase> items = ((Database)this.Parent.Parent).Dependencies.Find(this.Parent.Id, this.Id, 0);
            items.ForEach(item =>
            {
                if ((item.ObjectType == Enums.ObjectType.Function) || (item.ObjectType == Enums.ObjectType.View))
                {
                    if (item.Status != Enums.ObjectStatusType.CreateStatus)
                        list.Add(item.Drop());
                    if (item.Status != Enums.ObjectStatusType.DropStatus)
                        list.Add(item.Create());
                }
            });
            return list;
        }

        public SQLScriptList Alter(Enums.ScripActionType typeStatus)
        {
            SQLScriptList list = new SQLScriptList();
            string sql = "ALTER TABLE " + Parent.FullName + " ALTER COLUMN " + this.ToSql(false) + "\r\nGO\r\n";
            list.Add(sql, 0, typeStatus);
            return list;
        }
        
        /// <summary>
        /// Compara solo las propiedades de dos campos relacionadas con los Identity. Si existen
        /// diferencias, devuelve falso, caso contrario, true.
        /// </summary>
        public static Boolean CompareIdentity(Column origen, Column destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (origen.IsIdentity != destino.identity) return false;
            if (origen.IsIdentityForReplication != destino.identityForReplication) return false;
            if (origen.IdentityIncrement != destino.IdentityIncrement) return false;
            if (origen.IdentitySeed != destino.IdentitySeed) return false;
            return true;
        }

        public static Boolean CompareRule(Column origen, Column destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if ((origen.Rule.Name != null) && (destino.Rule.Name == null)) return false;
            if ((origen.Rule.Name == null) && (destino.Rule.Name != null)) return false;
            if (origen.Rule.Name != null)
                if (!origen.Rule.Name.Equals(destino.Rule.Name)) return false;
            return true;
        }

        /// <summary>
        /// Compara dos campos y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(Column origen, Column destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (!origen.ComputedFormula.Equals(destino.ComputedFormula)) return false;
            if (origen.IsComputed != destino.isComputed) return false;
            //if (origen.Position != destino.Position) return false;
            if (!origen.IsComputed)
            {
                if (origen.IsXmlDocument != destino.IsXmlDocument) return false;
                if ((origen.XmlSchema == null) && (destino.XmlSchema != null)) return false;
                if (origen.XmlSchema != null)
                    if (!origen.XmlSchema.Equals(destino.XmlSchema)) return false;
                if (origen.IsNullable != destino.IsNullable) return false;
                if (origen.IsFileStream != destino.IsFileStream) return false;
                if (origen.IsSparse != destino.IsSparse) return false;
                if (!origen.Collation.Equals(destino.Collation)) return false;                
                if (!origen.Type.Equals(destino.type, StringComparison.CurrentCultureIgnoreCase)) return false;
                //Si el tipo de campo es custom, no compara size del campo.
                if (!origen.IsUserDefinedType)
                {
                    if (origen.Precision != destino.precision) return false;
                    if (origen.Scale != destino.scale) return false;
                    //Si el tamaño de un campo Text cambia, entonces por la opcion TextInRowLimit.
                    if ((origen.Size != destino.size) && (origen.Type.Equals(destino.Type, StringComparison.CurrentCultureIgnoreCase)) && (!origen.Type.Equals("text", StringComparison.CurrentCultureIgnoreCase))) return false;
                }
                
            }
            else
            {
                if (origen.IsPersisted != destino.IsPersisted) return false;
            }
            if (!CompareIdentity(origen, destino)) return false;
            return CompareRule(origen, destino);
        }

        public int CompareTo(Column other)
        {
            return this.Id.CompareTo(other.Id);
        }
    }
}
