using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
