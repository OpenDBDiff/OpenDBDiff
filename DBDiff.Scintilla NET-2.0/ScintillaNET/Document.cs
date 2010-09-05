using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Scintilla
{
	public class DocumentHandler : ScintillaHelperBase
	{
		internal DocumentHandler(Scintilla scintilla) : base(scintilla) { }

		public Document Current
		{
			get
			{
				return new Document(Scintilla,NativeScintilla.GetDocPointer());
			}
			set
            {
				NativeScintilla.SetDocPointer(value.Handle);
            }
		}

		public Document Create()
		{
			return new Document(Scintilla, NativeScintilla.CreateDocument());
		}
	}

	public class Document : ScintillaHelperBase
	{
		private IntPtr _handle;
		public IntPtr Handle
		{
			get
			{
				return _handle;
			}
			set
			{
				_handle = value;
			}
		}

		internal Document(Scintilla scintilla, IntPtr handle) : base(scintilla) 
		{
			_handle = handle;
		}

		//	No, you aren't looking at COM, move along.
		public void AddRef()
		{
			NativeScintilla.AddRefDocument(_handle);
		}

		public void Release()
		{
			NativeScintilla.ReleaseDocument(_handle);
		}

		public override bool Equals(object obj)
		{
			Document d = obj as Document;

			if (_handle == IntPtr.Zero)
				return false;

			return _handle.Equals(d._handle);
		}

		public override int GetHashCode()
		{
			return _handle.GetHashCode();
		}
	}
}
