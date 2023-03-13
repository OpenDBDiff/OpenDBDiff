namespace OpenDBDiff.Settings.Schema
{
    public class OptionIgnore
    {
        public Assemblies Assemblies { get; set; }
        public Constraints Constraints { get; set; }
        public Indexes Indexes { get; set; }
        public Tables Tables { get; set; }

        public bool FullTextPath { get; set; }
        public bool FullText { get; set; }
        public bool Roles { get; set; }
        public bool Users { get; set; }
        public bool NotForReplication { get; set; }
        public bool Assemblie { get; set; }
        public bool Rules { get; set; }
        public bool Synonyms { get; set; }
        public bool DDLTriggers { get; set; }
        public bool ExtendedProperties { get; set; }
        public bool TableFileGroup { get; set; }
        public bool Function { get; set; }
        public bool StoredProcedure { get; set; }
        public bool View { get; set; }
        public bool Table { get; set; }
        public bool UserDataType { get; set; }
        public bool Trigger { get; set; }
        public bool XMLSchema { get; set; }
        public bool Schema { get; set; }
        public bool Permission { get; set; }
        public bool Constraint { get; set; }
        public bool Index { get; set; }
        public bool PartitionScheme { get; set; }
        public bool PartitionFunction { get; set; }
    }
}
