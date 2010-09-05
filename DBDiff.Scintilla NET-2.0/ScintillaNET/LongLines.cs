using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace ScintillaNet
{
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class LongLines : ScintillaHelperBase
	{
		internal LongLines(Scintilla scintilla) : base(scintilla) { }

		internal bool ShouldSerialize()
		{
			return ShouldSerializeEdgeColor() ||
				ShouldSerializeEdgeColumn() ||
				ShouldSerializeEdgeMode();
		}

		#region EdgeMode
		public EdgeMode EdgeMode
		{
			get
			{
				return (EdgeMode)NativeScintilla.GetEdgeMode();
			}
			set
			{
				NativeScintilla.SetEdgeMode((int)value);
			}
		}

		private bool ShouldSerializeEdgeMode()
		{
			return EdgeMode != EdgeMode.None;
		}

		private void ResetEdgeMode()
		{
			EdgeMode = EdgeMode.None;
		}
		#endregion

		#region EdgeColumn
		public int EdgeColumn
		{
			get
			{
				return NativeScintilla.GetEdgeColumn();
			}
			set
			{
				NativeScintilla.SetEdgeColumn(value);
			}
		}

		private bool ShouldSerializeEdgeColumn()
		{
			return EdgeColumn != 0;
		}

		private void ResetEdgeColumn()
		{
			EdgeColumn = 0;
		}
		#endregion

		#region EdgeColor
		public Color EdgeColor
		{
			get
			{
				if (Scintilla.ColorBag.ContainsKey("LongLines.EdgeColor"))
					return Scintilla.ColorBag["LongLines.EdgeColor"];

				return Color.Silver;
			}
			set
			{
				if (value == Color.Silver)
					Scintilla.ColorBag.Remove("LongLines.EdgeColor");

				Scintilla.ColorBag["LongLines.EdgeColor"] = value;
				NativeScintilla.SetEdgeColour(Utilities.ColorToRgb(value));
			}
		}

		private bool ShouldSerializeEdgeColor()
		{
			return EdgeColor != Color.Silver;
		}

		private void ResetEdgeColor()
		{
			EdgeColor = Color.Silver;
		}
		#endregion
	}
}
