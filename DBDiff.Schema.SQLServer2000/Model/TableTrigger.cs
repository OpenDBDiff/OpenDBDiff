using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer2000.Model
{
    public class TableTrigger:SchemaBase
    {        
        private string text;

        public TableTrigger(Table parent)
            : base("[", "]", StatusEnum.ObjectTypeEnum.Trigger)
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
            return trigger;
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

        public string ToSQL()
        {
            return text+"GO\r\n";
        }

        public override string ToSQLAdd()
        {
            return text + "GO\r\n";
        }

        public override string ToSQLDrop()
        {
            return "DROP TRIGGER [" + Name + "]\r\nGO\r\n";
        }
    }
}
