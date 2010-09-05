using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.Sybase.Model
{
    public class TableTrigger : SybaseSchemaBase
    {        
        private string text;
        private Boolean isDisabled;
        private Boolean insteadOf;
        private Boolean notForReplication;

        public TableTrigger(Table parent):base(StatusEnum.ObjectTypeEnum.Trigger)
        {
            this.Parent = parent;
        }

        /// <summary>
        /// Clona el objeto en una nueva instancia.
        /// </summary>
        public TableTrigger Clone(Table parent)
        {
            TableTrigger trigger = new TableTrigger(parent);
            trigger.Text = this.Text;
            trigger.Status = this.Status;
            trigger.Name = this.Name;
            trigger.IsDisabled = this.IsDisabled;
            trigger.InsteadOf = this.InsteadOf;
            trigger.NotForReplication = this.NotForReplication;
            trigger.Owner = this.Owner;
            trigger.Id = this.Id;
            return trigger;
        }

        public Boolean InsteadOf
        {
            get { return insteadOf; }
            set { insteadOf = value; }
        }

        public Boolean IsDisabled
        {
            get { return isDisabled; }
            set { isDisabled = value; }
        }

        public Boolean NotForReplication
        {
            get { return notForReplication; }
            set { notForReplication = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>
        /// Convierte el schema del trigger en XML.
        /// </summary>        
        public string ToXML()
        {
            string xml = "";
            xml += "<TRIGGER name=\"" + Name + "\">\r\n";
            xml += "<CODE>" + text + "</CODE>";
            xml += "</TRIGGER>r\n";
            return xml;
        }

        /// <summary>
        /// Compara dos triggers y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(TableTrigger origen, TableTrigger destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (!origen.Text.Equals(destino.Text)) return false;
            if (origen.InsteadOf != destino.InsteadOf) return false;
            if (origen.IsDisabled != destino.IsDisabled) return false;
            if (origen.NotForReplication != destino.NotForReplication) return false;
            return true;
        }

        public string ToSQL()
        {
            return text+"GO\r\n";
        }

        public string ToSQLDrop()
        {
            return "DROP TRIGGER " + FullName + "\r\nGO\r\n";
        }
    }
}
