using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Schema.Model
{
    public interface ISchemaBase
    {
        ISchemaBase Parent { get; set;}
        StatusEnum.ObjectStatusType Status { get; set;}
        string Name { get; set;}
        string FullName { get; }
        int Id { get; set;}
        StatusEnum.ObjectTypeEnum ObjectType { get; set;}
        Boolean GetWasInsertInDiffList(StatusEnum.ScripActionType action);
        void SetWasInsertInDiffList(StatusEnum.ScripActionType action);
        string ToSQLDrop();
        string ToSQLAdd();
    }
}
