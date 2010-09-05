using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.ComponentModel;

namespace ScintillaNet
{
	/// <summary>
	/// ScintillaNET derived class for handling printing of source code from a Scintilla control.
	/// </summary>
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class PrintDocument : System.Drawing.Printing.PrintDocument
	{
		private Scintilla m_oScintillaControl;

		private int m_iPosition;
		private int m_iPrintEnd;
		private int m_iCurrentPage;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="oScintillaControl">Scintilla control being printed</param>
		public PrintDocument(Scintilla oScintillaControl)
		{
			m_oScintillaControl = oScintillaControl;
			DefaultPageSettings = new ScintillaNet.PageSettings();
		}

		
		internal bool ShouldSerialize()
		{
			return base.DocumentName != "document" || OriginAtMargins;
		}

		/// <summary>
		/// Method called after the Print method is called and before the first page of the document prints
		/// </summary>
		/// <param name="e">A PrintPageEventArgs that contains the event data</param>
		protected override void OnBeginPrint(PrintEventArgs e)
		{
			base.OnBeginPrint(e);

			m_iPosition = 0;
			m_iPrintEnd = m_oScintillaControl.TextLength;
			m_iCurrentPage = 1;
		}

		/// <summary>
		/// Method called when the last page of the document has printed
		/// </summary>
		/// <param name="e">A PrintPageEventArgs that contains the event data</param>
		protected override void OnEndPrint(PrintEventArgs e)
		{
			base.OnEndPrint(e);
		}

		/// <summary>
		/// Method called when printing a page
		/// </summary>
		/// <param name="e">A PrintPageEventArgs that contains the event data</param>
		protected override void OnPrintPage(PrintPageEventArgs e)
		{
			base.OnPrintPage(e);

			PageSettings oPageSettings = null;
			HeaderInformation oHeader = ((PageSettings)DefaultPageSettings).Header;
			FooterInformation oFooter = ((PageSettings)DefaultPageSettings).Footer;
			Rectangle oPrintBounds = e.MarginBounds;
			bool bIsPreview = this.PrintController.IsPreview;

			// When not in preview mode, adjust graphics to account for hard margin of the printer
			if (!bIsPreview)
			{
				e.Graphics.TranslateTransform(-e.PageSettings.HardMarginX, -e.PageSettings.HardMarginY);
			}

			// Get the header and footer provided if using Scintilla.Printing.PageSettings
			if (e.PageSettings is PageSettings)
			{
				oPageSettings = (PageSettings)e.PageSettings;

				oHeader = oPageSettings.Header;
				oFooter = oPageSettings.Footer;

				m_oScintillaControl.NativeInterface.SetPrintMagnification(oPageSettings.FontMagnification);
				m_oScintillaControl.NativeInterface.SetPrintColourMode((int)oPageSettings.ColorMode);
			}

			// Draw the header and footer and get remainder of page bounds
			oPrintBounds = DrawHeader(e.Graphics, oPrintBounds, oHeader);
			oPrintBounds = DrawFooter(e.Graphics, oPrintBounds, oFooter);

			// When not in preview mode, adjust page bounds to account for hard margin of the printer
			if (!bIsPreview)
			{
				oPrintBounds.Offset((int)-e.PageSettings.HardMarginX, (int)-e.PageSettings.HardMarginY);
			}
			DrawCurrentPage(e.Graphics, oPrintBounds);

			// Increment the page count and determine if there are more pages to be printed
			m_iCurrentPage++;
			e.HasMorePages = (m_iPosition < m_iPrintEnd);
		}

		private Rectangle DrawHeader(Graphics oGraphics, Rectangle oBounds, PageInformation oHeader)
		{
			if (oHeader.Display)
			{
				Rectangle oHeaderBounds = new Rectangle(oBounds.Left, oBounds.Top, oBounds.Width, oHeader.Height);

				oHeader.Draw(oGraphics, oHeaderBounds, this.DocumentName, m_iCurrentPage);

				return new Rectangle(
					oBounds.Left, oBounds.Top + oHeaderBounds.Height + oHeader.Margin,
					oBounds.Width, oBounds.Height - oHeaderBounds.Height - oHeader.Margin
					);
			}
			else
			{
				return oBounds;
			}
		}

		private Rectangle DrawFooter(Graphics oGraphics, Rectangle oBounds, PageInformation oFooter)
		{
			if (oFooter.Display)
			{
				int iHeight = oFooter.Height;
				Rectangle oFooterBounds = new Rectangle(oBounds.Left, oBounds.Bottom - iHeight, oBounds.Width, iHeight);

				oFooter.Draw(oGraphics, oFooterBounds, this.DocumentName, m_iCurrentPage);

				return new Rectangle(
					oBounds.Left, oBounds.Top,
					oBounds.Width, oBounds.Height - oFooterBounds.Height - oFooter.Margin
					);
			}
			else
			{
				return oBounds;
			}
		}

		private void DrawCurrentPage(Graphics oGraphics, Rectangle oBounds)
		{
			Point[] oPoints = {
                new Point(oBounds.Left, oBounds.Top),
                new Point(oBounds.Right, oBounds.Bottom)
                };
			oGraphics.TransformPoints(CoordinateSpace.Device, CoordinateSpace.Page, oPoints);

			PrintRectangle oPrintRectangle = new PrintRectangle(oPoints[0].X, oPoints[0].Y, oPoints[1].X, oPoints[1].Y);

			RangeToFormat oRangeToFormat = new RangeToFormat();
			oRangeToFormat.hdc = oRangeToFormat.hdcTarget = oGraphics.GetHdc();
			oRangeToFormat.rc = oRangeToFormat.rcPage = oPrintRectangle;
			oRangeToFormat.chrg.cpMin = m_iPosition;
			oRangeToFormat.chrg.cpMax = m_iPrintEnd;

			m_iPosition = m_oScintillaControl.NativeInterface.FormatRange(true, ref oRangeToFormat);
			
		}

		public new string DocumentName
		{
			get
			{
				return base.DocumentName;
			}
			set
            {
            	base.DocumentName = value;
            }
		}

		private bool ShouldSerializeDocumentName()
		{
			return DocumentName != "document";
		}

		private void ResetDocumentName()
		{
			DocumentName = "document";
		}

		public new bool OriginAtMargins
		{
			get
			{
				return base.OriginAtMargins;
			}
			set
            {
            	base.OriginAtMargins = value;
            }
		}

		private bool ShouldSerializeOriginAtMargins()
		{
			return OriginAtMargins;
		}

		private void ResetOriginAtMargins()
		{
			OriginAtMargins = false;
		}
	}
}
