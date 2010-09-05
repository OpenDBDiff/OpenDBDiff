using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace DBDiff.Scintilla
{
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class EndOfLine : ScintillaHelperBase
	{
		internal EndOfLine(Scintilla scintilla) : base(scintilla) { }

		internal bool ShouldSerialize()
		{
			return ShouldSerializeIsVisible() || ShouldSerializeMode() || ShouldSerializeConvertOnPaste();
		}

		#region ConvertOnPaste
		public bool ConvertOnPaste
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

		private bool ShouldSerializeConvertOnPaste()
		{
			return !ConvertOnPaste;
		}

		private void ResetConvertOnPaste()
		{
			ConvertOnPaste = true;
		} 
		#endregion

		#region Mode
		public EndOfLineMode Mode
		{
			get
			{
				return (EndOfLineMode)NativeScintilla.GetEolMode();
			}
			set
			{
				NativeScintilla.SetEolMode((int)value);
			}
		}

		private bool ShouldSerializeMode()
		{
			//	Yeah I'm assuming Windows, if this does ever make it to another platform 
			//	a check should be made to make it platform specific
			return Mode != EndOfLineMode.Crlf;
		}

		private void ResetMode()
		{
			Mode = EndOfLineMode.Crlf;
		} 
		#endregion

		#region IsVisible
		public bool IsVisible
		{
			get
			{
				return NativeScintilla.GetViewEol();
			}
			set
			{
				NativeScintilla.SetViewEol(value);
			}
		}

		private bool ShouldSerializeIsVisible()
		{
			return IsVisible;
		}

		private void ResetIsVisible()
		{
			IsVisible = false;
		} 
		#endregion

		public string EolString
		{
			get
			{
				switch (Mode)
				{
					case EndOfLineMode.CR:
						return "\r";
					case EndOfLineMode.LF:
						return "\n";
					case EndOfLineMode.Crlf:
						return "\r\n";
				}
				return "";
			}
		}

		private void ConvertAllLines(EndOfLineMode toMode)
		{
			NativeScintilla.ConvertEols((int)toMode);
		}
	}
}
