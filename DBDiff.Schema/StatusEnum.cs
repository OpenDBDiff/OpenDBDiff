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
            DropPartitionFunction = 29,
            AddFullText = 30,
            AddXMLSchema = 31,
            AlterAssembly = 32,
            UpdateTable = 33,
            AlterTable = 34,
            AlterIndex = 35,
            AddTable = 36,
            RebuildTable = 37,
            AlterColumnRestore = 38,
            AlterColumnFormulaRestore = 39,
            AlterFunction = 40,
            AlterView = 41,
            AlterProcedure = 42,
            AddIndex = 43,
            AddFunction = 44,
            AddView = 44,
            AddTrigger = 46,
            AddConstraint = 47,
            AddConstraintPK = 48,
            AddConstraintFK = 49,
            AlterConstraint = 50,
            EnabledTrigger = 51,
            AddSynonyms = 52,
            AddStoreProcedure = 53,
            DropOptions = 54,
            AddOptions = 55,
            AddPartitionFunction = 56,
            AlterPartitionFunction = 57,
            DropFullText = 58,
            DropUserDataType = 59,
            DropXMLSchema = 60,
            DropAssemblyUserDataType = 61,
            DropAssemblyFile = 62,
            DropAssembly = 63,
            DropDefault = 64,
            DropSchema = 65,
            DropUser = 66,
            DropRole = 67,
            DropFile = 68,
            DropFileGroup = 69,
            AddExtendedProperty = 70,
            DropExtendedProperty = 71
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
            PartitionFunction = 32
        }
    }
}
