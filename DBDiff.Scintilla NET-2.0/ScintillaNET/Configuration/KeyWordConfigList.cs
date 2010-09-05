using System;
using System.Collections.ObjectModel;
using System.Text;

namespace ScintillaNet.Configuration
{
	public class KeyWordConfigList : KeyedCollection<int, KeyWordConfig>
	{
		protected override int GetKeyForItem(KeyWordConfig item)
		{
			return item.List;
		}
	}

	public class KeyWordConfig
	{
		private int _list;
		public int List
		{
			get
			{
				return _list;
			}
			set
			{
				_list = value;
			}
		}

		private string _value;
		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
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

		/// <summary>
		/// Initializes a new instance of the KeyWordConfig class.
		/// </summary>
		/// <param name="list"></param>
		/// <param name="value"></param>
		/// <param name="inherit"></param>
		public KeyWordConfig(int list, string value, bool? inherit)
		{
			_list = list;
			_value = value;
			_inherit = inherit;
		}
	}
}
