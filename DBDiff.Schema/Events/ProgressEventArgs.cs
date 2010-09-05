using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Schema.Events
{
    public class ProgressEventArgs:EventArgs 
    {
        private int progress;
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public ProgressEventArgs(string message, int progress)
        {
            this.progress = progress;
            this.message = message;
        }

        public int Progress
        {
            get { return progress; }
            set { progress = value; }
        }
    }
}
