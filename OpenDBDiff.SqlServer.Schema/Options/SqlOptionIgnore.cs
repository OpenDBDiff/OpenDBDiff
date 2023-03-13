using OpenDBDiff.Abstractions.Schema.Model;
using System.Collections.Generic;

namespace OpenDBDiff.SqlServer.Schema.Options
{
    public class SqlOptionIgnore : IOptionsContainer<bool>
    {
        public SqlOptionIgnore(bool defaultValue)
        {
            FilterPartitionFunction = defaultValue;
            FilterPartitionScheme = defaultValue;
            FilterIndexFilter = defaultValue;
            FilterIndex = defaultValue;
            FilterConstraintPK = defaultValue;
            FilterConstraintFK = defaultValue;
            FilterConstraintUK = defaultValue;
            FilterConstraintCheck = defaultValue;
            FilterIndexFillFactor = defaultValue;
            FilterIndexIncludeColumns = defaultValue;
            FilterIndexRowLock = defaultValue;
            FilterColumnOrder = defaultValue;
            FilterColumnIdentity = defaultValue;
            FilterColumnCollation = defaultValue;
            FilterNotForReplication = defaultValue;
            FilterUsers = defaultValue;
            FilterRoles = defaultValue;
            FilterCLRAggregate = defaultValue;
            FilterCLRFunction = defaultValue;
            FilterCLRTrigger = defaultValue;
            FilterCLRUDT = defaultValue;
            FilterCLRStoredProcedure = defaultValue;
            FilterFullText = defaultValue;
            FilterFullTextPath = defaultValue;
            FilterTableLockEscalation = defaultValue;
            FilterTableChangeTracking = defaultValue;
            FilterConstraint = defaultValue;
            FilterFunction = defaultValue;
            FilterStoredProcedure = defaultValue;
            FilterView = defaultValue;
            FilterTable = defaultValue;
            FilterTableOption = defaultValue;
            FilterUserDataType = defaultValue;
            FilterTrigger = defaultValue;
            FilterSchema = defaultValue;
            FilterPermission = defaultValue;
            FilterXMLSchema = defaultValue;
            FilterTableFileGroup = defaultValue;
            FilterExtendedProperties = defaultValue;
            FilterDDLTriggers = defaultValue;
            FilterSynonyms = defaultValue;
            FilterRules = defaultValue;
            FilterAssemblies = defaultValue;
        }

        public SqlOptionIgnore(IOptionsContainer<bool> optionsContainer)
        {
            var options = optionsContainer.GetOptions();
            FilterPartitionFunction = options["FilterPartitionFunction"];
            FilterPartitionScheme = options["FilterPartitionScheme"];
            FilterIndexFilter = options["FilterIndexFilter"];
            FilterIndex = options["FilterIndex"];
            FilterConstraintPK = options["FilterConstraintPK"];
            FilterConstraintFK = options["FilterConstraintFK"];
            FilterConstraintUK = options["FilterConstraintUK"];
            FilterConstraintCheck = options["FilterConstraintCheck"];
            FilterIndexFillFactor = options["FilterIndexFillFactor"];
            FilterIndexIncludeColumns = options["FilterIndexIncludeColumns"];
            FilterIndexRowLock = options["FilterIndexRowLock"];
            FilterColumnOrder = options["FilterColumnOrder"];
            FilterColumnIdentity = options["FilterColumnIdentity"];
            FilterColumnCollation = options["FilterColumnCollation"];
            FilterNotForReplication = options["FilterNotForReplication"];
            FilterUsers = options["FilterUsers"];
            FilterRoles = options["FilterRoles"];
            FilterCLRAggregate = options["FilterCLRAggregate"];
            FilterCLRFunction = options["FilterCLRFunction"];
            FilterCLRTrigger = options["FilterCLRTrigger"];
            FilterCLRUDT = options["FilterCLRUDT"];
            FilterCLRStoredProcedure = options["FilterCLRStoredProcedure"];
            FilterFullText = options["FilterFullText"];
            FilterFullTextPath = options["FilterFullTextPath"];
            FilterTableLockEscalation = options["FilterTableLockEscalation"];
            FilterTableChangeTracking = options["FilterTableChangeTracking"];
            FilterConstraint = options["FilterConstraint"];
            FilterFunction = options["FilterFunction"];
            FilterStoredProcedure = options["FilterStoredProcedure"];
            FilterView = options["FilterView"];
            FilterTable = options["FilterTable"];
            FilterTableOption = options["FilterTableOption"];
            FilterUserDataType = options["FilterUserDataType"];
            FilterTrigger = options["FilterTrigger"];
            FilterSchema = options["FilterSchema"];
            FilterPermission = options["FilterPermission"];
            FilterXMLSchema = options["FilterXMLSchema"];
            FilterTableFileGroup = options["FilterTableFileGroup"];
            FilterExtendedProperties = options["FilterExtendedProperties"];
            FilterDDLTriggers = options["FilterDDLTriggers"];
            FilterSynonyms = options["FilterSynonyms"];
            FilterRules = options["FilterRules"];
            FilterAssemblies = options["FilterAssemblies"];

        }

