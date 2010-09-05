using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.MySQL.Model
{
    public class TableTrigger : MySQLSchemaBase
    {
        private string text;
        private string manipulation;
        private string timing;
        private string mode;

        public TableTrigger(Table parent):base(StatusEnum.ObjectTypeEnum.Trigger)
        {
            this.Parent = parent;
        }

        public TableTrigger Clone(Table parentObject)
        {
            TableTrigger item = new TableTrigger(parentObject);
            item.Id = this.Id;
            item.Manipulation = this.Manipulation;
            item.Mode = this.Mode;
            item.Name = this.Name;
            item.Owner = this.Owner;
            item.Text = this.Text;
            item.Timing = this.Timing;
            return item;
        }

        /// <summary>
        /// Indica el modo del servidor SQL en el momento en que fue creado el trigger.
        /// </summary>
        public string Mode
        {
            get { return mode; }
            set { mode = value; }
        }
        
        /// <summary>
        /// Momento en que va a ser ejecutado el trigger (BEFORE o AFTER).
        /// </summary>
        public string Timing
        {
            get { return timing; }
            set { timing = value; }
        }

        /// <summary>
        /// Codigo que va a ser ejecutado en el trigger.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>
        /// Indica en que evento se ejecuta el trigger (INSERT, UPDATE, DELETE).
        /// </summary>
        public string Manipulation
        {
            get { return manipulation; }
            set { manipulation = value; }
        }

        public string ToSQL()
        {
            string sql = "CREATE TRIGGER " + FullName + " " + Timing + " " + Manipulation;
            sql += "\t ON " + Parent.Name + " FOR EACH ROW " + text;
            return sql;
        }

        public override string ToSQLDrop()
        {
            return "DROP TRIGGER IF EXISTS " + FullName + ";";
        }

        /// <summary>
        /// Compara dos triggers y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(TableTrigger origen, TableTrigger destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (!origen.Text.Equals(destino.Text)) return false;
            if (!origen.Manipulation.Equals(destino.Manipulation)) return false;
            if (!origen.Timing.Equals(destino.Timing)) return false;
            if (!origen.Mode.Equals(destino.Mode)) return false;
            return true;
        }
    }
}
