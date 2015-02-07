using System.Collections.Generic;
using DBDiff.Schema.Model;

namespace DBDiff.Schema
{
    public class SqlAction
    {
        private Enums.ObjectType type;
        private Enums.ObjectStatusType action;
        private string name;
        private List<SqlAction> childs;

        public SqlAction(ISchemaBase item)
        {
            if ((item.ObjectType == Enums.ObjectType.Column) || (item.ObjectType == Enums.ObjectType.Index) || (item.ObjectType == Enums.ObjectType.Constraint))
                this.name = item.Name;
            else
                this.name = item.FullName;
            this.action = item.Status;
            this.type = item.ObjectType;
            childs = new List<SqlAction>();
        }

        public void Add(ISchemaBase item)
        {
            childs.Add(new SqlAction(item));
        }

        public SqlAction this[string name]
        {
            get
            {
                for (int j = 0; j < childs.Count; j++)
                {
                    if (childs[j].Name.Equals(name))
                        return childs[j];
                }
                return null;
            }
        }

        public string Name
        {
            get { return name; }
        }

        public Enums.ObjectType Type
        {
            get { return type; }
            set { type = value; }
        }

        public Enums.ObjectStatusType Action
        {
            get { return action; }
            set { action = value; }
        }

        public List<SqlAction> Childs
        {
            get { return childs; }
        }

        private string GetTypeName()
        {
            if (type == Enums.ObjectType.Table) return "TABLE";
            if (type == Enums.ObjectType.Column) return "COLUMN";
            if (type == Enums.ObjectType.Constraint) return "CONSTRAINT";
            if (type == Enums.ObjectType.Index) return "INDEX";
            if (type == Enums.ObjectType.View) return "VIEW";
            if (type == Enums.ObjectType.StoreProcedure) return "STORE PROCEDURE";
            if (type == Enums.ObjectType.Synonym) return "SYNONYM";
            if (type == Enums.ObjectType.Function) return "FUNCTION";
            if (type == Enums.ObjectType.Assembly) return "ASSEMBLY";
            if (type == Enums.ObjectType.Trigger) return "TRIGGER";
            return "";
        }

        private bool IsRoot
        {
            get
            {
                return ((this.Type != Enums.ObjectType.Function) && (this.Type != Enums.ObjectType.StoreProcedure) && (this.Type != Enums.ObjectType.View) && (this.Type != Enums.ObjectType.Table) && (this.Type != Enums.ObjectType.Database));
            }
        }

        public string Message
        {
            get
            {
                string message = "";
                if (action == Enums.ObjectStatusType.DropStatus)
                    message = "DROP " + GetTypeName() + " " + Name + "\r\n";
                if (action == Enums.ObjectStatusType.CreateStatus)
                    message = "ADD " + GetTypeName() + " " + Name + "\r\n";
                if ((action == Enums.ObjectStatusType.AlterStatus) || (action == Enums.ObjectStatusType.RebuildStatus) || (action == Enums.ObjectStatusType.RebuildDependenciesStatus))
                    message = "MODIFY " + GetTypeName() + " " + Name + "\r\n";

                childs.ForEach(item =>
                    {
                        if (item.IsRoot)
                            message += "    ";                        
                        message += item.Message;
                    });
                return message;
            }
        }
    }
}
