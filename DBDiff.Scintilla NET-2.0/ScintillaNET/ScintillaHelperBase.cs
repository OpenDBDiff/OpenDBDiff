using System;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace ScintillaNet
{
	public abstract class ScintillaHelperBase : IDisposable
	{
		private Scintilla _scintilla;
		private INativeScintilla _nativeScintilla;

		protected internal Scintilla Scintilla
		{
			get { return _scintilla; }
			set 
			{ 
				_scintilla = value;
				_nativeScintilla = (INativeScintilla)_scintilla;
			}
		}

		private bool _isDisposed = false;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsDisposed
		{
			get
			{
				return _isDisposed;
			}
			set
			{
				_isDisposed = value;
			}
		}

		private void _scintilla_Load(object sender, EventArgs e)
		{
			Initialize();
		}

		protected internal virtual void Initialize(){}

		protected internal INativeScintilla NativeScintilla
		{
			get { return _nativeScintilla; }
		}

		protected internal ScintillaHelperBase(Scintilla scintilla)
		{
			_scintilla	= scintilla;
			_nativeScintilla	= (INativeScintilla)scintilla;
		}

		public virtual void Dispose()
		{
			_isDisposed = true;
		}
	}
}
