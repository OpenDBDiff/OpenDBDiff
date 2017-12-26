using System;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public abstract class CLRCode : Code
    {
        public CLRCode(ISchemaBase parent, Enums.ObjectType type, Enums.ScripActionType addAction, Enums.ScripActionType dropAction)
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
