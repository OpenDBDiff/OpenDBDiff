using System;
using System.Collections;
using System.Text;
using System.Drawing;

namespace ScintillaNet
{
	public class LinesCollection : ScintillaHelperBase, ICollection
	{
		protected internal LinesCollection(Scintilla scintilla) : base(scintilla) { }

		public Line this[int index]
		{
			get
			{
				return new Line(Scintilla, index);
			}
		}

		public Line Current
		{
			get
			{
				return this[Scintilla.Caret.LineNumber];
			}
		}

		public Line FirstVisible
		{
			get
			{
				return this[NativeScintilla.GetFirstVisibleLine()];
			}
		}

		public int VisibleCount
		{
			get
			{
				return NativeScintilla.LinesOnScreen();
			}
		}

		public Line[] VisibleLines
		{
			get
			{
				Line[] ret = new Line[VisibleCount];

				int min = NativeScintilla.GetFirstVisibleLine();
				int max = min + VisibleCount;
				for (int i = min; i < max; i++)
					ret[i] = FromVisibleLineNumber(i);

				return ret;
			}
		}

		public Line FromPosition(int position)
		{
			return this[NativeScintilla.LineFromPosition(position)];
		}

		public Line FromVisibleLineNumber(int displayLine)
		{
			return new Line(Scintilla, NativeScintilla.DocLineFromVisible(displayLine));
		}

		public Line GetMaxLineWithState()
		{
			int line = NativeScintilla.GetMaxLineState();
			if (line < 0)
				return null;

			return this[line];
		}

		public void Hide(int startLine, int endLine)
		{
			NativeScintilla.HideLines(startLine, endLine);
		}

		private void Show(int startLine, int endLine)
		{
			NativeScintilla.ShowLines(startLine, endLine);
		}

		#region ICollection Members

		public void CopyTo(Array array, int index)
		{
			if(array == null)
				throw new ArgumentNullException("array");

			if(index < 0)
				throw new ArgumentOutOfRangeException("index");

			if(index >= array.Length)
				throw new ArgumentException("index is equal to or greater than the length of array.");

			int count = Count;
			if(count > array.Length - index)
				throw new ArgumentException("The number of elements in the source ICollection is greater than the available space from number to the end of the destination array.");

			for(int i=index; i<count; i++)
				array.SetValue(this[i], i);
		}

		public int Count
		{
			get { return NativeScintilla.GetLineCount(); }
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public object SyncRoot
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new LinesEnumerator(this);
		}

		#endregion

		#region LinesEnumerator
		private class LinesEnumerator : IEnumerator
		{
			private LinesCollection _lines;
			private int _index = -1;
			private int _count;

			public LinesEnumerator(LinesCollection lines)
			{
				_lines = lines;
				_count = lines.Count;
			}

			#region IEnumerator Members

			public object Current
			{
				get { return _lines[_index]; }
			}

			public bool MoveNext()
			{
				if(++_index >= _count)
					return false;

				return true;
			}

			public void Reset()
			{
				_index = -1;
			}

			#endregion
		}
		#endregion
	}

}
