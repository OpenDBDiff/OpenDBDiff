using OpenDBDiff.Abstractions.Schema.Model;
using System;
using System.Collections.Generic;

namespace OpenDBDiff.SqlServer.Schema.Options
{
    public class SqlOptionScript : IOptionsContainer<bool>
    {
        private Boolean alterObjectOnSchemaBinding = true;

        public SqlOptionScript()
        {
        }

        public SqlOptionScript(IOptionsContainer<bool> optionsContainer)
        {
            AlterObjectOnSchemaBinding = optionsContainer.GetOptions()["AlterObjectOnSchemaBinding"];
        }

        public Boolean AlterObjectOnSchemaBinding
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
