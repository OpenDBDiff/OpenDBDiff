using System.Collections.Generic;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
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
