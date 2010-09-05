using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Xml.Serialization;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.Sybase.Model
{
    public class Column : SybaseSchemaBase
    {
        private Boolean identity;
        private string type;
        private int size;
        private Boolean nullable;
        private int precision;
        private int scale;
        private Boolean hasIndexDependencies;
        private int position;

        public Column(Table parent):base(StatusEnum.ObjectTypeEnum.Column)
        {
            this.Parent = parent;            
        }

        /// <summary>
        /// Clona el objeto Column en una nueva instancia.
        /// </summary>
        public Column Clone(Table parent)
        {
            Column col = new Column(parent);
            col.Id = this.Id;
            col.Identity = this.Identity;
            col.HasIndexDependencies = this.HasIndexDependencies;
            col.Name = this.Name;
            col.Nullable = this.Nullable;
            col.Precision = this.Precision;
            col.Scale = this.Scale;
            col.Size = this.Size;
            col.Status = this.Status;
            col.Type = this.Type;
            col.Position = this.Position;
            //col.Constraints = this.Constraints.Clone(this);
            return col;
        }

        public int Position
        {
            get { return position; }
            set { position = value; }
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
        /// Indica si la columna es usada en un indice.
        /// </summary>
        public Boolean HasIndexDependencies
        {
            get { return hasIndexDependencies; }
            set { hasIndexDependencies = value; }
        }

        /// <summary>
        /// Indica si es campo es Identity o no.
        /// </summary>
        public Boolean Identity
        {
            get { return identity; }
            set { identity = value; }
        }

        /*public Constraints DependenciesConstraint(string columnName)
        {
            Table ParenTable = (Table)Parent;
            Constraints cons = new Constraints(ParenTable);
            for (int index = 0; index < ParenTable.Constraints.Count; index++)
            {
                if (ParenTable.Constraints[index].Columns.Find(columnName) != null)
                {
                    cons.Add(ParenTable.Constraints[index]);
                }
            }
            return cons;
        }*/

        /// <summary>
        /// Devuelve el schema de la columna en formato SQL.
        /// </summary>
        public string ToSQL(Boolean sqlConstraint)
        {
            string sql = "";
            sql += "[" + Name + "] ";
            sql += Type;
            if (Type.Equals("varbinary") || Type.Equals("varchar") || Type.Equals("char") || Type.Equals("nchar") || Type.Equals("nvarchar"))
            {
                if (Type.Equals("nchar") || Type.Equals("nvarchar"))
                    sql += " (" + (Size / 2).ToString(CultureInfo.InvariantCulture) + ")";
                else
                    sql += " (" + Size.ToString(CultureInfo.InvariantCulture) + ")";
            }
            if (Type.Equals("numeric") || Type.Equals("decimal")) sql += " (" + Precision.ToString(CultureInfo.InvariantCulture) + "," + Scale.ToString(CultureInfo.InvariantCulture) + ")";
            if (Identity) sql += " IDENTITY ";
            if (Nullable)
                sql += " NULL";
            else
                sql += " NOT NULL";
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
            if (destino != null)
            {
                StatusEnum.ObjectStatusType status = StatusEnum.ObjectStatusType.OriginalStatus;
                if (!Compare(this, destino))
                {
                    if (destino.Identity == this.Identity)
                    {

                    }
                }
                return status;
            }
            else
                throw new ArgumentNullException("destino");
        }

        /// <summary>
        /// Compara solo las propiedades de dos campos relacionadas con los Identity. Si existen
        /// diferencias, devuelve falso, caso contrario, true.
        /// </summary>
        public static Boolean CompareIdentity(Column origen, Column destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (origen.Identity != destino.identity) return false;
            return true;
        }

        /// <summary>
        /// Compara dos campos y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(Column origen, Column destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (origen.Nullable != destino.nullable) return false;
            if (origen.Precision != destino.precision) return false;
            if (origen.Scale != destino.scale) return false;
            //Si el tamaño de un campo Text cambia, entonces por la opcion TextInRowLimit.
            if ((origen.Size != destino.size) && (origen.Type.Equals(destino.Type)) && (!origen.Type.Equals("text"))) return false;
            if (!origen.Type.Equals(destino.type)) return false;
            return CompareIdentity(origen, destino);
        }
    }
}
