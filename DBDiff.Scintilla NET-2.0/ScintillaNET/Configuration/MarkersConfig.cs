using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ScintillaNet.Configuration
{
	public class MarkersConfig
	{
		private int? _alpha;
		public int? Alpha
		{
			get
			{
				return _alpha;
			}
			set
			{
				_alpha = value;
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

		private MarkerSymbol? _symbol;
		public MarkerSymbol? Symbol
		{
			get
			{
				return _symbol;
			}
			set
			{
				_symbol = value;
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

	public class MarkersConfigList : System.Collections.ObjectModel.KeyedCollection<int, MarkersConfig>
	{
		protected override int GetKeyForItem(MarkersConfig item)
		{
			return item.Number.Value;
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
}
