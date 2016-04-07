using System;

namespace DBDiff.Schema
{
    public class SQLScript : IComparable<SQLScript>
    {
        public SQLScript(int deepvalue, string sqlScript, int dependenciesCount, Enums.ScripActionType action)
        {
            SQL = sqlScript;
            Dependencies = dependenciesCount;
            Status = action;
            Deep = deepvalue;
            //childs = new SQLScriptList();
        }

        public SQLScript(string sqlScript, int dependenciesCount, Enums.ScripActionType action)
        {
            SQL = sqlScript;
            Dependencies = dependenciesCount;
            Status = action;
            //childs = new SQLScriptList();
        }

        /*public SQLScriptList Childs
        {
            get { return childs; }
            set { childs = value; }
        }*/

        public int Deep { get; set; }

        public Enums.ScripActionType Status { get; set; }

        public int Dependencies { get; set; }

        public string SQL { get; set; }

        public bool IsDropAction
        {
            get
            {
                return ((Status == Enums.ScripActionType.DropView) || (Status == Enums.ScripActionType.DropFunction) || (Status == Enums.ScripActionType.DropStoredProcedure));
            }
        }

        public bool IsAddAction
        {
            get
            {
                return ((Status == Enums.ScripActionType.AddView) || (Status == Enums.ScripActionType.AddFunction) || (Status == Enums.ScripActionType.AddStoredProcedure));
            }
        }

        public int CompareTo(SQLScript other)
        {
            if (this.Deep == other.Deep)
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
            else
                return this.Deep.CompareTo(other.Deep);
        }
    }
}