        public bool FilterTableChangeTracking { get; set; }

        public bool FilterTableLockEscalation { get; set; }

        public bool FilterFullTextPath { get; set; }

        public bool FilterFullText { get; set; }

        public bool FilterCLRAggregate { get; set; }

        public bool FilterCLRStoredProcedure { get; set; }

        public bool FilterCLRUDT { get; set; }

        public bool FilterCLRTrigger { get; set; }

        public bool FilterCLRFunction { get; set; }

        public bool FilterRoles { get; set; }

        public bool FilterUsers { get; set; }

        public bool FilterNotForReplication { get; set; }

        public bool FilterColumnCollation { get; set; }

        public bool FilterColumnIdentity { get; set; }

        public bool FilterColumnOrder { get; set; }

        public bool FilterIndexRowLock { get; set; }

        public bool FilterIndexIncludeColumns { get; set; }

        public bool FilterIndexFillFactor { get; set; }

        public bool FilterAssemblies { get; set; }

        public bool FilterRules { get; set; }

        public bool FilterSynonyms { get; set; }

        public bool FilterDDLTriggers { get; set; }

        public bool FilterExtendedProperties { get; set; }

        public bool FilterTableFileGroup { get; set; }

        public bool FilterFunction { get; set; }

        public bool FilterStoredProcedure { get; set; }

        public bool FilterView { get; set; }

        public bool FilterTable { get; set; }

        public bool FilterTableOption { get; set; }

        public bool FilterUserDataType { get; set; }

        public bool FilterTrigger { get; set; }

        public bool FilterXMLSchema { get; set; }

        public bool FilterSchema { get; set; }

        public bool FilterPermission { get; set; }

        public bool FilterConstraint { get; set; }

        public bool FilterConstraintCheck { get; set; }

        public bool FilterConstraintUK { get; set; }

        public bool FilterConstraintFK { get; set; }

        public bool FilterConstraintPK { get; set; }

        public bool FilterIndex { get; set; }

        public bool FilterIndexFilter { get; set; }

        public bool FilterPartitionScheme { get; set; }

        public bool FilterPartitionFunction { get; set; }

        public IDictionary<string, bool> GetOptions()
        {

            Dictionary<string, bool> options = new Dictionary<string, bool>();
            options.Add("FilterPartitionFunction", FilterPartitionFunction);
            options.Add("FilterPartitionScheme", FilterPartitionScheme);
            options.Add("FilterIndexFilter", FilterIndexFilter);
            options.Add("FilterIndex", FilterIndex);
            options.Add("FilterConstraintPK", FilterConstraintPK);
            options.Add("FilterConstraintFK", FilterConstraintFK);
            options.Add("FilterConstraintUK", FilterConstraintUK);
            options.Add("FilterConstraintCheck", FilterConstraintCheck);
            options.Add("FilterIndexFillFactor", FilterIndexFillFactor);
            options.Add("FilterIndexIncludeColumns", FilterIndexIncludeColumns);
            options.Add("FilterIndexRowLock", FilterIndexRowLock);
            options.Add("FilterColumnOrder", FilterColumnOrder);
            options.Add("FilterColumnIdentity", FilterColumnIdentity);
            options.Add("FilterColumnCollation", FilterColumnCollation);
            options.Add("FilterNotForReplication", FilterNotForReplication);
            options.Add("FilterUsers", FilterUsers);
            options.Add("FilterRoles", FilterRoles);
            options.Add("FilterCLRAggregate", FilterCLRAggregate);
            options.Add("FilterCLRFunction", FilterCLRFunction);
            options.Add("FilterCLRTrigger", FilterCLRTrigger);
            options.Add("FilterCLRUDT", FilterCLRUDT);
            options.Add("FilterCLRStoredProcedure", FilterCLRStoredProcedure);
            options.Add("FilterFullText", FilterFullText);
            options.Add("FilterFullTextPath", FilterFullTextPath);
            options.Add("FilterTableLockEscalation", FilterTableLockEscalation);
            options.Add("FilterTableChangeTracking", FilterTableChangeTracking);
            options.Add("FilterConstraint", FilterConstraint);
            options.Add("FilterFunction", FilterFunction);
            options.Add("FilterStoredProcedure", FilterStoredProcedure);
            options.Add("FilterView", FilterView);
            options.Add("FilterTable", FilterTable);
            options.Add("FilterTableOption", FilterTableOption);
            options.Add("FilterUserDataType", FilterUserDataType);
            options.Add("FilterTrigger", FilterTrigger);
            options.Add("FilterSchema", FilterSchema);
            options.Add("FilterPermission", FilterPermission);
            options.Add("FilterXMLSchema", FilterXMLSchema);
            options.Add("FilterTableFileGroup", FilterTableFileGroup);
            options.Add("FilterExtendedProperties", FilterExtendedProperties);
            options.Add("FilterDDLTriggers", FilterDDLTriggers);
            options.Add("FilterSynonyms", FilterSynonyms);
            options.Add("FilterRules", FilterRules);
            options.Add("FilterAssemblies", FilterAssemblies);
            return options;
        }
    }
}
