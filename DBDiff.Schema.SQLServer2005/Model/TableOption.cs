using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class TableOption : SQLServerSchemaBase
    {
        private string vale;

        public TableOption(ISchemaBase parent)
            : base(StatusEnum.ObjectTypeEnum.TableOption)
        {
            this.Parent = parent ;
        }

        /// <summary>
        /// Clona el objeto en una nueva instancia.
        /// </summary>
        public TableOption Clone(ISchemaBase parent)
        {
            TableOption option = new TableOption(parent);
            option.Name = this.Name;
            option.Status = this.Status;
            option.Value = this.Value;
            return option;
        }

        public string Value
        {
            get { return vale; }
            set { vale = value; }
        }

                /// <summary>
        /// Compara dos indices y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(TableOption origen, TableOption destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (!destino.Value.Equals(origen.Value)) return false;
            return true;
        }

        public override string ToSQLDrop()
        {
            if (this.Name.Equals("TextInRow"))
                return "EXEC sp_tableoption " + Parent.Name + ", 'text in row','off'\r\nGO\r\n";
            if (this.Name.Equals("LargeValues"))
                return "EXEC sp_tableoption " + Parent.Name + ", 'large value types out of row','0'\r\nGO\r\n";
            if (this.Name.Equals("VarDecimal"))
                return "EXEC sp_tableoption " + Parent.Name + ", 'vardecimal storage format','0'\r\nGO\r\n";
            return "";
        }

        public string ToSQL()
        {
            if (this.Name.Equals("TextInRow"))
                return "EXEC sp_tableoption " + Parent.Name + ", 'text in row'," + vale + "\r\nGO\r\n";
            if (this.Name.Equals("LargeValues"))
                return "EXEC sp_tableoption " + Parent.Name + ", 'large value types out of row'," + vale + "\r\nGO\r\n";
            if (this.Name.Equals("VarDecimal"))
                return "EXEC sp_tableoption " + Parent.Name + ", 'vardecimal storage format','1'\r\nGO\r\n";
            return "";
        }

        public override string ToSQLAdd()
        {
            return ToSQL();
        }
    }
}
