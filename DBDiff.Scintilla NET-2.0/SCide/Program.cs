using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SCide
{
	static class Program
	{
		public static frmMdi FrmMdi = null;
		public static frmDocument ActiveDocument
		{
			get
			{
				return FrmMdi.ActiveDocument;
			}
		}
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new frmMdi());
		}
	}
}