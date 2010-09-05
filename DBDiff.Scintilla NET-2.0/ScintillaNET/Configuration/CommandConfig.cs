using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ScintillaNet.Configuration
{
	public class CommandBindingConfigList : List<CommandBindingConfig>
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

		private bool? _allowDuplicateBindings;
		public bool? AllowDuplicateBindings
		{
			get
			{
				return _allowDuplicateBindings;
			}
			set
			{
				_allowDuplicateBindings = value;
			}
		}
	}

	public struct CommandBindingConfig
	{
		public KeyBinding KeyBinding;
		public bool? ReplaceCurrent;
		public BindableCommand BindableCommand;

		/// <summary>
		/// Initializes a new instance of the CommandBindingConfig structure.
		/// </summary>
		/// <param name="keyBinding"></param>
		/// <param name="replaceCurrent"></param>
		/// <param name="bindableCommand"></param>
		public CommandBindingConfig(KeyBinding keyBinding, bool? replaceCurrent, BindableCommand bindableCommand)
		{
			KeyBinding = keyBinding;
			ReplaceCurrent = replaceCurrent;
			BindableCommand = bindableCommand;
		}
	}

}
