using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;

namespace ScintillaNet.Configuration
{
	public class IndicatorConfigList : KeyedCollection<int, IndicatorConfig>
	{
		protected override int GetKeyForItem(IndicatorConfig item)
		{
			return item.Number;
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

	public class IndicatorConfig
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

		private Color _color;
		public Color Color
		{
			get
			{
				return _color;
			}
			set
			{
				_color = value;
			}
		}

		private int _number;
		public int Number
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

		private IndicatorStyle? _style;
		public IndicatorStyle? Style
		{
			get
			{
				return _style;
			}
			set
			{
				_style = value;
			}
		}

		private bool? _isDrawnUnder;
		public bool? IsDrawnUnder
		{
			get
			{
				return _isDrawnUnder;
			}
			set
			{
				_isDrawnUnder = value;
			}
		}
	}
}
