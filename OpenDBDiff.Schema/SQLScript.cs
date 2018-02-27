using System;

namespace OpenDBDiff.Schema
{
    public class SQLScript : IComparable<SQLScript>
    {
        public SQLScript(int deepvalue, string sqlScript, int dependenciesCount, ScriptAction action)
        {
            SQL = sqlScript;
            Dependencies = dependenciesCount;
            Status = action;
            Deep = deepvalue;
            //childs = new SQLScriptList();
        }

        public SQLScript(string sqlScript, int dependenciesCount, ScriptAction action)
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

        public ScriptAction Status { get; set; }

        public int Dependencies { get; set; }

        public string SQL { get; set; }

        public bool IsDropAction
        {
            get
            {
                return ((Status == ScriptAction.DropView) || (Status == ScriptAction.DropFunction) || (Status == ScriptAction.DropStoredProcedure));
            }
        }

        public bool IsAddAction
        {
            get
            {
                return ((Status == ScriptAction.AddView) || (Status == ScriptAction.AddFunction) || (Status == ScriptAction.AddStoredProcedure));
            }
        }

        public int CompareTo(SQLScript other)
        {
            if (this.Deep == other.Deep)
            {
                if (this.Status == other.Status)
                {
                    if (this.Status == ScriptAction.DropTable || this.Status == ScriptAction.DropConstraint || this.Status == ScriptAction.DropTrigger)
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
