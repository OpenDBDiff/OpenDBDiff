using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Schema
{
    public class StatusEnum
    {
        /// <summary>
        /// OriginalStatus = El objeto no sufrio modificaciones.
        /// CreateStatus = El objeto se debe crear.
        /// DropStatus = El objeto se debe eliminar.
        /// AlterStatus = El objeto sufrio modificaciones.
        /// AlterRebuildStatus = El objeto sufrio modificaciones, pero se debe hacer un DROP y ADD.
        /// AlterPropertiesStatus = El objeto sufrio modificaciones en sus propiedades, pero no en su estructura.
        /// </summary>
        public enum ObjectStatusType
        {
            OriginalStatus = 0,
            AlterStatus = 2,
            AlterRebuildStatus = 4,
            AlterRebuildDependeciesStatus = 8,
            AlterStatusUpdate = 16,
            CreateStatus = 32,
            DropStatus = 64,
            DisabledStatus = 128,
            AlterDisabledStatus = 256,
            ChangeFileGroup = 512,
            ChangeOwner = 1024
        }

        public enum ScripActionType
        {
            UseDatabase = 1,
            AddFileGroup = 2,
            AddFile = 3,
            AlterFile = 4,
            AlterFileGroup = 5,
            UnbindRuleColumn = 6,
            UnbindRuleType = 7,
            DropRule = 8,
            AddRule = 9,
            DropSynonyms = 10,
            DropStoreProcedure = 11,
            DropView = 12,            
            DropTrigger = 13,
            DropConstraintFK = 14,
            DropConstraint = 15,            
            DropConstraintPK = 16,
            DropIndex = 17,
            DropTable = 18,
            AddSchema = 19,
            AddDefault = 20,            
            AddXMLSchema = 21,
            AddUserDataType = 22,
            AddAssembly = 23,
            AlterAssembly = 24,
            AlterTable = 25,
            AlterIndex = 26,            
            AddTable = 27,            
            RebuildTable = 28,
            AddConstraint = 29,
            AddConstraintPK = 30,
            AddConstraintFK = 31,
            AlterConstraint = 32,            
            AddIndex = 33,
            AddView = 34,
            AddTrigger = 35,
            EnabledTrigger = 36,
            AddSynonyms = 37,
            AddStoreProcedure = 38,
            AlterOptions = 39,
            DropAssembly = 40,
            DropUserDataType = 41,
            DropXMLSchema = 42,
            DropDefault = 43,
            DropSchema = 44,
            DropFile = 45,
            DropFileGroup = 46
        }

        public enum ObjectTypeEnum
        {
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
            Assembly = 21
        }
    }
}
