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

        public MessageLog(string description, string fullDescription, LogType type)
        {
            this.Description = description;
            this.FullDescription = fullDescription;
            this.Type = type;
        }

        public LogType Type { get; private set; }

        public string FullDescription { get; private set; }

        public string Description { get; private set; }
    }
}
