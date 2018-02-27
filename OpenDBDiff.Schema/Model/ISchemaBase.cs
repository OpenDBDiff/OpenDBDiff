using System;

namespace OpenDBDiff.Schema.Model
{
    public interface ISchemaBase
    {
        ISchemaBase Clone(ISchemaBase parent);
        int DependenciesCount { get; }
        string FullName { get; }
        int Id { get; set; }
        Boolean HasState(ObjectStatus statusFind);
        string Name { get; set; }
        string Owner { get; set; }
        ISchemaBase Parent { get; set; }
        ObjectStatus Status { get; set; }
        Boolean IsSystem { get; set; }
        ObjectType ObjectType { get; set; }
        Boolean GetWasInsertInDiffList(ScriptAction action);
        void SetWasInsertInDiffList(ScriptAction action);
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
