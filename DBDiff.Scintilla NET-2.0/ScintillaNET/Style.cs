using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ScintillaNet
{
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class StyleCollection : ScintillaHelperBase
	{
		internal StyleCollection(Scintilla scintilla) : base(scintilla)
		{
			Bits = 7;

			//	Defaulting CallTip Settings to Platform defaults
			Style s = CallTip;
			s.ForeColor = SystemColors.InfoText;
			s.BackColor = SystemColors.Info;
			s.Font = SystemFonts.StatusFont;

			//	Making Line Number's BackColor have a named system color
			//	instead of just the value
			LineNumber.BackColor = SystemColors.Control;
		}

		internal bool ShouldSerialize()
		{
			return ShouldSerializeBits() ||
				ShouldSerializeBraceBad() ||
				ShouldSerializeBraceLight() ||
				ShouldSerializeCallTip() ||
				ShouldSerializeControlChar() ||
				ShouldSerializeDefault() ||
				ShouldSerializeIndentGuide() ||
				ShouldSerializeLastPredefined() ||
				ShouldSerializeLineNumber() ||
				ShouldSerializeMax();
		}

		public void Reset()
		{
			for (int i = 0; i < 32; i++)
				this[i].Reset();
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Style this[int index]
		{
			get
			{
				return new Style(index, Scintilla);
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Style this[StylesCommon index]
		{
			get
			{
				return new Style((int)index, Scintilla);
			}
		}

		#region Bits
		[Obsolete("The modern style indicators make this obsolete, this should always be 7")]
		public int Bits
		{
			get
			{
				return NativeScintilla.GetStyleBits();
			}
			set
			{
				NativeScintilla.SetStyleBits(value);
			}
		}

		private bool ShouldSerializeBits()
		{
			return Bits != 7;
		}

		private void ResetBits()
		{
			Bits = 7;
		} 
		#endregion

		#region BraceBad
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Style BraceBad
		{
			get
			{
				return this[StylesCommon.BraceBad];
			}
		}

		private bool ShouldSerializeBraceBad()
		{
			return BraceBad.ShouldSerialize();
		} 
		#endregion

		#region BraceLight
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Style BraceLight
		{
			get
			{
				return this[StylesCommon.BraceLight];
			}
		}

		private bool ShouldSerializeBraceLight()
		{
			return BraceLight.ShouldSerialize();
		} 
		#endregion

		#region CallTip
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Style CallTip
		{
			get
			{
				return this[StylesCommon.CallTip];
			}
		}

		private bool ShouldSerializeCallTip()
		{
			return CallTip.ShouldSerialize();
		} 
		#endregion

		#region ControlChar
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Style ControlChar
		{
			get
			{
				return this[StylesCommon.ControlChar];
			}
		}

		private bool ShouldSerializeControlChar()
		{
			return ControlChar.ShouldSerialize();
		} 
		#endregion

		#region Default
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Style Default
		{
			get
			{
				return this[StylesCommon.Default];
			}
		}
		private bool ShouldSerializeDefault()
		{
			return BraceBad.ShouldSerialize();
		} 
		#endregion

		#region IndentGuide
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Style IndentGuide
		{
			get
			{
				return this[StylesCommon.IndentGuide];
			}
		}
		private bool ShouldSerializeIndentGuide()
		{
			return IndentGuide.ShouldSerialize();
		} 
		#endregion

		#region LastPredefined
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Style LastPredefined
		{
			get
			{
				return this[StylesCommon.LastPredefined];
			}
		}
		private bool ShouldSerializeLastPredefined()
		{
			return LastPredefined.ShouldSerialize();
		} 
		#endregion

		#region LineNumber
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Style LineNumber
		{
			get
			{
				return this[StylesCommon.LineNumber];
			}
		}
		private bool ShouldSerializeLineNumber()
		{
			return LineNumber.ShouldSerialize();
		} 
		#endregion

		#region Max
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Style Max
		{
			get
			{
				return this[StylesCommon.Max];
			}
		}
		private bool ShouldSerializeMax()
		{
			return Max.ShouldSerialize();
		} 
		#endregion

		#region Public Methods
		public void ClearAll()
		{
			NativeScintilla.StyleClearAll();
		}

		public int GetEndStyled()
		{
			return NativeScintilla.GetEndStyled();
		}

		public byte GetStyleAt(int position)
		{
			return NativeScintilla.GetStyleAt(position);
		}

		public void ResetDefault()
		{
			NativeScintilla.StyleResetDefault();
		} 

		public void ClearDocumentStyle()
		{
			NativeScintilla.ClearDocumentStyle();
		}
		#endregion
	}

	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class Style : ScintillaHelperBase
	{
		internal Style(int index, Scintilla scintilla)
			: base(scintilla)
		{
			_index = index;
		}

		internal bool ShouldSerialize()
		{
			return ShouldSerializeBackColor() ||
				ShouldSerializeBold() ||
				ShouldSerializeCase() ||
				ShouldSerializeCharacterSet() ||
				ShouldSerializeFontName() ||
				ShouldSerializeForeColor() ||
				ShouldSerializeIsChangeable() ||
				ShouldSerializeIsHotspot() ||
				ShouldSerializeIsSelectionEolFilled() ||
				ShouldSerializeIsVisible() ||
				ShouldSerializeItalic() ||
				ShouldSerializeSize() ||
				ShouldSerializeUnderline();
		}

		public void Reset()
		{
			ResetBackColor();
			ResetBold();
			ResetCase();
			ResetCharacterSet();
			ResetFontName();
			ResetForeColor();
			ResetIsChangeable();
			ResetIsHotspot();
			ResetIsSelectionEolFilled();
			ResetIsVisible();
			ResetItalic();
			ResetSize();
			ResetUnderline();
		}

		#region Public Methods
		public void Apply(int length)
		{
			Apply(NativeScintilla.GetCurrentPos(), length);
		}

		public void Apply(int position, int length)
		{
			NativeScintilla.StartStyling(position, 0xff);
			NativeScintilla.SetStyling(length, _index);
		}

		public int GetTextWidth(string text)
		{
			return NativeScintilla.TextWidth(_index, text);
		}

		public override string  ToString()
		{
 			 return "Style" + _index.ToString();
		}

		public void CopyTo(Style target)
		{
			target.BackColor = BackColor;
			target.Bold = Bold;
			target.Case = Case;
			target.CharacterSet = CharacterSet;
			target.FontName = FontName;
			target.ForeColor = ForeColor;
			target.IsChangeable = IsChangeable;
			target.IsHotspot = IsHotspot;
			target.IsSelectionEolFilled = IsSelectionEolFilled;
			target.IsVisible = IsVisible;
			target.Italic = Italic;
			target.Size = Size;
			target.Underline = Underline;
		}

		#endregion

		#region ForeColor
		public Color ForeColor
		{
			get
			{
				if (Scintilla.UseForeColor && !(_index == Constants.STYLE_CALLTIP))
					return Scintilla.ForeColor;

				if (Scintilla.ColorBag.ContainsKey(ToString() + ".ForeColor"))
					return Scintilla.ColorBag[ToString() + ".ForeColor"];

				return Utilities.RgbToColor(NativeScintilla.StyleGetFore(_index));
			}
			set
			{
				SetForeColorInternal(value);

				if (_index == (int)StylesCommon.CallTip)
					Scintilla.CallTip.SetForeColorInternal(value);
			}
		}

		internal void SetForeColorInternal(Color value)
		{
			Scintilla.ColorBag[ToString() + ".ForeColor"] = value;
			NativeScintilla.StyleSetFore(_index, Utilities.ColorToRgb(value));

			if (_index == (int)StylesCommon.CallTip)
				NativeScintilla.CallTipSetFore(Utilities.ColorToRgb(value));
		}

		private Color getDefaultForeColor()
		{
			if (_index == (int)StylesCommon.CallTip)
				return SystemColors.InfoText;
			else if (Scintilla.UseForeColor)
				return Scintilla.ForeColor;

			return Color.FromArgb(0, 0, 0);
		}

		private bool ShouldSerializeForeColor()
		{
			return ForeColor != getDefaultForeColor();
		}

		private void ResetForeColor()
		{
			ForeColor = getDefaultForeColor();
		}

		internal bool ForeColorNotSet()
		{
			return !Scintilla.ColorBag.ContainsKey(ToString() + ".ForeColor");
		}
		#endregion

		#region BackColor
		public Color BackColor
		{
			get
			{
				if (Scintilla.UseBackColor && !(_index == Constants.STYLE_CALLTIP || _index == Constants.STYLE_LINENUMBER))
					return Scintilla.BackColor;

				if (Scintilla.ColorBag.ContainsKey(ToString() + ".BackColor"))
					return Scintilla.ColorBag[ToString() + ".BackColor"];

				return Utilities.RgbToColor(NativeScintilla.StyleGetBack(_index));
			}
			set
			{
				SetBackColorInternal(value);

				if (_index == (int)StylesCommon.CallTip)
					Scintilla.CallTip.SetBackColorInternal(value);
			}
		}

		internal void SetBackColorInternal(Color value)
		{
			NativeScintilla.StyleSetBack(_index, Utilities.ColorToRgb(value));
			Scintilla.ColorBag[ToString() + ".BackColor"] = value;

			if (_index == (int)StylesCommon.CallTip)
				NativeScintilla.CallTipSetBack(Utilities.ColorToRgb(value));
		}

		private Color getDefaultBackColor()
		{
			if (_index == (int)StylesCommon.CallTip)
				return SystemColors.Info;
			else if (_index == (int)StylesCommon.LineNumber)
				return SystemColors.Control;
			else if (Scintilla.UseBackColor)
				return Scintilla.BackColor;

			return Color.FromArgb(0xff, 0xff, 0xff);
		}

		private bool ShouldSerializeBackColor()
		{
			return BackColor != getDefaultBackColor();
		}

		private void ResetBackColor()
		{
			BackColor = getDefaultBackColor();
		}

		internal bool BackColorNotSet()
		{
			return !Scintilla.ColorBag.ContainsKey(ToString() + ".BackColor");
		}

		#endregion

		#region FontName

		public string FontName
		{
			get
			{
				if (Scintilla.UseFont && !(_index == Constants.STYLE_CALLTIP))
					return Scintilla.Font.Name;

				//	Scintilla has trouble returning some font names, especially those
				//	with spaces in it. They get truncated. So we're storing ourselves.
				//	Oh yeah I wrote the code for SCI_STYLEGETFONT in Scintilla so what 
				//	does that tell you?
				if (!Scintilla.PropertyBag.ContainsKey(ToString() + ".FontName"))
				{
					string fontName;
					NativeScintilla.StyleGetFont(_index, out fontName);
					return fontName;
				}

				return Scintilla.PropertyBag[ToString() + ".FontName"].ToString();
			}
			set
			{
				NativeScintilla.StyleSetFont(_index, value);
				Scintilla.PropertyBag[ToString() + ".FontName"] = value;
				Scintilla.PropertyBag[ToString() + ".FontSet"] = true;

			}
		}

		private bool ShouldSerializeFontName()
		{
			return FontName != getDefaultFont().Name;
		}

		private void ResetFontName()
		{
			FontName = getDefaultFont().Name;
		}

		#endregion

		#region Bold
		public bool Bold
		{
			get
			{
				if (Scintilla.UseFont && !(_index == Constants.STYLE_CALLTIP))
					return Scintilla.Font.Bold;

				return NativeScintilla.StyleGetBold(_index);
			}
			set
			{
				NativeScintilla.StyleSetBold(_index, value);
				Scintilla.PropertyBag[ToString() + ".FontSet"] = true;
			}
		}

		private bool ShouldSerializeBold()
		{
			return Bold != getDefaultFont().Bold;
		}

		private void ResetBold()
		{
			Bold = getDefaultFont().Bold;
		} 
		#endregion

		#region Index

		private int _index = 0;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Index
		{
			get
			{
				return _index;
			}
		}
		
		#endregion

		#region Italic
		public bool Italic
		{
			get
			{
				if (Scintilla.UseFont && !(_index == Constants.STYLE_CALLTIP))
					return Scintilla.Font.Italic;

				return NativeScintilla.StyleGetItalic(_index);
			}
			set
			{
				NativeScintilla.StyleSetItalic(_index, value);
				Scintilla.PropertyBag[ToString() + ".FontSet"] = true;
			}
		}

		private bool ShouldSerializeItalic()
		{
			return Italic != getDefaultFont().Italic;
		}

		private void ResetItalic()
		{
			Italic = getDefaultFont().Italic;
		} 
		#endregion

		#region Underline
		public bool Underline
		{
			get
			{
				if (Scintilla.UseFont && !(_index == Constants.STYLE_CALLTIP))
					return Scintilla.Font.Underline;

				return NativeScintilla.StyleGetUnderline(_index);
			}
			set
			{
				NativeScintilla.StyleSetUnderline(_index, value);
				Scintilla.PropertyBag[ToString() + ".FontSet"] = true;
			}
		}

		private bool ShouldSerializeUnderline()
		{
			return Underline != getDefaultFont().Underline;
		}

		private void ResetUnderline()
		{
			Underline = getDefaultFont().Underline;
		} 
		#endregion

		#region CharacterSet
		public CharacterSet CharacterSet
		{
			get
			{
				if (Scintilla.UseFont && !(_index == Constants.STYLE_CALLTIP))
					return (CharacterSet)Scintilla.Font.GdiCharSet;

				return (CharacterSet)NativeScintilla.StyleGetCharacterSet(_index);
			}
			set
			{
				NativeScintilla.StyleSetCharacterSet(_index, (int)value);
				Scintilla.PropertyBag[ToString() + ".FontSet"] = true;
			}
		}

		private CharacterSet getDefaultCharacterSet()
		{
			return (CharacterSet)getDefaultFont().GdiCharSet;
		}

		private bool ShouldSerializeCharacterSet()
		{
			return CharacterSet != getDefaultCharacterSet();
		}

		private void ResetCharacterSet()
		{
			CharacterSet = getDefaultCharacterSet();
		} 
		#endregion

		#region Font
		public Font Font
		{
			get
			{
				FontStyle fs = FontStyle.Regular;
				if (Bold) fs |= FontStyle.Bold;
				if (Italic) fs |= FontStyle.Italic;
				if (Underline) fs |= FontStyle.Underline;

				return new Font(FontName, Size, fs, GraphicsUnit.Point, (byte)CharacterSet);
			}
			set
			{
				CharacterSet = (CharacterSet)value.GdiCharSet;
				FontName = value.Name;
				Size = value.SizeInPoints;
				Bold = value.Bold;
				Italic = value.Italic;
				Underline = value.Underline;
			}
		}

		private bool ShouldSerializeFont()
		{
			//	We never serialize the font property, we let the component
			//	properties do the work.
			return false;
		}

		internal void ResetFont()
		{
			Font = getDefaultFont();
			Scintilla.PropertyBag.Remove(ToString() + ".FontSet");
		}

		internal bool FontNotSet()
		{
			return !Scintilla.PropertyBag.ContainsKey(ToString() + ".FontSet");
		}

		private Font getDefaultFont()
		{
			if (_index == (int)StylesCommon.CallTip)
				return SystemFonts.StatusFont;
			else if (Scintilla.UseFont)
				return Scintilla.Font;

			return new Font("Verdana", 8);
		}

		#endregion

		#region IsSelectionEolFilled
		public bool IsSelectionEolFilled
		{
			get
			{
				return NativeScintilla.StyleGetEOLFilled(_index);
			}
			set
			{
				NativeScintilla.StyleSetEOLFilled(_index, value);
			}
		}

		private bool ShouldSerializeIsSelectionEolFilled()
		{
			return IsSelectionEolFilled;
		}

		private void ResetIsSelectionEolFilled()
		{
			IsSelectionEolFilled = false;
		} 
		#endregion

		#region Case
		public StyleCase Case
		{
			get
			{
				return (StyleCase)NativeScintilla.StyleGetCase(_index);
			}
			set
			{
				NativeScintilla.StyleSetCase(_index, (int)value);
			}
		}

		private bool ShouldSerializeCase()
		{
			return Case != StyleCase.Mixed;
		}

		private void ResetCase()
		{
			Case = StyleCase.Mixed;
		} 
		#endregion

		#region IsVisible
		public bool IsVisible
		{
			get
			{
				return NativeScintilla.StyleGetVisible(_index);
			}
			set
			{
				NativeScintilla.StyleSetVisible(_index, value);
			}
		}

		private bool ShouldSerializeIsVisible()
		{
			return !IsVisible;
		}

		private void ResetIsVisible()
		{
			IsVisible = true;
		} 
		#endregion

		#region Size

		//	There are 2 problems with Font Sizes, first Scintilla seems to
		//	accept them just fine, but always returns 8. Also it only supports
		//	integer font sizes, and .NET tends to use non integer values like 8.5
		//	which means that it would always be serialized. The solution? store our
		//	own value.
		public float Size
		{
			get
			{
				if (Scintilla.UseFont && !(_index == Constants.STYLE_CALLTIP))
					return Scintilla.Font.SizeInPoints;

				if (!Scintilla.PropertyBag.ContainsKey(ToString() + ".Size"))
					return (float)NativeScintilla.StyleGetSize(_index);

				return (float)Scintilla.PropertyBag[ToString() + ".Size"];
			}
			set
			{
				NativeScintilla.StyleSetSize(_index, (int)value);
				Scintilla.PropertyBag[ToString() + ".Size"] = value;
				Scintilla.PropertyBag[ToString() + ".FontSet"] = true;
			}
		}

		private bool ShouldSerializeSize()
		{
			return Size != getDefaultFont().SizeInPoints;
		}

		private void ResetSize()
		{
			Size = getDefaultFont().SizeInPoints;
		}
		#endregion

		#region IsHotspot
		public bool IsHotspot
		{
			get
			{
				return NativeScintilla.StyleGetHotSpot(_index); ;
			}
			set
			{
				NativeScintilla.StyleSetHotSpot(_index, value);
			}
		}

		private bool ShouldSerializeIsHotspot()
		{
			return IsHotspot;
		}

		private void ResetIsHotspot()
		{
			IsHotspot = false;
		} 
		#endregion

		#region IsChangeable
		public bool IsChangeable
		{
			get
			{
				return NativeScintilla.StyleGetChangeable(_index); ;
			}
			set
			{
				NativeScintilla.StyleSetChangeable(_index, value);
			}
		}

		private bool ShouldSerializeIsChangeable()
		{
			return !IsChangeable;
		}

		private void ResetIsChangeable()
		{
			IsChangeable = true;
		} 
		#endregion
	}

}

