using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class Column : SQLServerSchemaBase, IComparable<Column>
    {
        public Column(ISchemaBase parent)
            : base(parent, Enums.ObjectType.Column)
        {
            ComputedFormula = "";
            Collation = "";
            this.Default = new Default(this);
            this.Rule = new Rule(this);
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
            col.ComputedFormula = this.ComputedFormula;
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

        public ColumnConstraint DefaultConstraint { get; set; }

        public Rule Rule { get; set; }

        public Default Default { get; set; }

        public Boolean IsFileStream { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is XML document.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is XML document; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsXmlDocument { get; set; }

        /// <summary>
        /// Gets or sets the XML schema.
        /// </summary>
        /// <value>The XML schema.</value>
        public string XmlSchema { get; set; }

        public Boolean IsSparse { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is user defined type.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is user defined type; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsUserDefinedType { get; set; }

        public int DataUserTypeId { get; set; }

        /// <summary>
        /// Gets or sets the column position.
        /// </summary>
        /// <value>The position.</value>
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets the scale (only in numeric or decimal datatypes).
        /// </summary>
        /// <value>The scale.</value>
        public int Scale { get; set; }

        /// <summary>
        /// Gets or sets the precision (only in numeric or decimal datatypes).
        /// </summary>
        /// <value>The precision.</value>
        public int Precision { get; set; }

        /// <summary>
        /// Gets or sets the collation (only in text datatypes).
        /// </summary>
        /// <value>The collation.</value>
        public string Collation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Column"/> is nullable.
        /// </summary>
        /// <value><c>true</c> if nullable; otherwise, <c>false</c>.</value>
        public Boolean IsNullable { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is persisted (only in Computed columns).
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is persisted; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsPersisted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has index dependencies.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has index dependencies; otherwise, <c>false</c>.
        /// </value>
        public Boolean HasIndexDependencies { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has computed dependencies.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has computed dependencies; otherwise, <c>false</c>.
        /// </value>
        public Boolean HasComputedDependencies { get; set; }

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
            if (isFileStream != this.IsFileStream) return true;
            return ((Position != newPosition) || HasComputedDependencies || HasIndexDependencies || IsComputed || Type.ToLower().Equals("timestamp"));
        }

        /// <summary>
        /// Gets or sets the computed formula (only in Computed columns).
        /// </summary>
        /// <value>The computed formula.</value>
        public string ComputedFormula { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is computed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is computed; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsComputed { get; set; }

        /// <summary>
        /// Gets a value indicating whether this column is BLOB.
        /// </summary>
        /// <value><c>true</c> if this column is BLOB; otherwise, <c>false</c>.</value>
        public Boolean IsBLOB
        {
            get
            {
                return Type.Equals("varchar(MAX)") || Type.Equals("nvarchar(MAX)") || Type.Equals("varbinary(MAX)") || Type.Equals("text") || Type.Equals("image") || Type.Equals("ntext") || Type.Equals("xml");
            }
        }

        public Boolean IsText
        {
            get
            {
                return Type.Equals("varchar(MAX)") || Type.Equals("nvarchar(MAX)") || Type.Equals("ntext") || Type.Equals("text") || Type.Equals("nvarchar") || Type.Equals("varchar") || Type.Equals("xml") || Type.Equals("char") || Type.Equals("nchar");
            }
        }

        public Boolean IsBinary
        {
            get
            {
                return Type.Equals("varbinary") || Type.Equals("varbinary(MAX)") || Type.Equals("image") || Type.Equals("binary");
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this field is identity for replication.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this field is identity for replication; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsIdentityForReplication { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this field is identity.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this field is identity; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsIdentity { get; set; }

        /// <summary>
        /// Gets or sets the identity increment (only if the field is Identity).
        /// </summary>
        /// <value>The identity increment.</value>
        public int IdentityIncrement { get; set; }

        /// <summary>
        /// Gets or sets the identity seed (only if the field is Identity).
        /// </summary>
        /// <value>The identity seed.</value>
        public int IdentitySeed { get; set; }

        /// <summary>
        /// Indica si el campo es Row Guid
        /// </summary>
        public Boolean IsRowGuid { get; set; }

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
                    tl = ((Database)this.Parent.Parent).UserTypes[Type].Type.ToLower();

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
                            sql += "(DOCUMENT " + XmlSchema + ")";
                        else
                            sql += "(CONTENT " + XmlSchema + ")";
                    }
                }
                if (Type.Equals("numeric") || Type.Equals("decimal")) sql += " (" + Precision.ToString(CultureInfo.InvariantCulture) + "," + Scale.ToString(CultureInfo.InvariantCulture) + ")";
                if (((Database)Parent.Parent).Info.Version >= DatabaseInfo.VersionTypeEnum.SQLServer2008)
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
                sql += "AS " + ComputedFormula;
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
        public static Boolean CompareIdentity(Column origin, Column destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (origin == null) throw new ArgumentNullException("origin");
            if (origin.IsIdentity != destination.IsIdentity) return false;
            if (origin.IsIdentityForReplication != destination.IsIdentityForReplication) return false;
            if (origin.IdentityIncrement != destination.IdentityIncrement) return false;
            if (origin.IdentitySeed != destination.IdentitySeed) return false;
            return true;
        }

        public static Boolean CompareRule(Column origin, Column destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (origin == null) throw new ArgumentNullException("origin");
            if ((origin.Rule.Name != null) && (destination.Rule.Name == null)) return false;
            if ((origin.Rule.Name == null) && (destination.Rule.Name != null)) return false;
            if (origin.Rule.Name != null)
                if (!origin.Rule.Name.Equals(destination.Rule.Name)) return false;
            return true;
        }

        /// <summary>
        /// Compara dos campos y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(Column origin, Column destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (origin == null) throw new ArgumentNullException("origin");
            if (!origin.ComputedFormula.Equals(destination.ComputedFormula)) return false;
            if (origin.IsComputed != destination.IsComputed) return false;
            //if (origin.Position != destination.Position) return false;
            if (!origin.IsComputed)
            {
                if (origin.IsXmlDocument != destination.IsXmlDocument) return false;
                if ((origin.XmlSchema == null) && (destination.XmlSchema != null)) return false;
                if (origin.XmlSchema != null)
                    if (!origin.XmlSchema.Equals(destination.XmlSchema)) return false;
                if (origin.IsNullable != destination.IsNullable) return false;
                if (origin.IsFileStream != destination.IsFileStream) return false;
                if (origin.IsSparse != destination.IsSparse) return false;
                if (!origin.Collation.Equals(destination.Collation)) return false;
                if (!origin.Type.Equals(destination.Type, StringComparison.CurrentCultureIgnoreCase)) return false;
                //Si el tipo de campo es custom, no compara size del campo.
                if (!origin.IsUserDefinedType)
                {
                    if (origin.Precision != destination.Precision) return false;
                    if (origin.Scale != destination.Scale) return false;
                    //Si el tamaño de un campo Text cambia, entonces por la opcion TextInRowLimit.
                    if ((origin.Size != destination.Size) && (origin.Type.Equals(destination.Type, StringComparison.CurrentCultureIgnoreCase)) && (!origin.Type.Equals("text", StringComparison.CurrentCultureIgnoreCase))) return false;
                }

            }
            else
            {
                if (origin.IsPersisted != destination.IsPersisted) return false;
            }
            if (!CompareIdentity(origin, destination)) return false;
            return CompareRule(origin, destination);
        }

        public int CompareTo(Column other)
        {
            return this.Id.CompareTo(other.Id);
        }
    }
}
