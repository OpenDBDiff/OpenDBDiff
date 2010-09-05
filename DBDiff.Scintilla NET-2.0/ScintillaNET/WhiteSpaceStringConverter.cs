using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Windows.Forms;

namespace DBDiff.Scintilla
{
	public class WhiteSpaceStringConverter : TypeConverter
	{
		private static readonly Regex rr = new Regex("\\{0x([0123456789abcdef]{1,4})\\}", RegexOptions.IgnoreCase);

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(char) || sourceType == typeof(string))
				return true;

			return false;
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(char) || destinationType == typeof(string))
				return true;

			return false;
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			string val = convertFrom(value.ToString());


			if (context.PropertyDescriptor.ComponentType == typeof(char))
				return val[0];

			return val;
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			string val = convertTo(value.ToString());

			if (destinationType == typeof(char))
				return val[0];

			return val;
		}

		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			return true;
		}


		private string convertTo(string nativeString)
		{
			StringBuilder sb = new StringBuilder();
			foreach (char c in nativeString)
			{
				if ((int)c > 32)
				{
					sb.Append(c);
					continue;
				}

				switch ((int)c)
				{
					case 9:
						sb.Append("{TAB}");
						break;
					case 10:
						sb.Append("{LF}");
						break;
					case 13:
						sb.Append("{CR}");
						break;
					case 32:
						sb.Append("{SPACE}");
						break;
					default:
						sb.Append("{0x" + ((int)c).ToString("x4") + "}");
						break;
				}
			}
			return sb.ToString();
		}


		private string convertFrom(string value)
		{
			Match m = rr.Match(value);
			while (m.Success)
			{
				int val = int.Parse(m.Value.Substring(3, m.Length - 4), NumberStyles.AllowHexSpecifier);
				value = value.Replace(m.Value, ((char)val).ToString());
				m = rr.Match(value);
			}
			value = value.Replace("{TAB}", "\t").Replace("{LF}", "\r").Replace("{CR}", "\n").Replace("{SPACE}", " ");

			return value;
		}


	}
}
