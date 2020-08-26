using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using System;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    public class ExtendedProperty : SQLServerSchemaBase, ISchemaBase
    {
        public ExtendedProperty(ISchemaBase parent)
            : base(parent, ObjectType.ExtendedProperty)
        {
        }

        public override string FullName
        {
            get
            {
                string normal = "[" + Level0name + "]" + (String.IsNullOrEmpty(Level1name) ? "" : ".[" + Level1name + "]") + (String.IsNullOrEmpty(Level2name) ? "" : ".[" + Level2name + "]");
                if ((String.IsNullOrEmpty(Level1type)) || (String.IsNullOrEmpty(Level2type)))
                    return normal;
                if (!Level2type.Equals("TRIGGER"))
                    return normal;
                else
                    return "[" + Level0name + "].[" + Level2name + "]";
            }
        }

        public string Level2name { get; set; }

        public string Level2type { get; set; }

        public string Level1name { get; set; }

        public string Level1type { get; set; }

        public string Level0name { get; set; }

        public string Level0type { get; set; }

        public string Value { get; set; }

        public override SQLScript Create()
        {
            ScriptAction action = ScriptAction.AddExtendedProperty;
            return new SQLScript(this.ToSqlAdd(), 0, action);
        }

        public override SQLScript Drop()
        {
            ScriptAction action = ScriptAction.DropExtendedProperty;
            return new SQLScript(this.ToSqlDrop(), 0, action);
        }

        public override ObjectStatus Status { get; set; }

        public override string ToSqlAdd()
        {
            string sql = "EXEC sys.sp_addextendedproperty @name=N'" + Name + "', @value=N'" + Value + "' ,";
            sql += "@level0type=N'" + Level0type + "',@level0name=N'" + Level0name + "'";
            if (!String.IsNullOrEmpty(Level1name))
                sql += ", @level1type=N'" + Level1type + "',@level1name=N'" + Level1name + "'";
            if (!String.IsNullOrEmpty(Level2name))
                sql += ", @level2type=N'" + Level2type + "',@level2name=N'" + Level2name + "'";

            return sql + "\r\nGO\r\n";
        }

        public override string ToSqlDrop()
        {
            string sql = "EXEC sys.sp_dropextendedproperty @name=N'" + Name + "', @value=N'" + Value + "' ,";
            sql += "@level0type=N'" + Level0type + "',@level0name=N'" + Level0name + "'";
            if (!String.IsNullOrEmpty(Level1name))
                sql += ", @level1type=N'" + Level1type + "',@level1name=N'" + Level1name + "'";

            if (!String.IsNullOrEmpty(Level2name))
                sql += ", @level2type=N'" + Level2type + "',@level2name=N'" + Level2name + "'";
            return sql + "\r\nGO\r\n";
        }

        public override string ToSql()
        {
            return ToSqlAdd();
        }

        public override SQLScriptList ToSqlDiff(System.Collections.Generic.ICollection<ISchemaBase> schemas)
        {
            SQLScriptList list = new SQLScriptList();
            if (this.Parent.Status != ObjectStatus.Create)
            {
                if (this.Status == ObjectStatus.Create)
                    list.Add(this.Create());
                if (this.Status == ObjectStatus.Drop)
                    list.Add(this.Drop());
            }
            return list;
        }
    }
}
