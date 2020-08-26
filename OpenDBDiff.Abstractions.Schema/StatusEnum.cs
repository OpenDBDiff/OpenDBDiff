using System;
using System.ComponentModel;

namespace OpenDBDiff.Abstractions.Schema
{
    /// <summary>
    /// Original = The object has no modifications.
    /// Create = The object must be created.
    /// Drop = The object must be deleted.
    /// Alter = The object has modifications.
    /// AlterRebuild = The object has modifications, but a DROP and ADD must be done too.
    /// AlterProperties = The object has modifications in its properties, but not in its structure.
    /// </summary>
    [Flags]
    public enum ObjectStatus
    {
        /// <summary>
        /// The object is unaltered
        /// </summary>
        Original = 0,

        /// <summary>
        /// The object was altered
        /// </summary>
        Alter = 2,

        AlterBody = 4,

        /// <summary>
        /// The object was altered but requires a rebuild
        /// </summary>
        Rebuild = 8,

        /// <summary>
        /// The object has properties altered, but not in it's structure
        /// </summary>
        RebuildDependencies = 16,

        Update = 32,
        Create = 64,

        /// <summary>
        /// The object should be dropped
        /// </summary>
        Drop = 128,

        Disabled = 256,

        /// <summary>
        /// The owner of the object changed
        /// </summary>
        ChangeOwner = 512,

        DropOlder = 1024,
        Bind = 2048,

        /// <summary>
        /// The permission set of the object changed
        /// </summary>
        PermissionSet = 4096,

        AlterWhitespace = 8192
    }

    public enum ScriptAction
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

        DropFullTextIndex = 10,
        DropConstraintFK = 11,
        DropConstraint = 12,
        DropConstraintPK = 13,
        DropSynonyms = 14,
        DropStoredProcedure = 15,
        DropTrigger = 16,
        DropView = 17,
        DropFunction = 17,
        DropIndex = 18,
        DropTable = 20,
        AlterColumnFormula = 21,
        AlterColumn = 22,
        AddRole = 23,
        AddUser = 24,
        AddSchema = 25,
        AddDefault = 26,
        AddAssembly = 27,
        AddAssemblyFile = 28,
        AddUserDataType = 29,
        AddTableType = 30,
        AlterPartitionFunction = 31,
        AddPartitionFunction = 32,
        AddPartitionScheme = 33,
        AddFullText = 34,
        AddXMLSchema = 35,
        AlterAssembly = 36,
        UpdateTable = 37,
        AlterTable = 38,
        AlterIndex = 39,
        AlterFullTextIndex = 40,
        AddTable = 41,
        RebuildTable = 42,
        AlterColumnRestore = 43,
        AlterColumnFormulaRestore = 44,
        AlterFunction = 45,
        AlterView = 46,
        AlterProcedure = 47,
        AddIndex = 48,
        AddFunction = 49,
        AddView = 49, /*AddFunction and AddView must have the same number!!!*/
        AddTrigger = 50,
        AddConstraint = 51,
        AddConstraintPK = 52,
        AddConstraintFK = 53,
        AlterConstraint = 54,
        AddFullTextIndex = 55,
        EnabledTrigger = 56,
        AddSynonyms = 57,
        AddStoredProcedure = 58,
        DropOptions = 59,
        AddOptions = 60,

        AlterTableChangeTracking = 61,

        DropFullText = 62,
        DropTableType = 63,
        DropUserDataType = 64,
        DropXMLSchema = 65,
        DropAssemblyUserDataType = 66,
        DropAssemblyFile = 67,
        DropAssembly = 68,
        DropDefault = 69,

        DropPartitionScheme = 70,
        DropPartitionFunction = 71,

        DropSchema = 72,
        DropUser = 73,
        DropRole = 74,
        DropFile = 75,
        DropFileGroup = 76,
        AddExtendedProperty = 77,
        DropExtendedProperty = 78
    }

    public enum ObjectType
    {
        None = 0,
        Table = 1,
        Column = 2,
        Trigger = 3,
        Constraint = 4,

        [Description("Constraint Column")]
        ConstraintColumn = 5,

        Index = 6,

        [Description("Index Column")]
        IndexColumn = 7,

        [Description("User Data Type")]
        UserDataType = 8,

        [Description("XML Schema")]
        XMLSchema = 9,

        View = 10,
        Function = 11,

        [Description("Stored Procedure")]
        StoredProcedure = 12,

        [Description("Table Option")]
        TableOption = 13,

        Database = 14,
        Schema = 15,

        [Description("File Group")]
        FileGroup = 16,

        File = 17,
        Default = 18,
        Rule = 19,
        Synonym = 20,
        Assembly = 21,
        User = 22,
        Role = 23,

        [Description("Full Text")]
        FullText = 24,

        [Description("Assembly File")]
        AssemblyFile = 25,

        [Description("CLR Stored Procedure")]
        CLRStoredProcedure = 26,

        [Description("CLR Trigger")]
        CLRTrigger = 27,

        [Description("CLR Function")]
        CLRFunction = 28,

        [Description("Extended Property")]
        ExtendedProperty = 30,

        Partition = 31,

        [Description("Partition Function")]
        PartitionFunction = 32,

        [Description("Partition Scheme")]
        PartitionScheme = 33,

        [Description("Table Type")]
        TableType = 34,

        [Description("Full Text Index")]
        FullTextIndex = 35
    }
}
