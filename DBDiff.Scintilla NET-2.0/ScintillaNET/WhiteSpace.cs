using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace ScintillaNet
{
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class WhiteSpace : ScintillaHelperBase
	{
		internal WhiteSpace(Scintilla scintilla) : base(scintilla) { }

		internal bool ShouldSerialize()
		{
			return
				ShouldSerializeBackColor() ||
				ShouldSerializeForeColor() ||
				ShouldSerializeMode() ||
				ShouldSerializeUseWhiteSpaceBackColor() ||
				ShouldSerializeUseWhiteSpaceForeColor();
		}

		#region Mode
		public WhiteSpaceMode Mode
		{
			get
			{
				return (WhiteSpaceMode)NativeScintilla.GetViewWS();
			}
			set
			{
				NativeScintilla.SetViewWS((int)value);
			}
		}

		private bool ShouldSerializeMode()
		{
			return Mode != WhiteSpaceMode.Invisible;
		}

		private void ResetMode()
		{
			Mode = WhiteSpaceMode.Invisible;
		} 
		#endregion

		#region BackColor
		public Color BackColor
		{
			get
			{
				if (Scintilla.ColorBag.ContainsKey("WhiteSpace.BackColor"))
					return Scintilla.ColorBag["WhiteSpace.BackColor"];

				return SystemColors.Window;
			}
			set
			{
				if (value == Color.Transparent)
					Scintilla.ColorBag.Remove("WhiteSpace.BackColor");
				else
					Scintilla.ColorBag["WhiteSpace.BackColor"] = value;

				NativeScintilla.SetWhitespaceBack(true, Utilities.ColorToRgb(value));
			}
		}

		public bool ShouldSerializeBackColor()
		{
			return BackColor != SystemColors.Window;
		}

		private void ResetBackColor()
		{
			BackColor = SystemColors.Window;
		}
		#endregion

		#region ForeColor
		public Color ForeColor
		{
			get
			{
				if (Scintilla.ColorBag.ContainsKey("WhiteSpace.ForeColor"))
					return Scintilla.ColorBag["WhiteSpace.ForeColor"];

				return SystemColors.WindowText;
			}
			set
			{
				if (value == Color.Transparent)
					Scintilla.ColorBag.Remove("WhiteSpace.ForeColor");
				else
					Scintilla.ColorBag["WhiteSpace.ForeColor"] = value;

				NativeScintilla.SetWhitespaceFore(true, Utilities.ColorToRgb(value));
			}
		}

		public bool ShouldSerializeForeColor()
		{
			return ForeColor != SystemColors.WindowText;
		}

		private void ResetForeColor()
		{
			ForeColor = SystemColors.WindowText;
		}
		#endregion

		#region UseWhiteSpaceForeColor
		private bool _useWhiteSpaceForeColor = true;
		public bool UseWhiteSpaceForeColor
		{
			get
			{
				return _useWhiteSpaceForeColor;
			}
			set
			{
				_useWhiteSpaceForeColor = value;
				ForeColor = ForeColor;
			}
		}

		private bool ShouldSerializeUseWhiteSpaceForeColor()
		{
			return !UseWhiteSpaceForeColor;
		}

		private void ResetUseWhiteSpaceForeColor()
		{
			UseWhiteSpaceForeColor = true;
		}
		#endregion

		#region UseWhiteSpaceBackColor
		private bool _useWhiteSpaceBackColor = true;
		public bool UseWhiteSpaceBackColor
		{
			get
			{
				return _useWhiteSpaceBackColor;
			}
			set
			{
				_useWhiteSpaceBackColor = value;
				BackColor = BackColor;
			}
		}

		private bool ShouldSerializeUseWhiteSpaceBackColor()
		{
			return !UseWhiteSpaceBackColor;
		}

		private void ResetUseWhiteSpaceBackColor()
		{
			UseWhiteSpaceBackColor = true;
		}
		#endregion
	}
}
