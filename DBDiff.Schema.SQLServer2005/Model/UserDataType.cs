using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Globalization;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class UserDataType : SQLServerSchemaBase
    {
        private string type;
        private int size;
        private Boolean allowNull;
        private int precision;
        private int scale;
        private Default _default;
        private Rule rule;
        private Columns columnsDependencies;

        public UserDataType(ISchemaBase parent)
            : base(StatusEnum.ObjectTypeEnum.UserDataType)
        {          
            this.Parent = parent;
            this._default = new Default(this);
            this.rule = new Rule(this);
            this.columnsDependencies = new Columns(null);
        }

        /// <summary>
        /// Clona el objeto Column en una nueva instancia.
        /// </summary>
        public UserDataType Clone(ISchemaBase parent)
        {
            UserDataType item = new UserDataType(parent);
            item.Name = this.Name;
            item.Id = this.Id;
            item.Owner = this.Owner;
            item.Guid = this.Guid;
            item.AllowNull = this.AllowNull;
            item.Precision = this.Precision;
            item.Scale = this.Scale;
            item.Size = this.Size;
            item.Type = this.Type;            
            item.Default = this.Default.Clone(this);
            item.Rule = this.Rule.Clone(this);
            item.ColumnsDependencies = this.ColumnsDependencies.Clone(null);
            return item;
        }

        public Columns ColumnsDependencies
        {
            get { return columnsDependencies; }
            set { columnsDependencies = value; }
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

        /// <summary>
        /// Cantidad de digitos que permite el campo (solo para campos Numeric).
        /// </summary>
        public int Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// Cantidad de decimales que permite el campo (solo para campos Numeric).
        /// </summary>
        public int Precision
        {
            get { return precision; }
            set { precision = value; }
        }

        public Boolean AllowNull
        {
            get { return allowNull; }
            set { allowNull = value; }
        }

        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Compara dos indices y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(UserDataType origen, UserDataType destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (origen.Scale != destino.Scale) return false;
            if (origen.Precision != destino.Precision) return false;
            if (origen.AllowNull != destino.AllowNull) return false;
            if (origen.Size != destino.Size) return false;
            if (!origen.Type.Equals(destino.Type)) return false;
            if (!CompareDefault(origen, destino)) return false;
            if (!CompareRule(origen, destino)) return false;
            return true;
        }

        public static Boolean CompareRule(UserDataType origen, UserDataType destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if ((origen.Rule.Name != null) && (destino.Rule.Name == null)) return false;
            if ((origen.Rule.Name == null) && (destino.Rule.Name != null)) return false;
            if (origen.Rule.Name != null)
                if (!origen.Rule.Name.Equals(destino.Rule.Name)) return false;
            return true;
        }

        public static Boolean CompareDefault(UserDataType origen, UserDataType destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if ((origen.Default.Name != null) && (destino.Default.Name == null)) return false;
            if ((origen.Default.Name == null) && (destino.Default.Name != null)) return false;
            if (origen.Default.Name != null)
                if (!origen.Default.Name.Equals(destino.Default.Name)) return false;
            return true;
        }

        public string ToSQL()
        {
            string sql = "CREATE TYPE " + FullName + " FROM [" + type + "]";
            if (Type.Equals("binary") || Type.Equals("varbinary") || Type.Equals("varchar") || Type.Equals("char") || Type.Equals("nchar") || Type.Equals("nvarchar")) sql += "(" + Size.ToString(CultureInfo.InvariantCulture) + ")";
            if (Type.Equals("numeric") || Type.Equals("decimal")) sql += " (" + Precision.ToString(CultureInfo.InvariantCulture) + "," + Scale.ToString(CultureInfo.InvariantCulture) + ")";
            if (AllowNull)
                sql += " NULL";
            else
                sql += " NOT NULL";
            sql += "\r\nGO\r\n";
            return sql + ToSQLAddBinds();
        }

        public override string ToSQLAdd()
        {
            return ToSQL();
        }

        private string ToSQLAddBinds()
        {
            string sql = "";
            if (!String.IsNullOrEmpty(Default.Name))
                sql += Default.ToSQLAddBind();
            if (!String.IsNullOrEmpty(Rule.Name))
                sql += Rule.ToSQLAddBind();
            return sql;
        }

        public override StatusEnum.ObjectStatusType Status
        {
            set
            {
                base.Status = value;
                foreach (Column col in ColumnsDependencies)
                {
                    if (((Database)this.Parent).Tables.Exists(col.Parent.FullName) && ((Database)this.Parent).Tables[col.Parent.FullName].Columns.Exists(col.FullName))
                    {
                        if (col.HasToRebuildOnlyConstraint)
                            ((Database)this.Parent).Tables[col.Parent.FullName].Columns[col.FullName].Status = StatusEnum.ObjectStatusType.AlterRebuildDependeciesStatus;
                        else
                        {
                            if (col.IsComputed)
                                ((Database)this.Parent).Tables[col.Parent.FullName].Columns[col.FullName].Status = StatusEnum.ObjectStatusType.CreateStatus;
                            else
                                ((Database)this.Parent).Tables[col.Parent.FullName].Columns[col.FullName].Status = StatusEnum.ObjectStatusType.AlterStatus;
                        }
                    }
                }
            }
        }

        public override string ToSQLDrop()
        {
            return "DROP TYPE " + FullName + "\r\nGO\r\n";
        }
    }
}
