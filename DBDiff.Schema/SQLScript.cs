using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Schema
{
    public class SQLScript : IComparable<SQLScript>
    {
        private string sql;
        private int dependencies;
        private StatusEnum.ScripActionType status;

        public StatusEnum.ScripActionType Status
        {
            get { return status; }
            set { status = value; }
        }

        public int Dependencies
        {
            get { return dependencies; }
            set { dependencies = value; }
        }

        public string SQL
        {
            get { return sql; }
            set { sql = value; }
        }

        public int CompareTo(SQLScript other)
        {
            if (this.Status == other.Status)
            {
                if (this.Status == StatusEnum.ScripActionType.DropTable || this.Status == StatusEnum.ScripActionType.DropConstraint || this.Status == StatusEnum.ScripActionType.DropTrigger)
                    return other.Dependencies.CompareTo(this.Dependencies);
                else
                    return this.Dependencies.CompareTo(other.Dependencies);
            }
            else
                return this.Status.CompareTo(other.Status);
        }
    }
}
