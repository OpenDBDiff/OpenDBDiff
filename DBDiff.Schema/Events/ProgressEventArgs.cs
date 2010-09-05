using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Schema.Events
{
    public class ProgressEventArgs:EventArgs 
    {
        private double progress;

        public ProgressEventArgs(double progress)
        {
            this.progress = progress;
        }

        public double Progress
        {
            get { return progress; }
            set { progress = value; }
        }
    }
}
