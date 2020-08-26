using OpenDBDiff.Abstractions.Schema.Model;
using System;

namespace OpenDBDiff.Abstractions.Ui
{
    public class OptionControl : System.Windows.Forms.UserControl
    {
        public event OptionEventHandler OptionSaved;
        public delegate void OptionEventHandler(IOption option);
        public new virtual void Load(IOption option)
        {
            throw new NotImplementedException("Load option not implemented");
        }

        public virtual void Save()
        {
            throw new NotImplementedException("Save not implemented");
        }

        protected virtual void FireOptionChanged(IOption option)
        {
            OptionSaved?.Invoke(option);
        }
    }
}
