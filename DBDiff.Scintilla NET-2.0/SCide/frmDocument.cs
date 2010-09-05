using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI;
using WeifenLuo.WinFormsUI.Docking;
using ScintillaNet;

namespace SCide
{
	public partial class frmDocument : DockContent
	{
		private string _filePath = null;
		public string FilePath
		{
			get { return _filePath; }
			set
			{
				_filePath = value;
			}
		}
        
		public frmDocument()
		{
			InitializeComponent();
		}

		public Scintilla Document
		{
			get
			{
				return sciDocument;
			}
		}

		public bool Save()
		{
			if (String.IsNullOrEmpty(_filePath))
				return SaveAs();

			return save(_filePath);
		}

		public bool SaveAs()
		{
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				_filePath = saveFileDialog.FileName;
				return save(_filePath);
			}
			else
			{
				return false;
			}
		}

		private bool save(string path)
		{
			using (FileStream fs = File.Create(path))
			using (BinaryWriter bw = new BinaryWriter(fs))
				bw.Write(sciDocument.RawText, 0, sciDocument.RawText.Length - 1); // Omit trailing NULL

			sciDocument.IsDirty = false;
			return true;
		}

		private void sciDocument_SavePointReached(object sender, EventArgs e)
		{
			addOrRemoveAsteric();
		}

		private void sciDocument_SavePointLeft(object sender, EventArgs e)
		{
			addOrRemoveAsteric();
		}

		private void addOrRemoveAsteric()
		{
			if (sciDocument.IsDirty)
			{
				if (!Text.EndsWith(" *"))
					Text += " *";
			}
			else
			{
				if (Text.EndsWith(" *"))
					Text = Text.Substring(0, Text.Length - 2);
			}
		}
	}
}
