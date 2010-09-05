using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace DBDiff.Front
{
    public interface IFront
    {
        Point Location { get; set; }
        string Name { get; set; }
        Size Size { get; set; }
        int TabIndex { get; set; }
        bool Visible { get; set; }

        Boolean TestConnection();
        string ConnectionString { get; }
        string ErrorConnection { get; }
    }
}
