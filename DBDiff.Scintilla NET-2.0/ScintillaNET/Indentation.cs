using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ScintillaNet
{
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class Indentation : ScintillaHelperBase
	{
		internal Indentation(Scintilla scintilla) : base(scintilla) { }

		internal bool ShouldSerialize()
		{
			return ShouldSerializeBackspaceUnindents() ||
				ShouldSerializeIndentWidth() ||
				ShouldSerializeShowGuides() || 
				ShouldSerializeTabIndents() ||
				ShouldSerializeTabWidth() ||
				ShouldSerializeUseTabs();
		}
		
		#region TabWidth
		public int TabWidth
		{
			get
			{
				return NativeScintilla.GetTabWidth();
			}
			set
			{
				NativeScintilla.SetTabWidth(value);
			}
		}

		private bool ShouldSerializeTabWidth()
		{
			return TabWidth != 8;
		}

		private void ResetTabWidth()
		{
			TabWidth = 8;
		}
		#endregion

		#region UseTabs
		public bool UseTabs
		{
			get
			{
				return NativeScintilla.GetUseTabs();
			}
			set
			{
				NativeScintilla.SetUseTabs(value);
			}
		}

		private bool ShouldSerializeUseTabs()
		{
			return !UseTabs;
		}

		private void ResetUseTabs()
		{
			UseTabs = true;
		} 
		#endregion

		#region IndentWidth
		public int IndentWidth
		{
			get
			{
				return NativeScintilla.GetIndent();
			}
			set
            {
				NativeScintilla.SetIndent(value);
            }
		}

		private bool ShouldSerializeIndentWidth()
		{
			return IndentWidth != 0;
		}

		private void ResetIndentWidth()
		{
			IndentWidth = 0;
		}

		#endregion	

		#region TabIndents
		public bool TabIndents
		{
			get
			{
				return NativeScintilla.GetTabIndents();
			}
			set
            {
				NativeScintilla.SetTabIndents(value);
            }
		}

		private bool ShouldSerializeTabIndents()
		{
			return !TabIndents;
		}

		private void ResetTabIndents()
		{
			TabIndents = false;
		}
		#endregion

		#region BackspaceUnindents
		public bool BackspaceUnindents
		{
			get
			{
				return NativeScintilla.GetBackSpaceUnIndents();
			}
			set
			{
				NativeScintilla.SetBackSpaceUnIndents(value);
			}
		}

		private bool ShouldSerializeBackspaceUnindents()
		{
			return BackspaceUnindents;

		}

		private void ResetBackspaceUnindents()
		{
			BackspaceUnindents = false;
		} 
		#endregion

		#region ShowGuides
		public bool ShowGuides
		{
			get
			{
				return NativeScintilla.GetIndentationGuides();
			}
			set
			{
				NativeScintilla.SetIndentationGuides(value);
			}
		}

		private bool ShouldSerializeShowGuides()
		{
			return ShowGuides;
		}

		private void ResetShowGuides()
		{
			ShowGuides = false;
		} 
		#endregion


		#region Smart indenting support
		/// <summary>
		/// Enables the Smart Indenter so that On enter, it indents the next line.
		/// </summary>
		private SmartIndent _smartIndentType = SmartIndent.None;
		public SmartIndent SmartIndentType
		{
			get { return _smartIndentType; }
			set
			{
				_smartIndentType = value;
			}
		}

		private bool ShouldSerializeSmartIndentType()
		{
			return _smartIndentType != SmartIndent.None;
		}

		private void ResetSmartIndentType()
		{
			_smartIndentType = SmartIndent.None;
		}



		/// <summary>
		/// If Smart Indenting is enabled, this delegate will be added to the CharAdded multicast event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void CheckSmartIndent(char ch)
		{
			char newline = (Scintilla.EndOfLine.Mode == EndOfLineMode.CR) ? '\r' : '\n';

			switch (SmartIndentType)
			{
				case SmartIndent.None:
					return;
				case SmartIndent.Simple:
					if (ch == newline)
					{
						Line curLine = Scintilla.Lines.Current;
						curLine.Indentation = curLine.Previous.Indentation;
						Scintilla.CurrentPos = curLine.IndentPosition;
					}
					break;
				case SmartIndent.CPP:
				case SmartIndent.CPP2:
					if (ch == newline)
					{
						Line curLine = Scintilla.Lines.Current;
						Line tempLine = curLine;
						int previousIndent;
						string tempText;

						do
						{
							tempLine = tempLine.Previous;
							previousIndent = tempLine.Indentation;
							tempText = tempLine.Text.Trim();
							if (tempText.Length == 0) previousIndent = -1;
						}
						while ((tempLine.Number > 1) && (previousIndent < 0));

						if (tempText.EndsWith("{"))
						{
							int bracePos = Scintilla.CurrentPos - 1;
							while (bracePos > 0 && Scintilla.CharAt(bracePos) != '{') bracePos--;
							if (bracePos > 0 && Scintilla.Styles.GetStyleAt(bracePos) == 10)
								previousIndent += TabWidth;
						}
						curLine.Indentation = previousIndent;
						Scintilla.CurrentPos =  curLine.IndentPosition;
					}
					else if (ch == '}')
					{
						int position = Scintilla.CurrentPos;
						Line curLine = Scintilla.Lines.Current;
						int previousIndent = curLine.Previous.Indentation;
						int match = Scintilla.SafeBraceMatch(position - 1);
						if (match != -1)
						{
							previousIndent = Scintilla.Lines.FromPosition(match).Indentation;
							curLine.Indentation =  previousIndent;
						}
					}
					break;
			}
		}

		/// <summary>
		/// For Custom Smart Indenting, assign a handler to this delegate property.
		/// </summary>
		public EventHandler<CharAddedEventArgs> SmartIndentCustomAction;

		/// <summary>
		/// Smart Indenting helper method
		/// </summary>
		/// <param name="line"></param>
		/// <param name="indent"></param>
		private void IndentLine(int line, int indent)
		{
			if (indent < 0)
			{
				return;
			}

			int selStart = Scintilla.Selection.Start;
			int selEnd = Scintilla.Selection.End;

			Line l = Scintilla.Lines[line];
			int posBefore = l.IndentPosition;
			l.Indentation = indent;

			int posAfter = l.IndentPosition;
			int posDifference = posAfter - posBefore;

			if (posAfter > posBefore)
			{
				// Move selection on
				if (selStart >= posBefore)
				{
					selStart += posDifference;
				}

				if (selEnd >= posBefore)
				{
					selEnd += posDifference;
				}
			}
			else if (posAfter < posBefore)
			{
				// Move selection back
				if (selStart >= posAfter)
				{
					if (selStart >= posBefore)
						selStart += posDifference;
					else
						selStart = posAfter;
				}
				if (selEnd >= posAfter)
				{
					if (selEnd >= posBefore)
						selEnd += posDifference;
					else
						selEnd = posAfter;
				}
			}

			Scintilla.Selection.Start = selStart;
			Scintilla.Selection.End = selEnd;
		}
		#endregion
	}
}
