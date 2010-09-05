using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
namespace DBDiff.Scintilla
{
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class CallTip : ScintillaHelperBase
	{
		internal CallTip(Scintilla scintilla) : base(scintilla)
		{
			//	Go ahead and enable this. It's all pretty idiosyncratic IMO. For one
			//	thing you can't turn it off. We set the CallTip styles by default
			//	anyhow.
			NativeScintilla.CallTipUseStyle(10);
			Scintilla.BeginInvoke(new MethodInvoker(delegate() 
			{
				HighlightTextColor = HighlightTextColor;
				ForeColor = ForeColor;
				BackColor = BackColor;
			}));
		}

		internal bool ShouldSerialize()
		{
			return ShouldSerializeBackColor() ||
				ShouldSerializeForeColor() ||
				ShouldSerializeHighlightEnd() ||
				ShouldSerializeHighlightStart() ||
				ShouldSerializeHighlightTextColor();
		}

		private int _lastPos = -1;

		private OverloadList _overloadList = null;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public OverloadList OverloadList
		{
			get
			{
				return _overloadList;
			}
			set
			{
				_overloadList = value;
			}
		}

		private string _message = null;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Message
        {
        	get 
        	{
        		return _message; 
        	}
        	set
        	{
        		_message = value;
        	}
        }

		#region ShowOverload
		public void ShowOverload(OverloadList overloadList, int position, uint startIndex, int highlightStart, int highlightEnd)
		{
			_lastPos = position;
			_overloadList = overloadList;
			unchecked
			{
				_overloadList.CurrentIndex = (int)startIndex;
			}
			_highlightEnd = highlightEnd;
			_highlightStart = highlightStart;
			ShowOverloadInternal();
		}

		public void ShowOverload(OverloadList overloadList, int position)
		{
			ShowOverload(overloadList, position, 0, -1, -1);
		}

		public void ShowOverload(OverloadList overloadList, int position, int highlightStart, int highlightEnd)
		{
			ShowOverload(overloadList, position, 0, highlightStart, highlightEnd);
		}

		public void ShowOverload(OverloadList overloadList, uint startIndex)
		{
			ShowOverload(overloadList, -1, startIndex, -1, -1);
		}

		public void ShowOverload(OverloadList overloadList, uint startIndex, int highlightStart, int highlightEnd)
		{
			ShowOverload(overloadList, -1, startIndex, highlightStart, highlightEnd);
		}

		public void ShowOverload(OverloadList overloadList)
		{
			ShowOverload(overloadList, -1, 0, -1, -1);
		}

		public void ShowOverload(OverloadList overloadList, int highlightStart, int highlightEnd)
		{
			ShowOverload(overloadList, -1, 0, highlightStart, highlightEnd);
		}

		public void ShowOverload(int position, uint startIndex)
		{
			ShowOverload(_overloadList, position, startIndex, -1, -1);
		}

		public void ShowOverload(int position, uint startIndex, int highlightStart, int highlightEnd)
		{
			ShowOverload(_overloadList, position, startIndex, highlightStart, highlightEnd);
		}

		public void ShowOverload(int position)
		{
			ShowOverload(_overloadList, position, 0, -1, -1);
		}
		
		public void ShowOverload(int position, int highlightStart, int highlightEnd)
		{
			ShowOverload(_overloadList, position, 0, highlightStart, highlightEnd);
		}

		public void ShowOverload(uint startIndex)
		{
			ShowOverload(_overloadList, -1, startIndex, -1, -1);
		}

		public void ShowOverload(uint startIndex, int highlightStart, int highlightEnd)
		{
			ShowOverload(_overloadList, -1, startIndex, highlightStart, highlightEnd);
		}

		public void ShowOverload(int highlightStart, int highlightEnd)
		{
			ShowOverload(_overloadList, -1, 0, highlightStart, highlightEnd);
		}

		public void ShowOverload()
		{
			ShowOverload(_overloadList, -1, 0, -1, -1);
		}

		#endregion


		#region Show
		public void Show(string message, int position, int highlightStart, int higlightEnd)
		{
			_lastPos = position;
			if (position < 0)
				position = NativeScintilla.GetCurrentPos();				

			_overloadList = null;
			_message = message;
			NativeScintilla.CallTipShow(position, message);
			HighlightStart = highlightStart;
			HighlightEnd = HighlightEnd;
		}

		public void Show(string message)
		{
			Show(message, -1, -1, -1);
		}

		public void Show(string message, int highlightStart, int higlightEnd)
		{
			Show(message, -1, highlightStart, higlightEnd);
		}

		public void Show(string message, int position)
		{
			Show(message, position, -1, -1);
		}

		public void Show(int position)
		{
			Show(_message, position, -1, -1);
		}

		public void Show(int position, int highlightStart, int higlightEnd)
		{
			Show(_message, position, highlightStart, higlightEnd);
		}

		public void Show(int highlightStart, int higlightEnd)
		{
			Show(_message, -1, highlightStart, higlightEnd);
		}

		public void Show()
		{
			Show(_message, -1, -1, -1);
		}

		#endregion



		internal void ShowOverloadInternal()
		{
			int pos = _lastPos;
			if (pos < 0)
				pos = NativeScintilla.GetCurrentPos();

			string s = "\u0001 {1} of {2} \u0002 {0}"; 
			s = string.Format(s, _overloadList.Current, _overloadList.CurrentIndex + 1, _overloadList.Count);
			NativeScintilla.CallTipCancel(); 
			NativeScintilla.CallTipShow(pos, s);
			NativeScintilla.CallTipSetHlt(_highlightStart, _highlightEnd);
		}

		public void Cancel()
		{
			NativeScintilla.CallTipCancel();
		}

		#region ForeColor
		public Color ForeColor
		{
			get
			{
				if (Scintilla.ColorBag.ContainsKey("CallTip.ForeColor"))
					return Scintilla.ColorBag["CallTip.ForeColor"];

				return SystemColors.InfoText;
			}
			set
			{
				SetForeColorInternal(value);

				Scintilla.Styles.CallTip.SetForeColorInternal(value);
			}
		}

		internal void SetForeColorInternal(Color value)
		{
			if (value == SystemColors.InfoText)
				Scintilla.ColorBag.Remove("CallTip.ForeColor");
			else
				Scintilla.ColorBag["CallTip.ForeColor"] = value;

			NativeScintilla.CallTipSetFore(Utilities.ColorToRgb(value));
		}

		private bool ShouldSerializeForeColor()
		{
			return ForeColor != SystemColors.InfoText;
		}

		private void ResetForeColor()
		{
			ForeColor = SystemColors.InfoText;
		}
		#endregion

		#region BackColor
		public Color BackColor
		{
			get
			{
				if (Scintilla.ColorBag.ContainsKey("CallTip.BackColor"))
					return Scintilla.ColorBag["CallTip.BackColor"];

				return SystemColors.Info;
			}
			set
			{
				SetBackColorInternal(value);

				Scintilla.Styles.CallTip.SetBackColorInternal(value);
			}
		}

		internal void SetBackColorInternal(Color value)
		{
			if (value == SystemColors.Info)
				Scintilla.ColorBag.Remove("CallTip.BackColor");
			else
				Scintilla.ColorBag["CallTip.BackColor"] = value;
			
			NativeScintilla.CallTipSetBack(Utilities.ColorToRgb(value));
		}
		private bool ShouldSerializeBackColor()
		{
			return BackColor != SystemColors.Info;
		}

		private void ResetBackColor()
		{
			BackColor = SystemColors.Info;
		}
		#endregion

		#region HighlightTextColor
		public Color HighlightTextColor
		{
			//	Note the default Color of this is SystemColors.Highlight, instead
			//	of HighlightText, which one would normally think. However since 
			//	there is no Contrasting HighlightBackColor a light Highlight Color
			//	on the light InfoTip background is nearly impossible to see.
			get
			{
				if (Scintilla.ColorBag.ContainsKey("CallTip.HighlightTextColor"))
					return Scintilla.ColorBag["CallTip.HighlightTextColor"];

				return SystemColors.Highlight;
			}
			set
			{
				if (value == SystemColors.Highlight)
					Scintilla.ColorBag.Remove("CallTip.HighlightTextColor");
				else
					Scintilla.ColorBag["CallTip.HighlightTextColor"] = value;

				NativeScintilla.CallTipSetForeHlt(Utilities.ColorToRgb(value));
			}
		}

		private bool ShouldSerializeHighlightTextColor()
		{
			return HighlightTextColor != SystemColors.Highlight;
		}

		private void ResetHighlightTextColor()
		{
			HighlightTextColor = SystemColors.Highlight;
		}
		#endregion

		#region HighlightStart
		private int _highlightStart = -1;
		public int HighlightStart
		{
			get
			{
				return _highlightStart;
			}
			set
			{
				_highlightStart = value;
				NativeScintilla.CallTipSetHlt(_highlightStart, _highlightStart);
			}
		}

		private bool ShouldSerializeHighlightStart()
		{
			return _highlightStart >= 0;
		}

		private void ResetHighlightStart()
		{
			_highlightStart = -1;
		} 
		#endregion

		#region HighlightEnd
		private int _highlightEnd = -1;
		public int HighlightEnd
		{
			get
			{
				return _highlightEnd;
			}
			set
			{
				_highlightEnd = value;
				NativeScintilla.CallTipSetHlt(_highlightStart, _highlightEnd);
			}
		}

		private bool ShouldSerializeHighlightEnd()
		{
			return _highlightEnd >= 0;
		}

		private void ResetHighlightEnd()
		{
			_highlightEnd = -1;
		} 
		#endregion

		#region IsActive
		public bool IsActive
		{
			get
			{
				return NativeScintilla.CallTipActive();
			}
		}
		#endregion

		#region Hide
		public void Hide()
		{
			NativeScintilla.CallTipCancel();
		}
		#endregion


		public const char UpArrow = '\u0001';
		public const char DownArrow = '\u0002';
	}

	public class OverloadList : List<string>
	{
		private int _currentIndex;
		public int CurrentIndex
		{
			get
			{
				return _currentIndex;
			}
			internal set
			{
				_currentIndex = value;
			}
		}

		public string Current
		{
			get
			{
				return this[_currentIndex];
			}
			set
			{
				_currentIndex = this.IndexOf(value);
			}
		}

		public OverloadList() : base(){ }
		public OverloadList(IEnumerable<string> collection) : base(collection) { }
		public OverloadList(int capacity) : base(capacity) { }
	}
}
