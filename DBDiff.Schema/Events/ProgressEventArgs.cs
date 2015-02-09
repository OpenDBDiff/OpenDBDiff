using System;

namespace DBDiff.Schema.Events
{
    public class ProgressEventArgs:EventArgs 
    {
        public string Message { get; set; }

        public ProgressEventArgs(string message, int progress)
        {
            this.Progress = progress;
            this.Message = message;
        }

        public int Progress { get; set; }
    }
}
