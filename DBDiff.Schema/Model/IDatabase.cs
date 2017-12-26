using System.Collections.Generic;

namespace DBDiff.Schema.Model
{
    public interface IDatabase : ISchemaBase
    {
        bool IsCaseSensity { get; }
        SqlAction ActionMessage { get; }
        IOption Options { get; }

        SQLScriptList ToSqlDiff(List<ISchemaBase> selectedSchemas);
        ISchemaBase Find(string objectFullName);
    }
}
