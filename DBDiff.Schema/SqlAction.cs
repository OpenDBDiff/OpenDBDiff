using System.Collections.Generic;
using DBDiff.Schema.Model;

namespace DBDiff.Schema
{
    public class SqlAction
    {
        public SqlAction(ISchemaBase item)
        {
            if ((item.ObjectType == Enums.ObjectType.Column) || (item.ObjectType == Enums.ObjectType.Index) || (item.ObjectType == Enums.ObjectType.Constraint))
                this.Name = item.Name;
            else
                this.Name = item.FullName;
            this.Action = item.Status;
            this.Type = item.ObjectType;
            Childs = new List<SqlAction>();
        }

        public void Add(ISchemaBase item)
        {
            Childs.Add(new SqlAction(item));
        }

        public SqlAction this[string name]
        {
            get
            {
                for (int j = 0; j < Childs.Count; j++)
                {
                    if (Childs[j].Name.Equals(name))
                        return Childs[j];
                }
                return null;
            }
        }

        public string Name { get; private set; }

        public Enums.ObjectType Type { get; set; }

        public Enums.ObjectStatusType Action { get; set; }

        public List<SqlAction> Childs { get; private set; }

        private string GetTypeName()
        {
            if (Type == Enums.ObjectType.Table) return "TABLE";
            if (Type == Enums.ObjectType.Column) return "COLUMN";
            if (Type == Enums.ObjectType.Constraint) return "CONSTRAINT";
            if (Type == Enums.ObjectType.Index) return "INDEX";
            if (Type == Enums.ObjectType.View) return "VIEW";
            if (Type == Enums.ObjectType.StoredProcedure) return "STORED PROCEDURE";
            if (Type == Enums.ObjectType.Synonym) return "SYNONYM";
            if (Type == Enums.ObjectType.Function) return "FUNCTION";
            if (Type == Enums.ObjectType.Assembly) return "ASSEMBLY";
            if (Type == Enums.ObjectType.Trigger) return "TRIGGER";
            return "";
        }

        private bool IsRoot
        {
            get
            {
                return ((this.Type != Enums.ObjectType.Function) && (this.Type != Enums.ObjectType.StoredProcedure) && (this.Type != Enums.ObjectType.View) && (this.Type != Enums.ObjectType.Table) && (this.Type != Enums.ObjectType.Database));
            }
        }

        public string Message
        {
            get
            {
                string message = "";
                if (Action == Enums.ObjectStatusType.DropStatus)
                    message = "DROP " + GetTypeName() + " " + Name + "\r\n";
                if (Action == Enums.ObjectStatusType.CreateStatus)
                    message = "ADD " + GetTypeName() + " " + Name + "\r\n";
                if ((Action == Enums.ObjectStatusType.AlterStatus) || (Action == Enums.ObjectStatusType.RebuildStatus) || (Action == Enums.ObjectStatusType.RebuildDependenciesStatus))
                    message = "MODIFY " + GetTypeName() + " " + Name + "\r\n";

                Childs.ForEach(item =>
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
