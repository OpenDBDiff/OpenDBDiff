using OpenDBDiff.Abstractions.Schema.Model;
using System;
using System.Collections.Generic;

namespace OpenDBDiff.SqlServer.Schema.Options
{
    public class SqlOptionScript : IOptionsContainer<bool>
    {
        private Boolean alterObjectOnSchemaBinding = true;
        private Boolean useAlterInsteadRebuildForTables = false;

        public SqlOptionScript()
        {
        }

        public SqlOptionScript(IOptionsContainer<bool> optionsContainer)
        {
          AlterObjectOnSchemaBinding = optionsContainer.GetOptions()["AlterObjectOnSchemaBinding"];
          UseAlterInsteadRebuildForTables = optionsContainer.GetOptions()["UseAlterInsteadRebuildForTables"];
        }

        public Boolean AlterObjectOnSchemaBinding
        {
          get { return alterObjectOnSchemaBinding; }
          set { alterObjectOnSchemaBinding = value; }
        }
        
        public Boolean UseAlterInsteadRebuildForTables
        {
            get { return useAlterInsteadRebuildForTables; }
            set { useAlterInsteadRebuildForTables = value; }
        }

        public IDictionary<string, bool> GetOptions()
        {
          Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
          dictionary.Add("AlterObjectOnSchemaBinding", AlterObjectOnSchemaBinding);
          dictionary.Add("UseAlterInsteadRebuildForTables", UseAlterInsteadRebuildForTables);
          return dictionary;
        }

        public void SetOptions(IDictionary<string, bool> options)
        {
          AlterObjectOnSchemaBinding = options["AlterObjectOnSchemaBinding"];
          UseAlterInsteadRebuildForTables = options["UseAlterInsteadRebuildForTables"];
        }
    }
}
