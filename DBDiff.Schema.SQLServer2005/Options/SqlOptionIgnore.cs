using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Schema.SQLServer.Generates.Options
{
    public class SqlOptionIgnore
    {
        private Boolean filterIndex = true;        
        private Boolean filterSchema = true;
        private Boolean filterXMLSchema = true;
        private Boolean filterTrigger = true;
        private Boolean filterUserDataType = true;
        private Boolean filterTableOption = true;
        private Boolean filterTableLockEscalation = true;
        private Boolean filterTableChangeTracking = true;
        private Boolean filterTable = true;
        private Boolean filterView = true;
        private Boolean filterStoreProcedure = true;
        private Boolean filterFunction = true;
        private Boolean filterTableFileGroup = true;
        private Boolean filterExtendedPropertys = true;
        private Boolean filterDDLTriggers = true;
        private Boolean filterSynonyms = true;
        private Boolean filterRules = true;        
        private Boolean filterFullText = true;
        private Boolean filterFullTextPath = false;
        private Boolean filterConstraint = true;
        private Boolean filterConstraintPK = true;
        private Boolean filterConstraintFK = true;
        private Boolean filterConstraintUK = true;
        private Boolean filterConstraintCheck = true;

        private Boolean filterIndexFillFactor = true;
        private Boolean filterIndexRowLock = true;
        private Boolean filterIndexIncludeColumns = true;
        private Boolean filterIndexFilter = true;

        private Boolean filterColumnOrder = true;
        private Boolean filterColumnIdentity = true;
        private Boolean filterColumnCollation = true;
        private Boolean filterNotForReplication = true;
        
        private Boolean filterUsers = true;
        private Boolean filterRoles = true;        
        private Boolean filterPartitionScheme = true;
        private Boolean filterPartitionFunction = true;

        private Boolean filterAssemblies = true;
        private Boolean filterCLRStoreProcedure = true;
        private Boolean filterCLRFunction = true;
        private Boolean filterCLRTrigger = true;
        private Boolean filterCLRUDT = true;

        public SqlOptionIgnore(Boolean defaultValue)
        {
            FilterConstraint = defaultValue;
            FilterFunction = defaultValue;
            FilterStoreProcedure = defaultValue;
            FilterView = defaultValue;
            FilterTable = defaultValue;
            FilterTableOption = defaultValue;
            FilterUserDataType = defaultValue;
            FilterTrigger = defaultValue;
            FilterSchema = defaultValue;
            FilterXMLSchema = defaultValue;
            FilterTableFileGroup = defaultValue;
            FilterExtendedPropertys = defaultValue;
            FilterDDLTriggers = defaultValue;
            FilterSynonyms = defaultValue;
            FilterRules = defaultValue;
            FilterAssemblies = defaultValue;
        }

        public Boolean FilterTableChangeTracking
        {
            get { return filterTableChangeTracking; }
            set { filterTableChangeTracking = value; }
        }

        public Boolean FilterTableLockEscalation
        {
            get { return filterTableLockEscalation; }
            set { filterTableLockEscalation = value; }
        }

        public Boolean FilterFullTextPath
        {
            get { return filterFullTextPath; }
            set { filterFullTextPath = value; }
        }

        public Boolean FilterFullText
        {
            get { return filterFullText; }
            set { filterFullText = value; }
        }

        public Boolean FilterCLRStoreProcedure
        {
            get { return filterCLRStoreProcedure; }
            set { filterCLRStoreProcedure = value; }
        }

        public Boolean FilterCLRUDT
        {
            get { return filterCLRUDT; }
            set { filterCLRUDT = value; }
        }

        public Boolean FilterCLRTrigger
        {
            get { return filterCLRTrigger; }
            set { filterCLRTrigger = value; }
        }

        public Boolean FilterCLRFunction
        {
            get { return filterCLRFunction; }
            set { filterCLRFunction = value; }
        }

        public Boolean FilterRoles
        {
            get { return filterRoles; }
            set { filterRoles = value; }
        }

        public Boolean FilterUsers
        {
            get { return filterUsers; }
            set { filterUsers = value; }
        }

        public Boolean FilterNotForReplication
        {
            get { return filterNotForReplication; }
            set { filterNotForReplication = value; }
        }

        public Boolean FilterColumnCollation
        {
            get { return filterColumnCollation; }
            set { filterColumnCollation = value; }
        }

        public Boolean FilterColumnIdentity
        {
            get { return filterColumnIdentity; }
            set { filterColumnIdentity = value; }
        }

        public Boolean FilterColumnOrder
        {
            get { return filterColumnOrder; }
            set { filterColumnOrder = value; }
        }

        public Boolean FilterIndexRowLock
        {
            get { return filterIndexRowLock; }
            set { filterIndexRowLock = value; }
        }

        public Boolean FilterIndexIncludeColumns
        {
            get { return filterIndexIncludeColumns; }
            set { filterIndexIncludeColumns = value; }
        }

        public Boolean FilterIndexFillFactor
        {
            get { return filterIndexFillFactor; }
            set { filterIndexFillFactor = value; }
        }

        public Boolean FilterAssemblies
        {
            get { return filterAssemblies; }
            set { filterAssemblies = value; }
        }

        public Boolean FilterRules
        {
            get { return filterRules; }
            set { filterRules = value; }
        }

        public Boolean FilterSynonyms
        {
            get { return filterSynonyms; }
            set { filterSynonyms = value; }
        }

        public Boolean FilterDDLTriggers
        {
            get { return filterDDLTriggers; }
            set { filterDDLTriggers = value; }
        }

        public Boolean FilterExtendedPropertys
        {
            get { return filterExtendedPropertys; }
            set { filterExtendedPropertys = value; }
        }

        public Boolean FilterTableFileGroup
        {
            get { return filterTableFileGroup; }
            set { filterTableFileGroup = value; }
        }

        public Boolean FilterFunction
        {
            get { return filterFunction; }
            set { filterFunction = value; }
        }

        public Boolean FilterStoreProcedure
        {
            get { return filterStoreProcedure; }
            set { filterStoreProcedure = value; }
        }

        public Boolean FilterView
        {
            get { return filterView; }
            set { filterView = value; }
        }

        public Boolean FilterTable
        {
            get { return filterTable; }
            set { filterTable = value; }
        }

        public Boolean FilterTableOption
        {
            get { return filterTableOption; }
            set { filterTableOption = value; }
        }

        public Boolean FilterUserDataType
        {
            get { return filterUserDataType; }
            set { filterUserDataType = value; }
        }

        public Boolean FilterTrigger
        {
            get { return filterTrigger; }
            set { filterTrigger = value; }
        }

        public Boolean FilterXMLSchema
        {
            get { return filterXMLSchema; }
            set { filterXMLSchema = value; }
        }

        public Boolean FilterSchema
        {
            get { return filterSchema; }
            set { filterSchema = value; }
        }

        public Boolean FilterConstraint
        {
            get { return filterConstraint; }
            set { filterConstraint = value; }
        }

        public Boolean FilterConstraintCheck
        {
            get { return filterConstraintCheck; }
            set { filterConstraintCheck = value; }
        }

        public Boolean FilterConstraintUK
        {
            get { return filterConstraintUK; }
            set { filterConstraintUK = value; }
        }

        public Boolean FilterConstraintFK
        {
            get { return filterConstraintFK; }
            set { filterConstraintFK = value; }
        }

        public Boolean FilterConstraintPK
        {
            get { return filterConstraintPK; }
            set { filterConstraintPK = value; }
        }

        public Boolean FilterIndex
        {
            get { return filterIndex; }
            set { filterIndex = value; }
        }

        public Boolean FilterIndexFilter
        {
            get { return filterIndexFilter; }
            set { filterIndexFilter = value; }
        }

        public Boolean FilterPartitionScheme
        {
            get { return filterPartitionScheme; }
            set { filterPartitionScheme = value; }
        }

        public Boolean FilterPartitionFunction
        {
            get { return filterPartitionFunction; }
            set { filterPartitionFunction = value; }
        }
    }
}
