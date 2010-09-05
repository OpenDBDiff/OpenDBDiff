using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Schema.Events
{
    public class ProgressEventHandler
    {
        public delegate void ProgressHandler(ProgressEventArgs e);

        public static event ProgressHandler OnProgress;

        public static void RaiseOnChange(ProgressEventArgs e)
        {
            if (OnProgress != null) OnProgress(e);
        }


    }
}
