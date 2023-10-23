using CommandLine;
using System.Collections.Generic;

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
        
        [Option('i', "ignorefilters", Required = false, HelpText = "String that encodes ignore filters to be apply for the comparison. Example: FilterColumnOrder=False;FilterColumnCollation=False")]
        public string IgnoreFilters { get; set; }
    }
}
