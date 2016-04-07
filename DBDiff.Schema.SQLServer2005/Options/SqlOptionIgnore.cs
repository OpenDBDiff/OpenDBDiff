using System;

namespace DBDiff.Schema.SQLServer.Generates.Options
{
    public class SqlOptionIgnore
    {
        public SqlOptionIgnore(Boolean defaultValue)
        {
            FilterPartitionFunction = true;
            FilterPartitionScheme = true;
            FilterIndexFilter = true;
            FilterIndex = true;
            FilterConstraintPK = true;
            FilterConstraintFK = true;
            FilterConstraintUK = true;
            FilterConstraintCheck = true;
            FilterIndexFillFactor = true;
            FilterIndexIncludeColumns = true;
            FilterIndexRowLock = true;
            FilterColumnOrder = true;
            FilterColumnIdentity = true;
            FilterColumnCollation = true;
            FilterNotForReplication = true;
            FilterUsers = true;
            FilterRoles = true;
            FilterCLRFunction = true;
            FilterCLRTrigger = true;
            FilterCLRUDT = true;
            FilterCLRStoredProcedure = true;
            FilterFullText = true;
            FilterFullTextPath = false;
            FilterTableLockEscalation = true;
            FilterTableChangeTracking = true;
            FilterConstraint = defaultValue;
            FilterFunction = defaultValue;
            FilterStoredProcedure = defaultValue;
            FilterView = defaultValue;
            FilterTable = defaultValue;
            FilterTableOption = defaultValue;
            FilterUserDataType = defaultValue;
            FilterTrigger = defaultValue;
            FilterSchema = defaultValue;
            FilterXMLSchema = defaultValue;
            FilterTableFileGroup = defaultValue;
            FilterExtendedProperties = defaultValue;
            FilterDDLTriggers = defaultValue;
            FilterSynonyms = defaultValue;
            FilterRules = defaultValue;
            FilterAssemblies = defaultValue;
        }

        public Boolean FilterTableChangeTracking { get; set; }

        public Boolean FilterTableLockEscalation { get; set; }

        public Boolean FilterFullTextPath { get; set; }

        public Boolean FilterFullText { get; set; }

        public Boolean FilterCLRStoredProcedure { get; set; }

        public Boolean FilterCLRUDT { get; set; }

        public Boolean FilterCLRTrigger { get; set; }

        public Boolean FilterCLRFunction { get; set; }

        public Boolean FilterRoles { get; set; }

        public Boolean FilterUsers { get; set; }

        public Boolean FilterNotForReplication { get; set; }

        public Boolean FilterColumnCollation { get; set; }

        public Boolean FilterColumnIdentity { get; set; }

        public Boolean FilterColumnOrder { get; set; }

        public Boolean FilterIndexRowLock { get; set; }

        public Boolean FilterIndexIncludeColumns { get; set; }

        public Boolean FilterIndexFillFactor { get; set; }

        public Boolean FilterAssemblies { get; set; }

        public Boolean FilterRules { get; set; }

        public Boolean FilterSynonyms { get; set; }

        public Boolean FilterDDLTriggers { get; set; }

        public Boolean FilterExtendedProperties { get; set; }

        public Boolean FilterTableFileGroup { get; set; }

        public Boolean FilterFunction { get; set; }

        public Boolean FilterStoredProcedure { get; set; }

        public Boolean FilterView { get; set; }

        public Boolean FilterTable { get; set; }

        public Boolean FilterTableOption { get; set; }

        public Boolean FilterUserDataType { get; set; }

        public Boolean FilterTrigger { get; set; }

        public Boolean FilterXMLSchema { get; set; }

        public Boolean FilterSchema { get; set; }

        public Boolean FilterConstraint { get; set; }

        public Boolean FilterConstraintCheck { get; set; }

        public Boolean FilterConstraintUK { get; set; }

        public Boolean FilterConstraintFK { get; set; }

        public Boolean FilterConstraintPK { get; set; }

        public Boolean FilterIndex { get; set; }

        public Boolean FilterIndexFilter { get; set; }

        public Boolean FilterPartitionScheme { get; set; }

        public Boolean FilterPartitionFunction { get; set; }
    }
}
