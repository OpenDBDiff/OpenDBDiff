using System.Collections.Generic;

namespace OpenDBDiff.Schema.Model
{
    public interface IDatabase : ISchemaBase
    {
        bool IsCaseSensity { get; }
        SqlAction ActionMessage { get; }
        IOption Options { get; }

        new SQLScriptList ToSqlDiff(ICollection<ISchemaBase> selectedSchemas);
        ISchemaBase Find(string objectFullName);
    }
}
