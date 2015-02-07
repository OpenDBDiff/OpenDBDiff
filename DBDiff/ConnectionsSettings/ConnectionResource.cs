namespace DBDiff.ConnectionsSettings
{
    public class ConnectionResource
    {
        private string name;
        private string type;
        private string connectionStringSource;
        private string connectionStringDestination;

        public string ConnectionStringDestination
        {
            get { return connectionStringDestination; }
            set { connectionStringDestination = value; }
        }

        public string ConnectionStringSource
        {
            get { return connectionStringSource; }
            set { connectionStringSource = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

    }
}
