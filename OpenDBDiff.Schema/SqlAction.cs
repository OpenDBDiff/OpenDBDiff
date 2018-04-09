using System.Collections.Generic;
using OpenDBDiff.Schema.Model;

namespace OpenDBDiff.Schema
{
    public class SqlAction
    {
        public SqlAction(ISchemaBase item)
        {
            if ((item.ObjectType == ObjectType.Column) || (item.ObjectType == ObjectType.Index) || (item.ObjectType == ObjectType.Constraint))
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

        public ObjectType Type { get; set; }

        public ObjectStatus Action { get; set; }

        public List<SqlAction> Childs { get; private set; }

        private string GetTypeName()
        {
            if (Type == ObjectType.Table) return "TABLE";
            if (Type == ObjectType.Column) return "COLUMN";
            if (Type == ObjectType.Constraint) return "CONSTRAINT";
            if (Type == ObjectType.Index) return "INDEX";
            if (Type == ObjectType.View) return "VIEW";
            if (Type == ObjectType.StoredProcedure) return "STORED PROCEDURE";
            if (Type == ObjectType.Synonym) return "SYNONYM";
            if (Type == ObjectType.Function) return "FUNCTION";
            if (Type == ObjectType.Assembly) return "ASSEMBLY";
            if (Type == ObjectType.Trigger) return "TRIGGER";
            return "";
        }

        private bool IsRoot
        {
            get
            {
                return ((this.Type != ObjectType.Function) && (this.Type != ObjectType.StoredProcedure) && (this.Type != ObjectType.View) && (this.Type != ObjectType.Table) && (this.Type != ObjectType.Database));
            }
        }

        public string Message
        {
            get
            {
                string message = "";
                if (Action == ObjectStatus.Drop)
                    message = "DROP " + GetTypeName() + " " + Name + "\r\n";
                if (Action == ObjectStatus.Create)
                    message = "ADD " + GetTypeName() + " " + Name + "\r\n";
                if ((Action == ObjectStatus.Alter) || (Action == ObjectStatus.Rebuild) || (Action == ObjectStatus.RebuildDependencies))
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
