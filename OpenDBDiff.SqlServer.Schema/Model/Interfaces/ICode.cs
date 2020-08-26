using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using System.Collections.Generic;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    public interface ICode : ISchemaBase
    {
        SQLScriptList Rebuild();
        List<string> DependenciesIn { get; set; }
        List<string> DependenciesOut { get; set; }
        bool IsSchemaBinding { get; set; }
        string Text { get; set; }
    }
}
