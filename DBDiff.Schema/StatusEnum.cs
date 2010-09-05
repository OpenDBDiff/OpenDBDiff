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
            AlterRebuildStatus = 4,
            AlterRebuildDependenciesStatus = 8,
            UpdateStatus = 16,
            CreateStatus = 32,
            DropStatus = 64,
            DisabledStatus = 128,
            ChangeOwner = 256,
            DropOlderStatus = 512,
            BindStatus = 1024
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
            DropView = 15,
            DropTrigger = 16,
            DropFunction = 17,
            DropIndex = 18,
            DropTable = 19,
            AlterColumnFormula = 20,
            AlterColumn = 21,
            AddRole = 22,
            AddUser = 23,
            AddSchema = 24,
            AddDefault = 25,            
            DropXMLSchema = 26,
            AddAssembly = 27,
            AddUserDataType = 28,
            AddFullText = 29,
            AddXMLSchema = 30,
            AlterAssembly = 31,
            UpdateTable = 32,
            AlterTable = 33,
            AlterIndex = 34,
            AddTable = 35,
            RebuildTable = 36,
            AlterColumnRestore = 37,
            AlterColumnFormulaRestore = 38,
            AlterFunction = 39,
            AlterView = 40,
            AlterProcedure = 41,
            AddIndex = 42,
            AddFunction = 43,
            AddView = 44,
            AddTrigger = 45,
            AddConstraint = 46,
            AddConstraintPK = 47,
            AddConstraintFK = 48,
            AlterConstraint = 49,
            EnabledTrigger = 50,
            AddSynonyms = 51,
            AddStoreProcedure = 52,
            AlterOptions = 53,
            DropFullText = 54,
            DropUserDataType = 55,
            DropAssembly = 56,
            DropDefault = 57,
            DropSchema = 58,
            DropUser = 59,
            DropRole = 60,
            DropFile = 61,
            DropFileGroup = 62
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
            FullText = 24
        }
    }
}
