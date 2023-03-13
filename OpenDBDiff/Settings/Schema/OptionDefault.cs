namespace OpenDBDiff.Settings.Schema
{
    public class OptionDefault
    {
        public string DefaultIntegerValue { get; set; }
        public string DefaultRealValue { get; set; }
        public string DefaultTextValue { get; set; }
        public string DefaultDateValue { get; set; }
        public string DefaultVariantValue { get; set; }
        public string DefaultNTextValue { get; set; }
        public string DefaultBlobValue { get; set; }
        public string DefaultUniqueValue { get; set; }
        public bool UseDefaultValueIfExists { get; set; }
        public string DefaultTime { get; set; }
        public string DefaultXml { get; set; }
    }
}
