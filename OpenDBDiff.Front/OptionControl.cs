using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDBDiff.Schema.Model;

namespace OpenDBDiff.Front
{
    public class OptionControl : System.Windows.Forms.UserControl
    {
        public new virtual void Load(IOption option)
        {
            throw new NotImplementedException("Load option not implemented");
        }

        public virtual void Save()
        {
            throw new NotImplementedException("Save not implemented");
        }
    }
}
