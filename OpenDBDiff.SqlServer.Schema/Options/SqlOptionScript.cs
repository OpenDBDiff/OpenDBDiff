using OpenDBDiff.Abstractions.Schema.Model;
using System.Collections.Generic;

namespace OpenDBDiff.SqlServer.Schema.Options
{
    public class SqlOptionScript : IOptionsContainer<bool>
    {
        private bool alterObjectOnSchemaBinding = true;

        public SqlOptionScript()
        {
        }

        public SqlOptionScript(IOptionsContainer<bool> optionsContainer)
        {
            AlterObjectOnSchemaBinding = optionsContainer.GetOptions()["AlterObjectOnSchemaBinding"];
        }

        public bool AlterObjectOnSchemaBinding
        {
            get { return alterObjectOnSchemaBinding; }
            set { alterObjectOnSchemaBinding = value; }
        }

        public IDictionary<string, bool> GetOptions()
        {
            return new Dictionary<string, bool>() { { "AlterObjectOnSchemaBinding", AlterObjectOnSchemaBinding } };
        }
    }
}
