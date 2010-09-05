using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class StoreProcedure : SQLServerSchemaBase 
    {
        private string text;

        public StoreProcedure(ISchemaBase parent):base(StatusEnum.ObjectTypeEnum.StoreProcedure)
        {
            this.Parent = parent;            
        }

        /// <summary>
        /// Clona el objeto en una nueva instancia.
        /// </summary>
        public StoreProcedure Clone(ISchemaBase parent)
        {
            StoreProcedure item = new StoreProcedure(parent);
            item.Text = this.Text;
            item.Status = this.Status;
            item.Name = this.Name;
            item.Id = this.Id;
            item.Owner = this.Owner;
            item.Guid = this.Guid;
            return item;
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public string ToSQL()
        {
            return text + "GO\r\n";
        }

        public override string ToSQLAdd()
        {
            return ToSQL();
        }

        public override string ToSQLDrop()
        {
            return "DROP PROCEDURE " + FullName + "\r\nGO\r\n";
        }

        /// <summary>
        /// Compara dos store procedures y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(StoreProcedure origen, StoreProcedure destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (!origen.Text.Equals(destino.Text)) return false;
            return true;
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == StatusEnum.ObjectStatusType.DropStatus)
            {
                listDiff.Add(ToSQLDrop(), 0, StatusEnum.ScripActionType.DropStoreProcedure);
            }
            if (this.Status == StatusEnum.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSQL(), 0, StatusEnum.ScripActionType.AddStoreProcedure);
            }
            return listDiff;
        }
    }
}
