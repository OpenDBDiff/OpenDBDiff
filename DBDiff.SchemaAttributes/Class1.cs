using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.SchemaAttributes
{
    [AttributeUsage(AttributeTargets.Class|
    AttributeTargets.Property,
    AllowMultiple = true)]
    public class DatabaseRootAttribute:Attribute 
    {

    }
}
