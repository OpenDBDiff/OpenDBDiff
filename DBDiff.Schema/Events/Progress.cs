using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Schema.Events
{
    public class Progress
    {
        public delegate void ProgressHandler(object sender, ProgressEventArgs e);
    }
}
