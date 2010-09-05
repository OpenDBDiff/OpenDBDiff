using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class FullTextIndex : SQLServerSchemaBase
    {
        private Boolean isDisabled;
        private string fullText;
        private string index;
        private string changeTrackingState;
        private string fileGroup;
        private List<FullTextIndexColumn> columns;

        public FullTextIndex(ISchemaBase parent)
            : base(parent, Enums.ObjectType.FullTextIndex)
        {
            columns = new List<FullTextIndexColumn>();
        }

        public override ISchemaBase Clone(ISchemaBase parent)
        {
            FullTextIndex index = new FullTextIndex(parent);
            index.ChangeTrackingState = this.ChangeTrackingState;
            index.FullText = this.FullText;
            index.Name = this.Name;
            index.FileGroup = this.FileGroup;
            index.Id = this.Id;
            index.Index = this.Index;
            index.IsDisabled = this.IsDisabled;
            index.Status = this.Status;
            index.Owner = this.Owner;
            index.Columns = this.Columns;
            this.ExtendedProperties.ForEach(item => index.ExtendedProperties.Add(item));
            return index;
        }

        public string FileGroup
        {
            get { return fileGroup; }
            set { fileGroup = value; }
        }

        public Boolean IsDisabled
        {
            get { return isDisabled; }
            set { isDisabled = value; }
        }

        public string Index
        {
            get { return index; }
            set { index = value; }
        }

        public string FullText
        {
            get { return fullText; }
            set { fullText = value; }
        }

        public string ChangeTrackingState
        {
            get { return changeTrackingState; }
            set { changeTrackingState = value; }
        }

        public override string FullName
        {
            get
            {
                return this.Name;
            }
        }

        public List<FullTextIndexColumn> Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        public override SQLScript Create()
        {
            Enums.ScripActionType action = Enums.ScripActionType.AddFullTextIndex;
            if (!GetWasInsertInDiffList(action))
            {
                SetWasInsertInDiffList(action);
                return new SQLScript(this.ToSqlAdd(), Parent.DependenciesCount, action);
            }
            else
                return null;
        }

        public override SQLScript Drop()
        {
            Enums.ScripActionType action = Enums.ScripActionType.DropFullTextIndex;
            if (!GetWasInsertInDiffList(action))
            {
                SetWasInsertInDiffList(action);
                return new SQLScript(this.ToSqlDrop(), Parent.DependenciesCount, action);
            }
            else
                return null;
        }

        public override string ToSqlAdd()
        {
            string sql = "CREATE FULLTEXT INDEX ON " + Parent.FullName + "( ";
            columns.ForEach (item => { sql += "[" + item.ColumnName + "] LANGUAGE [" + item.Language + "],"; });
            sql = sql.Substring(0,sql.Length -1);
            sql += ")\r\n";
            if (((Database)this.RootParent).Info.Version == DatabaseInfo.VersionTypeEnum.SQLServer2008)
            {
                sql += "KEY INDEX " + Index + " ON ([" + FullText + "]";
                sql += ", FILEGROUP [" + FileGroup + "]";
                sql += ") WITH (CHANGE_TRACKING " + ChangeTrackingState + ")";
            }
            else
            {
                sql += "KEY INDEX " + Index + " ON [" + FullText + "]";
                sql += " WITH CHANGE_TRACKING " + ChangeTrackingState;
            }
            sql += "\r\nGO\r\n";
            if (!this.IsDisabled)
                sql += "ALTER FULLTEXT INDEX ON " + Parent.FullName + " ENABLE\r\nGO\r\n";
            return sql;
        }

        public string ToSqlEnabled()
        {
            if (this.IsDisabled)
                return "ALTER FULLTEXT INDEX ON " + Parent.FullName + " DISABLE\r\nGO\r\n";
            else
                return "ALTER FULLTEXT INDEX ON " + Parent.FullName + " ENABLE\r\nGO\r\n";
        }

        public override string ToSqlDrop()
        {
            return "DROP FULLTEXT INDEX ON " + Parent.FullName + "\r\nGO\r\n";
        }

        public override string ToSql()
        {
            return ToSqlAdd();
        }

        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList list = new SQLScriptList();
            if (this.Status != Enums.ObjectStatusType.OriginalStatus)
                RootParent.ActionMessage[Parent.FullName].Add(this);

            if (this.HasState(Enums.ObjectStatusType.DropStatus))
                list.Add(Drop());
            if (this.HasState(Enums.ObjectStatusType.CreateStatus))
                list.Add(Create());
            if (this.HasState(Enums.ObjectStatusType.AlterStatus))
            {
                list.Add(Drop());
                list.Add(Create());
            }
            if (this.Status == Enums.ObjectStatusType.DisabledStatus)
            {
                list.Add(this.ToSqlEnabled(), Parent.DependenciesCount, Enums.ScripActionType.AlterFullTextIndex);
            }
            /*if (this.Status == StatusEnum.ObjectStatusType.ChangeFileGroup)
            {
                listDiff.Add(this.ToSQLDrop(this.FileGroup), ((Table)Parent).DependenciesCount, StatusEnum.ScripActionType.DropIndex);
                listDiff.Add(this.ToSQLAdd(), ((Table)Parent).DependenciesCount, StatusEnum.ScripActionType.AddIndex);
            }*/
            list.AddRange(this.ExtendedProperties.ToSqlDiff());
            return list;
        }

        public Boolean Compare(FullTextIndex destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (!this.ChangeTrackingState.Equals(destino.ChangeTrackingState)) return false;
            if (!this.FullText.Equals(destino.FullText)) return false;
            if (!this.Index.Equals(destino.Index)) return false;
            if (this.IsDisabled != destino.IsDisabled) return false;
            if (this.Columns.Count != destino.Columns.Count) return false;
            if (this.Columns.Exists(item => { return !destino.Columns.Exists(item2 => item2.ColumnName.Equals(item.ColumnName)); })) return false;
            if (destino.Columns.Exists(item => { return !this.Columns.Exists(item2 => item2.ColumnName.Equals(item.ColumnName)); })) return false;

            return true;
        }
    }
}
