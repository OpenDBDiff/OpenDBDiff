using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Drawing;
namespace ScintillaNet
{
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class Margin : ScintillaHelperBase
	{
		protected internal Margin(Scintilla scintilla, int number) : base(scintilla) 
		{
			_number = number;
		}

		internal bool ShouldSerialize()
		{
			return ShouldSerializeIsFoldMargin() ||
				ShouldSerializeIsClickable() ||
				ShouldSerializeType() ||
				ShouldSerializeWidth() ||
				ShouldSerializeAutoToggleMarkerNumber() ||
				ShouldSerializeIsMarkerMargin();
		}

		public void Reset()
		{
			ResetAutoToggleMarkerNumber();
			ResetIsClickable();
			ResetIsFoldMargin();
			ResetIsMarkerMargin();
			ResetType();
			ResetWidth();
		}

		public override string ToString()
		{
			if (_number == 0)
				return "(Default Line Numbers)";
			else if (_number == 1)
				return "(Default Markers)";
			else if (_number == 2)
				return "(Default Folds)";

			return base.ToString();
		}

		#region Number
		private int _number;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Number
		{
			get
			{
				return _number;
			}
		} 
		#endregion

		#region Type
		public MarginType Type
		{
			get
			{
				return (MarginType)NativeScintilla.GetMarginTypeN(_number);
			}
			set
			{
				NativeScintilla.SetMarginTypeN(_number, (int)value);
			}
		}

		private bool ShouldSerializeType()
		{
			//	Margin 0 defaults to Number, all the rest
			//	default to Symbol
			if (_number == 0)
				return Type != MarginType.Number;

			return Type != MarginType.Symbol;
		}

		internal void ResetType()
		{
			if (_number == 0)
				Type = MarginType.Number;
			else
				Type = MarginType.Symbol;
		} 
		#endregion

		#region Mask
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Mask
		{
			get
			{
				return NativeScintilla.GetMarginMaskN(_number);
			}
			set
			{
				NativeScintilla.SetMarginMaskN(_number, value);
			}
		}

		#endregion

		#region IsFoldMargin
		public bool IsFoldMargin
		{
			get
			{
				return (Mask & Constants.SC_MASK_FOLDERS) == Constants.SC_MASK_FOLDERS;
			}
			set
			{
				if (value)
					Mask |= Constants.SC_MASK_FOLDERS;
				else
					Mask &= ~Constants.SC_MASK_FOLDERS;
			}
		}

		private bool ShouldSerializeIsFoldMargin()
		{
			if (_number == 2)
				return !IsFoldMargin;

			return IsFoldMargin;
		}

		internal void ResetIsFoldMargin()
		{
			if (_number == 2)
				IsFoldMargin = true;
			else
				IsFoldMargin = false;
		} 
		#endregion

		#region IsMarkerMargin
		public bool IsMarkerMargin
		{
			//	As best as I can divine, this value is like SC_MASK_FOLDERS but applies
			//	instead to regular markers. I can't seem to find it in any of the documentation
			//	or even a constant defined for it though.
			get
			{
				return (Mask & 0x1FFFFFF) == 0x1FFFFFF;
			}

			set
			{
				if (value)
					Mask |= 0x1FFFFFF;
				else
					Mask &= ~0x1FFFFFF;
			}
		}

		private bool ShouldSerializeIsMarkerMargin()
		{
			if (_number == 1)
				return !IsMarkerMargin;
			else
				return IsMarkerMargin;
		}

		internal void ResetIsMarkerMargin()
		{
			if (_number == 1)
				IsMarkerMargin = true;
			else
				IsMarkerMargin = false;
		}  
		#endregion

		#region Width
		public int Width
		{
			get
			{
				return NativeScintilla.GetMarginWidthN(_number);
			}
			set
			{
				NativeScintilla.SetMarginWidthN(_number, value);
			}
		}

		private bool ShouldSerializeWidth()
		{
			//	Margin 1 defaults to 16, all the rest
			//	default to 0
			if (_number == 1)
				return Width != 16;

			return Width != 0;
		}

		internal void ResetWidth()
		{
			if (_number == 1)
				Width = 16;
			else
				Width = 0;
		} 
		#endregion

		#region IsClickable
		public bool IsClickable
		{
			get
			{
				return NativeScintilla.GetMarginSensitiveN(_number);
			}
			set
			{
				NativeScintilla.SetMarginSensitiveN(_number, value);
			}
		}

		private bool ShouldSerializeIsClickable()
		{
			if (_number == 2)
				return !IsClickable;

			return IsClickable;
		}

		internal void ResetIsClickable()
		{
			if (_number == 2)
				IsClickable = true;
			else 
				IsClickable = false;
		}
		#endregion

		#region AutoToggleMarkerNumber
		private int _autoToggleMarkerNumber = -1;
		public int AutoToggleMarkerNumber
		{
			get
			{
				return _autoToggleMarkerNumber;
			}
			set
			{
				_autoToggleMarkerNumber = value;
			}
		}

		private bool ShouldSerializeAutoToggleMarkerNumber()
		{
			return _autoToggleMarkerNumber != -1;
		}

		private void ResetAutoToggleMarkerNumber()
		{
			_autoToggleMarkerNumber = -1;
		} 
		#endregion

		public Rectangle GetClientRectangle()
		{
			int left = 0;
			for (int i = 0; i < _number; i++)
				left += NativeScintilla.GetMarginWidthN(i);

			return new Rectangle(left, 0, Width, Scintilla.ClientSize.Height);
		}
	}

	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class MarginCollection : ScintillaHelperBase, ICollection<Margin>
	{
		#region Individual Margin Elements

		#region Margin0
		private Margin _margin0;
		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Margin Margin0
		{
			get
			{
				return _margin0;
			}
		}

		private bool ShouldSerializeMargin0()
		{
			return _margin0.ShouldSerialize();
		}

		private void ResetMargin0()
		{
			_margin0.Reset();
		} 
		#endregion

		#region Margin1
		private Margin _margin1;
		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Margin Margin1
		{
			get
			{
				return _margin1;
			}
		}

		private bool ShouldSerializeMargin1()
		{
			return _margin1.ShouldSerialize();
		}

		private void ResetMargin1()
		{
			_margin0.Reset();
		} 
		#endregion

		#region Margin2
		private Margin _margin2;
		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Margin Margin2
		{
			get
			{
				return _margin2;
			}
		}

		private bool ShouldSerializeMargin2()
		{
			return _margin2.ShouldSerialize();
		}

		private void ResetMargin2()
		{
			_margin0.Reset();
		}

		#endregion

		#region Margin3
		private Margin _margin3;
		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Margin Margin3
		{
			get
			{
				return _margin3;
			}
		}

		private bool ShouldSerializeMargin3()
		{
			return _margin3.ShouldSerialize();
		}

		private void ResetMargin3()
		{
			_margin0.Reset();
		} 
		#endregion

		#region Margin4
		private Margin _margin4;
		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Margin Margin4
		{
			get
			{
				return _margin4;
			}
		}

		private bool ShouldSerializeMargin4()
		{
			return _margin4.ShouldSerialize();
		}

		private void ResetMargin4()
		{
			_margin0.Reset();
		} 
		#endregion
		#endregion

		protected internal MarginCollection(Scintilla scintilla)
			: base(scintilla)
		{
			_margin0 = new Margin(scintilla, 0);
			_margin1 = new Margin(scintilla, 1);
			_margin2 = new Margin(scintilla, 2);
			_margin3 = new Margin(scintilla, 3);
			_margin4 = new Margin(scintilla, 4);

			_margin2.IsFoldMargin = true;
			_margin2.IsClickable = true;
		}

		public Margin this[int number]
		{
			get
			{
				Debug.Assert(number >= 0, "Number must be greater than or equal to 0");
				Debug.Assert(number < 5, "Number must be less than 5");

				switch (number)
				{
					case 0:
						return _margin0;
					case 1:
						return _margin1;
					case 2:
						return _margin2;
					case 3:
						return _margin3;
					case 4:
						return _margin4;
				}

				throw new ArgumentException("number", "Number must be greater than or equal to 0 AND less than 5");
			}
		}

		internal bool ShouldSerialize()
		{
			return ShouldSerializeFoldMarginColor() ||
				ShouldSerializeFoldMarginHighlightColor() ||
				ShouldSerializeLeft() ||
				ShouldSerializeRight() ||
				ShouldSerializeMargin0() ||
				ShouldSerializeMargin1() ||
				ShouldSerializeMargin2() ||
				ShouldSerializeMargin3() ||
				ShouldSerializeMargin4();
		}

		public void Reset()
		{
			ResetMargin0();
			ResetMargin1();
			ResetMargin2();
			ResetMargin3();
			ResetMargin4();
		}

		#region Left
		public int Left
		{
			get
			{
				return NativeScintilla.GetMarginLeft();
			}
			set
			{
				NativeScintilla.SetMarginLeft(value);
			}
		}

		private bool ShouldSerializeLeft()
		{
			return Left != 1;
		}

		private void ResetLeft()
		{
			Left = 1;
		} 
		#endregion

		#region Right
		public int Right
		{
			get
			{
				return NativeScintilla.GetMarginRight();
			}
			set
			{
				NativeScintilla.SetMarginRight(value);
			}
		}

		private bool ShouldSerializeRight()
		{
			return Right != 1;
		}

		private void ResetRight()
		{
			Right = 1;
		}

		#endregion

		#region FoldMarginColor
		public Color FoldMarginColor
		{
			get
			{
				if (Scintilla.ColorBag.ContainsKey("Margins.FoldMarginColor"))
					return Scintilla.ColorBag["Margins.FoldMarginColor"];

				return Color.Transparent;
			}
			set
			{
				if (value == Color.Transparent)
					Scintilla.ColorBag.Remove("Margins.FoldMarginColor");
				else
					Scintilla.ColorBag["Margins.FoldMarginColor"] = value;


				NativeScintilla.SetFoldMarginColour(true, Utilities.ColorToRgb(value));

			}
		}

		private bool ShouldSerializeFoldMarginColor()
		{
			return FoldMarginColor != Color.Transparent;
		}

		private void ResetFoldMarginColor()
		{
			FoldMarginColor = Color.Transparent;
		}
		#endregion

		#region FoldMarginHighlightColor
		public Color FoldMarginHighlightColor
		{
			get
			{
				if (Scintilla.ColorBag.ContainsKey("Margins.FoldMarginHighlightColor"))
					return Scintilla.ColorBag["Margins.FoldMarginHighlightColor"];

				return Color.Transparent;
			}
			set
			{
				if (value == Color.Transparent)
					Scintilla.ColorBag.Remove("Margins.FoldMarginHighlightColor");
				else
					Scintilla.ColorBag["Margins.FoldMarginHighlightColor"] = value;


				NativeScintilla.SetFoldMarginColour(true, Utilities.ColorToRgb(value));

			}
		}

		private bool ShouldSerializeFoldMarginHighlightColor()
		{
			return FoldMarginHighlightColor != Color.Transparent;
		}

		private void ResetFoldMarginHighlightColor()
		{
			FoldMarginHighlightColor = Color.Transparent;
		}
		#endregion



		#region ICollection<Margin> Members

		public void Add(Margin item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void Clear()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool Contains(Margin item)
		{
			return true;
		}

		public void CopyTo(Margin[] array, int arrayIndex)
		{
			Array.Copy(ToArray(), 0, array, arrayIndex, 5);
		}

		public Margin[] ToArray()
		{
			return new Margin[]
			{
				_margin0,
				_margin1,
				_margin2,
				_margin3,
				_margin4
			};
		}

		[Browsable(false)]
		public int Count
		{
			get { return 5; }
		}

		[Browsable(false)]
		public bool IsReadOnly
		{
			get { return true; }
		}

		public bool Remove(Margin item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IEnumerable<Margin> Members

		public IEnumerator<Margin> GetEnumerator()
		{
			return new List<Margin>(ToArray()).GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return new List<Margin>(ToArray()).GetEnumerator();
		}

		#endregion
	}

}
