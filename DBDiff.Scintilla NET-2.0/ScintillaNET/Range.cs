using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace ScintillaNet
{
	/// <summary>
	/// A range within the editor. Start and End are both Positions.
	/// </summary>
	public class Range : ScintillaHelperBase, IComparable
	{
		public bool Collapsed
		{
			get { return _start == _end; }
		}
		private int _end;
		public virtual int End
		{
			get
			{
				return _end;
			}
			set
			{
				_end = value;
			}
		}

		private int _start;
		public virtual int Start
        {
        	get
        	{
        		return _start;
        	}
        	set
        	{
        		_start = value;
        	}
        }

		
		protected internal Range() : base(null) { }

		public Range(int start, int end, Scintilla scintilla) : base(scintilla)
		{
			if(start < end)
			{
				_start	= start;
				_end	= end;
			}
			else
			{
				_start	= end;
				_end	= start;
			}
		}

		public int Length
		{
			get
			{
				return _end - _start;
			}
		}

		public string Text
		{
			get
			{
				if(Start < 0 || End < 0 || Scintilla == null)
					return string.Empty;

				TextRange rng	= new TextRange();
				rng.lpstrText	= Marshal.AllocHGlobal(Length);
				rng.chrg.cpMin	= _start;
				rng.chrg.cpMax	= _end;
				int len = (int)NativeScintilla.GetTextRange(ref rng);

				string ret = Utilities.PtrToStringUtf8(rng.lpstrText, len);
				Marshal.FreeHGlobal(rng.lpstrText);
				return ret;

			}
			set
			{
				NativeScintilla.SetTargetStart(_start);
				NativeScintilla.SetTargetEnd(_end);
				NativeScintilla.ReplaceTarget(-1, value);
			}
		}

		public byte[] StyledText
		{
			get
			{
				if(Start < 0 || End < 0 || Scintilla == null)
					return new byte[0];

				int bufferLength	= (Length * 2) + 2;
				TextRange rng		= new TextRange();
				rng.lpstrText		= Marshal.AllocHGlobal(bufferLength);
				rng.chrg.cpMin		= _start;
				rng.chrg.cpMax		= _end;
				
				NativeScintilla.GetStyledText(ref rng);

				byte[] ret = new byte[bufferLength];
					Marshal.Copy(rng.lpstrText, ret, 0, bufferLength);

				Marshal.FreeHGlobal(rng.lpstrText);
				return ret;
			}
		}


		public void Copy() 
		{
			Copy(CopyFormat.Text);
		}

		public void Copy(CopyFormat format)
		{
			if(format == CopyFormat.Text)
			{
				NativeScintilla.CopyRange(_start, _end);
			}
			else if(format == CopyFormat.Rtf)
			{
				throw new NotImplementedException("Someday...");
			}
			else
			{

			}
		}

		public void Select()
		{
			NativeScintilla.SetSel(_start, _end);
		}

		public void GotoStart()
		{
			NativeScintilla.GotoPos(_start);
		}

		public void GotoEnd()
		{
			NativeScintilla.GotoPos(_end);
		}

		public bool PositionInRange(int position)
		{
			return position >= _start && position <= _end;
		}

		public bool IntersectsWith(Range otherRange)
		{
			return otherRange.PositionInRange(_start) | otherRange.PositionInRange(_end) | PositionInRange(otherRange.Start) | PositionInRange(otherRange.End);
		}

		public void SetStyle(int style)
		{
			SetStyle(0xff, style);
		}

		public void SetStyle(byte styleMask, int style)
		{
			NativeScintilla.StartStyling(_start, (int)styleMask);
			NativeScintilla.SetStyling(Length, style);
		}


		//	Chris Rickard 7/10/2007
		//	Woo hoo! Modern Indicator support. We won't even
		//	mess with legacy indicators as they'll be removed
		//	from Scintilla Someday
		public void SetIndicator(int indicator)
		{
			NativeScintilla.SetIndicatorCurrent(indicator);
			NativeScintilla.IndicatorFillRange(_start, Length);
		}
		
		//	Now the Scintilla documentation is a little unclear to me,
		//	but it seems as though the whole indicator value doesn't 
		//	really do anything yet, but may in the future.
		public void SetIndicator(int indicator, int value)
		{
			NativeScintilla.SetIndicatorValue(value);
			NativeScintilla.SetIndicatorCurrent(indicator);
			NativeScintilla.IndicatorFillRange(_start, Length);
		}

		public void ClearIndicator(int indicator)
		{
			NativeScintilla.SetIndicatorCurrent(indicator);
			NativeScintilla.IndicatorClearRange(_start, Length);
		}

		public void ClearIndicator(Indicator indicator)
		{
			NativeScintilla.SetIndicatorCurrent(indicator.Number);
			NativeScintilla.IndicatorClearRange(_start, Length);
		}

		public override bool Equals(object obj)
		{
			Range r = obj as Range;
			if(r == null)
				return false;

			return r._start == _start && r._end == _end;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#region StartingLine
		public Line StartingLine
		{
			get
			{
				return new Line(Scintilla, startingLine);
			}
		}

		private int startingLine
		{
			get
			{
				return NativeScintilla.LineFromPosition(_start);
			}
		} 
		#endregion

		#region EndingLine
		private int endingLine
		{
			get
			{
				return NativeScintilla.LineFromPosition(_end);
			}
		}

		public Line EndingLine
		{
			get
			{
				return new Line(Scintilla, endingLine);
			}
		} 
		#endregion

		public bool IsMultiLine
		{
			get
			{
				return !StartingLine.Equals(EndingLine);
			}
		}

		public void HideLines()
		{
			NativeScintilla.HideLines(startingLine, endingLine);
		}

		public void ShowLines()
		{
			NativeScintilla.ShowLines(startingLine, endingLine);
		}

		public void SplitLines(int pixelWidth)
		{
			NativeScintilla.SetTargetStart(_start);
			NativeScintilla.SetTargetEnd(_end);
			NativeScintilla.LinesSplit(pixelWidth);
		}

		public void SplitLines()
		{
			SplitLines(0);
		}

		public void JoinLines()
		{
			NativeScintilla.SetTargetStart(_start);
			NativeScintilla.SetTargetEnd(_end);
			NativeScintilla.LinesJoin();
		}

		public void Colorize()
		{
			NativeScintilla.Colourise(_start, _end);
		}

		/// <summary>
		/// Removes trailing spaces from each line
		/// </summary>
		public void StripTrailingSpaces()
		{
			NativeScintilla.BeginUndoAction();

			for (int line = startingLine; line < endingLine; line++)
			{
				int lineStart = NativeScintilla.PositionFromLine(line);
				int lineEnd = NativeScintilla.GetLineEndPosition(line);
				int i = lineEnd - 1;
				char ch = NativeScintilla.GetCharAt(i);
				while ((i >= lineStart) && ((ch == ' ') || (ch == '\t')))
				{
					i--;
					ch = NativeScintilla.GetCharAt(i);
				}
				if (i == lineStart - 1)
				{
					ch = NativeScintilla.GetCharAt(i + 1);
					while (i < lineEnd && ch == '\t')
					{
						i++;
						ch = NativeScintilla.GetCharAt(i + 1);
					}
				}
				if (i < (lineEnd - 1))
				{
					NativeScintilla.SetTargetStart(i + 1);
					NativeScintilla.SetTargetEnd(lineEnd);
					NativeScintilla.ReplaceTarget(0, string.Empty);
				}
			}
			NativeScintilla.EndUndoAction();
		}


		/// <summary>
		/// Expands all folds
		/// </summary>
		public void ExpandAllFolds()
		{
			for (int i = startingLine; i < endingLine; i++)
			{
				NativeScintilla.SetFoldExpanded(i, true);
				NativeScintilla.ShowLines(i + 1, i + 1);
			}
		}

		/// <summary>
		/// Collapses all folds
		/// </summary>
		public void CollapseAllFolds()
		{
			for (int i = startingLine; i < endingLine; i++)
			{
				int maxSubOrd = NativeScintilla.GetLastChild(i, -1);
				NativeScintilla.SetFoldExpanded(i, false);
				NativeScintilla.HideLines(i + 1, maxSubOrd);
			}
		}


		public override string ToString()
		{
		
			return "{Start=" + _start + ", End=" + _end + ", Length=" + Length + "}";
		}

		#region IComparable Members

		public int CompareTo(object otherObj)
		{
			Range other = otherObj as Range;

			if(other == null)
				return 1;

			if(other._start < _start)
				return 1;
			else if(other._start > _start)
				return -1;

			//	Starts must equal, lets try ends
			if(other._end < _end)
				return 1;
			else if(other._end > _end)
				return -1;

			//	Start and End equal. Comparitavely the same
			return 0;			
		}
		#endregion
	}

	public class ManagedRange : Range, IDisposable
	{
		internal bool PendingDeletion = false;
		public virtual void Change(int newStart, int newEnd)
		{
			base.Start	= newStart;
			base.End	= newEnd;
		}

		//	This is important for determining how to treat 
		//	managed ranges during insert and delete events.
		public virtual bool IsPoint { get { return false; } }

		public override int Start
		{
			get
			{
				return base.Start;
			}
			set
			{
				base.Start = value; 
			}
		}

		public override int End
		{
			get
			{
				return base.End;
			}
			set
			{
				base.End = value;
			}
		}

		protected internal ManagedRange() { }

		protected internal ManagedRange(Range range) : this(range.Start, range.End, range.Scintilla)
		{
			
		}

		public ManagedRange(int start, int end, Scintilla scintilla) : base(start, end, scintilla)
		{
			
		}
		
		protected internal virtual void Paint(Graphics g)
		{

		}
		

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#region IDisposable Members
		public override void Dispose()
		{
			Scintilla.ManagedRanges.Remove(this);
			Scintilla = null;
			IsDisposed = true;
		}
		#endregion
	}
}
