using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace DBDiff.Scintilla
{
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class Clipboard : ScintillaHelperBase
	{
		internal Clipboard(Scintilla scintilla) : base(scintilla) { }

		internal bool ShouldSerialize()
		{
			return ShouldSerializeConvertEndOfLineOnPaste();
		}

		public void Copy()
		{
			NativeScintilla.Copy();
		}

		public void Copy(string text)
		{
			NativeScintilla.CopyText(text.Length, text);
		}

		public void Copy(Range rangeToCopy)
		{
			Copy(rangeToCopy.Start, rangeToCopy.End);
		}

		public void Copy(int positionStart, int positionEnd)
		{
			NativeScintilla.CopyRange(positionStart, positionEnd);
		}

		public void Cut()
		{
			NativeScintilla.Cut();
		}

		public void Paste()
		{
			NativeScintilla.Paste();
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanPaste
		{
			get
			{
				return NativeScintilla.CanPaste();
			}
		}

		#region ConvertEndOfLineOnPaste
		/// <summary>
		/// This is the same as EndOfLine.ConvertOnPaste
		/// </summary>
		public bool ConvertEndOfLineOnPaste
		{
			get
			{
				return NativeScintilla.GetPasteConvertEndings();
			}
			set
			{
				NativeScintilla.SetPasteConvertEndings(value);
			}
		}

		private bool ShouldSerializeConvertEndOfLineOnPaste()
		{
			return !ConvertEndOfLineOnPaste;
		}

		private void ResetConvertEndOfLineOnPaste()
		{
			ConvertEndOfLineOnPaste = true;
		} 
		#endregion
	}
}


