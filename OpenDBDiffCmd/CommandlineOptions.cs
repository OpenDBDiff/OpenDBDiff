﻿using CommandLine;
using CommandLine.Text;

namespace OpenDBDiff.OCDB
{
    public class CommandlineOptions
    {
        [Option('b', "before", Required = true, HelpText = "Connection string of database before changes are applied.")]
        public string Before { get; set; }

        [Option('a', "after", Required = true, HelpText = "Connection string of database after changes are applied.")]
        public string After { get; set; }

        [Option('o', "outputfile", Required = false, HelpText = "Output file of action script. If omitted, script is written to the console.")]
        public string OutputFile { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
