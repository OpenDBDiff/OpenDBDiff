using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;


namespace ScintillaNet.Configuration
{
	public class MarginConfigList : List<MarginConfig>
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

		private int? _left;
		public int? Left
		{
			get
			{
				return _left;
			}
			set
			{
				_left = value;
			}
		}

		private int? _right;
		public int? Right
		{
			get
			{
				return _right;
			}
			set
			{
				_right = value;
			}
		}

		private Color _foldMarginColor;
		public Color FoldMarginColor
		{
			get
			{
				return _foldMarginColor;
			}
			set
			{
				_foldMarginColor = value;
			}
		}

		private Color _foldMarginHighlightColor;
		public Color FoldMarginHighlightColor
		{
			get
			{
				return _foldMarginHighlightColor;
			}
			set
			{
				_foldMarginHighlightColor = value;
			}
		} 
	}

	public class MarginConfig
	{
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


		private MarginType? _type;
		public MarginType? Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		private bool? _isFoldMargin;
		public bool? IsFoldMargin
		{
			get
			{
				return _isFoldMargin;
			}
			set
			{
				_isFoldMargin = value;
			}
		}

		private bool? _isMarkerMargin;
		public bool? IsMarkerMargin
		{
			get
			{
				return _isMarkerMargin;
			}
			set
			{
				_isMarkerMargin = value;
			}
		}

		private int? _width;
		public int? Width
		{
			get
			{
				return _width;
			}
			set
			{
				_width = value;
			}
		}

		private bool? _isClickable;
		public bool? IsClickable
		{
			get
			{
				return _isClickable;
			}
			set
			{
				_isClickable = value;
			}
		}

		private int? _autoToggleMarkerNumber;
		public int? AutoToggleMarkerNumber
		{
			get
			{
				return _autoToggleMarkerNumber;
			}
			set
			{
				_autoToggleMarkerNumber = value;
			}
		}

	}

}
