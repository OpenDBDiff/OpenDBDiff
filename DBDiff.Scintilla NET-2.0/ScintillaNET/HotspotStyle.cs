using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;

namespace ScintillaNet
{
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class HotspotStyle : ScintillaHelperBase
	{
		internal HotspotStyle(Scintilla scintilla) : base(scintilla) 
		{
			ActiveForeColor = SystemColors.HotTrack;
			ActiveBackColor = SystemColors.Window;
		}

		internal bool ShouldSerialize()
		{
			return ShouldSerializeActiveBackColor() || 
				ShouldSerializeActiveForeColor() ||
				ShouldSerializeActiveUnderline() ||
				ShouldSerializeSingleLine() ||
				ShouldSerializeUseActiveBackColor() ||
				ShouldSerializeUseActiveForeColor();
		}

		#region ActiveFore
		public Color ActiveForeColor
		{
			get
			{
				if (Scintilla.ColorBag.ContainsKey("HotspotStyle.ActiveForeColor"))
					return Scintilla.ColorBag["HotspotStyle.ActiveForeColor"];

				return Utilities.RgbToColor(NativeScintilla.GetHotSpotActiveFore());
			}
			set
			{
				if (value.IsKnownColor)
					Scintilla.ColorBag["HotspotStyle.ActiveForeColor"] = value;
				else
					Scintilla.ColorBag.Remove("HotspotStyle.ActiveForeColor");

				NativeScintilla.SetHotspotActiveFore(_useActiveForeColor, Utilities.ColorToRgb(value));
			}
		}

		private bool ShouldSerializeActiveForeColor()
		{
			return ActiveForeColor != SystemColors.HotTrack;
		}

		private void ResetActiveForeColor()
		{
			ActiveForeColor = SystemColors.HotTrack;
		}

		#endregion

		#region ActiveBack
		public Color ActiveBackColor
		{
			get
			{
				if (Scintilla.ColorBag.ContainsKey("HotspotStyle.ActiveBackColor"))
					return Scintilla.ColorBag["HotspotStyle.ActiveBackColor"];

				return Utilities.RgbToColor(NativeScintilla.GetHotSpotActiveBack());
			}
			set
			{
				if (value.IsKnownColor)
					Scintilla.ColorBag["HotspotStyle.ActiveBackColor"] = value;
				else
					Scintilla.ColorBag.Remove("HotspotStyle.ActiveBackColor");

				NativeScintilla.SetHotspotActiveBack(_useActiveBackColor, Utilities.ColorToRgb(value));
			}
		}

		private bool ShouldSerializeActiveBackColor()
		{
			return ActiveBackColor != SystemColors.Window;
		}

		private void ResetActiveBackColor()
		{
			ActiveBackColor = SystemColors.Window;
		}

		#endregion

		#region ActiveUnderline
		public bool ActiveUnderline
		{
			get
			{
				return NativeScintilla.GetHotSpotActiveUnderline();
			}
			set
			{
				NativeScintilla.SetHotspotActiveUnderline(value);
			}
		}

		private bool ShouldSerializeActiveUnderline()
		{
			return !ActiveUnderline;
		}

		private void ResetActiveUnderline()
		{
			ActiveUnderline = true;
		}

		#endregion

		#region SingleLine
		public bool SingleLine
		{
			get
			{
				return NativeScintilla.GetHotSpotSingleLine();
			}
			set
			{
				NativeScintilla.SetHotspotSingleLine(value);
			}
		}

		private bool ShouldSerializeSingleLine()
		{
			return !SingleLine;
		}

		private void ResetSingleLine()
		{
			SingleLine = true;
		}

		#endregion

		#region UseActiveForeColor
		private bool _useActiveForeColor = true;
		public bool UseActiveForeColor
		{
			get
			{
				return _useActiveForeColor;
			}
			set
			{
				_useActiveForeColor = value;
			}
		}

		private bool ShouldSerializeUseActiveForeColor()
		{
			return !UseActiveForeColor;
		}

		private void ResetUseActiveForeColor()
		{
			UseActiveForeColor = true;
		}

		#endregion

		#region UseActiveBackColor
		private bool _useActiveBackColor = true;
		public bool UseActiveBackColor
		{
			get
			{
				return _useActiveBackColor;
			}
			set
			{
				_useActiveBackColor = value;
			}
		}

		private bool ShouldSerializeUseActiveBackColor()
		{
			return !UseActiveBackColor;
		}

		private void ResetUseActiveBackColor()
		{
			UseActiveBackColor = true;
		}


		#endregion
	}
}
