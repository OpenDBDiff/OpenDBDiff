using System;

namespace DBDiff.Schema.SQLServer.Generates.Options
{
    public class SqlOptionScript
    {
        private Boolean alterObjectOnSchemaBinding = true;

        public Boolean AlterObjectOnSchemaBinding
        {
            get { return alterObjectOnSchemaBinding; }
            set { alterObjectOnSchemaBinding = value; }
        }
    }
}
