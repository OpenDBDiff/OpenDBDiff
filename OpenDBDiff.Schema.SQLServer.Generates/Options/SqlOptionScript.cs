using System;
using System.Collections.Generic;
using OpenDBDiff.Schema.Model;

namespace OpenDBDiff.Schema.SQLServer.Generates.Options
{
    public class SqlOptionScript : Schema.Model.IOptionsContainer<bool>
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
