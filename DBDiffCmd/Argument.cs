namespace DBDiff.OCDB
{
    using System;

    public class Argument
    {
        private string connectionString1;
        private string connectionString2;
        private string outputFile;
        
        public Argument(string[] commandline)
        {
            for (int i = 0; i < commandline.Length; i++)
            {
                if (commandline[i].Length > 4)
                {
                    if (commandline[i].Substring(0, 4).Equals("CN1=", StringComparison.CurrentCultureIgnoreCase))
                        connectionString1 = commandline[i].Substring(4, commandline[i].Length - 4).Trim();
                    if (commandline[i].Substring(0, 4).Equals("CN2=", StringComparison.CurrentCultureIgnoreCase))
                        connectionString2 = commandline[i].Substring(4, commandline[i].Length - 4).Trim();
                    if (commandline[i].Substring(0, 2).Equals("F=", StringComparison.CurrentCultureIgnoreCase))
                        outputFile = commandline[i].Substring(2, commandline[i].Length - 2).Trim();
                    if (String.Compare(commandline[i], "/legacy", true) == 0)
                        this.OutputAll = true;
                }
            }
            if (String.IsNullOrEmpty(connectionString1) || String.IsNullOrEmpty(ConnectionString1) || String.IsNullOrEmpty(outputFile))
            {
                System.Console.WriteLine("\r\n Example of use:\r\n");
                System.Console.WriteLine("    OCDB CN1=\"Destination/Target\" CN2=\"Source\" F=OuputScript.sql\r\n");
                System.Console.WriteLine(" (where CN1 and CN2 are SQL Server 2005+ Connection Strings)");
                Console.WriteLine(" NOTE: Optional /legacy switch ouputs the old script.\r\n");
            }
        }

        public bool OutputAll
        {
            get;
            private set;
        }

        public string OutputFile
        {
            get { return outputFile; }
            set { outputFile = value; }
        }

        public string ConnectionString2
        {
            get { return connectionString2; }
            set { connectionString2 = value; }
        }

        public string ConnectionString1
        {
            get { return connectionString1; }
            set { connectionString1 = value; }
        }

        public bool Validate()
        {
            if (String.IsNullOrEmpty(connectionString1))
                throw new Exception("The target connection string is missing");
            if (String.IsNullOrEmpty(connectionString2))
                throw new Exception("The destination connection string is missing");
            if (String.IsNullOrEmpty(outputFile))
                throw new Exception("The output destination is missing");
            return true;
        }
    }
}
