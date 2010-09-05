using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace DBDiff.Front
{
    public interface IFront
    {
        Point Location { get; set; }
        string Name { get; set; }
        Size Size { get; set; }
        int TabIndex { get; set; }
        bool Visible { get; set; }
        DockStyle Dock { get; set; }
        Boolean TestConnection();
        string ConnectionString { get; set; }
        string ErrorConnection { get; }
        string DatabaseName { get; }
        string Text { get; set; }
        AnchorStyles Anchor { get; set; }
    }
}
