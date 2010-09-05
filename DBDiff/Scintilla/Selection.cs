using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace DBDiff.Scintilla
{
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class Selection : ScintillaHelperBase
	{
		protected internal Selection(Scintilla scintilla) : base(scintilla) 
		{			
			NativeScintilla.SetSelBack(true, Utilities.ColorToRgb(BackColor));
			NativeScintilla.SetSelFore(true, Utilities.ColorToRgb(ForeColor));
		}

		internal bool ShouldSerialize()
		{
			return ShouldSerializeBackColor() | 
				ShouldSerializeBackColorUnfocused() | 
				ShouldSerializeForeColor() | 
				ShouldSerializeForeColorUnfocused() | 
				ShouldSerializeHidden() |
				ShouldSerializeHideSelection() |
				ShouldSerializeMode();
		}

		#region Non-designable Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Range Range
		{
			get
			{
				return new Range(NativeScintilla.GetSelectionStart(), NativeScintilla.GetSelectionEnd(), Scintilla);
			}
			set
            {
				NativeScintilla.SetSel(value.Start, value.End);
            }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Start
		{
			get
			{
				return NativeScintilla.GetSelectionStart();
			}
			set
			{
				NativeScintilla.SetSelectionStart(value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int End
		{
			get
			{
				return NativeScintilla.GetSelectionEnd();
			}
			set
			{
				NativeScintilla.SetSelectionEnd(value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Length
		{
			get
			{
				return Math.Abs(End - Start);
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Text
		{
			get
			{
				string s;
				NativeScintilla.GetSelText(out s);
				return s;
			}
			set
			{
				if(string.IsNullOrEmpty(value))
					Clear();
				else
					NativeScintilla.ReplaceSel(value);
			}
		}
		#endregion

		#region Designable Properties
		#region ForeColor

		public Color ForeColor
		{
			get
			{
				if (Scintilla.ColorBag.ContainsKey("Selection.ForeColor"))
					return Scintilla.ColorBag["Selection.ForeColor"];

				return SystemColors.HighlightText;
			}
			set
			{				
				if (ForeColor == SystemColors.HighlightText)
					Scintilla.ColorBag.Remove("Selection.ForeColor");
				else
					Scintilla.ColorBag["Selection.ForeColor"] = value;
				
				if (Scintilla.ContainsFocus)
					NativeScintilla.SetSelFore(true, Utilities.ColorToRgb(value));
			}
		}

		private bool ShouldSerializeForeColor()
		{
			return ForeColor != SystemColors.HighlightText;
		}

		private void ResetForeColor()
		{
			ForeColor = SystemColors.HighlightText;
		}
		#endregion

		#region ForeColorUnfocused
		public Color ForeColorUnfocused
		{
			get
			{
				if (Scintilla.ColorBag.ContainsKey("Selection.ForeColorUnfocused"))
					return Scintilla.ColorBag["Selection.ForeColorUnfocused"];

				return SystemColors.HighlightText;
			}
			set
			{
				if (value == ForeColorUnfocused)
					return;

				if (ForeColorUnfocused == SystemColors.HighlightText)
					Scintilla.ColorBag.Remove("Selection.ForeColorUnfocused");
				else
					Scintilla.ColorBag["Selection.ForeColorUnfocused"] = value;
				
				if(!Scintilla.ContainsFocus)
					NativeScintilla.SetSelFore(true, Utilities.ColorToRgb(value));
			}
		}

		private bool ShouldSerializeForeColorUnfocused()
		{
			return ForeColorUnfocused != SystemColors.HighlightText;
		}

		private void ResetForeColorUnfocused()
		{
			ForeColorUnfocused = SystemColors.HighlightText;
		}
		#endregion

		#region BackColorUnfocused
		private Color _backColorUnfocused = Color.LightGray;
		public Color BackColorUnfocused
		{
			get
			{
				if (Scintilla.ColorBag.ContainsKey("Selection.BackColorUnfocused"))
					return Scintilla.ColorBag["Selection.BackColorUnfocused"];

				return Color.LightGray;
			}
			set
			{
				if (value == BackColorUnfocused)
					return;

				if (BackColorUnfocused == Color.LightGray)
					Scintilla.ColorBag.Remove("Selection.BackColorUnfocused");
				else
					Scintilla.ColorBag["Selection.BackColorUnfocused"] = value;
				
				if(!Scintilla.ContainsFocus)
					NativeScintilla.SetSelBack(true, Utilities.ColorToRgb(value));
			}
		}

		private bool ShouldSerializeBackColorUnfocused()
		{
			return BackColorUnfocused != Color.LightGray;
		}

		private void ResetBackColorUnfocused()
		{
			BackColorUnfocused = Color.LightGray;
		}
		#endregion

		#region BackColor
		
		public Color BackColor
		{
			get
			{
				if (Scintilla.ColorBag.ContainsKey("Selection.BackColor"))
					return Scintilla.ColorBag["Selection.BackColor"];

				return SystemColors.Highlight;
			}
			set
			{
				if (value == BackColor)
					return;

				if (BackColor == SystemColors.Highlight)
					Scintilla.ColorBag.Remove("Selection.BackColor");
				else
					Scintilla.ColorBag["Selection.BackColor"] = value;

				if (Scintilla.ContainsFocus)
					NativeScintilla.SetSelBack(true, Utilities.ColorToRgb(value));
			}
		}

		private bool ShouldSerializeBackColor()
		{
			return BackColor != SystemColors.Highlight;
		}

		private void ResetBackColor()
		{
			BackColor = SystemColors.Highlight;
		}
		#endregion

		#region Hidden
		private bool _hidden = false;
		public bool Hidden
		{
			get
			{
				return _hidden;
			}
			set
			{
				_hidden = value;
				NativeScintilla.HideSelection(value);
			}
		}

		private bool ShouldSerializeHidden()
		{
			return _hidden;
		}

		private void ResetHidden()
		{
			Hidden = false;
		}

		#endregion

		#region HideSelection
		
		
		private bool _hideSelection = false;
		public bool HideSelection
		{
			get
			{
				return _hideSelection;
			}
			set
			{
				_hideSelection = value;
				
				if (!Scintilla.ContainsFocus)
					NativeScintilla.HideSelection(value);
			}
		}

		private bool ShouldSerializeHideSelection()
		{
			return _hideSelection;
		}

		private void ResetHideSelection()
		{
			_hideSelection = false;
		}

		#endregion

		#region Mode
		public SelectionMode Mode
		{
			get
			{
				return (SelectionMode)NativeScintilla.GetSelectionMode();
			}
			set
			{
				NativeScintilla.SetSelectionMode((int)value);
			}
		}

		private bool ShouldSerializeMode()
		{
			return Mode != SelectionMode.Stream;
		}

		private void ResetMode()
		{
			Mode = SelectionMode.Stream;
		}
		#endregion

		[Browsable(false)]
		public bool IsRectangle
		{
			get
			{
				return NativeScintilla.SelectionIsRectangle();
			}
		}
		#endregion

		#region Methods
		public void SelectAll()
		{
			NativeScintilla.SelectAll();
		}

		public void SelectNone()
		{
			NativeScintilla.SetSel(-1, -1);
		}

		public void Clear()
		{
			NativeScintilla.Clear();
		}

		#endregion
	}

}

