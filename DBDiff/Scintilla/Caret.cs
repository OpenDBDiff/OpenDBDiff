using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace DBDiff.Scintilla
{
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class CaretInfo : ScintillaHelperBase
	{
		protected internal CaretInfo(Scintilla scintilla) : base(scintilla) 
		{
			BlinkRate = SystemInformation.CaretBlinkTime;
			Width = SystemInformation.CaretWidth;
		}

		protected internal bool ShouldSerialize()
		{
			return ShouldSerializeBlinkRate() ||
					ShouldSerializeColor() ||
					ShouldSerializeCurrentLineBackgroundColor() ||
					ShouldSerializeWidth() ||
					ShouldSerializeHighlightCurrentLine() ||
					ShouldSerializeCurrentLineBackgroundAlpha() ||
					ShouldSerializeStyle() ||
					ShouldSerializeIsSticky();
		}

		public void EnsureVisible()
		{
			NativeScintilla.ScrollCaret();
		}

		public override string ToString()
		{
			return ShouldSerialize() ? base.ToString() : string.Empty;
		}

		public void ChooseCaretX()
		{
			NativeScintilla.ChooseCaretX();
		}

		#region Width

		public int Width
		{
			get
			{
				return NativeScintilla.GetCaretWidth();
			}
			set
			{
				NativeScintilla.SetCaretWidth(value);
			}
		}

		private bool ShouldSerializeWidth()
		{
			return Width != SystemInformation.CaretWidth;
		}

		private void ResetWidth()
		{
			Width = SystemInformation.CaretWidth;
		}
		#endregion

		#region	CaretStyle
		public CaretStyle Style
		{
			get
			{
				return (CaretStyle)NativeScintilla.GetCaretStyle();
			}
			set
			{
				NativeScintilla.SetCaretStyle((int)value);
			}
		}

		private bool ShouldSerializeStyle()
		{
			return Style != CaretStyle.Line;
		}

		private void ResetStyle()
		{
			Style = CaretStyle.Line;
		}
		#endregion

		#region Color

		public Color Color
		{
			get
			{
				if (Scintilla.ColorBag.ContainsKey("Caret.Color"))
					return Scintilla.ColorBag["Caret.Color"];

				Color c = Utilities.RgbToColor(NativeScintilla.GetCaretFore());
				
				if (c == Color.FromArgb(0,0,0))
					return Color.Black;

				return c;
			}
			set
			{
				if (value == Color)
					return;

				if (value.IsKnownColor)
				{
					if (Color == Color.Black)
						Scintilla.ColorBag.Remove("Caret.Color");
					else
						Scintilla.ColorBag["Caret.Color"] = value;
				}

				NativeScintilla.SetCaretFore(Utilities.ColorToRgb(value));
			}
		}

		private bool ShouldSerializeColor()
		{
			return Color != Color.Black;
		}

		private void ResetColor()
		{
			Color = Color.Black;
		}
		#endregion Color

		#region CurrentLineBackgroundColor


		public Color CurrentLineBackgroundColor
		{
			get
			{
				if (Scintilla.ColorBag.ContainsKey("Caret.CurrentLineBackgroundColor"))
					return Scintilla.ColorBag["Caret.CurrentLineBackgroundColor"];

				Color c = Utilities.RgbToColor(NativeScintilla.GetCaretLineBack());

				if (c.ToArgb() == Color.Yellow.ToArgb())
					return Color.Yellow;

				return c;
			}
			set
			{
				

				if (value == Color)
					return;

				if (value.IsKnownColor)
				{
					if (Color == Color.Yellow)
						Scintilla.ColorBag.Remove("Caret.CurrentLineBackgroundColor");
					else
						Scintilla.ColorBag["Caret.CurrentLineBackgroundColor"] = value;
				}

				NativeScintilla.SetCaretLineBack(Utilities.ColorToRgb(value));
			}
		}

		private bool ShouldSerializeCurrentLineBackgroundColor()
		{
			return CurrentLineBackgroundColor != Color.Yellow;
		}

		private void ResetCurrentLineBackgroundColor()
		{
			CurrentLineBackgroundColor = Color.Yellow;
		}
		#endregion

		#region HighlightCurrentLine
		
		public bool HighlightCurrentLine
		{
			get
			{
				return NativeScintilla.GetCaretLineVisible();
			}
			set
			{
				NativeScintilla.SetCaretLineVisible(value);
			}
		}

		private bool ShouldSerializeHighlightCurrentLine()
		{
			return HighlightCurrentLine;
		}

		private void ResetHighlightCurrentLine()
		{
			HighlightCurrentLine = false;
		}
		#endregion

		#region CurrentLineBackgroundAlpha

		public int CurrentLineBackgroundAlpha
		{
			get
			{
				return NativeScintilla.GetCaretLineBackAlpha();
			}
			set
			{
				NativeScintilla.SetCaretLineBackAlpha(value);
			}
		}

		private bool ShouldSerializeCurrentLineBackgroundAlpha()
		{
			return CurrentLineBackgroundAlpha != 256;
		}

		private void ResetCurrentLineBackgroundAlpha()
		{
			CurrentLineBackgroundAlpha = 256;
		}

		#endregion

		#region BlinkRate

		public int BlinkRate
		{
			get
			{
				return NativeScintilla.GetCaretPeriod();
			}
			set
			{
				NativeScintilla.SetCaretPeriod(value);
			}
		}

		private bool ShouldSerializeBlinkRate()
		{
			return BlinkRate != SystemInformation.CaretBlinkTime;
		}

		private void ResetBlinkRate()
		{
			BlinkRate = SystemInformation.CaretBlinkTime;
		}
		#endregion

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Position
		{
			get
			{
				return NativeScintilla.GetCurrentPos();
			}
			set
			{
				NativeScintilla.SetCurrentPos(value);
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Anchor
		{
			get
			{
				return NativeScintilla.GetAnchor();
			}
			set
			{
				NativeScintilla.SetAnchor(value);
			}
		}


		#region IsSticky
		public bool IsSticky
		{
			get
			{
				return NativeScintilla.GetCaretSticky();
			}
			set
			{
				NativeScintilla.SetCaretSticky(value);
			}
		}

		private bool ShouldSerializeIsSticky()
		{
			return IsSticky;
		}

		private void ResetIsSticky()
		{
			IsSticky = false;
		} 
		#endregion

		
		public void Goto(int position)
		{
			NativeScintilla.GotoPos(position);
		}



		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int LineNumber
		{
			get
			{
				return NativeScintilla.LineFromPosition(NativeScintilla.GetCurrentPos());
			}
			set
			{
				NativeScintilla.GotoLine(value);
			}
		}

		public void BringIntoView()
		{
			NativeScintilla.MoveCaretInsideView();
		}
	}
}
