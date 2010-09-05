using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Schema.Model
{
    public interface ISchemaBase
    {
        ISchemaBase Clone(ISchemaBase parent);
        int DependenciesCount { get; }
        string FullName { get; }
        int Id { get; set; }
        Boolean HasState(Enums.ObjectStatusType statusFind);
        string Name { get; set; }
        string Owner { get; set; }
        ISchemaBase Parent { get; set;}
        Enums.ObjectStatusType Status { get; set;}                                             
        Boolean IsSystem { get; set; }
        Enums.ObjectType ObjectType { get; set;}
        Boolean GetWasInsertInDiffList(Enums.ScripActionType action);
        void SetWasInsertInDiffList(Enums.ScripActionType action);        
        string ToSqlDrop();
        string ToSqlAdd();
        string ToSql();
        SQLScriptList ToSqlDiff();
        SQLScript Create();
        SQLScript Drop();
        int CompareFullNameTo(string name, string myName);
        Boolean IsCodeType { get; }
        IDatabase RootParent { get; }
    }
}
