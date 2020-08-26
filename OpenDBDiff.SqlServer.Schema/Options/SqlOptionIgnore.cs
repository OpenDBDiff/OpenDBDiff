using OpenDBDiff.Abstractions.Schema.Model;
using System;
using System.Collections.Generic;

namespace OpenDBDiff.SqlServer.Schema.Options
{
    public class SqlOptionIgnore : IOptionsContainer<bool>
    {
        public SqlOptionIgnore(bool defaultValue)
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
            FilterXMLSchema = options["FilterXMLSchema"];
            FilterTableFileGroup = options["FilterTableFileGroup"];
            FilterExtendedProperties = options["FilterExtendedProperties"];
            FilterDDLTriggers = options["FilterDDLTriggers"];
            FilterSynonyms = options["FilterSynonyms"];
            FilterRules = options["FilterRules"];
            FilterAssemblies = options["FilterAssemblies"];

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
