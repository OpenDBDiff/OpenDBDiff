using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace ScintillaNet
{
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class UndoRedo : ScintillaHelperBase
	{
		internal UndoRedo(Scintilla scintilla) : base(scintilla) { }

		internal bool ShouldSerialize()
		{
			return ShouldSerializeIsUndoEnabled();
		}

		#region CanUndo
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanUndo
		{
			get
			{
				return NativeScintilla.CanUndo();
			}
		}
		#endregion

		#region CanRedo
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanRedo
		{
			get
			{
				return NativeScintilla.CanRedo();
			}
		}
		#endregion

		#region UndoEnabled
		public bool IsUndoEnabled
		{
			get
			{
				return NativeScintilla.GetUndoCollection();
			}
			set
			{
				NativeScintilla.SetUndoCollection(value);
			}
		}

		private bool ShouldSerializeIsUndoEnabled()
		{
			return !IsUndoEnabled;
		}

		private void ResetIsUndoEnabled()
		{
			IsUndoEnabled = true;
		}
		#endregion

		public void BeginUndoAction()
		{
			NativeScintilla.BeginUndoAction();
		}

		public void EndUndoAction()
		{
			NativeScintilla.EndUndoAction();
		}

		public void Undo()
		{
			NativeScintilla.Undo();
		}

		public void Redo()
		{
			NativeScintilla.Redo();
		}

		public void EmptyUndoBuffer()
		{
			NativeScintilla.EmptyUndoBuffer();
		}

	}
}
