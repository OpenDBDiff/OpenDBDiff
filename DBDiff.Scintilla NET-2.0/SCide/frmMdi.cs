using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI;
using WeifenLuo.WinFormsUI.Docking;

using ScintillaNet;

namespace SCide
{
	public partial class frmMdi : Form
	{
		public int _newDocumentCount = 0;
		public frmDocument ActiveDocument
		{
			get
			{
				return dockPanel.ActiveDocument as frmDocument;
			}
		}

		public frmMdi()
		{
			//	I personally really dislike the OfficeXP look on Windows XP with the blue.
			ToolStripManager.RenderMode = ToolStripManagerRenderMode.Professional;
			((ToolStripProfessionalRenderer)ToolStripManager.Renderer).ColorTable.UseSystemColors = true;

			InitializeComponent();
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewDocument();
		}

		public frmDocument NewDocument()
		{
			frmDocument doc = new frmDocument();
			doc.Text = "Document " + ++_newDocumentCount;
			doc.Show(dockPanel);
			return doc;
		}

		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Close();
		}

		private void printToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.Printing.Print();
		}

		private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.Printing.PrintPreview();
		}

		private void frmMdi_Load(object sender, EventArgs e)
		{
			NewDocument();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Save();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.SaveAs();
		}

		private void saveAllStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (frmDocument doc in dockPanel.Documents)
			{
				doc.Activate();
				doc.Save();
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFile();
		}

		public frmDocument OpenFile()
		{
			if (openFileDialog.ShowDialog() != DialogResult.OK)
				return null;

			return OpenFile(openFileDialog.FileName);
		}

		public frmDocument OpenFile(string fileName)
		{
			frmDocument doc = new frmDocument();
			doc.Text = new FileInfo(fileName).Name;
			doc.Show(dockPanel);

			byte[] data = null;
			using (FileStream fs = File.Open(openFileDialog.FileName, FileMode.Open))
			{
				data = new byte[fs.Length];
				fs.Read(data, 0, (int)fs.Length);
			}

			doc.Document.RawText = data;
			doc.FilePath = fileName;
			return doc;
		}

		private void undoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.UndoRedo.Undo();
		}

		private void redoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.UndoRedo.Redo();
		}

		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.Clipboard.Cut();
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.Clipboard.Copy();
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.Clipboard.Paste();
		}

		private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.Selection.SelectAll();
		}

		private void findToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.FindReplace.ShowFind();
		}

		private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.FindReplace.ShowReplace();
		}

		private void findInFilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//	Coming someday...
		}

		private void replaceInFilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//	Coming someday...
		}

		private void goToToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.GoTo.ShowGoToDialog();
		}

		private void toggleBookmarkToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.Lines.Current.AddMarker(0);
		}

		private void previosBookmarkToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//	 I've got to redo this whole FindNextMarker/FindPreviousMarker Scheme
			Line l = ActiveDocument.Document.Lines.Current.FindPreviousMarker(1);
			if (l != null)
				l.Goto();
		}

		private void nextBookmarkToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//	 I've got to redo this whole FindNextMarker/FindPreviousMarker Scheme
			Line l = ActiveDocument.Document.Lines.Current.FindNextMarker(1);
			if (l != null)
				l.Goto();
		}

		private void clearBookmarsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.Markers.DeleteAll(0);
		}

		private void dropToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.DropMarkers.Drop();
		}

		private void collectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.DropMarkers.Collect();
		}

		private void makeUpperCaseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.Commands.Execute(BindableCommand.UpperCase);
		}

		private void makeLowerCaseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.Commands.Execute(BindableCommand.LowerCase);
		}

		private void commentStreamToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.Commands.Execute(BindableCommand.StreamComment);
		}

		private void commentLineToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.Commands.Execute(BindableCommand.LineComment);
		}

		private void uncommentLineToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.Commands.Execute(BindableCommand.LineUncomment);
		}

		private void autocompleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.AutoComplete.Show();
		}

		private void insertSnippetToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.Snippets.ShowSnippetList();
		}

		private void surroundWithToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.Snippets.ShowSurroundWithList();
		}

		private void whiteSpaceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			whiteSpaceToolStripMenuItem.Checked = !whiteSpaceToolStripMenuItem.Checked;

			if (whiteSpaceToolStripMenuItem.Checked)
			{
				ActiveDocument.Document.WhiteSpace.Mode = WhiteSpaceMode.VisibleAlways;
			}
			else
			{
				ActiveDocument.Document.WhiteSpace.Mode = WhiteSpaceMode.Invisible;
			}
		}

		private void wordWrapToolStripMenuItem_Click(object sender, EventArgs e)
		{
			wordWrapToolStripMenuItem.Checked = !wordWrapToolStripMenuItem.Checked;

			if (wordWrapToolStripMenuItem.Checked)
			{
				ActiveDocument.Document.LineWrap.Mode = WrapMode.Word;
			}
			else
			{
				ActiveDocument.Document.LineWrap.Mode = WrapMode.None;
			}
		}

		private void endOfLineToolStripMenuItem_Click(object sender, EventArgs e)
		{
			endOfLineToolStripMenuItem.Checked = !endOfLineToolStripMenuItem.Checked;
			ActiveDocument.Document.EndOfLine.IsVisible = endOfLineToolStripMenuItem.Checked;
		}

		private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.Zoom ++;
		}

		private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.Zoom --;
		}

		private void resetZoomToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.Zoom = 0;
		}

		private void foldLevelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.Lines.Current.FoldExpanded = true;
		}

		private void unfoldLevelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.Lines.Current.FoldExpanded = false;
		}

		private void foldAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Line l in ActiveDocument.Document.Lines)
			{
				l.FoldExpanded = true;
			}
		}

		private void unfoldAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Line l in ActiveDocument.Document.Lines)
			{
				l.FoldExpanded = true;
			}
		}

		private void cToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.ConfigurationManager.Language = "cs";
			ActiveDocument.Document.Indentation.SmartIndentType = SmartIndent.CPP;
		}

		private void plainTextToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.ConfigurationManager.Language = "";
		}

		private void hTMLToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.ConfigurationManager.Language = "html";
		}

		private void mSSQLToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.ConfigurationManager.Language = "mssql";
		}

		private void vBScriptToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.ConfigurationManager.Language = "vbscript";
		}

		private void pythonToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.ConfigurationManager.Language = "python";
		}

		private void xMLToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.ConfigurationManager.Language = "xml";
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new frmAbout().ShowDialog(this);
		}

		private void lineNumbersToolStripMenuItem_Click(object sender, EventArgs e)
		{
			lineNumbersToolStripMenuItem.Checked = !lineNumbersToolStripMenuItem.Checked;
			if (lineNumbersToolStripMenuItem.Checked)
				ActiveDocument.Document.Margins.Margin0.Width = 35;
			else
				ActiveDocument.Document.Margins.Margin0.Width = 0;
		}

		private void navigateForwardToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.DocumentNavigation.NavigateForward();
		}

		private void navigateBackwardToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveDocument.Document.DocumentNavigation.NavigateBackward();
		}

	}
}