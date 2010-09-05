using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Schema.SQLServer.Options
{
    public class SqlOptionFilter
    {
        private Boolean filterIndex = true;
        private Boolean filterConstraint = true;
        private Boolean filterSchema = true;
        private Boolean filterXMLSchema = true;
        private Boolean filterTrigger = true;
        private Boolean filterUserDataType = true;
        private Boolean filterTableOption = true;
        private Boolean filterTable = true;
        private Boolean filterView = true;
        private Boolean filterStoreProcedure = true;
        private Boolean filterFunction = true;
        private Boolean filterTableFileGroup = true;
        private Boolean filterExtendedPropertys = true;
        private Boolean filterColumnPosition = true;
        private Boolean filterDDLTriggers = true;
        private Boolean filterSynonyms = true;
        private Boolean filterRules = true;
        private Boolean filterAssemblies = true;

        public SqlOptionFilter()
        {
            new SqlOptionFilter(true);
        }

        public SqlOptionFilter(Boolean defaultValue)
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
            FilterColumnPosition = defaultValue;
            FilterDDLTriggers = defaultValue;
            FilterSynonyms = defaultValue;
            FilterRules = defaultValue;
            FilterAssemblies = defaultValue;
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

        public Boolean FilterColumnPosition
        {
            get { return filterColumnPosition; }
            set { filterColumnPosition = value; }
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

        public Boolean FilterIndex
        {
            get { return filterIndex; }
            set { filterIndex = value; }
        }
    }
}
