using System;
using System.Collections.Generic;
using System.Text;

namespace ScintillaNet.Configuration
{
	public class LexerPropertiesConfig : Dictionary<string, string>
	{

		private bool? _inherit;
		public bool? Inherit
		{
			get
			{
				return _inherit;
			}
			set
			{
				_inherit = value;
			}
		}
	}
}
