using OpenDBDiff.SqlServer.Schema.Options;

namespace OpenDBDiff.CLI.Extensions
{
    public static class SqlOptionIgnoreExtensions
    {
        public static void SetOptions(this SqlOptionIgnore filter, CommandlineOptions options)
        {
            filter.FilterPartitionFunction = !options.IgnorePartitionFunction;
            filter.FilterPartitionScheme = !options.IgnorePartitionScheme;
            filter.FilterIndexFilter = !options.IgnoreIndexFilter;
            filter.FilterIndex = !options.IgnoreIndex;
            filter.FilterConstraintPK = !options.IgnoreConstraintPK;
            filter.FilterConstraintFK = !options.IgnoreConstraintFK;
            filter.FilterConstraintUK = !options.IgnoreConstraintUK;
            filter.FilterConstraintCheck = !options.IgnoreConstraintCheck;
            filter.FilterIndexFillFactor = !options.IgnoreIndexFillFactor;
            filter.FilterIndexIncludeColumns = !options.IgnoreIndexIncludeColumns;
            filter.FilterIndexRowLock = !options.IgnoreIndexRowLock;
            filter.FilterColumnOrder = !options.IgnoreColumnOrder;
            filter.FilterColumnIdentity = !options.IgnoreColumnIdentity;
            filter.FilterColumnCollation = !options.IgnoreColumnCollation;
            filter.FilterNotForReplication = !options.IgnoreNotForReplication;
            filter.FilterUsers = !options.IgnoreUsers;
            filter.FilterRoles = !options.IgnoreRoles;
            filter.FilterCLRFunction = !options.IgnoreCLRFunction;
            filter.FilterCLRTrigger = !options.IgnoreCLRTrigger;
            filter.FilterCLRUDT = !options.IgnoreCLRUDT;
            filter.FilterCLRStoredProcedure = !options.IgnoreCLRStoredProcedure;
            filter.FilterFullText = !options.IgnoreFullText;
            filter.FilterFullTextPath = !options.IgnoreFullTextPath;
            filter.FilterTableLockEscalation = !options.IgnoreTableLockEscalation;
            filter.FilterTableChangeTracking = !options.IgnoreTableChangeTracking;
            filter.FilterConstraint = !options.IgnoreConstraint;
            filter.FilterFunction = !options.IgnoreFunction;
            filter.FilterStoredProcedure = !options.IgnoreStoredProcedure;
            filter.FilterView = !options.IgnoreView;
            filter.FilterTable = !options.IgnoreTable;
            filter.FilterTableOption = !options.IgnoreTableOption;
            filter.FilterUserDataType = !options.IgnoreUserDataType;
            filter.FilterTrigger = !options.IgnoreTrigger;
            filter.FilterSchema = !options.IgnoreSchema;
            filter.FilterXMLSchema = !options.IgnoreXMLSchema;
            filter.FilterTableFileGroup = !options.IgnoreTableFileGroup;
            filter.FilterExtendedProperties = !options.IgnoreExtendedProperties;
            filter.FilterDDLTriggers = !options.IgnoreDDLTriggers;
            filter.FilterSynonyms = !options.IgnoreSynonyms;
            filter.FilterRules = !options.IgnoreRules;
            filter.FilterAssemblies = !options.IgnoreAssemblies;
        }
    }
}
