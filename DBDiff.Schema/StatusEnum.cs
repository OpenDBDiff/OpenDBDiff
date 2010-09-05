using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Schema
{
    public static class Enums
    {
        /// <summary>
        /// OriginalStatus = El objeto no sufrio modificaciones.
        /// CreateStatus = El objeto se debe crear.
        /// DropStatus = El objeto se debe eliminar.
        /// AlterStatus = El objeto sufrio modificaciones.
        /// AlterRebuildStatus = El objeto sufrio modificaciones, pero se debe hacer un DROP y ADD.
        /// AlterPropertiesStatus = El objeto sufrio modificaciones en sus propiedades, pero no en su estructura.
        /// </summary>
        [Flags]
        public enum ObjectStatusType
        {
            OriginalStatus = 0,
            AlterStatus = 2,
            AlterBodyStatus = 4,
            RebuildStatus = 8,
            RebuildDependenciesStatus = 16,
            UpdateStatus = 32,
            CreateStatus = 64,
            DropStatus = 128,
            DisabledStatus = 256,
            ChangeOwner = 512,
            DropOlderStatus = 1024,
            BindStatus = 2048,
            PermisionSet = 4096
        }

        public enum ScripActionType
        {
            None = 0,
            UseDatabase = 1,
            AddFileGroup = 2,
            AddFile = 3,
            AlterFile = 4,
            AlterFileGroup = 5,
            UnbindRuleColumn = 6,
            UnbindRuleType = 7,
            DropRule = 8,
            AddRule = 9,
            DropConstraintFK = 10,
            DropConstraint = 11,
            DropConstraintPK = 12,
            DropSynonyms = 13,
            DropStoreProcedure = 14,            
            DropTrigger = 16,
            DropView = 17,
            DropFunction = 17,
            DropIndex = 18,
            DropTable = 19,
            AlterColumnFormula = 20,
            AlterColumn = 21,
            AddRole = 22,
            AddUser = 23,
            AddSchema = 24,
            AddDefault = 25,                        
            AddAssembly = 26,
            AddAssemblyFile = 27,
            AddUserDataType = 28,
            AddTableType = 29,
            DropPartitionScheme = 30,
            DropPartitionFunction = 31,
            AddFullText = 32,
            AddXMLSchema = 33,
            AlterAssembly = 34,
            UpdateTable = 35,
            AlterTable = 36,
            AlterIndex = 37,
            AddTable = 38,
            RebuildTable = 39,
            AlterColumnRestore = 40,
            AlterColumnFormulaRestore = 41,
            AlterFunction = 42,
            AlterView = 43,
            AlterProcedure = 44,
            AddIndex = 45,
            AddFunction = 46,
            AddView = 46,
            AddTrigger = 47,
            AddConstraint = 48,
            AddConstraintPK = 49,
            AddConstraintFK = 50,
            AlterConstraint = 51,
            EnabledTrigger = 52,
            AddSynonyms = 53,
            AddStoreProcedure = 54,
            DropOptions = 55,
            AddOptions = 56,
            AddPartitionFunction = 57,
            AlterPartitionFunction = 58,
            AddPartitionScheme = 59,
            DropFullText = 60,
            DropTableType = 61,
            DropUserDataType = 62,
            DropXMLSchema = 63,
            DropAssemblyUserDataType = 64,
            DropAssemblyFile = 65,
            DropAssembly = 66,
            DropDefault = 67,
            DropSchema = 68,
            DropUser = 69,
            DropRole = 70,
            DropFile = 71,
            DropFileGroup = 72,
            AddExtendedProperty = 73,
            DropExtendedProperty = 74
        }

        public enum ObjectType
        {
            None = 0,
            Table = 1,
            Column = 2,
            Trigger = 3,
            Constraint = 4,
            ConstraintColumn = 5,
            Index = 6,
            IndexColumn = 7,
            UserDataType = 8,
            XMLSchema = 9,
            View = 10,
            Function = 11,
            StoreProcedure = 12,
            TableOption = 13,
            Database = 14,
            Schema = 15,
            FileGroup = 16,
            File = 17,
            Default = 18,
            Rule = 19,
            Synonym = 20,
            Assembly = 21,
            User = 22,
            Role = 23,
            FullText = 24,
            AssemblyFile = 25,
            CLRStoreProcedure = 26,
            CLRTrigger = 27,
            CLRFunction = 28,
            ExtendedProperty = 30,
            Partition = 31,
            PartitionFunction = 32,
            PartitionScheme = 33,
            TableType = 34
        }
    }
}
