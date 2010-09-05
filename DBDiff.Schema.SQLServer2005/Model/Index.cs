using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class Index : SQLServerSchemaBase
    {
        public enum IndexTypeEnum
        {
            Heap = 0,
            Clustered = 1,
            Nonclustered = 2,
            XML = 3
        }

        private Boolean allow_row_locks;
        private Boolean allow_page_locks;
        private Boolean ignore_dup_key;
        private Boolean isPadded;
        private Boolean isDisabled;
        private short fillFactor;
        private IndexTypeEnum type;
        private Boolean isPrimaryKey;
        private Boolean isUniqueKey;
        private Boolean isAutoStatistics;
        private Boolean sortInTempDb;
        private IndexColumns columns;
        private string fileGroup;

        public Index(ISchemaBase table)
            : base(Enums.ObjectType.Index)
        {
            Parent = table;
            columns = new IndexColumns(table);
        }

        public Index Clone(ISchemaBase parent)
        {
            Index index = new Index(parent);
            index.AllowPageLocks = this.AllowPageLocks;
            index.AllowRowLocks = this.AllowRowLocks;
            index.Columns = this.Columns.Clone();
            index.FillFactor = this.FillFactor;
            index.FileGroup = this.FileGroup;
            index.Id = this.Id;
            index.IgnoreDupKey = this.IgnoreDupKey;
            index.IsAutoStatistics = this.IsAutoStatistics;
            index.IsDisabled = this.IsDisabled;
            index.IsPadded = this.IsPadded;
            index.IsPrimaryKey = this.IsPrimaryKey;
            index.IsUniqueKey = this.IsUniqueKey;
            index.Name = this.Name;
            index.SortInTempDb = this.SortInTempDb;
            index.Status = this.Status;
            index.Type = this.Type;
            index.Owner = this.Owner;
            index.Guid = this.Guid;
            return index;
        }

        /// <summary>
        /// Gets or sets the file group.
        /// </summary>
        /// <value>The file group.</value>
        public string FileGroup
        {
            get { return fileGroup; }
            set { fileGroup = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [sort in temp db].
        /// </summary>
        /// <value><c>true</c> if [sort in temp db]; otherwise, <c>false</c>.</value>
        public Boolean SortInTempDb
        {
            get { return sortInTempDb; }
            set { sortInTempDb = value; }
        }

        public IndexColumns Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this index is auto statistics on/off.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this index is auto statistics; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsAutoStatistics
        {
          get { return isAutoStatistics; }
          set { isAutoStatistics = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this index is unique key.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this index is unique key; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsUniqueKey
        {
            get { return isUniqueKey; }
            set { isUniqueKey = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this index is primary key.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this index is primary key; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsPrimaryKey
        {
            get { return isPrimaryKey; }
            set { isPrimaryKey = value; }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public IndexTypeEnum Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Gets or sets the fill factor.
        /// </summary>
        /// <value>The fill factor.</value>
        public short FillFactor
        {
            get { return fillFactor; }
            set { fillFactor = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this index is disabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this index is disabled; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsDisabled
        {
            get { return isDisabled; }
            set { isDisabled = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this index is padded.
        /// </summary>
        /// <value><c>true</c> if this index is padded; otherwise, <c>false</c>.</value>
        public Boolean IsPadded
        {
            get { return isPadded; }
            set { isPadded = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [ignore dup key].
        /// </summary>
        /// <value><c>true</c> if [ignore dup key]; otherwise, <c>false</c>.</value>
        public Boolean IgnoreDupKey
        {
            get { return ignore_dup_key; }
            set { ignore_dup_key = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow page locks].
        /// </summary>
        /// <value><c>true</c> if [allow page locks]; otherwise, <c>false</c>.</value>
        public Boolean AllowPageLocks
        {
            get { return allow_page_locks; }
            set { allow_page_locks = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow row locks].
        /// </summary>
        /// <value><c>true</c> if [allow row locks]; otherwise, <c>false</c>.</value>
        public Boolean AllowRowLocks
        {
            get { return allow_row_locks; }
            set { allow_row_locks = value; }
        }

        /// <summary>
        /// Compara dos indices y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(Index origen, Index destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (origen.AllowPageLocks != destino.AllowPageLocks) return false;
            if (origen.AllowRowLocks != destino.AllowRowLocks) return false;
            if (origen.FillFactor != destino.FillFactor) return false;
            if (origen.IgnoreDupKey != destino.IgnoreDupKey) return false;
            if (origen.IsAutoStatistics != destino.IsAutoStatistics) return false;
            if (origen.IsDisabled != destino.IsDisabled) return false;
            if (origen.IsPadded != destino.IsPadded) return false;
            if (origen.IsPrimaryKey != destino.IsPrimaryKey) return false;
            if (origen.IsUniqueKey != destino.IsUniqueKey) return false;
            if (origen.Type != destino.Type) return false;
            if (origen.SortInTempDb != destino.SortInTempDb) return false;
            if (!IndexColumns.Compare(origen.Columns, destino.Columns)) return false;
            //return true;
            return CompareFileGroup(origen,destino);
        }

        public static Boolean CompareExceptIsDisabled(Index origen, Index destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (origen.AllowPageLocks != destino.AllowPageLocks) return false;
            if (origen.AllowRowLocks != destino.AllowRowLocks) return false;
            if (origen.FillFactor != destino.FillFactor) return false;
            if (origen.IgnoreDupKey != destino.IgnoreDupKey) return false;
            if (origen.IsAutoStatistics != destino.IsAutoStatistics) return false;            
            if (origen.IsPadded != destino.IsPadded) return false;
            if (origen.IsPrimaryKey != destino.IsPrimaryKey) return false;
            if (origen.IsUniqueKey != destino.IsUniqueKey) return false;
            if (origen.Type != destino.Type) return false;
            if (origen.SortInTempDb != destino.SortInTempDb) return false;
            if (!IndexColumns.Compare(origen.Columns, destino.Columns)) return false;
            //return true;
            return CompareFileGroup(origen, destino);
        }

        public static Boolean CompareFileGroup(Index origen, Index destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (origen.FileGroup != null)
            {
                if (!origen.FileGroup.Equals(destino.FileGroup)) return false;
            }
            return true;
        }

        public override string ToSql()
        {
            StringBuilder sql = new StringBuilder();
            string includes = "";
            if ((Type == IndexTypeEnum.Clustered) && (IsUniqueKey)) sql.Append("CREATE UNIQUE CLUSTERED ");
            if ((Type == IndexTypeEnum.Clustered) && (!IsUniqueKey)) sql.Append("CREATE CLUSTERED ");
            if ((Type == IndexTypeEnum.Nonclustered) && (IsUniqueKey)) sql.Append("CREATE UNIQUE NONCLUSTERED ");
            if ((Type == IndexTypeEnum.Nonclustered) && (!IsUniqueKey)) sql.Append("CREATE NONCLUSTERED ");
            if (Type == IndexTypeEnum.XML) sql.Append("CREATE PRIMARY XML ");
            sql.Append("INDEX [" + Name + "] ON " + Parent.FullName + "\r\n(\r\n");
            /*Ordena la coleccion de campos del Indice en funcion de la propieda IsIncluded*/
            Columns.Sort();             
            for (int j = 0; j < Columns.Count; j++)
            {
                if (!Columns[j].IsIncluded)
                {
                    sql.Append("\t[" + Columns[j].Name + "]");
                    if (Type != IndexTypeEnum.XML)
                    {
                        if (this.Columns[j].Order) sql.Append(" DESC"); else sql.Append(" ASC");
                    }
                    if (j < Columns.Count - 1) sql.Append(",");
                    sql.Append("\r\n");
                }
                else
                {
                    if (String.IsNullOrEmpty(includes)) includes = ") INCLUDE (";
                    includes += "[" + Columns[j].Name + "],";
                }
            }
            if (!String.IsNullOrEmpty(includes)) includes = includes.Substring(0, includes.Length - 1);
            sql.Append(includes);
            sql.Append(") WITH (");            
            if (IsPadded) sql.Append("PAD_INDEX = ON, "); else sql.Append("PAD_INDEX  = OFF, ");
            if (IsAutoStatistics) sql.Append("STATISTICS_NORECOMPUTE = ON, "); else sql.Append("STATISTICS_NORECOMPUTE  = OFF, ");
            if (Type != IndexTypeEnum.XML)
                if ((IgnoreDupKey) && (IsUniqueKey)) sql.Append("IGNORE_DUP_KEY = ON, "); else sql.Append("IGNORE_DUP_KEY  = OFF, ");
            if (AllowRowLocks) sql.Append("ALLOW_ROW_LOCKS = ON, "); else sql.Append("ALLOW_ROW_LOCKS  = OFF, ");
            if (AllowPageLocks) sql.Append("ALLOW_PAGE_LOCKS = ON"); else sql.Append("ALLOW_PAGE_LOCKS  = OFF");
            if (FillFactor != 0) sql.Append(", FILLFACTOR = " + FillFactor.ToString());
            sql.Append(")");
            if (!String.IsNullOrEmpty(FileGroup)) sql.Append(" ON [" + FileGroup + "]");
            sql.Append("\r\nGO\r\n");
            if (IsDisabled)
                sql.Append("ALTER INDEX [" + Name + "] ON " + ((Table)Parent).FullName + " DISABLE\r\nGO\r\n");
            return sql.ToString();
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        public override string ToSqlDrop()
        {
            return ToSqlDrop(null);
        }

        public string ToSqlDrop(string FileGroupName)
        {
            string sql = "DROP INDEX [" + Name + "] ON " + Parent.FullName;
            if (!String.IsNullOrEmpty(FileGroupName)) sql += " WITH (MOVE TO [" + FileGroupName + "])";
            sql += "\r\nGO\r\n";
            return sql;
        }

        public override SQLScript Create()
        {
            Enums.ScripActionType action = Enums.ScripActionType.AddIndex;
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
            Enums.ScripActionType action = Enums.ScripActionType.DropIndex;
            if (!GetWasInsertInDiffList(action))
            {
                SetWasInsertInDiffList(action);
                return new SQLScript(this.ToSqlDrop(), Parent.DependenciesCount, action);
            }
            else
                return null;
        }

        public string ToSQLEnabledDisabled()
        {
            StringBuilder sql = new StringBuilder();
            if (this.IsDisabled)
                return "ALTER INDEX [" + Name + "] ON " + Parent.FullName + " DISABLE\r\nGO\r\n";
            else
                return "ALTER INDEX [" + Name + "] ON " + Parent.FullName + " REBUILD\r\nGO\r\n";
        }

        public SQLScriptList ToSQLDiff()
        {
            SQLScriptList list = new SQLScriptList();

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
                list.Add(this.ToSQLEnabledDisabled(), Parent.DependenciesCount, Enums.ScripActionType.AlterIndex);
            }
            /*if (this.Status == StatusEnum.ObjectStatusType.ChangeFileGroup)
            {
                listDiff.Add(this.ToSQLDrop(this.FileGroup), ((Table)Parent).DependenciesCount, StatusEnum.ScripActionType.DropIndex);
                listDiff.Add(this.ToSQLAdd(), ((Table)Parent).DependenciesCount, StatusEnum.ScripActionType.AddIndex);
            }*/
            return list;
        }
    }
}
