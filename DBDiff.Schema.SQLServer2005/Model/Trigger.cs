using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Trigger : SQLServerSchemaBase
    {        
        private string text;
        private Boolean isDisabled;
        private Boolean insteadOf;
        private Boolean notForReplication;
        private Boolean isDDLTrigger;

        public Trigger(ISchemaBase parent)
            : base(StatusEnum.ObjectTypeEnum.Trigger)
        {
            this.Parent = parent;
        }

        /// <summary>
        /// Clona el objeto en una nueva instancia.
        /// </summary>
        public Trigger Clone(ISchemaBase parent)
        {
            Trigger trigger = new Trigger(parent);
            trigger.Text = this.Text;
            trigger.Status = this.Status;
            trigger.Name = this.Name;
            trigger.IsDisabled = this.IsDisabled;
            trigger.InsteadOf = this.InsteadOf;
            trigger.NotForReplication = this.NotForReplication;
            trigger.Owner = this.Owner;
            trigger.Id = this.Id;
            trigger.IsDDLTrigger = this.IsDDLTrigger;
            trigger.Guid = this.Guid;
            return trigger;
        }

        public Boolean IsDDLTrigger
        {
            get { return isDDLTrigger; }
            set { isDDLTrigger = value; }
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
        public static Boolean Compare(Trigger origen, Trigger destino)
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

        public override string ToSQLAdd()
        {
            return ToSQL();
        }

        public override string ToSQLDrop()
        {
            if (!IsDDLTrigger)
                return "DROP TRIGGER " + FullName + "\r\nGO\r\n";
            else
                return "DROP TRIGGER " + FullName + " ON DATABASE\r\nGO\r\n";
        }

        public string ToSQLEnabledDisabled()
        {
            if (!IsDDLTrigger)
            {
                if (IsDisabled)
                    return "ALTER TABLE " + Parent.FullName + " DISABLE TRIGGER [" + Name + "]\r\nGO\r\n";
                else
                    return "ALTER TABLE " + Parent.FullName + " ENABLE TRIGGER [" + Name + "]\r\nGO\r\n";
            }
            else
            {
                if (IsDisabled)
                    return "DISABLE TRIGGER [" + Name + "]\r\nGO\r\n";
                else
                    return "ENABLE TRIGGER [" + Name + "]\r\nGO\r\n";
            }
        }

        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList list = new SQLScriptList();
            if (this.Status == StatusEnum.ObjectStatusType.DropStatus)
                list.Add(this.ToSQLDrop(), 0, StatusEnum.ScripActionType.DropTrigger);
            if (this.Status == StatusEnum.ObjectStatusType.CreateStatus)
                list.Add(this.ToSQL(), 0, StatusEnum.ScripActionType.AddTrigger);
            if (this.HasState(StatusEnum.ObjectStatusType.AlterStatus))
            {
                list.Add(this.ToSQLDrop(), 0, StatusEnum.ScripActionType.DropTrigger);
                list.Add(this.ToSQL(), 0, StatusEnum.ScripActionType.AddTrigger);
            }
            if (this.HasState(StatusEnum.ObjectStatusType.DisabledStatus))
                list.Add(this.ToSQLEnabledDisabled(), 0, StatusEnum.ScripActionType.EnabledTrigger);
            return list;
        }
    }
}
