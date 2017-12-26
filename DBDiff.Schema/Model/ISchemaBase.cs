using System;

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
        ISchemaBase Parent { get; set; }
        Enums.ObjectStatusType Status { get; set; }
        Boolean IsSystem { get; set; }
        Enums.ObjectType ObjectType { get; set; }
        Boolean GetWasInsertInDiffList(Enums.ScripActionType action);
        void SetWasInsertInDiffList(Enums.ScripActionType action);
        void ResetWasInsertInDiffList();
        string ToSqlDrop();
        string ToSqlAdd();
        string ToSql();
        SQLScriptList ToSqlDiff(System.Collections.Generic.ICollection<ISchemaBase> schemas);
        SQLScript Create();
        SQLScript Drop();
        int CompareFullNameTo(string name, string myName);
        Boolean IsCodeType { get; }
        IDatabase RootParent { get; }
    }
}
