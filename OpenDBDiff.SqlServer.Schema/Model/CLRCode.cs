using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using System;

namespace OpenDBDiff.SqlServer.Schema.Model
{
    public abstract class CLRCode : Code
    {
        public CLRCode(ISchemaBase parent, ObjectType type, ScriptAction addAction, ScriptAction dropAction)
            : base(parent, type, addAction, dropAction)
        {
        }

        public string AssemblyMethod { get; set; }

        public string AssemblyExecuteAs { get; set; }

        public string AssemblyName { get; set; }

        public Boolean IsAssembly { get; set; }

        public string AssemblyClass { get; set; }

        public int AssemblyId { get; set; }

        public override Boolean IsCodeType
        {
            get { return true; }
        }
    }
}
