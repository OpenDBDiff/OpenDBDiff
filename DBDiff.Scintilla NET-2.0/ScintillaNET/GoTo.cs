using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ScintillaNet
{
	public class GoTo : ScintillaHelperBase
	{
		internal GoTo(Scintilla scintilla)
			: base(scintilla)
		{}

		public void Position(int pos)
		{
			NativeScintilla.GotoPos(pos);
		}

		public void Line(int number)
		{
			NativeScintilla.GotoLine(number);
		}

		public void ShowGoToDialog()
		{
			GoToDialog gd = new GoToDialog();

			gd.CurrentLineNumber = Scintilla.Lines.Current.Number;
			gd.MaximumLineNumber = Scintilla.Lines.Count;
			gd.Scintilla = Scintilla;

			if (gd.ShowDialog() == DialogResult.OK)
				Line(gd.GotoLineNumber);
		
			Scintilla.Focus();
		}
	}
}
