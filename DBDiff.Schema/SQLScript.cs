using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Schema
{
    public class SQLScript : IComparable<SQLScript>
    {
        private string sql;
        private int dependencies;
        private Enums.ScripActionType status;

        public SQLScript(string sqlScript, int dependenciesCount, Enums.ScripActionType action)
        {
            sql = sqlScript;
            dependencies = dependenciesCount;
            status = action;
        }

        public Enums.ScripActionType Status
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
                if (this.Status == Enums.ScripActionType.DropTable || this.Status == Enums.ScripActionType.DropConstraint || this.Status == Enums.ScripActionType.DropTrigger)
                    return other.Dependencies.CompareTo(this.Dependencies);
                else
                    return this.Dependencies.CompareTo(other.Dependencies);
            }
            else
                return this.Status.CompareTo(other.Status);
        }
    }
}
