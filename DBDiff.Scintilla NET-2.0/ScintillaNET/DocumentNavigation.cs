using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
namespace DBDiff.Scintilla
{
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class DocumentNavigation : ScintillaHelperBase
	{
		bool _supressNext = false;
		private Timer t = null;

		internal DocumentNavigation(Scintilla scintilla) : base(scintilla) 
		{
			t = new Timer();
			t.Interval = _navigationPointTimeout;
			t.Tick += new EventHandler(t_Tick);
			scintilla.SelectionChanged += new EventHandler(scintilla_SelectionChanged);
		}

		internal bool ShouldSerialize()
		{
			return ShouldSerializeIsEnabled() || ShouldSerializeMaxHistorySize();
		}

		public void Reset()
		{
			_backwardStack.Clear();
			_forewardStack.Clear();
			ResetIsEnabled();
			ResetMaxHistorySize();
		}

		private void t_Tick(object sender, EventArgs e)
		{
			t.Enabled = false;
			int pos = NativeScintilla.GetCurrentPos();
			if ((_forewardStack.Count == 0 || _forewardStack.Current.Start != pos) && (_backwardStack.Count == 0 || _backwardStack.Current.Start != pos))
				_backwardStack.Push(newRange(pos));
		}

		private void scintilla_SelectionChanged(object sender, EventArgs e)
		{
			if (!_isEnabled)
				return;

			if (!_supressNext)
			{
				t.Enabled = false;
				t.Enabled = true;
			}
			else
			{
				_supressNext = false;
			}
		}

		#region IsEnabled
		private bool _isEnabled = true;
		public bool IsEnabled
		{
			get
			{
				return _isEnabled;
			}
			set
			{
				_isEnabled = value;
			}
		}
		private bool ShouldSerializeIsEnabled()
		{
			return !_isEnabled;
		}

		private void ResetIsEnabled()
		{
			_isEnabled = true;
		} 
		#endregion

		#region MaxHistorySize
		private int _maxHistorySize = 50;
		public int MaxHistorySize
		{
			get
			{
				return _maxHistorySize;
			}
			set
			{
				_maxHistorySize = value;
				_backwardStack.MaxCount = value;
				_forewardStack.MaxCount = value;
			}
		}

		private bool ShouldSerializeMaxHistorySize()
		{
			return _maxHistorySize != 50;
		}

		private void ResetMaxHistorySize()
		{
			_maxHistorySize = 50;
		}

		#endregion

		#region BackwardStack
		public FakeStack _backwardStack = new FakeStack();
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FakeStack BackwardStack
		{
			get
			{
				return _backwardStack;
			}
			set
			{
				_backwardStack = value;
			}
		} 
		#endregion

		#region ForewardStack
		public FakeStack _forewardStack = new FakeStack();
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FakeStack ForewardStack
		{
			get
			{
				return _forewardStack;
			}
			set
			{
				_forewardStack = value;
			}
		} 
		#endregion

		#region NavigationPointTimeout
		private int _navigationPointTimeout = 200;
		public int NavigationPointTimeout
		{
			get
			{
				return _navigationPointTimeout;
			}
			set
			{
				_navigationPointTimeout = value;
			}
		}

		private bool ShouldSerializeNavigationPointTimeout()
		{
			return _navigationPointTimeout != 200;
		}

		private void ResetNavigationPointTimeout()
		{
			_navigationPointTimeout = 200;
		} 
		#endregion

		public void NavigateBackward()
		{
			if (_backwardStack.Count == 0)
				return;

			int currentPos = Scintilla.Caret.Position;
			if (currentPos == _backwardStack.Current.Start && _backwardStack.Count == 1)
				return;

			int pos = _backwardStack.Pop().Start;

			if (pos != currentPos)
			{
				_forewardStack.Push(newRange(currentPos));
				Scintilla.Caret.Goto(pos);
			}
			else
			{
				_forewardStack.Push(newRange(pos));
				Scintilla.Caret.Goto(_backwardStack.Current.Start);
			}

			_supressNext = true;
		}

		public void NavigateForward()
		{
			if (!CanNavigateForward)
				return;

			int pos = _forewardStack.Pop().Start;
			_backwardStack.Push(newRange(pos));
			Scintilla.Caret.Goto(pos);

			_supressNext = true;
		}

		[Browsable(false)]
		public bool CanNavigateBackward
		{
			get
            {
				if (_backwardStack.Count == 0 || (NativeScintilla.GetCurrentPos() == _backwardStack.Current.Start && _backwardStack.Count == 1))
					return false;

				return true;
            }
		}

		[Browsable(false)]
		public bool CanNavigateForward
		{
			get
			{
				return _forewardStack.Count > 0;
			}
		}

		private NavigationPont newRange(int pos)
		{
			NavigationPont mr = new NavigationPont(pos, Scintilla);
			Scintilla.ManagedRanges.Add(mr);
			return mr;
		}

		public class FakeStack : List<NavigationPont>
		{
			private int _maxCount = 50;
            public int MaxCount
            {
            	get 
            	{
            		return _maxCount; 
            	}
            	set
            	{
            		_maxCount = value;
            	}
            }

			public NavigationPont Pop()
			{
				NavigationPont ret = this[Count - 1];
				RemoveAt(Count - 1);
				return ret;
			}

			public void Push(NavigationPont value)
			{
				Add(value);
				if (Count > MaxCount)
					RemoveAt(0);
			}

			public NavigationPont Current
			{
				get
				{
					return this[Count - 1];
				}
			}
		}

		public class NavigationPont : ManagedRange
		{
			/// <summary>
			/// Initializes a new instance of the NavigationPont class.
			/// </summary>
			public NavigationPont(int pos, Scintilla scintilla) : base(pos, pos, scintilla)
			{
			}
			public override void Dispose()
			{
				Scintilla.DocumentNavigation.ForewardStack.Remove(this);
				Scintilla.DocumentNavigation.BackwardStack.Remove(this);

				base.Dispose();
			}
		}
	}
}
