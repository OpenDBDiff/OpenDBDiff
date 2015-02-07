using System;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class ExtendedProperty:SQLServerSchemaBase, ISchemaBase
    {
        private Enums.ObjectStatusType status;
        private string value;
        private string level0type;
        private string level0name;
        private string level1type;
        private string level1name;
        private string level2type;
        private string level2name;

        public ExtendedProperty(ISchemaBase parent)
            : base(parent, Enums.ObjectType.ExtendedProperty)
        {
        }

        public override string FullName
        {
            get
            {
                string normal = "[" + Level0name + "]" + (String.IsNullOrEmpty(level1name) ? "" : ".[" + level1name + "]") + (String.IsNullOrEmpty(level2name) ? "" : ".[" + level2name + "]");
                if ((String.IsNullOrEmpty(level1type)) || (String.IsNullOrEmpty(level2type)))
                    return normal;
                if (!level2type.Equals("TRIGGER"))
                    return normal;
                else
                    return "[" + Level0name + "].[" + level2name + "]";
            }
        }
        
        public string Level2name
        {
            get { return level2name; }
            set { level2name = value; }
        }

        public string Level2type
        {
            get { return level2type; }
            set { level2type = value; }
        }

        public string Level1name
        {
            get { return level1name; }
            set { level1name = value; }
        }

        public string Level1type
        {
            get { return level1type; }
            set { level1type = value; }
        }

        public string Level0name
        {
            get { return level0name; }
            set { level0name = value; }
        }

        public string Level0type
        {
            get { return level0type; }
            set { level0type = value; }
        }

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public override SQLScript Create()
        {
            Enums.ScripActionType action = Enums.ScripActionType.AddExtendedProperty;
            return new SQLScript(this.ToSqlAdd(), 0, action);
        }

        public override SQLScript Drop()
        {
            Enums.ScripActionType action = Enums.ScripActionType.DropExtendedProperty;
            return new SQLScript(this.ToSqlDrop(), 0, action);
        }

        public override Enums.ObjectStatusType Status
        {
            get { return status; }
            set { status = value; }
        }

        public override string ToSqlAdd()
        {
            string sql = "EXEC sys.sp_addextendedproperty @name=N'" + Name + "', @value=N'" + value + "' ,";
            sql += "@level0type=N'" + level0type + "',@level0name=N'" + level0name + "'";
            if (!String.IsNullOrEmpty(level1name))
                sql += ", @level1type=N'" + level1type + "',@level1name=N'" + level1name + "'";
            if (!String.IsNullOrEmpty(level2name))
                sql += ", @level2type=N'" + level2type + "',@level2name=N'" + level2name + "'";

            return sql + "\r\nGO\r\n";
        }

        public override string ToSqlDrop()
        {
            string sql = "EXEC sys.sp_dropextendedproperty @name=N'" + Name + "', @value=N'" + value + "' ,";
            sql += "@level0type=N'" + level0type + "',@level0name=N'" + level0name + "'";
            if (!String.IsNullOrEmpty(level1name))
                sql += ", @level1type=N'" + level1type + "',@level1name=N'" + level1name + "'";

            if (!String.IsNullOrEmpty(level2name))
                sql += ", @level2type=N'" + level2type + "',@level2name=N'" + level2name + "'";
            return sql + "\r\nGO\r\n";
        }

        public override string ToSql()
        {
            return ToSqlAdd();
        }

        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList list = new SQLScriptList();
            if (this.Parent.Status != Enums.ObjectStatusType.CreateStatus)
            {
                if (this.Status == Enums.ObjectStatusType.CreateStatus)
                    list.Add(this.Create());
                if (this.Status == Enums.ObjectStatusType.DropStatus)
                    list.Add(this.Drop());
            }
            return list;
        }
    }
}
