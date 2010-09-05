using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace ScintillaNet
{
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class LineWrap : ScintillaHelperBase
	{
		internal LineWrap(Scintilla scintilla) : base(scintilla) { }

		internal bool ShouldSerialize()
		{
			return ShouldSerializeLayoutCache() ||
				ShouldSerializePositionCacheSize() ||
				ShouldSerializeStartIndent() ||
				ShouldSerializeVisualFlags() ||
				ShouldSerializeVisualFlagsLocation() ||
				ShouldSerializeMode();
		}

		#region WrapMode
		public WrapMode Mode
		{
			get
			{
				return (WrapMode)NativeScintilla.GetWrapMode();
			}
			set
			{
				NativeScintilla.SetWrapMode((int)value);
			}
		}

		private bool ShouldSerializeMode()
		{
			return Mode != WrapMode.None;
		}

		private void ResetMode()
		{
			Mode = WrapMode.None;
		} 
		#endregion

		#region VisualFlags
		[Editor(typeof(ScintillaNet.Design.FlagEnumUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public WrapVisualFlag VisualFlags
		{
			get
			{
				return (WrapVisualFlag)NativeScintilla.GetWrapVisualFlags();
			}
			set
			{
				NativeScintilla.SetWrapVisualFlags((int)value);
			}
		}

		private bool ShouldSerializeVisualFlags()
		{
			return VisualFlags != WrapVisualFlag.None;
		}

		private void ResetVisualFlags()
		{
			VisualFlags = WrapVisualFlag.None;
		} 
		#endregion

		#region VisualFlagsLocation
		[Editor(typeof(ScintillaNet.Design.FlagEnumUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public WrapVisualLocation VisualFlagsLocation
		{
			get
			{
				return (WrapVisualLocation)NativeScintilla.GetWrapVisualFlagsLocation();
			}
			set
			{
				NativeScintilla.SetWrapVisualFlagsLocation((int)value);
			}
		}

		private bool ShouldSerializeVisualFlagsLocation()
		{
			return VisualFlagsLocation != WrapVisualLocation.Default;
		}

		private void ResetVisualFlagsLocation()
		{
			VisualFlagsLocation = WrapVisualLocation.Default;
		}
		#endregion

		#region StartIndent
		public int StartIndent
		{
			get
			{
				return NativeScintilla.GetWrapStartIndent();
			}
			set
			{
				NativeScintilla.SetWrapStartIndent(value);
			}
		}

		private bool ShouldSerializeStartIndent()
		{
			return StartIndent != 0;
		}

		private void ResetStartIndent()
		{
			StartIndent = 0;
		} 
		#endregion

		#region LayoutCache
		public LineCache LayoutCache
		{
			get
			{
				return (LineCache)NativeScintilla.GetLayoutCache();
			}
			set
			{
				NativeScintilla.SetLayoutCache((int)value);
			}
		}

		private bool ShouldSerializeLayoutCache()
		{
			return LayoutCache != LineCache.Caret;
		}

		private void ResetLayoutCache()
		{
			LayoutCache = LineCache.Caret;
		} 
		#endregion

		#region PositionCacheSize
		public int PositionCacheSize
		{
			get
			{
				return NativeScintilla.GetPositionCache();
			}
			set
			{
				NativeScintilla.SetPositionCache(value);
			}
		}

		private bool ShouldSerializePositionCacheSize()
		{
			return PositionCacheSize != 1024;
		}

		private void ResetPositionCacheSize()
		{
			PositionCacheSize = 1024;
		} 
		#endregion
	}
}
