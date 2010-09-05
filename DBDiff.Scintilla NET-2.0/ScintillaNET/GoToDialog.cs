using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ScintillaNet
{
	public partial class GoToDialog : Form
	{
		private Scintilla _scintilla;
		public Scintilla Scintilla
		{
			get { return _scintilla; }
			set { _scintilla = value; }
		}

		private int _currentLineNumber;
		public int CurrentLineNumber
		{
			get { return _currentLineNumber; }
			set
			{
				_currentLineNumber = value;
			}
		}

		private int _maximumLineNumber;
		public int MaximumLineNumber
		{
			get { return _maximumLineNumber; }
			set
			{
				_maximumLineNumber = value;
			}
		}

		private int _gotoLineNumber;
		public int GotoLineNumber
		{
			get { return _gotoLineNumber; }
			set
			{
				_gotoLineNumber = value;
			}
		}
        
		public GoToDialog()
		{
			InitializeComponent();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (int.TryParse(txtGotoLine.Text, out _gotoLineNumber))
			{
				//	Line #s are 0 based but the users don't think that way
				_gotoLineNumber--;
				if (_gotoLineNumber < 0 || _gotoLineNumber >= _maximumLineNumber)
					err.SetError(txtGotoLine, "Go to line # must be greater than 0 and less than " + (_maximumLineNumber + 1).ToString());
				else
					DialogResult = DialogResult.OK;
			}
			else
			{
				err.SetError(txtGotoLine, "Go to line # must be a numeric value");
			}			
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			moveFormAwayFromSelection();
		}

		private void GoToDialog_Load(object sender, EventArgs e)
		{
			string displayLine = (_currentLineNumber + 1).ToString();

			txtCurrentLine.Text = displayLine;
			txtMaxLine.Text = _maximumLineNumber.ToString();
			txtGotoLine.Text = displayLine;

			txtGotoLine.Select();
		}

		//	This was taken from FindReplaceDialog. Obviously some refactoring is called for
		//	since we have common code. However I'm holding off on this because I'm coming
		//	up with some other ideas for the FindReplaceDialog. Right now every scintilla
		//	gets its own FindReplaceDialog, but they really need to be sharable across 
		//	multiple scintillas much like how DropMarkers work.

		private void moveFormAwayFromSelection()
		{
			if (!Visible)
				return;

			int pos = Scintilla.Caret.Position;
			int x = Scintilla.PointXFromPosition(pos);
			int y = Scintilla.PointYFromPosition(pos);

			Point cursorPoint = Scintilla.PointToScreen(new Point(x, y));

			Rectangle r = new Rectangle(Location, Size);
			if (r.Contains(cursorPoint))
			{
				Point newLocation;
				if (cursorPoint.Y < (Screen.PrimaryScreen.Bounds.Height / 2))
				{
					// Top half of the screen
					newLocation = Scintilla.PointToClient(
						new Point(Location.X, cursorPoint.Y + Scintilla.Lines.Current.Height * 2)
						);
				}
				else
				{
					// Bottom half of the screen
					newLocation = Scintilla.PointToClient(
						new Point(Location.X, cursorPoint.Y - Height - (Scintilla.Lines.Current.Height * 2))
						);
				}
				newLocation = Scintilla.PointToScreen(newLocation);
				Location = newLocation;
			}
		}
	}
}

