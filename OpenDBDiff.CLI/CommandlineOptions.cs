using CommandLine;

namespace OpenDBDiff.CLI
{
    public class CommandlineOptions
    {
        [Option('b', "before", Required = true, HelpText = "Connection string of database before changes are applied.")]
        public string Before { get; set; }

        [Option('a', "after", Required = true, HelpText = "Connection string of database after changes are applied.")]
        public string After { get; set; }

        [Option('o', "outputfile", Required = false, HelpText = "Output file of action script. If omitted, script is written to the console.")]
        public string OutputFile { get; set; }

        [Option("ignore-partition-function", Required = false, Default =false)]
        public bool IgnorePartitionFunction { get; set; }

        [Option("ignore-partition-scheme", Required = false, Default =false)]
        public bool IgnorePartitionScheme { get; set; }

        [Option("ignore-index-filter", Required = false, Default =false)]
        public bool IgnoreIndexFilter { get; set; }

        [Option("ignore-index", Required = false, Default =false)]
        public bool IgnoreIndex { get; set; }

        [Option("ignore-constraint-pk", Required = false, Default =false)]
        public bool IgnoreConstraintPK { get; set; }

        [Option("ignore-constraint-fk", Required = false, Default =false)]
        public bool IgnoreConstraintFK { get; set; }

        [Option("ignore-constraint-uk", Required = false, Default =false)]
        public bool IgnoreConstraintUK { get; set; }

        [Option("ignore-constraint-check", Required = false, Default =false)]
        public bool IgnoreConstraintCheck { get; set; }

        [Option("ignore-index-fill-factor", Required = false, Default =false)]
        public bool IgnoreIndexFillFactor { get; set; }

        [Option("ignore-index-include-columns", Required = false, Default =false)]
        public bool IgnoreIndexIncludeColumns { get; set; }

        [Option("ignore-index-row-lock", Required = false, Default =false)]
        public bool IgnoreIndexRowLock { get; set; }

        [Option("ignore-column-order", Required = false, Default =false)]
        public bool IgnoreColumnOrder { get; set; }

        [Option("ignore-column-identity", Required = false, Default =false)]
        public bool IgnoreColumnIdentity { get; set; }

        [Option("ignore-column-collation", Required = false, Default =false)]
        public bool IgnoreColumnCollation { get; set; }

        [Option("ignore-not-for-replication", Required = false, Default =false)]
        public bool IgnoreNotForReplication { get; set; }

        [Option("ignore-users", Required = false, Default =false)]
        public bool IgnoreUsers { get; set; }

        [Option("ignore-roles", Required = false, Default =false)]
        public bool IgnoreRoles { get; set; }

        [Option("ignore-clr-function", Required = false, Default =false)]
        public bool IgnoreCLRFunction { get; set; }

        [Option("ignore-clr-trigger", Required = false, Default =false)]
        public bool IgnoreCLRTrigger { get; set; }

        [Option("ignore-clrudt", Required = false, Default =false)]
        public bool IgnoreCLRUDT { get; set; }

        [Option("ignore-clr-stored-procedure", Required = false, Default =false)]
        public bool IgnoreCLRStoredProcedure { get; set; }

        [Option("ignore-fulltext", Required = false, Default =false)]
        public bool IgnoreFullText { get; set; }

        [Option("ignore-fulltext-path", Required = false, Default =false)]
        public bool IgnoreFullTextPath { get; set; }

        [Option("ignore-table-lock-escalation", Required = false, Default =false)]
        public bool IgnoreTableLockEscalation { get; set; }

        [Option("ignore-table-change-tracking", Required = false, Default =false)]
        public bool IgnoreTableChangeTracking { get; set; }

        [Option("ignore-constraint", Required = false, Default =false)]
        public bool IgnoreConstraint { get; set; }

        [Option("ignore-function", Required = false, Default =false)]
        public bool IgnoreFunction { get; set; }

        [Option("IgnoreStoredProcedure", Required = false, Default =false)]
        public bool IgnoreStoredProcedure { get; set; }

        [Option("ignore-view", Required = false, Default =false)]
        public bool IgnoreView { get; set; }

        [Option("ignore-table", Required = false, Default =false)]
        public bool IgnoreTable { get; set; }

        [Option("ignore-table-option", Required = false, Default =false)]
        public bool IgnoreTableOption { get; set; }

        [Option("ignore-user-datatype", Required = false, Default =false)]
        public bool IgnoreUserDataType { get; set; }

        [Option("ignore-trigger", Required = false, Default =false)]
        public bool IgnoreTrigger { get; set; }

        [Option("ignore-schema", Required = false, Default =false)]
        public bool IgnoreSchema { get; set; }

        [Option("ignore-xml-schema", Required = false, Default =false)]
        public bool IgnoreXMLSchema { get; set; }

        [Option("ignore-table-filegroup", Required = false, Default =false)]
        public bool IgnoreTableFileGroup { get; set; }

        [Option("ignore-extended-properties", Required = false, Default =false)]
        public bool IgnoreExtendedProperties { get; set; }

        [Option("ignore-ddl-triggers", Required = false, Default =false)]
        public bool IgnoreDDLTriggers { get; set; }

        [Option("ignore-synonyms", Required = false, Default =false)]
        public bool IgnoreSynonyms { get; set; }

        [Option("ignore-rules", Required = false, Default =false)]
        public bool IgnoreRules { get; set; }

        [Option("ignore-assemblies", Required = false, Default =false)]
        public bool IgnoreAssemblies { get; set; }
    }
}
