using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Schema.Model
{
    public interface ISchemaBase
    {
        ISchemaBase Parent { get; set;}
        Enums.ObjectStatusType Status { get; set;}
        Boolean HasState(Enums.ObjectStatusType statusFind);
        string Name { get; set;}
        string Owner { get; set; }
        string FullName { get; }
        int Id { get; set;}
        int DependenciesCount { get; }
        Boolean IsSystem { get; set; }
        Enums.ObjectType ObjectType { get; set;}
        Boolean GetWasInsertInDiffList(Enums.ScripActionType action);
        void SetWasInsertInDiffList(Enums.ScripActionType action);        
        string ToSqlDrop();
        string ToSqlAdd();
        string ToSql();
        SQLScript Create();
        SQLScript Drop();
        Boolean IsCodeType { get; }        
    }
}
