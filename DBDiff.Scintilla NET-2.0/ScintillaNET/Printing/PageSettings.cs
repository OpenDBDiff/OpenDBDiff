using System;
using System.Drawing;
using System.Drawing.Printing;
using System.ComponentModel;

namespace ScintillaNet
{
	
	/// <summary>
	/// ScintillaNET derived class for handling printed page settings.  It holds information 
	/// on how and what to print in the header and footer of pages.
	/// </summary>
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class PageSettings : System.Drawing.Printing.PageSettings
	{
		/// <summary>
		/// Default header style used when no header is provided.
		/// </summary>
		public static readonly PageInformation DefaultHeader = new PageInformation(PageInformationBorder.Bottom, InformationType.DocumentName, InformationType.Nothing, InformationType.PageNumber);
		/// <summary>
		/// Default footer style used when no footer is provided.
		/// </summary>
		public static readonly PageInformation DefaultFooter = new PageInformation(PageInformationBorder.Top, InformationType.Nothing, InformationType.Nothing, InformationType.Nothing);

		private HeaderInformation m_oHeader;
		private FooterInformation m_oFooter;
		private short m_sFontMagnification;
		private PrintColorMode m_eColorMode;

		/// <summary>
		/// Default constructor
		/// </summary>
		public PageSettings()
		{
			m_oHeader = new HeaderInformation(PageInformationBorder.Bottom, InformationType.DocumentName, InformationType.Nothing, InformationType.PageNumber);
			m_oFooter = new FooterInformation(PageInformationBorder.Top, InformationType.Nothing, InformationType.Nothing, InformationType.Nothing);
			m_sFontMagnification = 0;
			m_eColorMode = PrintColorMode.Normal;

			// Set default margins to 1/2 inch (50/100ths)
			base.Margins.Top = 50;
			base.Margins.Left = 50;
			base.Margins.Right = 50;
			base.Margins.Bottom = 50;
		}

		internal bool ShouldSerialize()
		{
			return ShouldSerializeColor() ||
				ShouldSerializeColorMode() ||
				ShouldSerializeFontMagnification() ||
				ShouldSerializeFooter() ||
				ShouldSerializeHeader() ||
				ShouldSerializeLandscape() ||
				ShouldSerializeMargins();
		}

		#region Properties

		/// <summary>
		/// Page Information printed in header of the page
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeaderInformation Header
		{
			get { return m_oHeader; }
			set { m_oHeader = value; }
		}

		private bool ShouldSerializeHeader()
		{
			return m_oHeader.ShouldSerialize();
		}

		/// <summary>
		/// Page Information printed in the footer of the page
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FooterInformation Footer
		{
			get { return m_oFooter; }
			set { m_oFooter = value; }
		}

		private bool ShouldSerializeFooter()
		{
			return m_oFooter.ShouldSerialize();
		}

		/// <summary>
		/// Number of points to add or subtract to the size of each screen font during printing
		/// </summary>
		public short FontMagnification
		{
			get { return m_sFontMagnification; }
			set { m_sFontMagnification = value; }
		}

		private bool ShouldSerializeFontMagnification()
		{
			return m_sFontMagnification != 0;
		}

		private void ResetFontMagnification()
		{
			m_sFontMagnification = 0;
		}

		/// <summary>
		/// Method used to render colored text on a printer
		/// </summary>
		public PrintColorMode ColorMode
		{
			get { return m_eColorMode; }
			set { m_eColorMode = value; }
		}

		private bool ShouldSerializeColorMode()
		{
			return m_eColorMode != PrintColorMode.Normal;
		}

		private void ResetColorMode()
		{
			m_eColorMode = PrintColorMode.Normal;
		}

		#endregion


		//	All these properties below merely call into their base class.
		//	So why have new versions of these? The PageSettings class
		//	isn't designer friendly.

		[Browsable(false)]
		public new Rectangle Bounds
		{
			get
			{
				return base.Bounds;
			}
		}

		public new bool Color
		{
			get
			{
				return base.Color;
			}
			set
            {
            	base.Color = value;
            }
		}

		private bool ShouldSerializeColor()
		{
			return !Color;
		}

		private void ResetColor()
		{
			Color = true;
		}

		[Browsable(false)]
		public new float HardMarginX
		{
			get
			{
				return base.HardMarginX;
			}
		}

		[Browsable(false)]
		public new float HardMarginY
		{
			get
			{
				return base.HardMarginY;
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PaperSize PaperSize
		{
			get
			{
				return base.PaperSize as PaperSize;
			}
			set
            {
            	base.PaperSize  = value;
            }
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PaperSource PaperSource
		{
			get
			{
				return base.PaperSource;
			}
			set
            {
				base.PaperSource = value;
            }
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new RectangleF PrintableArea
		{
			get
			{
				return base.PrintableArea;
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PrinterResolution PrinterResolution
		{
			get
			{
				return base.PrinterResolution;
			}
			set
			{
				base.PrinterResolution = value;
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new PrinterSettings PrinterSettings
		{
			get
			{
				return base.PrinterSettings;
			}
			set
			{
				base.PrinterSettings = value;
			}
		}


		public new bool Landscape
		{
			get
			{
				return base.Landscape;
			}
			set
            {
            	base.Landscape = value;
            }
		}

		private bool ShouldSerializeLandscape()
		{
			return Landscape;
		}

		private void ResetLandscape()
		{
			Landscape = false;
		}

		public new Margins Margins
		{
			get
			{
				return base.Margins;
			}
			set
            {
            	base.Margins = value;
            }
		}

		private bool ShouldSerializeMargins()
		{
			return Margins.Bottom != 50 ||Margins.Left != 50 || Margins.Right != 50 || Margins.Bottom != 50;
		}

		private void ResetMargins()
		{
			Margins = new Margins(50,50,50,50);
		}



	}
}