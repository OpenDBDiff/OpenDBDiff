using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;

namespace ScintillaNet
{
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class Printing : ScintillaHelperBase
	{
		internal Printing(Scintilla scintilla) : base(scintilla) 
		{
			_printDocument = new PrintDocument(scintilla);
		}

		internal bool ShouldSerialize()
		{
			return ShouldSerializePageSettings() || ShouldSerializePrintDocument();
		}


		public bool Print()
		{
			return Print(true);
		}

		public bool Print(bool showPrintDialog)
		{
			if (showPrintDialog)
			{
				PrintDialog pd = new PrintDialog();
				pd.Document = _printDocument;
				pd.AllowCurrentPage = true;
				pd.AllowSelection = true;
				pd.AllowSomePages = true;
				pd.PrinterSettings = PageSettings.PrinterSettings;

				if (pd.ShowDialog(Scintilla) == DialogResult.OK)
				{
					_printDocument.PrinterSettings = pd.PrinterSettings;
					_printDocument.Print();
					return true;
				}

				return false;
			}

			_printDocument.Print();
			return true;
		}

		public DialogResult PrintPreview()
		{
			PrintPreviewDialog ppd = new PrintPreviewDialog();
			ppd.WindowState = FormWindowState.Maximized;

			ppd.Document = _printDocument;
			return ppd.ShowDialog();
		}

		public DialogResult PrintPreview(IWin32Window owner)
		{
			PrintPreviewDialog ppd = new PrintPreviewDialog();
			ppd.WindowState = FormWindowState.Maximized;

			ppd.Document = _printDocument;
			return ppd.ShowDialog(owner);
		}

		public DialogResult ShowPageSetupDialog()
		{
			PageSetupDialog psd = new PageSetupDialog();
			psd.PageSettings = PageSettings;
			psd.PrinterSettings = PageSettings.PrinterSettings;
			return psd.ShowDialog();
		}

		public DialogResult ShowPageSetupDialog(IWin32Window owner)
		{
			PageSetupDialog psd = new PageSetupDialog();
			psd.AllowPrinter = true;
			psd.PageSettings = PageSettings;
			psd.PrinterSettings = PageSettings.PrinterSettings;

			return psd.ShowDialog(owner);
		}

		private PrintDocument _printDocument;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PrintDocument PrintDocument
		{
			get
			{
				return _printDocument;
			}
			set
			{
				_printDocument = value;
			}
		}

		private bool ShouldSerializePrintDocument()
		{
			return _printDocument.ShouldSerialize();
		}


		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PageSettings PageSettings
		{
			get
			{
				return _printDocument.DefaultPageSettings as PageSettings;
			}
			set
			{
				_printDocument.DefaultPageSettings = value;
			}
		}

		private bool ShouldSerializePageSettings()
		{
			return PageSettings.ShouldSerialize();
		}
	}
}
