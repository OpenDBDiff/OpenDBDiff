using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Trigger : Code
    {        
        private Boolean isDisabled;
        private Boolean insteadOf;
        private Boolean notForReplication;
        private Boolean isDDLTrigger;

        public Trigger(ISchemaBase parent)
            : base(parent, Enums.ObjectType.Trigger, Enums.ScripActionType.AddTrigger, Enums.ScripActionType.DropTrigger)
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

        public override Boolean IsCodeType
        {
            get { return true; }
        }

        public override string ToSqlDrop()
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

        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList list = new SQLScriptList();
            if (this.Status == Enums.ObjectStatusType.DropStatus)
                list.Add(Drop());
            if (this.Status == Enums.ObjectStatusType.CreateStatus)
                list.Add(Create());
            if (this.HasState(Enums.ObjectStatusType.AlterStatus))
                list.AddRange(Rebuild());
            if (this.HasState(Enums.ObjectStatusType.DisabledStatus))
                list.Add(this.ToSQLEnabledDisabled(), 0, Enums.ScripActionType.EnabledTrigger);
            return list;
        }

        public override bool Compare(ICode obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            if (!this.ToSql().Equals(obj.ToSql())) return false;
            if (this.InsteadOf != ((Trigger)obj).InsteadOf) return false;
            if (this.IsDisabled != ((Trigger)obj).IsDisabled) return false;
            if (this.NotForReplication != ((Trigger)obj).NotForReplication) return false;
            return true;
        }
    }
}
