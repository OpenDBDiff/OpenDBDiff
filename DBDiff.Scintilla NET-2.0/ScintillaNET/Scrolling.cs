using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace ScintillaNet
{
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class Scrolling : ScintillaHelperBase
	{
		internal Scrolling(Scintilla scintilla) : base(scintilla) { }

		#region Scrollbars
		public ScrollBars ScrollBars
		{
			get
			{
				bool h = NativeScintilla.GetHScrollBar();
				bool v = NativeScintilla.GetVScrollBar();

				if (h && v)
					return ScrollBars.Both;
				else if (h)
					return ScrollBars.Horizontal;
				else if (v)
					return ScrollBars.Vertical;
				else
					return ScrollBars.None;
			}
			set
			{
				NativeScintilla.SetHScrollBar((value & ScrollBars.Horizontal) > 0);
				NativeScintilla.SetVScrollBar((value & ScrollBars.Vertical) > 0);
			}
		}

		private bool ShouldSerializeScrollBars()
		{
			return ScrollBars != ScrollBars.Both;
		}

		private void ResetScrollBars()
		{
			ScrollBars = ScrollBars.Both;
		}
		#endregion

		#region XOffset
		public int XOffset
		{
			get
			{
				return NativeScintilla.GetXOffset();
			}
			set
			{
				NativeScintilla.SetXOffset(value);
			}
		}

		private bool ShouldSerializeXOffset()
		{
			return XOffset != 0;
		}

		internal void ResetXOffset()
		{
			XOffset = 0;
		} 
		#endregion

		#region HorizontalWidth
		public int HorizontalWidth
		{
			get
			{
				return NativeScintilla.GetScrollWidth();
			}
			set
			{
				NativeScintilla.SetScrollWidth(value);
			}
		}

		private bool ShouldSerializeHorizontalWidth()
		{
			return HorizontalWidth != 2000;
		}

		internal void ResetHorizontalWidth()
		{
			HorizontalWidth = 2000;
		} 
		#endregion

		#region EndAtLastLine
		public bool EndAtLastLine
		{
			get
			{
				return NativeScintilla.GetEndAtLastLine();
			}
			set
			{
				NativeScintilla.SetEndAtLastLine(value);
			}
		}

		private bool ShouldSerializeEndAtLastLine()
		{
			return !EndAtLastLine;
		}

		internal void ResetEndAtLastLine()
		{
			EndAtLastLine = true;
		} 
		#endregion

		internal bool ShouldSerialize()
		{
			return ShouldSerializeEndAtLastLine() ||
				ShouldSerializeHorizontalWidth() ||
				ShouldSerializeScrollBars() ||
				ShouldSerializeXOffset();
		}

		public void ScrollBy(int columns, int lines)
		{
			NativeScintilla.LineScroll(columns, lines);
		}

		public void ScrollToCaret()
		{
			NativeScintilla.ScrollCaret();
		}
	}
}
