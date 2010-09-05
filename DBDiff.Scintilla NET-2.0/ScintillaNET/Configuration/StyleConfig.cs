using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ScintillaNet.Configuration
{
	public class StyleConfigList : List<StyleConfig>
	{
		private int? _bits;
		public int? Bits
		{
			get
			{
				return _bits;
			}
			set
			{
				_bits = value;
			}
		}

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

	public class StyleConfig
	{
		private bool? _bold;
		public bool? Bold
		{
			get
			{
				return _bold;
			}
			set
			{
				_bold = value;
			}
		}

		private StyleCase? _case;
		public StyleCase? Case
		{
			get
			{
				return _case;
			}
			set
			{
				_case = value;
			}
		}

		private CharacterSet? _characterSet;
		public CharacterSet? CharacterSet
		{
			get
			{
				return _characterSet;
			}
			set
			{
				_characterSet = value;
			}
		}

		private string _fontName;
		public string FontName
		{
			get
			{
				return _fontName;
			}
			set
			{
				_fontName = value;
			}
		}

		private Color _foreColor;
		public Color ForeColor
		{
			get
			{
				return _foreColor;
			}
			set
			{
				_foreColor = value;
			}
		}

		private Color _backColor;
		public Color BackColor
		{
			get
			{
				return _backColor;
			}
			set
			{
				_backColor = value;
			}
		}

		private bool? _isChangeable;
		public bool? IsChangeable
		{
			get
			{
				return _isChangeable;
			}
			set
			{
				_isChangeable = value;
			}
		}

		private bool? _isHotspot;
		public bool? IsHotspot
		{
			get
			{
				return _isHotspot;
			}
			set
			{
				_isHotspot = value;
			}
		}

		private bool? _isSelectionEolFilled;
		public bool? IsSelectionEolFilled
		{
			get
			{
				return _isSelectionEolFilled;
			}
			set
			{
				_isSelectionEolFilled = value;
			}
		}

		private bool? _isVisible;
		public bool? IsVisible
		{
			get
			{
				return _isVisible;
			}
			set
			{
				_isVisible = value;
			}
		}

		private bool? _italic;
		public bool? Italic
		{
			get
			{
				return _italic;
			}
			set
			{
				_italic = value;
			}
		}

		private int? _size;
		public int? Size
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
			}
		}


		private bool? _underline;
		public bool? Underline
		{
			get
			{
				return _underline;
			}
			set
			{
				_underline = value;
			}
		}

		private int? _number;
		public int? Number
		{
			get
			{
				return _number;
			}
			set
			{
				_number = value;
			}
		}

		private string _name;
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

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

		public override string ToString()
		{
			return "Name = \"" + _name + "\" Number=" + _number.ToString();
		}
	}

	public class ResolvedStyleList : Dictionary<int, StyleConfig>
	{

		public ResolvedStyleList()
		{
			
		}

		public StyleConfig FindByName(string name)
		{
			
			foreach (StyleConfig item in this.Values)
			{
				if (item.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
					return item;
			}

			return null;
		}

	}
}
