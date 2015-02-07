namespace DBDiff.Schema.Errors
{
    public class MessageLog
    {
        public enum LogType        
        {
            Information = 0,
            Warning = 1,
            Error = 2
        }
        private string description;
        private string fullDescription;
        private LogType type;

        public MessageLog(string description, string fullDescription, LogType type)
        {
            this.description = description;
            this.fullDescription = fullDescription;
            this.type = type;
        }

        public LogType Type
        {
            get { return type; }
        }

        public string FullDescription
        {
            get { return fullDescription; }
        }

        public string Description
        {
            get { return description; }
        }
        
    }
}
