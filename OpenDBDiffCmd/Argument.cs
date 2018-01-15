using System;

namespace OpenDBDiff.OCDB
{
    public class Argument
    {
        public Argument(string[] commandline)
        {
            for (int i = 0; i < commandline.Length; i++)
            {
                if (commandline[i].Length > 4)
                {
                    if (commandline[i].Substring(0, 4).Equals("CN1=", StringComparison.CurrentCultureIgnoreCase))
                        ConnectionString1 = commandline[i].Substring(4, commandline[i].Length - 4).Trim();
                    if (commandline[i].Substring(0, 4).Equals("CN2=", StringComparison.CurrentCultureIgnoreCase))
                        ConnectionString2 = commandline[i].Substring(4, commandline[i].Length - 4).Trim();
                    if (commandline[i].Substring(0, 2).Equals("F=", StringComparison.CurrentCultureIgnoreCase))
                        OutputFile = commandline[i].Substring(2, commandline[i].Length - 2).Trim();
                    if (String.Compare(commandline[i], "/legacy", true) == 0)
                        this.OutputAll = true;
                }
            }
            if (String.IsNullOrEmpty(ConnectionString1) || String.IsNullOrEmpty(ConnectionString2) || String.IsNullOrEmpty(OutputFile))
            {
                Console.WriteLine("\r\n Example of use:\r\n");
                Console.WriteLine("    OCDB CN1=\"Destination/Target\" CN2=\"Source\" F=OuputScript.sql\r\n");
                Console.WriteLine(" (where CN1 and CN2 are SQL Server 2005+ Connection Strings)");
                Console.WriteLine(" NOTE: Optional /legacy switch ouputs the old script.\r\n");
            }
        }

        public bool OutputAll
        {
            get;
            private set;
        }

        public string OutputFile { get; set; }

        public string ConnectionString2 { get; set; }

        public string ConnectionString1 { get; set; }

        public bool Validate()
        {
            if (String.IsNullOrEmpty(ConnectionString1))
                throw new Exception("The target connection string is missing");
            if (String.IsNullOrEmpty(ConnectionString2))
                throw new Exception("The destination connection string is missing");
            if (String.IsNullOrEmpty(OutputFile))
                throw new Exception("The output destination is missing");
            return true;
        }
    }
}
