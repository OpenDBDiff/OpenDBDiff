using System;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class Index : SQLServerSchemaBase
    {
        public enum IndexTypeEnum
        {
            Heap = 0,
            Clustered = 1,
            Nonclustered = 2,
            XML = 3,
            GEO = 4
        }

        public Index(ISchemaBase parent)
            : base(parent, Enums.ObjectType.Index)
        {
            FilterDefintion = "";
            Columns = new IndexColumns(parent);
        }

        public override ISchemaBase Clone(ISchemaBase parent)
        {
            Index index = new Index(parent)
            {
                AllowPageLocks = this.AllowPageLocks,
                AllowRowLocks = this.AllowRowLocks,
                Columns = this.Columns.Clone(),
                FillFactor = this.FillFactor,
                FileGroup = this.FileGroup,
                Id = this.Id,
                IgnoreDupKey = this.IgnoreDupKey,
                IsAutoStatistics = this.IsAutoStatistics,
                IsDisabled = this.IsDisabled,
                IsPadded = this.IsPadded,
                IsPrimaryKey = this.IsPrimaryKey,
                IsUniqueKey = this.IsUniqueKey,
                Name = this.Name,
                SortInTempDb = this.SortInTempDb,
                Status = this.Status,
                Type = this.Type,
                Owner = this.Owner,
                FilterDefintion = this.FilterDefintion
            };
            ExtendedProperties.ForEach(item => index.ExtendedProperties.Add(item));
            return index;
        }

        public string FileGroup { get; set; }

        public Boolean SortInTempDb { get; set; }

        public string FilterDefintion { get; set; }

        public IndexColumns Columns { get; set; }

        public Boolean IsAutoStatistics { get; set; }

        public Boolean IsUniqueKey { get; set; }

        public Boolean IsPrimaryKey { get; set; }

        public IndexTypeEnum Type { get; set; }

        public short FillFactor { get; set; }

        public Boolean IsDisabled { get; set; }

        public Boolean IsPadded { get; set; }

        public Boolean IgnoreDupKey { get; set; }

        public Boolean AllowPageLocks { get; set; }

        public Boolean AllowRowLocks { get; set; }

        public override string FullName
        {
            get
            {
                return Parent.FullName + ".[" + Name + "]";
            }
        }

        /// <summary>
        /// Compara dos indices y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(Index origin, Index destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (origin == null) throw new ArgumentNullException("origin");
            if (origin.AllowPageLocks != destination.AllowPageLocks) return false;
            if (origin.AllowRowLocks != destination.AllowRowLocks) return false;
            if (origin.FillFactor != destination.FillFactor) return false;
            if (origin.IgnoreDupKey != destination.IgnoreDupKey) return false;
            if (origin.IsAutoStatistics != destination.IsAutoStatistics) return false;
            if (origin.IsDisabled != destination.IsDisabled) return false;
            if (origin.IsPadded != destination.IsPadded) return false;
            if (origin.IsPrimaryKey != destination.IsPrimaryKey) return false;
            if (origin.IsUniqueKey != destination.IsUniqueKey) return false;
            if (origin.Type != destination.Type) return false;
            if (origin.SortInTempDb != destination.SortInTempDb) return false;
            if (!origin.FilterDefintion.Equals(destination.FilterDefintion)) return false;
            if (!IndexColumns.Compare(origin.Columns, destination.Columns)) return false;
            return CompareFileGroup(origin, destination);
        }

        public static Boolean CompareExceptIsDisabled(Index origin, Index destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (origin == null) throw new ArgumentNullException("origin");
            if (origin.AllowPageLocks != destination.AllowPageLocks) return false;
            if (origin.AllowRowLocks != destination.AllowRowLocks) return false;
            if (origin.FillFactor != destination.FillFactor) return false;
            if (origin.IgnoreDupKey != destination.IgnoreDupKey) return false;
            if (origin.IsAutoStatistics != destination.IsAutoStatistics) return false;
            if (origin.IsPadded != destination.IsPadded) return false;
            if (origin.IsPrimaryKey != destination.IsPrimaryKey) return false;
            if (origin.IsUniqueKey != destination.IsUniqueKey) return false;
            if (origin.Type != destination.Type) return false;
            if (origin.SortInTempDb != destination.SortInTempDb) return false;
            if (!origin.FilterDefintion.Equals(destination.FilterDefintion)) return false;
            if (!IndexColumns.Compare(origin.Columns, destination.Columns)) return false;
            //return true;
            return CompareFileGroup(origin, destination);
        }

        private static Boolean CompareFileGroup(Index origin, Index destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            if (origin == null) throw new ArgumentNullException("origin");
            if (origin.FileGroup != null)
            {
                if (!origin.FileGroup.Equals(destination.FileGroup)) return false;
            }
            return true;
        }

        public override string ToSql()
        {
            Database database = null;
            ISchemaBase current = this;
            while (database == null && current.Parent != null)
            {
                database = current.Parent as Database;
                current = current.Parent;
            }
            var isAzure10 = database.Info.Version == DatabaseInfo.VersionTypeEnum.SQLServerAzure10;

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
                        if (Columns[j].Order) sql.Append(" DESC"); else sql.Append(" ASC");
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
            sql.Append(")");
            if (!String.IsNullOrEmpty(FilterDefintion)) sql.Append("\r\n WHERE " + FilterDefintion + "\r\n");
            sql.Append(" WITH (");
            if (Parent.ObjectType == Enums.ObjectType.TableType)
            {
                if ((IgnoreDupKey) && (IsUniqueKey)) sql.Append("IGNORE_DUP_KEY = ON "); else sql.Append("IGNORE_DUP_KEY  = OFF ");
            }
            else
            {
                if (!isAzure10)
                {
                    if (IsPadded) sql.Append("PAD_INDEX = ON, "); else sql.Append("PAD_INDEX  = OFF, ");
                }

                if (IsAutoStatistics) sql.Append("STATISTICS_NORECOMPUTE = ON"); else sql.Append("STATISTICS_NORECOMPUTE  = OFF");
                if (Type != IndexTypeEnum.XML)
                    if ((IgnoreDupKey) && (IsUniqueKey)) sql.Append("IGNORE_DUP_KEY = ON, "); else sql.Append(", IGNORE_DUP_KEY  = OFF");

                if (!isAzure10)
                {
                    if (AllowRowLocks) sql.Append(", ALLOW_ROW_LOCKS = ON"); else sql.Append(", ALLOW_ROW_LOCKS  = OFF");
                    if (AllowPageLocks) sql.Append(", ALLOW_PAGE_LOCKS = ON"); else sql.Append(", ALLOW_PAGE_LOCKS  = OFF");
                    if (FillFactor != 0) sql.Append(", FILLFACTOR = " + FillFactor.ToString());
                }
            }
            sql.Append(")");
            if (!isAzure10)
            {
                if (!String.IsNullOrEmpty(FileGroup)) sql.Append(" ON [" + FileGroup + "]");
            }
            sql.Append("\r\nGO\r\n");
            if (IsDisabled)
                sql.Append("ALTER INDEX [" + Name + "] ON " + ((Table)Parent).FullName + " DISABLE\r\nGO\r\n");

            sql.Append(ExtendedProperties.ToSql());
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

        private string ToSqlDrop(string FileGroupName)
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
                return new SQLScript(ToSqlAdd(), Parent.DependenciesCount, action);
            }
            return null;
        }

        public override SQLScript Drop()
        {
            Enums.ScripActionType action = Enums.ScripActionType.DropIndex;
            if (!GetWasInsertInDiffList(action))
            {
                SetWasInsertInDiffList(action);
                return new SQLScript(ToSqlDrop(), Parent.DependenciesCount, action);
            }
            return null;
        }

        private string ToSqlEnabled()
        {
            if (IsDisabled)
                return "ALTER INDEX [" + Name + "] ON " + Parent.FullName + " DISABLE\r\nGO\r\n";
            return "ALTER INDEX [" + Name + "] ON " + Parent.FullName + " REBUILD\r\nGO\r\n";
        }

        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList list = new SQLScriptList();
            if (Status != Enums.ObjectStatusType.OriginalStatus)
                RootParent.ActionMessage[Parent.FullName].Add(this);

            if (HasState(Enums.ObjectStatusType.DropStatus))
                list.Add(Drop());
            if (HasState(Enums.ObjectStatusType.CreateStatus))
                list.Add(Create());
            if (HasState(Enums.ObjectStatusType.AlterStatus))
            {
                list.Add(Drop());
                list.Add(Create());
            }
            if (Status == Enums.ObjectStatusType.DisabledStatus)
            {
                list.Add(ToSqlEnabled(), Parent.DependenciesCount, Enums.ScripActionType.AlterIndex);
            }
            /*if (this.Status == StatusEnum.ObjectStatusType.ChangeFileGroup)
            {
                listDiff.Add(this.ToSQLDrop(this.FileGroup), ((Table)Parent).DependenciesCount, StatusEnum.ScripActionType.DropIndex);
                listDiff.Add(this.ToSQLAdd(), ((Table)Parent).DependenciesCount, StatusEnum.ScripActionType.AddIndex);
            }*/
            list.AddRange(ExtendedProperties.ToSqlDiff());
            return list;
        }
    }
}
