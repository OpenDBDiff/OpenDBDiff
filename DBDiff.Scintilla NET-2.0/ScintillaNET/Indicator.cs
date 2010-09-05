using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace ScintillaNet
{
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class IndicatorCollection : ScintillaHelperBase
	{
		internal IndicatorCollection(Scintilla scintilla) : base(scintilla){}
		public Indicator this[int number]
		{
			get
			{
				return new Indicator(number, Scintilla);
			}
		}

		public void Reset()
		{
			for (int i = 0; i < 32; i++)
				this[i].Reset();
		}

	}
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class Indicator : ScintillaHelperBase
	{
		private int _number;

		internal Indicator(int number, Scintilla scintilla) : base(scintilla)
		{
			_number	= number;
		}

		internal bool ShouldSerialize()
		{
			return ShouldSerializeColor() ||
				ShouldSerializeIsDrawnUnder() ||
				ShouldSerializeStyle();
		}

		public int Number
		{
			get
			{
				return _number;
			}
			set
            {
            	_number = value;
            }
		}

		#region Color
		public Color Color
		{
			get
			{
				if (Scintilla.ColorBag.ContainsKey(ToString() + ".Color"))
					return Scintilla.ColorBag[ToString() + ".Color"];

				return Utilities.RgbToColor(NativeScintilla.IndicGetFore(_number));
			}
			set
			{
				Scintilla.ColorBag[ToString() + ".Color"] = value;
				NativeScintilla.IndicSetFore(_number, Utilities.ColorToRgb(value));
			}
		}

		private bool ShouldSerializeColor()
		{
			return Color != getDefaultColor();
		}

		public void ResetColor()
		{
			Color = getDefaultColor();
		}

		private Color getDefaultColor()
		{
			if (_number == 0)
				return Color.FromArgb(0, 127, 0);
			else if (_number == 1)
				return Color.FromArgb(0, 0, 255);
			else if (_number == 2)
				return Color.FromArgb(255, 0, 0);
			else
				return Color.FromArgb(0, 0, 0);
		}
		#endregion


		#region Style
		public IndicatorStyle Style
		{
			get
			{
				return (IndicatorStyle)NativeScintilla.IndicGetStyle(_number);
			}
			set
			{
				NativeScintilla.IndicSetStyle(_number, (int)value);
			}
		}

		private bool ShouldSerializeStyle()
		{
			return Style != getDefaultStyle();
		}

		public void ResetStyle()
		{
			Style = getDefaultStyle();
		}

		private IndicatorStyle getDefaultStyle()
		{
			if (_number == 0)
				return IndicatorStyle.Squiggle;
			else if (_number == 1)
				return IndicatorStyle.TT;
			else
				return IndicatorStyle.Plain;
		} 
		#endregion

		#region IsDrawnUnder
		public bool IsDrawnUnder
		{
			get
			{
				return NativeScintilla.IndicGetUnder(_number);
			}
			set
			{
				NativeScintilla.IndicSetUnder(_number, value);
			}
		}

		private bool ShouldSerializeIsDrawnUnder()
		{
			return IsDrawnUnder;
		}

		public void ResetIsDrawnUnder()
		{
			IsDrawnUnder = false;
		}

		public void Reset()
		{
			ResetColor();
			ResetIsDrawnUnder();
			ResetStyle();
		} 
		#endregion

		public Range Search()
		{
			return Search(Scintilla.GetRange());
		}

		public Range Search(Range searchRange)
		{
			int foundStart = NativeScintilla.IndicatorEnd(_number, searchRange.Start);
			int foundEnd = NativeScintilla.IndicatorEnd(_number, foundStart);
			if (foundStart < 0 || foundStart > searchRange.End || foundStart == foundEnd)
				return null;


			return new Range(foundStart, foundEnd, Scintilla);
		}

		public Range Search(Range searchRange, Range startingAfterRange)
		{
			int start = startingAfterRange.End;
			if (start > NativeScintilla.GetTextLength())
				return null;

			int foundStart = NativeScintilla.IndicatorEnd(_number, start);
			int foundEnd = NativeScintilla.IndicatorEnd(_number, foundStart);
			if (foundStart < 0 || foundStart > searchRange.End || foundStart == foundEnd)
				return null;
			
			return new Range(foundStart, foundEnd, Scintilla);
		}


		public List<Range> SearchAll()
		{
			return SearchAll(Scintilla.GetRange());
		}
			

		public List<Range> SearchAll(Range searchRange)
		{
			Range foundRange = Scintilla.GetRange(-1, -1);

			List<Range> ret = new List<Range>();
			do
			{
				foundRange = Search(searchRange, foundRange);
				if (foundRange != null)
					ret.Add(foundRange);
			}
			while (foundRange != null);
			return ret;
		}

		public override string ToString()
		{
			return "Indicator" + _number;
		}
		
	}

	
}
