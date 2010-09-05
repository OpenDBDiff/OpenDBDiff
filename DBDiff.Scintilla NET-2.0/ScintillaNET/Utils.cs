using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace ScintillaNet
{
	public static class Utilities
	{
		public static int ColorToRgb(Color c)
		{
			return c.R + (c.G << 8) + (c.B << 16);
		}

		public static Color RgbToColor(int color)
		{
			return Color.FromArgb(color & 0x0000ff, (color & 0x00ff00) >> 8, (color & 0xff0000) >> 16);
		}

		/// <summary>
		/// Returns an HTML #XXXXXX format for a color. Unlike the ColorTranslator class it
		/// never returns named colors.
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public static string ColorToHtml(Color c)
		{
			return "#" + c.R.ToString("X2", null) + c.G.ToString("X2", null) + c.B.ToString("X2", null);
		}

		
		public static int SignedLoWord(IntPtr loWord)
		{
			return (short)((int)(long)loWord & 0xffff);
		}

		public static int SignedHiWord(IntPtr hiWord)
		{
			return (short)(((int)(long)hiWord >> 0x10) & 0xffff);
		}

		public unsafe static string PtrToStringUtf8(IntPtr ptr, int length)
		{
			if(ptr == IntPtr.Zero)
				return null;

			byte[] buff = new byte[length];
			Marshal.Copy(ptr, buff, 0, length);
			return System.Text.UTF8Encoding.UTF8.GetString(buff);
		}

		public unsafe static string MarshalStr(IntPtr p)
		{
			// instead of 
			// System.Runtime.InteropServices.Marshal.PtrToStringAuto(p)
			sbyte* ps = (sbyte*)p;
			int size = 0;
			for (; ps[size] != 0; ++size)
				;
			return new String(ps, 0, size);
		}

		public static uint GetMarkerMask(IEnumerable<int> markers)
		{
			uint mask = 0;
			foreach (int i in markers)
				mask |= (uint)i;
			return mask;
		}

		public static Keys GetKeys(char c)
		{
			switch (c)
			{
				case '/':
					return Keys.Oem2;
				case '`':
					return Keys.Oem3;
				case '[':
					return Keys.Oem4;
				case '\\':
					return Keys.Oem5;
				case ']':
					return Keys.Oem6;
				case '-':
					return (Keys)189;

			}

			return (Keys)Enum.Parse(typeof(Keys), c.ToString(), true);
		}

		public static Keys GetKeys(string s)
		{
			switch (s)
			{
				case "/":
					return Keys.Oem2;
				case "`":
					return Keys.Oem3;
				case "[":
					return Keys.Oem4;
				case "\\":
					return Keys.Oem5;
				case "]":
					return Keys.Oem6;
				case "-":
					return (Keys)189;
			}

			return (Keys)Enum.Parse(typeof(Keys), s, true);
		}

	}
}
