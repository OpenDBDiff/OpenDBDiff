using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
namespace ScintillaNet
{
	public partial class Scintilla	
	{
		// This file contains all the implementation of INativeScintilla
		#region SendMessageDirect (9 Overloads)
		/// <summary>
		/// This is the primary Native communication method with Scintilla
		/// used by this control. All the other overloads call into this one.
		/// </summary>
		IntPtr INativeScintilla.SendMessageDirect(uint msg, IntPtr wParam, IntPtr lParam)
		{
			if (!this.IsDisposed)
			{
				Message m = new Message();
				m.Msg = (int)msg;
				m.WParam = wParam;
				m.LParam = lParam;
				m.HWnd = Handle;

				//  DefWndProc is the Window Proc associated with the window
				//  class for this control created by Windows Forms. It will
				//  in turn call Scintilla's DefWndProc Directly. This has 
				//  the same net effect as using Scintilla's DirectFunction
				//  in that SendMessage isn't used to get the message to 
				//  Scintilla but requires 1 less PInvoke and I don't have
				//  to maintain the FunctionPointer and "this" reference
				DefWndProc(ref m);

				return m.Result;
			}
			else
			{
				return IntPtr.Zero;
			}
		}

		//  Various overloads provided for syntactical convinience.
		//  note that the return value is int (32 bit signed Integer). 
		//  If you are invoking a message that returns a pointer or
		//  handle like SCI_GETDIRECTFUNCTION or SCI_GETDOCPOINTER
		//  you MUST use the IntPtr overload to ensure 64bit compatibility

		/// <summary>
		/// Handles Scintilla Call Style:
		///    (,)
		/// </summary>
		/// <param name="msg">Scintilla Message Number</param>
		/// <returns></returns>
		int INativeScintilla.SendMessageDirect(uint msg)
		{
			return (int)_ns.SendMessageDirect(msg, IntPtr.Zero, IntPtr.Zero);
		}

		/// <summary>
		/// Handles Scintilla Call Style:
		///    (int,int)    
		/// </summary>
		/// <param name="msg">Scintilla Message Number</param>
		/// <param name="wParam">wParam</param>
		/// <param name="lParam">lParam</param>
		/// <returns></returns>
		int INativeScintilla.SendMessageDirect(uint msg, int wParam, int lParam)
		{
			return (int)_ns.SendMessageDirect(msg, (IntPtr)wParam, (IntPtr)lParam);
		}

		/// <summary>
		/// Handles Scintilla Call Style:
		///    (int,uint)    
		/// </summary>
		/// <param name="msg">Scintilla Message Number</param>
		/// <param name="wParam">wParam</param>
		/// <param name="lParam">lParam</param>
		/// <returns></returns>
		int INativeScintilla.SendMessageDirect(uint msg, int wParam, uint lParam)
		{
			//	Hrm, just found out that no explicit conversion exists from uint to
			//	IntPtr. So first it converts it to a signed int, then to IntPtr. Of
			//	course if you have a large uint, it will overflow causing an 
			//	ArithmiticOverflowException. So we first have to do the conversion
			//	ourselves to a signed in inside an unchecked region to prevent the
			//	exception.
			unchecked
			{
				int i = (int)lParam;
				return (int)_ns.SendMessageDirect(msg, (IntPtr)wParam, (IntPtr)i);
			}
			
		}

		/// <summary>
		/// Handles Scintilla Call Style:
		///    (int,)    
		/// </summary>
		/// <param name="msg">Scintilla Message Number</param>
		/// <param name="wParam">wParam</param>
		/// <returns></returns>
		int INativeScintilla.SendMessageDirect(uint msg, int wParam)
		{
			return (int)_ns.SendMessageDirect(msg, (IntPtr)wParam, IntPtr.Zero);
		}

		/// <summary>
		/// Handles Scintilla Call Style:
		///    (,int)    
		/// </summary>
		/// <param name="msg">Scintilla Message Number</param>
		/// <param name="NULL">always pass null--Unused parameter</param>
		/// <param name="lParam">lParam</param>
		/// <returns></returns>
		int INativeScintilla.SendMessageDirect(uint msg, VOID NULL, int lParam)
		{
			return (int)_ns.SendMessageDirect(msg, IntPtr.Zero, (IntPtr)lParam);
		}


		/// <summary>
		/// Handles Scintilla Call Style:
		///    (bool,int)    
		/// </summary>
		/// <param name="msg">Scintilla Message Number</param>
		/// <param name="wParam">boolean wParam</param>
		/// <param name="lParam">int lParam</param>
		/// <returns></returns>
		int INativeScintilla.SendMessageDirect(uint msg, bool wParam, int lParam)
		{
			return (int)_ns.SendMessageDirect(msg, (IntPtr)(wParam ? 1 : 0), (IntPtr)lParam);
		}

		/// <summary>
		/// Handles Scintilla Call Style:
		///    (bool,)    
		/// </summary>
		/// <param name="msg">Scintilla Message Number</param>
		/// <param name="wParam">boolean wParam</param>
		/// <returns></returns>
		int INativeScintilla.SendMessageDirect(uint msg, bool wParam)
		{
			return (int)_ns.SendMessageDirect(msg, (IntPtr)(wParam ? 1 : 0), IntPtr.Zero);
		}

		/// <summary>
		/// Handles Scintilla Call Style:
		///    (int,bool)    
		/// </summary>
		/// <param name="msg">Scintilla Message Number</param>
		/// <param name="wParam">int wParam</param>
		/// <param name="lParam">boolean lParam</param>
		/// <returns></returns>
		int INativeScintilla.SendMessageDirect(uint msg, int wParam, bool lParam)
		{
			return (int)_ns.SendMessageDirect(msg, (IntPtr)wParam, (IntPtr)(lParam ? 1 : 0));
		}

		/// <summary>
		/// Handles Scintilla Call Style:
		///    (,stringresult)    
		/// Notes:
		///  Helper method to wrap all calls to messages that take a char*
		///  in the lParam and returns a regular .NET String. This overload
		///  assumes there will be no wParam and obtains the string length
		///  by calling the message with a 0 lParam. 
		/// </summary>
		/// <param name="msg">Scintilla Message Number</param>
		/// <param name="text">String output</param>
		/// <returns></returns>
		int INativeScintilla.SendMessageDirect(uint msg, out string text)
		{
			int length = _ns.SendMessageDirect(msg, 0, 0);
			return _ns.SendMessageDirect(msg, IntPtr.Zero, out text, length);
		}

		/// <summary>
		/// Handles Scintilla Call Style:
		///    (int,stringresult)    
		/// Notes:
		///  Helper method to wrap all calls to messages that take a char*
		///  in the lParam and returns a regular .NET String. This overload
		///  assumes there will be no wParam and obtains the string length
		///  by calling the message with a 0 lParam. 
		/// </summary>
		/// <param name="msg">Scintilla Message Number</param>
		/// <param name="text">String output</param>
		/// <returns></returns>
		int INativeScintilla.SendMessageDirect(uint msg, int wParam, out string text)
		{
			int length = _ns.SendMessageDirect(msg, 0, 0);
			return _ns.SendMessageDirect(msg, (IntPtr)wParam, out text, length);
		}

		/// <summary>
		/// Handles Scintilla Call Style:
		///    (?)    
		/// Notes:
		///  Helper method to wrap all calls to messages that take a char*
		///  in the wParam and set a regular .NET String in the lParam. 
		///  Both the length of the string and an additional wParam are used 
		///  so that various string Message styles can be acommodated.
		/// </summary>
		/// <param name="msg">Scintilla Message Number</param>
		/// <param name="wParam">int wParam</param>
		/// <param name="text">String output</param>
		/// <param name="length">length of the input buffer</param>
		/// <returns></returns>
		unsafe int INativeScintilla.SendMessageDirect(uint msg, IntPtr wParam, out string text, int length)
		{
			IntPtr ret;

			//  Allocate a buffer the size of the string + 1 for 
			//  the NULL terminator. Scintilla always sets this
			//  regardless of the encoding
			byte[] buffer = new byte[length + 1];

			//  Get a direct pointer to the the head of the buffer
			//  to pass to the message along with the wParam. 
			//  Scintilla will fill the buffer with string data.
			fixed (byte* bp = buffer)
			{
				ret = _ns.SendMessageDirect(msg, wParam, (IntPtr)bp);

				//	If this string is NULL terminated we want to trim the
				//	NULL before converting it to a .NET String
				if (bp[length - 1] == 0)
					length--;
			}


			//  We always assume UTF8 encoding to ensure maximum
			//  compatibility. Manually changing the encoding to 
			//  something else will cuase 2 Byte characters to
			//  be interpreted as junk.
			text = _encoding.GetString(buffer, 0, length);

			return (int)ret;
		}

		/// <summary>
		/// Handles Scintilla Call Style:
		///    (int,string)    
		/// Notes:
		///  This helper method handles all messages that take
		///  const char* as an input string in the lParam. In
		///  some messages Scintilla expects a NULL terminated string
		///  and in others it depends on the string length passed in
		///  as wParam. This method handles both situations and will
		///  NULL terminate the string either way. 
		/// 
		/// </summary>
		/// <param name="msg">Scintilla Message Number</param>
		/// <param name="wParam">int wParam</param>
		/// <param name="lParam">string lParam</param>
		/// <returns></returns>
		unsafe int INativeScintilla.SendMessageDirect(uint msg, int wParam, string lParam)
		{
			//  Just as when retrieving we make to convert .NET's
			//  UTF-16 strings into a UTF-8 encoded byte array.
			fixed (byte* bp = _encoding.GetBytes(ZeroTerminated(lParam)))
				return (int)_ns.SendMessageDirect(msg, (IntPtr)wParam, (IntPtr)bp);
		}

		/// <summary>
		/// Handles Scintilla Call Style:
		///    (,string)    
		/// 
		/// Notes:
		///  This helper method handles all messages that take
		///  const char* as an input string in the lParam. In
		///  some messages Scintilla expects a NULL terminated string
		///  and in others it depends on the string length passed in
		///  as wParam. This method handles both situations and will
		///  NULL terminate the string either way. 
		/// 
		/// </summary>
		/// <param name="msg">Scintilla Message Number</param>
		/// <param name="NULL">always pass null--Unused parameter</param>
		/// <param name="lParam">string lParam</param>
		/// <returns></returns>
		unsafe int INativeScintilla.SendMessageDirect(uint msg, VOID NULL, string lParam)
		{
			//  Just as when retrieving we make to convert .NET's
			//  UTF-16 strings into a UTF-8 encoded byte array.
			fixed (byte* bp = _encoding.GetBytes(ZeroTerminated(lParam)))
				return (int)_ns.SendMessageDirect(msg, IntPtr.Zero, (IntPtr)bp);
		}

		/// <summary>
		/// Handles Scintilla Call Style:
		///    (string,string)    
		/// 
		/// Notes:
		///    Used by SCI_SETPROPERTY
		/// </summary>
		/// <param name="msg">Scintilla Message Number</param>
		/// <param name="wParam">string wParam</param>
		/// <param name="lParam">string lParam</param>
		/// <returns></returns>
		unsafe int INativeScintilla.SendMessageDirect(uint msg, string wParam, string lParam)
		{
			fixed (byte* bpw = _encoding.GetBytes(ZeroTerminated(wParam)))
			fixed (byte* bpl = _encoding.GetBytes(ZeroTerminated(lParam)))
				return (int)_ns.SendMessageDirect(msg, (IntPtr)bpw, (IntPtr)bpl);
		}

		/// <summary>
		/// Handles Scintilla Call Style:
		///    (string,stringresult)    
		/// 
		/// Notes:
		///  This one is used specifically by SCI_GETPROPERTY and SCI_GETPROPERTYEXPANDED
		///  so it assumes it's usage
		/// 
		/// </summary>
		/// <param name="msg">Scintilla Message Number</param>
		/// <param name="wParam">string wParam</param>
		/// <param name="stringResult">Stringresult output</param>
		/// <returns></returns>
		unsafe int INativeScintilla.SendMessageDirect(uint msg, string wParam, out string stringResult)
		{
			IntPtr ret;

			fixed (byte* bpw = _encoding.GetBytes(ZeroTerminated(wParam)))
			{
				int length = (int)_ns.SendMessageDirect(msg, (IntPtr)bpw, IntPtr.Zero);


				byte[] buffer = new byte[length + 1];

				fixed (byte* bpl = buffer)
					ret = _ns.SendMessageDirect(msg, (IntPtr)bpw, (IntPtr)bpl);

				stringResult = _encoding.GetString(buffer, 0, length);
			}

			return (int)ret;
		}

		/// <summary>
		/// Handles Scintilla Call Style:
		///    (string,int)    
		/// </summary>
		/// <param name="msg">Scintilla Message Number</param>
		/// <param name="wParam">string wParam</param>
		/// <param name="lParam">int lParam</param>
		/// <returns></returns>
		unsafe int INativeScintilla.SendMessageDirect(uint msg, string wParam, int lParam)
		{
			fixed (byte* bp = _encoding.GetBytes(ZeroTerminated(wParam)))
				return (int)_ns.SendMessageDirect(msg, (IntPtr)bp, (IntPtr)lParam);
		}

		/// <summary>
		/// Handles Scintilla Call Style:
		///    (string,)    
		/// </summary>
		/// <param name="msg">Scintilla Message Number</param>
		/// <param name="wParam">string wParam</param>
		/// <returns></returns>
		unsafe int INativeScintilla.SendMessageDirect(uint msg, string wParam)
		{
			fixed (byte* bp = _encoding.GetBytes(ZeroTerminated(wParam)))
				return (int)_ns.SendMessageDirect(msg, (IntPtr)bp, IntPtr.Zero);
		}

		private static string ZeroTerminated(string param)
		{
			if (string.IsNullOrEmpty(param))
				return "\0";
			else if (!param.EndsWith("\0"))
				return param + "\0";
			return param;
		}

		#endregion

		#region Text Retrieval and Modification
		int INativeScintilla.GetText(int length, out string text)
		{
			return (int)_ns.SendMessageDirect(Constants.SCI_GETTEXT, (IntPtr)length, out text, length);
		}

		void INativeScintilla.SetText(string text)
		{
			_ns.SendMessageDirect(Constants.SCI_SETTEXT, VOID.NULL, text);
		}

		void INativeScintilla.SetSavePoint()
		{
			_ns.SendMessageDirect(Constants.SCI_SETSAVEPOINT, 0, 0);
		}

		int INativeScintilla.GetLine(int line, out string text)
		{
			int length = _ns.SendMessageDirect(Constants.SCI_GETLINE, line, 0);
			if (length == 0)
			{
				text = string.Empty;
				return 0;
			}

			return _ns.SendMessageDirect(Constants.SCI_GETLINE, (IntPtr)line, out text, length);
		}

		void INativeScintilla.ReplaceSel(string text)
		{
			_ns.SendMessageDirect(Constants.SCI_REPLACESEL, VOID.NULL, text);
		}

		void INativeScintilla.SetReadOnly(bool readOnly)
		{
			_ns.SendMessageDirect(Constants.SCI_SETREADONLY, readOnly, 0);
		}

		bool INativeScintilla.GetReadOnly()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETREADONLY, 0, 0) != 0;
		}

		unsafe int INativeScintilla.GetTextRange(ref TextRange tr)
		{
			fixed(TextRange* trp = &tr)
				return (int)_ns.SendMessageDirect(Constants.SCI_GETTEXTRANGE, IntPtr.Zero, (IntPtr)trp);
		}

		void INativeScintilla.Allocate(int bytes)
		{
			_ns.SendMessageDirect(Constants.SCI_ALLOCATE, bytes, 0);
		}

		void INativeScintilla.AddText(int length, string s)
		{
			_ns.SendMessageDirect(Constants.SCI_ADDTEXT, length, s);
		}

		unsafe void INativeScintilla.AddStyledText(int length, byte[] s)
		{
			fixed(byte* bp = s)
				_ns.SendMessageDirect(Constants.SCI_ADDSTYLEDTEXT, (IntPtr)length, (IntPtr)bp);
		}

		void INativeScintilla.AppendText(int length, string s)
		{
			_ns.SendMessageDirect(Constants.SCI_APPENDTEXT, length, s);
		}

		void INativeScintilla.InsertText(int pos, string text)
		{
			_ns.SendMessageDirect(Constants.SCI_INSERTTEXT, pos, text);
		}

		void INativeScintilla.ClearAll()
		{
			_ns.SendMessageDirect(Constants.SCI_CLEARALL, 0, 0);
		}

		void INativeScintilla.ClearDocumentStyle()
		{
			_ns.SendMessageDirect(Constants.SCI_CLEARDOCUMENTSTYLE, 0, 0);
		}

		char INativeScintilla.GetCharAt(int position)
		{
			return (char)_ns.SendMessageDirect(Constants.SCI_GETCHARAT, position, 0);
		}

		byte INativeScintilla.GetStyleAt(int position)
		{
			return (byte)_ns.SendMessageDirect(Constants.SCI_GETSTYLEAT, position, 0);
		}

		unsafe void INativeScintilla.GetStyledText(ref TextRange tr)
		{
			fixed(TextRange* trp = &tr)
				_ns.SendMessageDirect(Constants.SCI_GETSTYLEDTEXT, IntPtr.Zero, (IntPtr)trp);
		}

		void INativeScintilla.SetStyleBits(int bits)
		{
			_ns.SendMessageDirect(Constants.SCI_SETSTYLEBITS, bits, 0);
		}

		int INativeScintilla.GetStyleBits()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETSTYLEBITS, 0, 0);
		}

		//	These 3 methods don't really make sense when targeting .NET
		//	which stores strings as UTF-16. I guess if you really wanted
		//	to have it it would make more sense to have the strings 
		//	as byte[]s instead.
		int INativeScintilla.TargetAsUtf8(out string s)
		{
			throw new NotSupportedException();
		}

		int INativeScintilla.EncodeFromUtf8(string utf8, out string encoded)
		{
			throw new NotSupportedException();
		}

		int INativeScintilla.SetLengthForEncode(int bytes)
		{
			throw new NotSupportedException();
		}
		#endregion

		#region Find Text
		unsafe int INativeScintilla.FindText(int searchFlags, ref TextToFind ttf)
		{
			fixed(TextToFind* ttfp = &ttf)
				return (int)_ns.SendMessageDirect(Constants.SCI_FINDTEXT, IntPtr.Zero, (IntPtr)ttfp);
		}

		void INativeScintilla.SearchAnchor()
		{
			_ns.SendMessageDirect(Constants.SCI_SEARCHANCHOR, 0, 0);
		}

		int INativeScintilla.SearchNext(int searchFlags, string text)
		{
			return _ns.SendMessageDirect(Constants.SCI_SEARCHNEXT, searchFlags, text);
		}

		int INativeScintilla.SearchPrev(int searchFlags, string text)
		{
			return _ns.SendMessageDirect(Constants.SCI_SEARCHPREV, searchFlags, text);
		}

		void INativeScintilla.SetTargetStart(int pos)
		{
			_ns.SendMessageDirect(Constants.SCI_SETTARGETSTART, pos, 0);
		}

		int INativeScintilla.GetTargetStart()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETTARGETSTART, 0, 0);
		}

		void INativeScintilla.SetTargetEnd(int pos)
		{
			_ns.SendMessageDirect(Constants.SCI_SETTARGETEND, pos, 0);
		}

		int INativeScintilla.GetTargetEnd()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETTARGETEND, 0, 0);
		}

		void INativeScintilla.TargetFromSelection()
		{
			_ns.SendMessageDirect(Constants.SCI_TARGETFROMSELECTION, 0, 0);
		}

		void INativeScintilla.SetSearchFlags(int searchFlags)
		{
			_ns.SendMessageDirect(Constants.SCI_SETSEARCHFLAGS, searchFlags, 0);
		}

		int INativeScintilla.GetSearchFlags()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETSEARCHFLAGS, 0, 0);
		}

		int INativeScintilla.SearchInTarget(int length, string text)
		{
			return _ns.SendMessageDirect(Constants.SCI_SEARCHINTARGET, length, text);
		}

		int INativeScintilla.ReplaceTarget(int length, string text)
		{
			return _ns.SendMessageDirect(Constants.SCI_REPLACETARGET, length, text);
		}

		int INativeScintilla.ReplaceTargetRE(int length, string text)
		{
			return _ns.SendMessageDirect(Constants.SCI_REPLACETARGETRE, length, text);
		}
		#endregion

		#region OverType
		void INativeScintilla.SetOvertype(bool overType)
		{
			_ns.SendMessageDirect(Constants.SCI_SETOVERTYPE, overType, 0);
		}

		bool INativeScintilla.GetOvertype()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETOVERTYPE, 0, 0) != 0;
		}
		#endregion

		#region Cut, copy and paste
		void INativeScintilla.Cut()
		{
			_ns.SendMessageDirect(Constants.SCI_CUT, 0, 0);
		}

		void INativeScintilla.Copy()
		{
			_ns.SendMessageDirect(Constants.SCI_COPY, 0, 0);
		}

		void INativeScintilla.Paste()
		{
			_ns.SendMessageDirect(Constants.SCI_PASTE, 0, 0);
		}

		void INativeScintilla.Clear()
		{
			_ns.SendMessageDirect(Constants.SCI_CLEAR, 0, 0);
		}

		bool INativeScintilla.CanPaste()
		{
			return _ns.SendMessageDirect(Constants.SCI_CANPASTE, 0, 0) != 0;
		}

		void INativeScintilla.CopyRange(int start, int end)
		{
			_ns.SendMessageDirect(Constants.SCI_COPYRANGE, start, end);
		}

		void INativeScintilla.CopyText(int length, string text)
		{
			_ns.SendMessageDirect(Constants.SCI_COPYTEXT, length, text);
		}

		void INativeScintilla.SetPasteConvertEndings(bool convert)
		{
			_ns.SendMessageDirect(Constants.SCI_SETPASTECONVERTENDINGS, convert, 0);
		}

		bool INativeScintilla.GetPasteConvertEndings()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETPASTECONVERTENDINGS, 0, 0) != 0;
		}
		#endregion

		#region Error Handling
		void INativeScintilla.SetStatus(int status)
		{
			_ns.SendMessageDirect(Constants.SCI_SETSTATUS, status, 0);
		}

		int INativeScintilla.GetStatus()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETSTATUS, 0, 0);
		}
		#endregion

		#region Undo and Redo
		void INativeScintilla.Undo()
		{
			_ns.SendMessageDirect(Constants.SCI_UNDO, 0, 0);
		}

		bool INativeScintilla.CanUndo()
		{
			return _ns.SendMessageDirect(Constants.SCI_CANUNDO, 0, 0) != 0;
		}

		void INativeScintilla.EmptyUndoBuffer()
		{
			_ns.SendMessageDirect(Constants.SCI_EMPTYUNDOBUFFER, 0, 0);
		}

		void INativeScintilla.Redo()
		{
			_ns.SendMessageDirect(Constants.SCI_REDO, 0, 0);
		}

		bool INativeScintilla.CanRedo()
		{
			return _ns.SendMessageDirect(Constants.SCI_CANREDO, 0, 0) != 0;
		}

		void INativeScintilla.SetUndoCollection(bool collectUndo)
		{
			_ns.SendMessageDirect(Constants.SCI_SETUNDOCOLLECTION, collectUndo, 0);
		}

		bool INativeScintilla.GetUndoCollection()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETUNDOCOLLECTION, 0, 0) != 0;
		}

		void INativeScintilla.BeginUndoAction()
		{
			_ns.SendMessageDirect(Constants.SCI_BEGINUNDOACTION, 0, 0);
		}

		void INativeScintilla.EndUndoAction()
		{
			_ns.SendMessageDirect(Constants.SCI_ENDUNDOACTION, 0, 0);
		}
		#endregion

		#region Selection and information
		int INativeScintilla.GetTextLength()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETTEXTLENGTH, 0, 0);
		}

		int INativeScintilla.GetLength()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETLENGTH, 0, 0);
		}

		int INativeScintilla.GetLineCount()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETLINECOUNT, 0, 0);
		}

		int INativeScintilla.GetFirstVisibleLine()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETFIRSTVISIBLELINE, 0, 0);
		}

		int INativeScintilla.LinesOnScreen()
		{
			return _ns.SendMessageDirect(Constants.SCI_LINESONSCREEN, 0, 0);
		}

		bool INativeScintilla.GetModify()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETMODIFY, 0, 0) != 0;
		}

		void INativeScintilla.SetSel(int anchorPos, int currentPos)
		{
			_ns.SendMessageDirect(Constants.SCI_SETSEL, anchorPos, currentPos);
		}

		void INativeScintilla.GotoPos(int position)
		{
			_ns.SendMessageDirect(Constants.SCI_GOTOPOS, position, 0);
		}

		void INativeScintilla.GotoLine(int line)
		{
			_ns.SendMessageDirect(Constants.SCI_GOTOLINE, line, 0);
		}

		void INativeScintilla.SetCurrentPos(int position)
		{
			_ns.SendMessageDirect(Constants.SCI_SETCURRENTPOS, position, 0);
		}

		int INativeScintilla.GetCurrentPos()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETCURRENTPOS, 0, 0);
		}

		void INativeScintilla.SetAnchor(int position)
		{
			_ns.SendMessageDirect(Constants.SCI_SETANCHOR, position, 0);
		}

		int INativeScintilla.GetAnchor()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETANCHOR, 0, 0);
		}

		void INativeScintilla.SetSelectionStart(int position)
		{
			_ns.SendMessageDirect(Constants.SCI_SETSELECTIONSTART, position, 0);
		}

		int INativeScintilla.GetSelectionStart()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETSELECTIONSTART, 0, 0);
		}

		void INativeScintilla.SetSelectionEnd(int position)
		{
			_ns.SendMessageDirect(Constants.SCI_SETSELECTIONEND, position, 0);
		}

		int INativeScintilla.GetSelectionEnd()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETSELECTIONEND, 0, 0);
		}

		void INativeScintilla.SelectAll()
		{
			_ns.SendMessageDirect(Constants.SCI_SELECTALL, 0, 0);
		}

		int INativeScintilla.LineFromPosition(int pos)
		{
			return _ns.SendMessageDirect(Constants.SCI_LINEFROMPOSITION, pos, 0);
		}

		int INativeScintilla.PositionFromLine(int line)
		{
			return _ns.SendMessageDirect(Constants.SCI_POSITIONFROMLINE, line, 0);
		}

		int INativeScintilla.GetLineEndPosition(int line)
		{
			return _ns.SendMessageDirect(Constants.SCI_GETLINEENDPOSITION, line, 0);
		}

		int INativeScintilla.LineLength(int line)
		{
			return _ns.SendMessageDirect(Constants.SCI_LINELENGTH, line, 0);
		}

		int INativeScintilla.GetColumn(int position)
		{
			return _ns.SendMessageDirect(Constants.SCI_GETCOLUMN, position, 0);
		}

		int INativeScintilla.FindColumn(int line, int column)
		{
			return _ns.SendMessageDirect(Constants.SCI_FINDCOLUMN, line, column);
		}

		int INativeScintilla.PositionFromPoint(int x, int y)
		{
			return _ns.SendMessageDirect(Constants.SCI_POSITIONFROMPOINT, x, y);
		}

		int INativeScintilla.PositionFromPointClose(int x, int y)
		{
			return _ns.SendMessageDirect(Constants.SCI_POSITIONFROMPOINTCLOSE, x, y);
		}

		int INativeScintilla.PointXFromPosition(int position)
		{
			return _ns.SendMessageDirect(Constants.SCI_POINTXFROMPOSITION, VOID.NULL, position);
		}

		int INativeScintilla.PointYFromPosition(int position)
		{
			return _ns.SendMessageDirect(Constants.SCI_POINTYFROMPOSITION, VOID.NULL, position);
		}

		void INativeScintilla.HideSelection(bool hide)
		{
			_ns.SendMessageDirect(Constants.SCI_HIDESELECTION, hide, 0);
		}

		void INativeScintilla.GetSelText(out string text)
		{
			int length = _ns.GetSelectionEnd() - _ns.GetSelectionStart() + 1;
			_ns.SendMessageDirect(Constants.SCI_GETSELTEXT, IntPtr.Zero, out text, length);
		}

		int INativeScintilla.GetCurLine(int textLen, out string text)
		{
			return _ns.SendMessageDirect(Constants.SCI_GETCURLINE, (IntPtr)textLen, out text, textLen);
		}

		bool INativeScintilla.SelectionIsRectangle()
		{
			return _ns.SendMessageDirect(Constants.SCI_SELECTIONISRECTANGLE, 0, 0) != 0;
		}

		void INativeScintilla.SetSelectionMode(int mode)
		{
			_ns.SendMessageDirect(Constants.SCI_SETSELECTIONMODE, mode, 0);
		}

		int INativeScintilla.GetSelectionMode()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETSELECTIONMODE, 0, 0);
		}

		int INativeScintilla.GetLineSelStartPosition(int line)
		{
			return _ns.SendMessageDirect(Constants.SCI_GETLINESELSTARTPOSITION, line, 0);
		}

		int INativeScintilla.GetLineSelEndPosition(int line)
		{
			return _ns.SendMessageDirect(Constants.SCI_GETLINESELENDPOSITION, line, 0);
		}

		void INativeScintilla.MoveCaretInsideView()
		{
			_ns.SendMessageDirect(Constants.SCI_MOVECARETINSIDEVIEW, 0, 0);
		}

		int INativeScintilla.WordEndPosition(int position, bool onlyWordCharacters)
		{
			return _ns.SendMessageDirect(Constants.SCI_WORDENDPOSITION, position, onlyWordCharacters);
		}

		int INativeScintilla.WordStartPosition(int position, bool onlyWordCharacters)
		{
			return _ns.SendMessageDirect(Constants.SCI_WORDSTARTPOSITION, position, onlyWordCharacters);
		}

		int INativeScintilla.PositionBefore(int position)
		{
			return _ns.SendMessageDirect(Constants.SCI_POSITIONBEFORE, position, 0);
		}

		int INativeScintilla.PositionAfter(int position)
		{
			return _ns.SendMessageDirect(Constants.SCI_POSITIONAFTER, position, 0);
		}

		int INativeScintilla.TextWidth(int styleNumber, string text)
		{
			return _ns.SendMessageDirect(Constants.SCI_TEXTWIDTH, styleNumber, text);
		}

		int INativeScintilla.TextHeight(int line)
		{
			return _ns.SendMessageDirect(Constants.SCI_TEXTHEIGHT, line, 0);
		}

		void INativeScintilla.ChooseCaretX()
		{
			_ns.SendMessageDirect(Constants.SCI_CHOOSECARETX, 0, 0);
		}
		#endregion

		#region Scrolling and automatic scrolling
		void INativeScintilla.LineScroll(int columns, int lines)
		{
			_ns.SendMessageDirect(Constants.SCI_LINESCROLL, columns, lines);
		}

		void INativeScintilla.ScrollCaret()
		{
			_ns.SendMessageDirect(Constants.SCI_SCROLLCARET, 0, 0);
		}

		void INativeScintilla.SetXCaretPolicy(int caretPolicy, int caretSlop)
		{
			_ns.SendMessageDirect(Constants.SCI_SETXCARETPOLICY, caretPolicy, caretSlop);
		}

		void INativeScintilla.SetYCaretPolicy(int caretPolicy, int caretSlop)
		{
			_ns.SendMessageDirect(Constants.SCI_SETYCARETPOLICY, caretPolicy, caretSlop);
		}

		void INativeScintilla.SetVisiblePolicy(int visiblePolicy, int visibleSlop)
		{
			_ns.SendMessageDirect(Constants.SCI_SETVISIBLEPOLICY, visiblePolicy, visibleSlop);
		}

		void INativeScintilla.SetHScrollBar(bool visible)
		{
			_ns.SendMessageDirect(Constants.SCI_SETHSCROLLBAR, visible, 0);
		}

		bool INativeScintilla.GetHScrollBar()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETHSCROLLBAR, 0, 0) != 0;
		}

		void INativeScintilla.SetVScrollBar(bool visible)
		{
			_ns.SendMessageDirect(Constants.SCI_SETVSCROLLBAR, visible, 0);
		}

		bool INativeScintilla.GetVScrollBar()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETVSCROLLBAR, 0, 0) != 0;
		}

		int INativeScintilla.GetXOffset()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETXOFFSET, 0, 0);
		}

		void INativeScintilla.SetXOffset(int xOffset)
		{
			_ns.SendMessageDirect(Constants.SCI_SETXOFFSET, xOffset, 0);
		}

		void INativeScintilla.SetScrollWidth(int pixelWidth)
		{
			_ns.SendMessageDirect(Constants.SCI_SETSCROLLWIDTH, pixelWidth, 0);
		}

		int INativeScintilla.GetScrollWidth()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETSCROLLWIDTH, 0, 0);
		}

		void INativeScintilla.SetEndAtLastLine(bool endAtLastLine)
		{
			_ns.SendMessageDirect(Constants.SCI_SETENDATLASTLINE, endAtLastLine, 0);
		}

		bool INativeScintilla.GetEndAtLastLine()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETENDATLASTLINE, 0, 0) != 0;
		}
		#endregion

		#region White space
		void INativeScintilla.SetViewWS(int wsMode)
		{
			_ns.SendMessageDirect(Constants.SCI_SETVIEWWS, wsMode, 0);
		}

		int INativeScintilla.GetViewWS()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETVIEWWS, 0, 0);
		}

		void INativeScintilla.SetWhitespaceFore(bool useWhitespaceForeColour, int colour)
		{
			_ns.SendMessageDirect(Constants.SCI_SETWHITESPACEFORE, useWhitespaceForeColour, colour);
		}

		void INativeScintilla.SetWhitespaceBack(bool useWhitespaceForeColour, int colour)
		{
			_ns.SendMessageDirect(Constants.SCI_SETWHITESPACEBACK, useWhitespaceForeColour, colour);
		}
		#endregion

		#region Cursor
		void INativeScintilla.SetCursor(int curType)
		{
			_ns.SendMessageDirect(Constants.SCI_SETCURSOR, curType, 0);
		}

		int INativeScintilla.GetCursor()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETCURSOR, 0, 0);
		}
		#endregion

		#region Mouse capture
		void INativeScintilla.SetMouseDownCaptures(bool captures)
		{
			_ns.SendMessageDirect(Constants.SCI_SETMOUSEDOWNCAPTURES, captures, 0);
		}

		bool INativeScintilla.GetMouseDownCaptures()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETMOUSEDOWNCAPTURES, 0, 0) != 0;
		}
		#endregion

		#region Line endings
		void INativeScintilla.SetEolMode(int eolMode)
		{
			_ns.SendMessageDirect(Constants.SCI_SETEOLMODE, eolMode, 0);
		}

		int INativeScintilla.GetEolMode()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETEOLMODE, 0, 0);
		}

		void INativeScintilla.ConvertEols(int eolMode)
		{
			_ns.SendMessageDirect(Constants.SCI_CONVERTEOLS, eolMode, 0);
		}

		void INativeScintilla.SetViewEol(bool visible)
		{
			_ns.SendMessageDirect(Constants.SCI_SETVIEWEOL, visible, 0);
		}

		bool INativeScintilla.GetViewEol()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETVIEWEOL, 0, 0) != 0;
		}
		#endregion

		#region Styling
		int INativeScintilla.GetEndStyled()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETENDSTYLED, 0, 0);
		}

		void INativeScintilla.StartStyling(int position, int mask)
		{
			_ns.SendMessageDirect(Constants.SCI_STARTSTYLING, position, mask);
		}

		void INativeScintilla.SetStyling(int length, int style)
		{
			_ns.SendMessageDirect(Constants.SCI_SETSTYLING, length, style);
		}

		void INativeScintilla.SetStylingEx(int length, string styles)
		{
			_ns.SendMessageDirect(Constants.SCI_SETSTYLINGEX, length, styles);
		}

		void INativeScintilla.SetLineState(int line, int value)
		{
			_ns.SendMessageDirect(Constants.SCI_SETLINESTATE, line, value);
		}

		int INativeScintilla.GetLineState(int line)
		{
			return _ns.SendMessageDirect(Constants.SCI_GETLINESTATE, line, 0);
		}

		int INativeScintilla.GetMaxLineState()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETMAXLINESTATE, 0, 0);
		}
		#endregion

		#region Style definition
		void INativeScintilla.StyleResetDefault()
		{
			_ns.SendMessageDirect(Constants.SCI_STYLERESETDEFAULT, 0, 0);
		}

		void INativeScintilla.StyleClearAll()
		{
			_ns.SendMessageDirect(Constants.SCI_STYLECLEARALL, 0, 0);
		}

		void INativeScintilla.StyleSetFont(int styleNumber, string fontName)
		{
			_ns.SendMessageDirect(Constants.SCI_STYLESETFONT, styleNumber, fontName);
		}

		void INativeScintilla.StyleGetFont(int styleNumber, out string fontName)
		{
			string s;
			int length = _ns.SendMessageDirect(Constants.SCI_STYLEGETFONT, 0, 0);
			_ns.SendMessageDirect(Constants.SCI_STYLEGETFONT, (IntPtr)styleNumber, out s, length);
			fontName = s;
		}

		void INativeScintilla.StyleSetSize(int styleNumber, int sizeInPoints)
		{
			_ns.SendMessageDirect(Constants.SCI_STYLESETSIZE, styleNumber, sizeInPoints);
		}

		int INativeScintilla.StyleGetSize(int styleNumber)
		{
			return _ns.SendMessageDirect(Constants.SCI_STYLEGETSIZE, 0, 0);
		}

		void INativeScintilla.StyleSetBold(int styleNumber, bool bold)
		{
			_ns.SendMessageDirect(Constants.SCI_STYLESETBOLD, styleNumber, bold);
		}

		bool INativeScintilla.StyleGetBold(int styleNumber)
		{
			return _ns.SendMessageDirect(Constants.SCI_STYLEGETBOLD, styleNumber, 0) != 0;
		}

		void INativeScintilla.StyleSetItalic(int styleNumber, bool italic)
		{
			_ns.SendMessageDirect(Constants.SCI_STYLESETITALIC, styleNumber, italic);
		}

		bool INativeScintilla.StyleGetItalic(int styleNumber)
		{
			return _ns.SendMessageDirect(Constants.SCI_STYLEGETITALIC, styleNumber, 0) != 0;
		}

		void INativeScintilla.StyleSetUnderline(int styleNumber, bool underline)
		{
			_ns.SendMessageDirect(Constants.SCI_STYLESETUNDERLINE, styleNumber, underline);
		}

		bool INativeScintilla.StyleGetUnderline(int styleNumber)
		{
			return _ns.SendMessageDirect(Constants.SCI_STYLEGETUNDERLINE, styleNumber, 0) != 0;
		}

		void INativeScintilla.StyleSetFore(int styleNumber, int colour)
		{
			_ns.SendMessageDirect(Constants.SCI_STYLESETFORE, styleNumber, colour);
		}

		int INativeScintilla.StyleGetFore(int styleNumber)
		{
			return _ns.SendMessageDirect(Constants.SCI_STYLEGETFORE, styleNumber, 0);
		}

		void INativeScintilla.StyleSetBack(int styleNumber, int colour)
		{
			_ns.SendMessageDirect(Constants.SCI_STYLESETBACK, styleNumber, colour);
		}

		int INativeScintilla.StyleGetBack(int styleNumber)
		{
			return _ns.SendMessageDirect(Constants.SCI_STYLEGETBACK, styleNumber, 0);
		}

		void INativeScintilla.StyleSetEOLFilled(int styleNumber, bool eolFilled)
		{
			_ns.SendMessageDirect(Constants.SCI_STYLESETEOLFILLED, styleNumber, eolFilled);
		}

		bool INativeScintilla.StyleGetEOLFilled(int styleNumber)
		{
			return _ns.SendMessageDirect(Constants.SCI_STYLEGETEOLFILLED, styleNumber, 0) != 0;
		}

		void INativeScintilla.StyleSetCharacterSet(int styleNumber, int charSet)
		{
			_ns.SendMessageDirect(Constants.SCI_STYLESETCHARACTERSET, styleNumber, charSet);
		}

		int INativeScintilla.StyleGetCharacterSet(int styleNumber)
		{
			return _ns.SendMessageDirect(Constants.SCI_STYLEGETCHARACTERSET, styleNumber, 0);
		}

		void INativeScintilla.StyleSetCase(int styleNumber, int caseMode)
		{
			_ns.SendMessageDirect(Constants.SCI_STYLESETCASE, styleNumber, caseMode);
		}

		int INativeScintilla.StyleGetCase(int styleNumber)
		{
			return _ns.SendMessageDirect(Constants.SCI_STYLEGETCASE, styleNumber, 0);
		}

		void INativeScintilla.StyleSetVisible(int styleNumber, bool visible)
		{
			_ns.SendMessageDirect(Constants.SCI_STYLESETVISIBLE, styleNumber, visible);
		}

		bool INativeScintilla.StyleGetVisible(int styleNumber)
		{
			return _ns.SendMessageDirect(Constants.SCI_STYLEGETVISIBLE, styleNumber, 0) != 0;
		}

		void INativeScintilla.StyleSetChangeable(int styleNumber, bool changeable)
		{
			_ns.SendMessageDirect(Constants.SCI_STYLESETCHANGEABLE, styleNumber, changeable);
		}

		bool INativeScintilla.StyleGetChangeable(int styleNumber)
		{
			return _ns.SendMessageDirect(Constants.SCI_STYLEGETCHANGEABLE, styleNumber, 0) != 0;
		}

		void INativeScintilla.StyleSetHotSpot(int styleNumber, bool hotspot)
		{
			_ns.SendMessageDirect(Constants.SCI_STYLESETHOTSPOT, styleNumber, hotspot);
		}

		bool INativeScintilla.StyleGetHotSpot(int styleNumber)
		{
			return _ns.SendMessageDirect(Constants.SCI_STYLEGETHOTSPOT, styleNumber, 0) != 0;
		}

		int INativeScintilla.GetHotSpotActiveBack()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETHOTSPOTACTIVEBACK, 0, 0);
		}

		int INativeScintilla.GetHotSpotActiveFore()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETHOTSPOTACTIVEFORE, 0, 0);
		}

		bool INativeScintilla.GetHotSpotActiveUnderline()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETHOTSPOTACTIVEUNDERLINE, 0, 0) != 0;
		}

		bool INativeScintilla.GetHotSpotSingleLine()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETHOTSPOTSINGLELINE, 0, 0) != 0;
		}

		int INativeScintilla.IndicatorEnd(int indicator, int position)
		{
			return _ns.SendMessageDirect(Constants.SCI_INDICATOREND, indicator, position);
		}

		int INativeScintilla.IndicatorStart(int indicator, int position)
		{
			return _ns.SendMessageDirect(Constants.SCI_INDICATORSTART, indicator, position);
		}

		int INativeScintilla.IndicatorValueAt(int indicator, int position)
		{
			return _ns.SendMessageDirect(Constants.SCI_INDICATORVALUEAT, indicator, position);
		}

		bool INativeScintilla.IndicGetUnder(int indicatorNumber)
		{
			return _ns.SendMessageDirect(Constants.SCI_INDICGETUNDER, indicatorNumber, 0) != 0;
		}
		
		void INativeScintilla.IndicSetUnder(int indicatorNumber, bool under)
		{
			_ns.SendMessageDirect(Constants.SCI_INDICSETUNDER, indicatorNumber, under);
		}

		void INativeScintilla.SetIndicatorValue(int value)
		{
			_ns.SendMessageDirect(Constants.SCI_SETINDICATORVALUE, value, 0);
		}

		void INativeScintilla.SetPositionCache(int size)
		{
			_ns.SendMessageDirect(Constants.SCI_SETPOSITIONCACHE, size, 0);
		}


		#endregion

		#region Caret, selection, and hotspot styles
		void INativeScintilla.SetSelFore(bool useSelectionForeColour, int colour)
		{
			_ns.SendMessageDirect(Constants.SCI_SETSELFORE, useSelectionForeColour, colour);
		}

		void INativeScintilla.SetSelBack(bool useSelectionBackColour, int colour)
		{
			_ns.SendMessageDirect(Constants.SCI_SETSELBACK, useSelectionBackColour, colour);
		}

		void INativeScintilla.SetCaretFore(int alpha)
		{
			_ns.SendMessageDirect(Constants.SCI_SETCARETFORE, alpha, 0);
		}

		int INativeScintilla.GetCaretFore()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETCARETFORE, 0, 0);
		}

		void INativeScintilla.SetCaretLineVisible(bool colour)
		{
			_ns.SendMessageDirect(Constants.SCI_SETCARETLINEVISIBLE, colour, 0);
		}

		bool INativeScintilla.GetCaretLineVisible()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETCARETLINEVISIBLE, 0, 0) != 0;
		}

		void INativeScintilla.SetCaretLineBack(int show)
		{
			_ns.SendMessageDirect(Constants.SCI_SETCARETLINEBACK, show, 0);
		}

		int INativeScintilla.GetCaretLineBack()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETCARETLINEBACK, 0, 0);
		}

		void INativeScintilla.SetCaretLineBackAlpha(int alpha)
		{
			_ns.SendMessageDirect(Constants.SCI_SETCARETLINEBACKALPHA, alpha, 0);
		}

		int INativeScintilla.GetCaretLineBackAlpha()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETCARETLINEBACKALPHA, 0, 0);
		}

		void INativeScintilla.SetCaretPeriod(int milliseconds)
		{
			_ns.SendMessageDirect(Constants.SCI_SETCARETPERIOD, milliseconds, 0);
		}

		int INativeScintilla.GetCaretPeriod()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETCARETPERIOD, 0, 0);
		}

		void INativeScintilla.SetCaretWidth(int pixels)
		{
			_ns.SendMessageDirect(Constants.SCI_SETCARETWIDTH, pixels, 0);
		}

		int INativeScintilla.GetCaretWidth()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETCARETWIDTH, 0, 0);
		}

		void INativeScintilla.SetHotspotActiveFore(bool useHotSpotForeColour, int colour)
		{
			_ns.SendMessageDirect(Constants.SCI_SETHOTSPOTACTIVEFORE, useHotSpotForeColour, colour);
		}

		void INativeScintilla.SetHotspotActiveBack(bool useHotSpotBackColour, int colour)
		{
			_ns.SendMessageDirect(Constants.SCI_SETHOTSPOTACTIVEBACK, useHotSpotBackColour, colour);
		}

		void INativeScintilla.SetHotspotActiveUnderline(bool underline)
		{
			_ns.SendMessageDirect(Constants.SCI_SETHOTSPOTACTIVEUNDERLINE, underline, 0);
		}

		void INativeScintilla.SetHotspotSingleLine(bool singleLine)
		{
			_ns.SendMessageDirect(Constants.SCI_SETHOTSPOTSINGLELINE, singleLine, 0);
		}

		void INativeScintilla.SetControlCharSymbol(int symbol)
		{
			_ns.SendMessageDirect(Constants.SCI_SETCONTROLCHARSYMBOL, symbol, 0);
		}

		int INativeScintilla.GetControlCharSymbol()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETCONTROLCHARSYMBOL, 0, 0);
		}

		bool INativeScintilla.GetCaretSticky()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETCARETSTICKY, 0, 0) != 0;
		}

		void INativeScintilla.SetCaretSticky(bool useCaretStickyBehaviour)
		{
			_ns.SendMessageDirect(Constants.SCI_SETCARETSTICKY, useCaretStickyBehaviour, 0);
		}

		void INativeScintilla.ToggleCaretSticky()
		{
			_ns.SendMessageDirect(Constants.SCI_TOGGLECARETSTICKY, 0, 0);
		}

		int INativeScintilla.GetCaretStyle()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETCARETSTYLE, 0, 0);
		}

		void INativeScintilla.SetCaretStyle(int style)
		{
			_ns.SendMessageDirect(Constants.SCI_SETCARETSTYLE, style, 0);
		}


		#endregion

		#region Margins
		void INativeScintilla.SetMarginTypeN(int margin, int type)
		{
			_ns.SendMessageDirect(Constants.SCI_SETMARGINTYPEN, margin, type);
		}

		int INativeScintilla.GetMarginTypeN(int margin)
		{
			return _ns.SendMessageDirect(Constants.SCI_GETMARGINTYPEN, margin, 0);
		}

		void INativeScintilla.SetMarginWidthN(int margin, int pixelWidth)
		{
			_ns.SendMessageDirect(Constants.SCI_SETMARGINWIDTHN, margin, pixelWidth);
		}

		int INativeScintilla.GetMarginWidthN(int margin)
		{
			return _ns.SendMessageDirect(Constants.SCI_GETMARGINWIDTHN, margin, 0);
		}

		void INativeScintilla.SetMarginMaskN(int margin, int mask)
		{
			_ns.SendMessageDirect(Constants.SCI_SETMARGINMASKN, margin, mask);
		}

		int INativeScintilla.GetMarginMaskN(int margin)
		{
			return _ns.SendMessageDirect(Constants.SCI_GETMARGINMASKN, margin, 0);
		}

		void INativeScintilla.SetMarginSensitiveN(int margin, bool sensitive)
		{
			_ns.SendMessageDirect(Constants.SCI_SETMARGINSENSITIVEN, margin, sensitive);
		}

		bool INativeScintilla.GetMarginSensitiveN(int margin)
		{
			return _ns.SendMessageDirect(Constants.SCI_GETMARGINSENSITIVEN, margin, 0) != 0;
		}

		void INativeScintilla.SetMarginLeft(int pixels)
		{
			_ns.SendMessageDirect(Constants.SCI_SETMARGINLEFT, 0, pixels);
		}

		int INativeScintilla.GetMarginLeft()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETMARGINLEFT, 0, 0);
		}

		void INativeScintilla.SetMarginRight(int pixels)
		{
			_ns.SendMessageDirect(Constants.SCI_SETMARGINRIGHT, 0, pixels);
		}

		int INativeScintilla.GetMarginRight()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETMARGINRIGHT, 0, 0);
		}

		void INativeScintilla.SetFoldMarginColour(bool useSetting, int colour)
		{
			_ns.SendMessageDirect(Constants.SCI_SETFOLDMARGINCOLOUR, useSetting, colour);
		}

		void INativeScintilla.SetFoldMarginHiColour(bool useSetting, int colour)
		{
			_ns.SendMessageDirect(Constants.SCI_SETFOLDMARGINHICOLOUR, useSetting, colour);
		}

		#endregion

		#region Other settings
		void INativeScintilla.SetUsePalette(bool allowPaletteUse)
		{
			_ns.SendMessageDirect(Constants.SCI_SETUSEPALETTE, allowPaletteUse, 0);
		}

		bool INativeScintilla.GetUsePalette()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETUSEPALETTE, 0, 0) != 0;
		}

		void INativeScintilla.SetBufferedDraw(bool isBuffered)
		{
			_ns.SendMessageDirect(Constants.SCI_SETBUFFEREDDRAW, isBuffered, 0);
		}

		bool INativeScintilla.GetBufferedDraw()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETBUFFEREDDRAW, 0, 0) != 0;
		}

		void INativeScintilla.SetTwoPhaseDraw(bool twoPhase)
		{
			_ns.SendMessageDirect(Constants.SCI_SETTWOPHASEDRAW, twoPhase, 0);
		}

		bool INativeScintilla.GetTwoPhaseDraw()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETTWOPHASEDRAW, 0, 0) != 0;
		}

		void INativeScintilla.SetCodePage(int codePage)
		{
			_ns.SendMessageDirect(Constants.SCI_SETCODEPAGE, codePage, 0);
			_encoding = Encoding.GetEncoding(codePage);
		}

		int INativeScintilla.GetCodePage()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETCODEPAGE, 0, 0);
		}

		void INativeScintilla.SetWordChars(string chars)
		{
			_ns.SendMessageDirect(Constants.SCI_SETWORDCHARS, VOID.NULL, chars);
		}

		void INativeScintilla.SetWhitespaceChars(string chars)
		{
			_ns.SendMessageDirect(Constants.SCI_SETWHITESPACECHARS, VOID.NULL, chars);
		}

		void INativeScintilla.SetCharsDefault()
		{
			_ns.SendMessageDirect(Constants.SCI_SETCHARSDEFAULT, 0, 0);
		}

		void INativeScintilla.GrabFocus()
		{
			_ns.SendMessageDirect(Constants.SCI_GRABFOCUS, 0, 0);
		}

		void INativeScintilla.SetFocus(bool focus)
		{
			_ns.SendMessageDirect(Constants.SCI_SETFOCUS, focus, 0);
		}

		bool INativeScintilla.GetFocus()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETFOCUS, 0, 0) != 0;
		}
		#endregion

		#region Brace highlighting
		void INativeScintilla.BraceHighlight(int pos1, int pos2)
		{
			_ns.SendMessageDirect(Constants.SCI_BRACEHIGHLIGHT, pos1, pos2);
		}

		void INativeScintilla.BraceBadLight(int pos1)
		{
			_ns.SendMessageDirect(Constants.SCI_BRACEBADLIGHT, pos1, 0);
		}

		int INativeScintilla.BraceMatch(int pos, int maxReStyle)
		{
			return _ns.SendMessageDirect(Constants.SCI_BRACEMATCH, pos, maxReStyle);
		}
		#endregion

		#region Tabs and Indentation Guides
		void INativeScintilla.SetTabWidth(int widthInChars)
		{
			_ns.SendMessageDirect(Constants.SCI_SETTABWIDTH, widthInChars, 0);
		}

		int INativeScintilla.GetTabWidth()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETTABWIDTH, 0, 0);
		}

		void INativeScintilla.SetUseTabs(bool useTabs)
		{
			_ns.SendMessageDirect(Constants.SCI_SETUSETABS, useTabs, 0);
		}

		bool INativeScintilla.GetUseTabs()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETUSETABS, 0, 0) != 0;
		}

		void INativeScintilla.SetIndent(int widthInChars)
		{
			_ns.SendMessageDirect(Constants.SCI_SETINDENT, widthInChars, 0);
		}

		int INativeScintilla.GetIndent()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETINDENT, 0, 0);
		}

		void INativeScintilla.SetTabIndents(bool tabIndents)
		{
			_ns.SendMessageDirect(Constants.SCI_SETTABINDENTS, tabIndents, 0);
		}

		bool INativeScintilla.GetTabIndents()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETTABINDENTS, 0, 0) != 0;
		}

		void INativeScintilla.SetBackSpaceUnIndents(bool bsUnIndents)
		{
			_ns.SendMessageDirect(Constants.SCI_SETBACKSPACEUNINDENTS, bsUnIndents, 0);
		}

		bool INativeScintilla.GetBackSpaceUnIndents()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETBACKSPACEUNINDENTS, 0, 0) != 0;
		}

		void INativeScintilla.SetLineIndentation(int line, int indentation)
		{
			_ns.SendMessageDirect(Constants.SCI_SETLINEINDENTATION, line, indentation);
		}

		int INativeScintilla.GetLineIndentation(int line)
		{
			return _ns.SendMessageDirect(Constants.SCI_GETLINEINDENTATION, line, 0);
		}

		int INativeScintilla.GetLineIndentPosition(int line)
		{
			return _ns.SendMessageDirect(Constants.SCI_GETLINEINDENTPOSITION, line, 0);
		}

		void INativeScintilla.SetIndentationGuides(bool view)
		{
			_ns.SendMessageDirect(Constants.SCI_SETINDENTATIONGUIDES, view, 0);
		}

		bool INativeScintilla.GetIndentationGuides()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETINDENTATIONGUIDES, 0, 0) != 0;
		}

		void INativeScintilla.SetHighlightGuide(int column)
		{
			_ns.SendMessageDirect(Constants.SCI_SETHIGHLIGHTGUIDE, column, 0);
		}

		int INativeScintilla.GetHighlightGuide()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETHIGHLIGHTGUIDE, 0, 0);
		}
		#endregion

		#region Markers
		void INativeScintilla.MarkerDefine(int markerNumber, int markerSymbol)
		{
			_ns.SendMessageDirect(Constants.SCI_MARKERDEFINE, markerNumber, markerSymbol);
		}

		void INativeScintilla.MarkerDefinePixmap(int markerNumber, string xpm)
		{
			_ns.SendMessageDirect(Constants.SCI_MARKERDEFINEPIXMAP, markerNumber, xpm);
		}

		void INativeScintilla.MarkerSetFore(int markerNumber, int colour)
		{
			_ns.SendMessageDirect(Constants.SCI_MARKERSETFORE, markerNumber, colour);
		}

		void INativeScintilla.MarkerSetBack(int markerNumber, int colour)
		{
			_ns.SendMessageDirect(Constants.SCI_MARKERSETBACK, markerNumber, colour);
		}

		void INativeScintilla.MarkerSetAlpha(int markerNumber, int alpha)
		{
			_ns.SendMessageDirect(Constants.SCI_MARKERSETALPHA, markerNumber, alpha);
		}

		int INativeScintilla.MarkerAdd(int line, int markerNumber)
		{
			return _ns.SendMessageDirect(Constants.SCI_MARKERADD, line, markerNumber);
		}

		void INativeScintilla.MarkerAddSet(int line, uint markerMask)
		{
			_ns.SendMessageDirect(Constants.SCI_MARKERADDSET, line, markerMask);
		}

		void INativeScintilla.MarkerDelete(int line, int markerNumber)
		{
			_ns.SendMessageDirect(Constants.SCI_MARKERDELETE, line, markerNumber);
		}

		void INativeScintilla.MarkerDeleteAll(int markerNumber)
		{
			_ns.SendMessageDirect(Constants.SCI_MARKERDELETEALL, markerNumber, 0);
		}

		int INativeScintilla.MarkerGet(int line)
		{
			return _ns.SendMessageDirect(Constants.SCI_MARKERGET, line, 0);
		}

		int INativeScintilla.MarkerNext(int lineStart, uint markerMask)
		{
			return _ns.SendMessageDirect(Constants.SCI_MARKERNEXT, lineStart, markerMask);
		}

		int INativeScintilla.MarkerPrevious(int lineStart, uint markerMask)
		{
			return _ns.SendMessageDirect(Constants.SCI_MARKERPREVIOUS, lineStart, markerMask);
		}

		int INativeScintilla.MarkerLineFromHandle(int handle)
		{
			return _ns.SendMessageDirect(Constants.SCI_MARKERLINEFROMHANDLE, handle, 0);
		}

		void INativeScintilla.MarkerDeleteHandle(int handle)
		{
			_ns.SendMessageDirect(Constants.SCI_MARKERDELETEHANDLE, handle, 0);
		}
		#endregion

		#region Indicators
		void INativeScintilla.IndicSetStyle(int indicatorNumber, int indicatorStyle)
		{
			_ns.SendMessageDirect(Constants.SCI_INDICSETSTYLE, indicatorNumber, indicatorStyle);
		}

		int INativeScintilla.IndicGetStyle(int indicatorNumber)
		{
			return _ns.SendMessageDirect(Constants.SCI_INDICGETSTYLE, indicatorNumber, 0);
		}

		void INativeScintilla.IndicSetFore(int indicatorNumber, int colour)
		{
			_ns.SendMessageDirect(Constants.SCI_INDICSETFORE, indicatorNumber, colour);
		}

		int INativeScintilla.IndicGetFore(int indicatorNumber)
		{
			return _ns.SendMessageDirect(Constants.SCI_INDICGETFORE, indicatorNumber, 0);
		}

		int INativeScintilla.GetIndicatorCurrent()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETINDICATORCURRENT, 0, 0);
		}

		int INativeScintilla.GetIndicatorValue()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETINDICATORVALUE, 0, 0);
		}

		int INativeScintilla.GetPositionCache()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETPOSITIONCACHE, 0, 0);
		}

		uint INativeScintilla.IndicatorAllOnFor(int position)
		{
			return (uint)_ns.SendMessageDirect(Constants.SCI_INDICATORALLONFOR, position, 0);
		}

		void INativeScintilla.IndicatorClearRange(int position, int fillLength)
		{
			_ns.SendMessageDirect(Constants.SCI_INDICATORCLEARRANGE, position, fillLength);
		}

		void INativeScintilla.IndicatorFillRange(int position, int fillLength)
		{
			_ns.SendMessageDirect(Constants.SCI_INDICATORFILLRANGE, position, fillLength);
		}

		void INativeScintilla.SetIndicatorCurrent(int indicator)
		{
			_ns.SendMessageDirect(Constants.SCI_SETINDICATORCURRENT, indicator, 0);
		}

		#endregion

		#region Autocompletion
		void INativeScintilla.RegisterImage(int type, string xpmData)
		{
			_ns.SendMessageDirect(Constants.SCI_REGISTERIMAGE, type, xpmData);
		}

		void INativeScintilla.ClearRegisteredImages()
		{
			_ns.SendMessageDirect(Constants.SCI_CLEARREGISTEREDIMAGES, 0, 0);
		}
		#endregion

		#region User lists
		void INativeScintilla.UserListShow(int listType, string list)
		{
			_ns.SendMessageDirect(Constants.SCI_USERLISTSHOW, listType, list);
		}
		#endregion

		#region Call tips
		void INativeScintilla.CallTipShow(int posStart, string definition)
		{
			_ns.SendMessageDirect(Constants.SCI_CALLTIPSHOW, posStart, definition);
		}

		void INativeScintilla.CallTipCancel()
		{
			_ns.SendMessageDirect(Constants.SCI_CALLTIPCANCEL, 0, 0);
		}

		bool INativeScintilla.CallTipActive()
		{
			return _ns.SendMessageDirect(Constants.SCI_CALLTIPACTIVE, 0, 0) != 0;
		}

		int INativeScintilla.CallTipGetPosStart()
		{
			return _ns.SendMessageDirect(Constants.SCI_CALLTIPPOSSTART, 0, 0);
		}

		void INativeScintilla.CallTipSetHlt(int hlStart, int hlEnd)
		{
			_ns.SendMessageDirect(Constants.SCI_CALLTIPSETHLT, hlStart, hlEnd);
		}

		void INativeScintilla.CallTipSetBack(int colour)
		{
			_ns.SendMessageDirect(Constants.SCI_CALLTIPSETBACK, colour, 0);
		}

		void INativeScintilla.CallTipSetFore(int colour)
		{
			_ns.SendMessageDirect(Constants.SCI_CALLTIPSETFORE, colour, 0);
		}

		void INativeScintilla.CallTipSetForeHlt(int colour)
		{
			_ns.SendMessageDirect(Constants.SCI_CALLTIPSETFOREHLT, colour, 0);
		}

		void INativeScintilla.CallTipUseStyle(int tabsize)
		{
			_ns.SendMessageDirect(Constants.SCI_CALLTIPUSESTYLE, tabsize, 0);
		}
		#endregion

		#region Keyboard commands
		void INativeScintilla.LineDown()
		{
			_ns.SendMessageDirect(Constants.SCI_LINEDOWN, 0, 0);
		}

		void INativeScintilla.LineDownExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_LINEDOWNEXTEND, 0, 0);
		}

		void INativeScintilla.LineDownRectExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_LINEDOWNRECTEXTEND, 0, 0);
		}

		void INativeScintilla.LineScrollDown()
		{
			_ns.SendMessageDirect(Constants.SCI_LINESCROLLDOWN, 0, 0);
		}

		void INativeScintilla.LineUp()
		{
			_ns.SendMessageDirect(Constants.SCI_LINEUP, 0, 0);
		}

		void INativeScintilla.LineUpExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_LINEUPEXTEND, 0, 0);
		}

		void INativeScintilla.LineUpRectExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_LINEUPRECTEXTEND, 0, 0);
		}

		void INativeScintilla.LineScrollUp()
		{
			_ns.SendMessageDirect(Constants.SCI_LINESCROLLUP, 0, 0);
		}

		void INativeScintilla.ParaDown()
		{
			_ns.SendMessageDirect(Constants.SCI_PARADOWN, 0, 0);
		}

		void INativeScintilla.ParaDownExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_PARADOWNEXTEND, 0, 0);
		}

		void INativeScintilla.ParaUp()
		{
			_ns.SendMessageDirect(Constants.SCI_PARAUP, 0, 0);
		}

		void INativeScintilla.ParaUpExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_PARAUPEXTEND, 0, 0);
		}

		void INativeScintilla.CharLeft()
		{
			_ns.SendMessageDirect(Constants.SCI_CHARLEFT, 0, 0);
		}

		void INativeScintilla.CharLeftExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_CHARLEFTEXTEND, 0, 0);
		}

		void INativeScintilla.CharLeftRectExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_CHARLEFTRECTEXTEND, 0, 0);
		}

		void INativeScintilla.CharRight()
		{
			_ns.SendMessageDirect(Constants.SCI_CHARRIGHT, 0, 0);
		}

		void INativeScintilla.CharRightExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_CHARRIGHTEXTEND, 0, 0);
		}

		void INativeScintilla.CharRightRectExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_CHARRIGHTRECTEXTEND, 0, 0);
		}

		void INativeScintilla.WordLeft()
		{
			_ns.SendMessageDirect(Constants.SCI_WORDLEFT, 0, 0);
		}

		void INativeScintilla.WordLeftExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_WORDLEFTEXTEND, 0, 0);
		}

		void INativeScintilla.WordRight()
		{
			_ns.SendMessageDirect(Constants.SCI_WORDRIGHT, 0, 0);
		}

		void INativeScintilla.WordRightExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_WORDRIGHTEXTEND, 0, 0);
		}

		void INativeScintilla.WordLeftEnd()
		{
			_ns.SendMessageDirect(Constants.SCI_WORDLEFTEND, 0, 0);
		}

		void INativeScintilla.WordLeftEndExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_WORDLEFTENDEXTEND, 0, 0);
		}

		void INativeScintilla.WordRightEnd()
		{
			_ns.SendMessageDirect(Constants.SCI_WORDRIGHTEND, 0, 0);
		}

		void INativeScintilla.WordRightEndExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_WORDRIGHTENDEXTEND, 0, 0);
		}

		void INativeScintilla.WordPartLeft()
		{
			_ns.SendMessageDirect(Constants.SCI_WORDPARTLEFT, 0, 0);
		}

		void INativeScintilla.WordPartLeftExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_WORDPARTLEFTEXTEND, 0, 0);
		}

		void INativeScintilla.WordPartRight()
		{
			_ns.SendMessageDirect(Constants.SCI_WORDPARTRIGHT, 0, 0);
		}

		void INativeScintilla.WordPartRightExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_WORDPARTRIGHTEXTEND, 0, 0);
		}

		void INativeScintilla.Home()
		{
			_ns.SendMessageDirect(Constants.SCI_HOME, 0, 0);
		}

		void INativeScintilla.HomeExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_HOMEEXTEND, 0, 0);
		}

		void INativeScintilla.HomeRectExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_HOMERECTEXTEND, 0, 0);
		}

		void INativeScintilla.HomeDisplay()
		{
			_ns.SendMessageDirect(Constants.SCI_HOMEDISPLAY, 0, 0);
		}

		void INativeScintilla.HomeDisplayExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_HOMEDISPLAYEXTEND, 0, 0);
		}

		void INativeScintilla.HomeWrap()
		{
			_ns.SendMessageDirect(Constants.SCI_HOMEWRAP, 0, 0);
		}

		void INativeScintilla.HomeWrapExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_HOMEWRAPEXTEND, 0, 0);
		}

		void INativeScintilla.VCHome()
		{
			_ns.SendMessageDirect(Constants.SCI_VCHOME, 0, 0);
		}

		void INativeScintilla.VCHomeExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_VCHOMEEXTEND, 0, 0);
		}

		void INativeScintilla.VCHomeRectExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_VCHOMERECTEXTEND, 0, 0);
		}

		void INativeScintilla.VCHomeWrap()
		{
			_ns.SendMessageDirect(Constants.SCI_VCHOMEWRAP, 0, 0);
		}

		void INativeScintilla.VCHomeWrapExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_VCHOMEWRAPEXTEND, 0, 0);
		}

		void INativeScintilla.LineEnd()
		{
			_ns.SendMessageDirect(Constants.SCI_LINEEND, 0, 0);
		}

		void INativeScintilla.LineEndExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_LINEENDEXTEND, 0, 0);
		}

		void INativeScintilla.LineEndRectExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_LINEENDRECTEXTEND, 0, 0);
		}

		void INativeScintilla.LineEndDisplay()
		{
			_ns.SendMessageDirect(Constants.SCI_LINEENDDISPLAY, 0, 0);
		}

		void INativeScintilla.LineEndDisplayExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_LINEENDDISPLAYEXTEND, 0, 0);
		}

		void INativeScintilla.LineEndWrap()
		{
			_ns.SendMessageDirect(Constants.SCI_LINEENDWRAP, 0, 0);
		}

		void INativeScintilla.LineEndWrapExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_LINEENDWRAPEXTEND, 0, 0);
		}

		void INativeScintilla.DocumentStart()
		{
			_ns.SendMessageDirect(Constants.SCI_DOCUMENTSTART, 0, 0);
		}

		void INativeScintilla.DocumentStartExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_DOCUMENTSTARTEXTEND, 0, 0);
		}

		void INativeScintilla.DocumentEnd()
		{
			_ns.SendMessageDirect(Constants.SCI_DOCUMENTEND, 0, 0);
		}

		void INativeScintilla.DocumentEndExtend()
		{
			_ns.SendMessageDirect(2319, 0, 0);
		}

		void INativeScintilla.PageUp()
		{
			_ns.SendMessageDirect(Constants.SCI_PAGEUP, 0, 0);
		}

		void INativeScintilla.PageUpExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_PAGEUPEXTEND, 0, 0);
		}

		void INativeScintilla.PageUpRectExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_PAGEUPRECTEXTEND, 0, 0);
		}

		void INativeScintilla.PageDown()
		{
			_ns.SendMessageDirect(Constants.SCI_PAGEDOWN, 0, 0);
		}

		void INativeScintilla.PageDownExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_PAGEDOWNEXTEND, 0, 0);
		}

		void INativeScintilla.PageDownRectExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_PAGEDOWNRECTEXTEND, 0, 0);
		}

		void INativeScintilla.StutteredPageUp()
		{
			_ns.SendMessageDirect(Constants.SCI_STUTTEREDPAGEUP, 0, 0);
		}

		void INativeScintilla.StutteredPageUpExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_STUTTEREDPAGEUPEXTEND, 0, 0);
		}

		void INativeScintilla.StutteredPageDown()
		{
			_ns.SendMessageDirect(Constants.SCI_STUTTEREDPAGEDOWN, 0, 0);
		}

		void INativeScintilla.StutteredPageDownExtend()
		{
			_ns.SendMessageDirect(Constants.SCI_STUTTEREDPAGEDOWNEXTEND, 0, 0);
		}

		void INativeScintilla.DeleteBack()
		{
			_ns.SendMessageDirect(Constants.SCI_DELETEBACK, 0, 0);
		}

		void INativeScintilla.DeleteBackNotLine()
		{
			_ns.SendMessageDirect(Constants.SCI_DELETEBACKNOTLINE, 0, 0);
		}

		void INativeScintilla.DelWordLeft()
		{
			_ns.SendMessageDirect(Constants.SCI_DELWORDLEFT, 0, 0);
		}

		void INativeScintilla.DelWordRight()
		{
			_ns.SendMessageDirect(Constants.SCI_DELWORDRIGHT, 0, 0);
		}

		void INativeScintilla.DelLineLeft()
		{
			_ns.SendMessageDirect(Constants.SCI_DELLINELEFT, 0, 0);
		}

		void INativeScintilla.DelLineRight()
		{
			_ns.SendMessageDirect(Constants.SCI_DELLINERIGHT, 0, 0);
		}

		void INativeScintilla.LineDelete()
		{
			_ns.SendMessageDirect(Constants.SCI_LINEDELETE, 0, 0);
		}

		void INativeScintilla.LineCut()
		{
			_ns.SendMessageDirect(Constants.SCI_LINECUT, 0, 0);
		}

		void INativeScintilla.LineCopy()
		{
			_ns.SendMessageDirect(Constants.SCI_LINECOPY, 0, 0);
		}

		void INativeScintilla.LineTranspose()
		{
			_ns.SendMessageDirect(Constants.SCI_LINETRANSPOSE, 0, 0);
		}

		void INativeScintilla.LineDuplicate()
		{
			_ns.SendMessageDirect(Constants.SCI_LINEDUPLICATE, 0, 0);
		}

		void INativeScintilla.LowerCase()
		{
			_ns.SendMessageDirect(Constants.SCI_LOWERCASE, 0, 0);
		}

		void INativeScintilla.UpperCase()
		{
			_ns.SendMessageDirect(Constants.SCI_UPPERCASE, 0, 0);
		}

		void INativeScintilla.Cancel()
		{
			_ns.SendMessageDirect(Constants.SCI_CANCEL, 0, 0);
		}

		void INativeScintilla.EditToggleOvertype()
		{
			_ns.SendMessageDirect(Constants.SCI_EDITTOGGLEOVERTYPE, 0, 0);
		}

		void INativeScintilla.NewLine()
		{
			_ns.SendMessageDirect(Constants.SCI_NEWLINE, 0, 0);
		}

		void INativeScintilla.FormFeed()
		{
			_ns.SendMessageDirect(Constants.SCI_FORMFEED, 0, 0);
		}

		void INativeScintilla.Tab()
		{
			_ns.SendMessageDirect(Constants.SCI_TAB, 0, 0);
		}

		void INativeScintilla.BackTab()
		{
			_ns.SendMessageDirect(Constants.SCI_BACKTAB, 0, 0);
		}

		void INativeScintilla.SelectionDuplicate()
		{
			_ns.SendMessageDirect(Constants.SCI_SELECTIONDUPLICATE, 0, 0);
		}
		#endregion

		#region Key bindings
		void INativeScintilla.AssignCmdKey(int keyDefinition, int sciCommand)
		{
			_ns.SendMessageDirect(Constants.SCI_ASSIGNCMDKEY, keyDefinition, sciCommand);
		}

		void INativeScintilla.ClearCmdKey(int keyDefinition)
		{
			_ns.SendMessageDirect(Constants.SCI_CLEARCMDKEY, keyDefinition, 0);
		}

		void INativeScintilla.ClearAllCmdKeys()
		{
			_ns.SendMessageDirect(Constants.SCI_CLEARALLCMDKEYS, 0, 0);
		}

		void INativeScintilla.Null()
		{
			_ns.SendMessageDirect(Constants.SCI_NULL, 0, 0);
		}
		#endregion

		#region Popup edit menu
		void INativeScintilla.UsePopUp(bool bEnablePopup)
		{
			_ns.SendMessageDirect(Constants.SCI_USEPOPUP, bEnablePopup, 0);
		}
		#endregion

		#region Macro recording
		void INativeScintilla.StartRecord()
		{
			_ns.SendMessageDirect(Constants.SCI_STARTRECORD, 0, 0);
		}

		void INativeScintilla.StopRecord()
		{
			_ns.SendMessageDirect(Constants.SCI_STOPRECORD, 0, 0);
		}
		#endregion

		#region Printing
		unsafe int INativeScintilla.FormatRange(bool bDraw, ref RangeToFormat pfr)
		{
			fixed(RangeToFormat* rtfp = &pfr)
				return (int)_ns.SendMessageDirect(Constants.SCI_FORMATRANGE, (IntPtr)(bDraw ? 1 : 0), (IntPtr)rtfp);
		}

		void INativeScintilla.SetPrintMagnification(int magnification)
		{
			_ns.SendMessageDirect(Constants.SCI_SETPRINTMAGNIFICATION, magnification, 0);
		}

		int INativeScintilla.GetPrintMagnification()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETPRINTMAGNIFICATION, 0, 0);
		}

		void INativeScintilla.SetPrintColourMode(int mode)
		{
			_ns.SendMessageDirect(Constants.SCI_SETPRINTCOLOURMODE, mode, 0);
		}

		int INativeScintilla.GetPrintColourMode()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETPRINTCOLOURMODE, 0, 0);
		}

		int INativeScintilla.GetPrintWrapMode()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETPRINTWRAPMODE, 0, 0);
		}

		void INativeScintilla.SetPrintWrapMode(int wrapMode)
		{
			_ns.SendMessageDirect(Constants.SCI_SETPRINTWRAPMODE, wrapMode, 0);
		}
		#endregion

		#region DirectAccess
		IntPtr INativeScintilla.GetDirectFunction()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETDIRECTFUNCTION, IntPtr.Zero, IntPtr.Zero);
		}

		IntPtr INativeScintilla.GetDirectPointer()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETDIRECTPOINTER, IntPtr.Zero, IntPtr.Zero);
		}
		#endregion

		#region Multiple views
		IntPtr INativeScintilla.GetDocPointer()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETDOCPOINTER, IntPtr.Zero, IntPtr.Zero);
		}

		void INativeScintilla.SetDocPointer(IntPtr pDoc)
		{
			_ns.SendMessageDirect(Constants.SCI_SETDOCPOINTER, IntPtr.Zero, pDoc);
		}

		IntPtr INativeScintilla.CreateDocument()
		{
			return (IntPtr)_ns.SendMessageDirect(Constants.SCI_CREATEDOCUMENT, 0, 0);
		}

		void INativeScintilla.AddRefDocument(IntPtr pDoc)
		{
			_ns.SendMessageDirect(Constants.SCI_ADDREFDOCUMENT, IntPtr.Zero, pDoc);
		}

		void INativeScintilla.ReleaseDocument(IntPtr pDoc)
		{
			_ns.SendMessageDirect(Constants.SCI_RELEASEDOCUMENT, IntPtr.Zero, pDoc);
		}
		#endregion

		#region Folding
		int INativeScintilla.VisibleFromDocLine(int docLine)
		{
			return _ns.SendMessageDirect(Constants.SCI_VISIBLEFROMDOCLINE, docLine, 0);
		}

		int INativeScintilla.DocLineFromVisible(int displayLine)
		{
			return _ns.SendMessageDirect(Constants.SCI_DOCLINEFROMVISIBLE, displayLine, 0);
		}

		void INativeScintilla.ShowLines(int lineStart, int lineEnd)
		{
			_ns.SendMessageDirect(Constants.SCI_SHOWLINES, lineStart, lineEnd);
		}

		void INativeScintilla.HideLines(int lineStart, int lineEnd)
		{
			_ns.SendMessageDirect(Constants.SCI_HIDELINES, lineStart, lineEnd);
		}

		bool INativeScintilla.GetLineVisible(int line)
		{
			return _ns.SendMessageDirect(Constants.SCI_GETLINEVISIBLE, line, 0) != 0;
		}

		void INativeScintilla.SetFoldLevel(int line, uint level)
		{
			_ns.SendMessageDirect(Constants.SCI_SETFOLDLEVEL, line, level);
		}

		uint INativeScintilla.GetFoldLevel(int line)
		{
			return (uint)_ns.SendMessageDirect(Constants.SCI_GETFOLDLEVEL, line, 0);
		}

		void INativeScintilla.SetFoldFlags(int flags)
		{
			_ns.SendMessageDirect(Constants.SCI_SETFOLDFLAGS, flags, 0);
		}

		int INativeScintilla.GetLastChild(int line, int level)
		{
			return _ns.SendMessageDirect(Constants.SCI_GETLASTCHILD, line, level);
		}

		int INativeScintilla.GetFoldParent(int line)
		{
			return _ns.SendMessageDirect(Constants.SCI_GETFOLDPARENT, line, 0);
		}

		void INativeScintilla.SetFoldExpanded(int line, bool expanded)
		{
			_ns.SendMessageDirect(Constants.SCI_SETFOLDEXPANDED, line, expanded);
		}

		bool INativeScintilla.GetFoldExpanded(int line)
		{
			return _ns.SendMessageDirect(Constants.SCI_GETFOLDEXPANDED, line, 0) != 0;
		}

		void INativeScintilla.ToggleFold(int line)
		{
			_ns.SendMessageDirect(Constants.SCI_TOGGLEFOLD, line, 0);
		}

		void INativeScintilla.EnsureVisible(int line)
		{
			_ns.SendMessageDirect(Constants.SCI_ENSUREVISIBLE, line, 0);
		}

		void INativeScintilla.EnsureVisibleEnforcePolicy(int line)
		{
			_ns.SendMessageDirect(Constants.SCI_ENSUREVISIBLEENFORCEPOLICY, line, 0);
		}
		#endregion

		#region Line wrapping
		void INativeScintilla.SetWrapMode(int wrapMode)
		{
			_ns.SendMessageDirect(Constants.SCI_SETWRAPMODE, wrapMode, 0);
		}

		int INativeScintilla.GetWrapMode()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETWRAPMODE, 0, 0);
		}

		void INativeScintilla.SetWrapVisualFlags(int wrapVisualFlags)
		{
			_ns.SendMessageDirect(Constants.SCI_SETWRAPVISUALFLAGS, wrapVisualFlags, 0);
		}

		int INativeScintilla.GetWrapVisualFlags()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETWRAPVISUALFLAGS, 0, 0);
		}

		void INativeScintilla.SetWrapVisualFlagsLocation(int wrapVisualFlagsLocation)
		{
			_ns.SendMessageDirect(Constants.SCI_SETWRAPVISUALFLAGSLOCATION, wrapVisualFlagsLocation, 0);
		}

		int INativeScintilla.GetWrapVisualFlagsLocation()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETWRAPVISUALFLAGSLOCATION, 0, 0);
		}

		void INativeScintilla.SetWrapStartIndent(int indent)
		{
			_ns.SendMessageDirect(Constants.SCI_SETWRAPSTARTINDENT, indent, 0);
		}

		int INativeScintilla.GetWrapStartIndent()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETWRAPSTARTINDENT, 0, 0);
		}

		void INativeScintilla.SetLayoutCache(int cacheMode)
		{
			_ns.SendMessageDirect(Constants.SCI_SETLAYOUTCACHE, cacheMode, 0);
		}

		int INativeScintilla.GetLayoutCache()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETLAYOUTCACHE, 0, 0);
		}

		void INativeScintilla.LinesJoin()
		{
			_ns.SendMessageDirect(Constants.SCI_LINESJOIN, 0, 0);
		}

		void INativeScintilla.LinesSplit(int pixelWidth)
		{
			_ns.SendMessageDirect(Constants.SCI_LINESSPLIT, pixelWidth, 0);
		}

		int INativeScintilla.WrapCount(int docLine)
		{
			return _ns.SendMessageDirect(Constants.SCI_WRAPCOUNT, docLine, 0);
		}
		#endregion

		#region Zooming
		void INativeScintilla.SetZoom(int zoomInPoints)
		{
			_ns.SendMessageDirect(Constants.SCI_SETZOOM, zoomInPoints, 0);
		}

		int INativeScintilla.GetZoom()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETZOOM, 0, 0);
		}
		#endregion

		#region Long lines
		void INativeScintilla.SetEdgeMode(int mode)
		{
			_ns.SendMessageDirect(Constants.SCI_SETEDGEMODE, mode, 0);
		}

		int INativeScintilla.GetEdgeMode()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETEDGEMODE, 0, 0);
		}

		void INativeScintilla.SetEdgeColumn(int column)
		{
			_ns.SendMessageDirect(Constants.SCI_SETEDGECOLUMN, column, 0);
		}

		int INativeScintilla.GetEdgeColumn()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETEDGECOLUMN, 0, 0);
		}

		void INativeScintilla.SetEdgeColour(int colour)
		{
			_ns.SendMessageDirect(Constants.SCI_SETEDGECOLOUR, colour, 0);
		}

		int INativeScintilla.GetEdgeColour()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETEDGECOLOUR, 0, 0);
		}
		#endregion

		#region Lexer
		void INativeScintilla.SetLexer(int lexer)
		{
			_ns.SendMessageDirect(Constants.SCI_SETLEXER, lexer, 0);
		}

		int INativeScintilla.GetLexer()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETLEXER, 0, 0);
		}

		void INativeScintilla.SetLexerLanguage(string name)
		{
			_ns.SendMessageDirect(Constants.SCI_SETLEXERLANGUAGE, VOID.NULL, name);
		}

		void INativeScintilla.LoadLexerLibrary(string path)
		{
			_ns.SendMessageDirect(Constants.SCI_LOADLEXERLIBRARY, VOID.NULL, path);
		}

		void INativeScintilla.Colourise(int start, int end)
		{
			_ns.SendMessageDirect(Constants.SCI_COLOURISE, start, end);
		}

		void INativeScintilla.SetProperty(string key, string value)
		{
			_ns.SendMessageDirect(Constants.SCI_SETPROPERTY, key, value);
		}

		void INativeScintilla.GetProperty(string key, out string value)
		{
			_ns.SendMessageDirect(Constants.SCI_GETPROPERTY, key, out value);
		}

		void INativeScintilla.GetPropertyExpanded(string key, out string value)
		{
			_ns.SendMessageDirect(Constants.SCI_GETPROPERTYEXPANDED, key, out value);
		}

		int INativeScintilla.GetPropertyInt(string key, int @default)
		{
			return _ns.SendMessageDirect(Constants.SCI_GETPROPERTYINT, key, @default);
		}

		void INativeScintilla.SetKeywords(int keywordSet, string keyWordList)
		{
			_ns.SendMessageDirect(Constants.SCI_SETKEYWORDS, keywordSet, keyWordList);
		}

		int INativeScintilla.GetStyleBitsNeeded()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETSTYLEBITSNEEDED, 0, 0);
		}

		#endregion

		#region Notifications
		private static readonly object _styleNeededEventKeyNative = new object();
		private static readonly object _charAddedEventKeyNative = new object();
		private static readonly object _savePointReachedEventKeyNative = new object();
		private static readonly object _savePointLeftEventKeyNative = new object();
		private static readonly object _modifyAttemptROEventKey = new object();
		private static readonly object _keyEventKey = new object();
		private static readonly object _doubleClickEventKey = new object();
		private static readonly object _updateUIEventKey = new object();
		private static readonly object _macroRecordEventKeyNative = new object();
		private static readonly object _marginClickEventKeyNative = new object();
		private static readonly object _modifiedEventKey = new object();
		private static readonly object _changeEventKey = new object();
		private static readonly object _needShownEventKey = new object();
		private static readonly object _paintedEventKey = new object();
		private static readonly object _userListSelectionEventKeyNative = new object();
		private static readonly object _uriDroppedEventKeyNative = new object();
		private static readonly object _dwellStartEventKeyNative = new object();
		private static readonly object _dwellEndEventKeyNative = new object();
		private static readonly object _zoomEventKey = new object();
		private static readonly object _hotSpotClickEventKey = new object();
		private static readonly object _hotSpotDoubleClickEventKey = new object();
		private static readonly object _callTipClickEventKeyNative = new object();
		private static readonly object _autoCSelectionEventKey = new object();
		private static readonly object _indicatorClickKeyNative = new object();
		private static readonly object _indicatorReleaseKeyNative = new object();

		internal void FireStyleNeeded(NativeScintillaEventArgs ea)
		{
			if(Events[_styleNeededEventKeyNative] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_styleNeededEventKeyNative])(this, ea);

			//	TODO: When this event fires it fires A LOT over and over again if
			//	you don't actually style the document. Additionally I'm making 2
			//	more calls to Scintilla to get the Start position of the style
			//	range. I need to come up with a good way to supress this logic
			//	unless the client app is actually interested in it. 

			//	Now that we've fired the Native event we do the same for the
			//	SourceEdit's StyleNeeded event. This code should look VERY familliar
			//	to anyone who's read the Scintilla Documentation
			int startPos	= _ns.GetEndStyled();
			int lineNumber	= _ns.LineFromPosition(startPos);
			startPos		= _ns.PositionFromLine(lineNumber);

			StyleNeededEventArgs snea = new StyleNeededEventArgs(new Range(startPos, ea.SCNotification.position, this));
			OnStyleNeeded(snea);
		}

		internal void FireCharAdded(NativeScintillaEventArgs ea)
		{
			
			if(Events[_charAddedEventKeyNative] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_charAddedEventKeyNative])(this, ea);

			OnCharAdded(new CharAddedEventArgs(ea.SCNotification.ch));
		}

		internal void FireSavePointReached(NativeScintillaEventArgs ea)
		{
			if(Events[_savePointReachedEventKeyNative] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_savePointReachedEventKeyNative])(this, ea);

			OnSavePointReached(EventArgs.Empty);
		}

		internal void FireSavePointLeft(NativeScintillaEventArgs ea)
		{
			if(Events[_savePointLeftEventKeyNative] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_savePointLeftEventKeyNative])(this, ea);

			OnSavePointLeft(EventArgs.Empty);
		}

		internal void FireModifyAttemptRO(NativeScintillaEventArgs ea)
		{
			if(Events[_modifyAttemptROEventKey] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_modifyAttemptROEventKey])(this, ea);

			OnReadOnlyModifyAttempt(EventArgs.Empty);
		}

		internal void FireKey(NativeScintillaEventArgs ea)
		{
			if(Events[_keyEventKey] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_keyEventKey])(this, ea);
		}

		internal void FireDoubleClick(NativeScintillaEventArgs ea)
		{
			if(Events[_doubleClickEventKey] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_doubleClickEventKey])(this, ea);
		}

		internal void FireUpdateUI(NativeScintillaEventArgs ea)
		{
			if(Events[_updateUIEventKey] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_updateUIEventKey])(this, ea);

			//	The SCN_UPDATEUI Notification message is sent whenever text, style or
			//	selection range changes. This means that the SelectionChangeEvent could
			//	potentially fire without the selection actually having changed. However
			//	I feel that it's important enough that SelectionChanged gets its own
			//	event.
			OnSelectionChanged(EventArgs.Empty);
		}

		internal void FireMacroRecord(NativeScintillaEventArgs ea)
		{
			if(Events[_macroRecordEventKeyNative] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_macroRecordEventKeyNative])(this, ea);

			OnMacroRecord(new MacroRecordEventArgs(ea));
		}

		internal void FireMarginClick(NativeScintillaEventArgs ea)
		{
			if(Events[_marginClickEventKeyNative] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_marginClickEventKeyNative])(this, ea);

			FireMarginClick(ea.SCNotification);
		}

		private const uint TEXT_MODIFIED_FLAGS = Constants.SC_MOD_BEFOREDELETE | Constants.SC_MOD_BEFOREINSERT |
													Constants.SC_MOD_DELETETEXT | Constants.SC_MOD_INSERTTEXT;

		internal void FireModified(NativeScintillaEventArgs ea)
		{
			//	First we fire the INativeScintilla Modified event.
			if(Events[_modifiedEventKey] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_modifiedEventKey])(this, ea);

			//	Now we use raw information from the Modified event to construct
			//	some more user friendly Events to fire.
			SCNotification scn	= ea.SCNotification;
			int modType			= scn.modificationType;

			if((modType & TEXT_MODIFIED_FLAGS) > 0)
			{
				TextModifiedEventArgs mea = new TextModifiedEventArgs
					(
					modType,
					(modType | Constants.SC_PERFORMED_USER) > 0,
					scn.line,
					scn.position,
					scn.length,
					scn.linesAdded,
					Utilities.PtrToStringUtf8(scn.text, scn.length)
					);

				//	These messages all get fired seperately hence the if else ifs
				if((modType & Constants.SC_MOD_BEFOREDELETE) > 0)
					OnBeforeTextDelete(mea);
				else if((modType & Constants.SC_MOD_BEFOREINSERT) > 0)
					OnBeforeTextInsert(mea);
				else if((modType & Constants.SC_MOD_DELETETEXT) > 0)
					OnTextDeleted(mea);
				else if((modType & Constants.SC_MOD_INSERTTEXT) > 0)
					OnTextInserted(mea);
			}
			else if((modType & Constants.SC_MOD_CHANGEFOLD) > 0)
			{
				FoldChangedEventArgs fea = new FoldChangedEventArgs(scn.line, scn.foldLevelNow, scn.foldLevelPrev, scn.modificationType);
				OnFoldChanged(fea);
			}
			else if((modType & Constants.SC_MOD_CHANGESTYLE) > 0)
			{
				StyleChangedEventArgs sea = new StyleChangedEventArgs(scn.position, scn.length, scn.modificationType);
				OnStyleChanged(sea);
			}
			else if((modType & Constants.SC_MOD_CHANGEMARKER) > 0)
			{
				MarkerChangedEventArgs mea = new MarkerChangedEventArgs(scn.line, scn.modificationType);
				OnMarkerChanged(mea);
			}

			OnDocumentChange(ea);
		}

		internal void FireChange(NativeScintillaEventArgs ea)
		{
			if(Events[_changeEventKey] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_changeEventKey])(this, ea);

			//	This one is already defined for us
			OnTextChanged(EventArgs.Empty);
		}

		internal void FireNeedShown(NativeScintillaEventArgs ea)
		{
			if(Events[_needShownEventKey] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_needShownEventKey])(this, ea);

			//	Again, this shoold look familiar...
			int pos			= ea.SCNotification.position;
			int firstLine	= _ns.LineFromPosition(pos);
			int lastLine	= _ns.LineFromPosition(pos + ea.SCNotification.length - 1);
			OnLinesNeedShown(new LinesNeedShownEventArgs(firstLine, lastLine));
		}


		internal void FirePainted(NativeScintillaEventArgs ea)
		{
			if(Events[_paintedEventKey] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_paintedEventKey])(this, ea);
		}

		internal void FireUserListSelection(NativeScintillaEventArgs ea)
		{
			if(Events[_userListSelectionEventKeyNative] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_userListSelectionEventKeyNative])(this, ea);
		}

		internal void FireUriDropped(NativeScintillaEventArgs ea)
		{
			if(Events[_uriDroppedEventKeyNative] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_uriDroppedEventKeyNative])(this, ea);
		}

		internal void FireDwellStart(NativeScintillaEventArgs ea)
		{
			if(Events[_dwellStartEventKeyNative] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_dwellStartEventKeyNative])(this, ea);

			OnDwellStart(new ScintillaMouseEventArgs(ea.SCNotification.x, ea.SCNotification.y, ea.SCNotification.position));
		}

		internal void FireDwellEnd(NativeScintillaEventArgs ea)
		{
			if(Events[_dwellEndEventKeyNative] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_dwellEndEventKeyNative])(this, ea);

			OnDwellEnd(EventArgs.Empty);
		}

		internal void FireZoom(NativeScintillaEventArgs ea)
		{
			if(Events[_zoomEventKey] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_zoomEventKey])(this, ea);

			OnZoomChanged(EventArgs.Empty);
		}

		internal void FireHotSpotClick(NativeScintillaEventArgs ea)
		{
			if(Events[_hotSpotClickEventKey] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_hotSpotClickEventKey])(this, ea);

			Point p = PointToClient(new Point(MousePosition.X, MousePosition.Y));
			OnHotspotClick(new ScintillaMouseEventArgs(p.X, p.Y, ea.SCNotification.position));
		}

		internal void FireHotSpotDoubleclick(NativeScintillaEventArgs ea)
		{
			if(Events[_hotSpotDoubleClickEventKey] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_hotSpotDoubleClickEventKey])(this, ea);

			Point p = PointToClient(new Point(MousePosition.X, MousePosition.Y));
			OnHotspotDoubleClick(new ScintillaMouseEventArgs(p.X, p.Y, ea.SCNotification.position));
		}

		internal void FireCallTipClick(NativeScintillaEventArgs ea)
		{
			if(Events[_callTipClickEventKeyNative] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_callTipClickEventKeyNative])(this, ea);

			FireCallTipClick(ea.SCNotification.position);
		}

		internal void FireAutoCSelection(NativeScintillaEventArgs ea)
		{
			if (Events[_autoCSelectionEventKey] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_autoCSelectionEventKey])(this, ea);
		}

		internal void FireIndicatorClick(NativeScintillaEventArgs ea)
		{
			if (Events[_indicatorClickKeyNative] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_indicatorClickKeyNative])(this, ea);

			OnIndicatorClick(new ScintillaMouseEventArgs(ea.SCNotification.x, ea.SCNotification.y, ea.SCNotification.position));
		}

		internal void FireIndicatorRelease(NativeScintillaEventArgs ea)
		{
			if (Events[_indicatorReleaseKeyNative] != null)
				((EventHandler<NativeScintillaEventArgs>)Events[_indicatorReleaseKeyNative])(this, ea);
		}
		
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.StyleNeeded
		{
			add { Events.AddHandler(_styleNeededEventKeyNative, value); }
			remove { Events.RemoveHandler(_styleNeededEventKeyNative, value); }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.CharAdded
		{
			add { Events.AddHandler(_charAddedEventKeyNative, value); }
			remove { Events.RemoveHandler(_charAddedEventKeyNative, value); }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.SavePointReached
		{
			add { Events.AddHandler(_savePointReachedEventKeyNative, value); }
			remove { Events.RemoveHandler(_savePointReachedEventKeyNative, value); }
		}
		event EventHandler<NativeScintillaEventArgs> INativeScintilla.SavePointLeft
		{
			add { Events.AddHandler(_savePointLeftEventKeyNative, value); }
			remove { Events.RemoveHandler(_savePointLeftEventKeyNative, value); }
		}

		event EventHandler<NativeScintillaEventArgs> INativeScintilla.ModifyAttemptRO
		{
			add { Events.AddHandler(_modifyAttemptROEventKey, value); }
			remove { Events.RemoveHandler(_modifyAttemptROEventKey, value); }
		}

		event EventHandler<NativeScintillaEventArgs> INativeScintilla.Key
		{
			add { Events.AddHandler(_keyEventKey, value); }
			remove { Events.RemoveHandler(_keyEventKey, value); }
		}

		event EventHandler<NativeScintillaEventArgs> INativeScintilla.DoubleClick
		{
			add { Events.AddHandler(_doubleClickEventKey, value); }
			remove { Events.RemoveHandler(_doubleClickEventKey, value); }
		}

		event EventHandler<NativeScintillaEventArgs> INativeScintilla.UpdateUI
		{
			add { Events.AddHandler(_updateUIEventKey, value); }
			remove { Events.RemoveHandler(_updateUIEventKey, value); }
		}

		event EventHandler<NativeScintillaEventArgs> INativeScintilla.MacroRecord
		{
			add { Events.AddHandler(_macroRecordEventKeyNative, value); }
			remove { Events.RemoveHandler(_macroRecordEventKeyNative, value); }
		}

		event EventHandler<NativeScintillaEventArgs> INativeScintilla.MarginClick
		{
			add { Events.AddHandler(_marginClickEventKeyNative, value); }
			remove { Events.RemoveHandler(_marginClickEventKeyNative, value); }
		}

		event EventHandler<NativeScintillaEventArgs> INativeScintilla.Modified
		{
			add { Events.AddHandler(_modifiedEventKey, value); }
			remove { Events.RemoveHandler(_modifiedEventKey, value); }
		}

		event EventHandler<NativeScintillaEventArgs> INativeScintilla.Change
		{
			add { Events.AddHandler(_changeEventKey, value); }
			remove { Events.RemoveHandler(_changeEventKey, value); }
		}

		event EventHandler<NativeScintillaEventArgs> INativeScintilla.NeedShown
		{
			add { Events.AddHandler(_needShownEventKey, value); }
			remove { Events.RemoveHandler(_needShownEventKey, value); }
		}

		event EventHandler<NativeScintillaEventArgs> INativeScintilla.Painted
		{
			add { Events.AddHandler(_paintedEventKey, value); }
			remove { Events.RemoveHandler(_paintedEventKey, value); }
		}

		event EventHandler<NativeScintillaEventArgs> INativeScintilla.UserListSelection
		{
			add { Events.AddHandler(_userListSelectionEventKeyNative, value); }
			remove { Events.RemoveHandler(_userListSelectionEventKeyNative, value); }
		}

		event EventHandler<NativeScintillaEventArgs> INativeScintilla.UriDropped
		{
			add { Events.AddHandler(_uriDroppedEventKeyNative, value); }
			remove { Events.RemoveHandler(_uriDroppedEventKeyNative, value); }
		}

		event EventHandler<NativeScintillaEventArgs> INativeScintilla.DwellStart
		{
			add { Events.AddHandler(_dwellStartEventKeyNative, value); }
			remove { Events.RemoveHandler(_dwellStartEventKeyNative, value); }
		}

		event EventHandler<NativeScintillaEventArgs> INativeScintilla.DwellEnd
		{
			add { Events.AddHandler(_dwellEndEventKeyNative, value); }
			remove { Events.RemoveHandler(_dwellEndEventKeyNative, value); }
		}

		event EventHandler<NativeScintillaEventArgs> INativeScintilla.Zoom
		{
			add { Events.AddHandler(_zoomEventKey, value); }
			remove { Events.RemoveHandler(_zoomEventKey, value); }
		}

		event EventHandler<NativeScintillaEventArgs> INativeScintilla.HotSpotClick
		{
			add { Events.AddHandler(_hotSpotClickEventKey, value); }
			remove { Events.RemoveHandler(_hotSpotClickEventKey, value); }
		}

		event EventHandler<NativeScintillaEventArgs> INativeScintilla.HotSpotDoubleclick
		{
			add { Events.AddHandler(_hotSpotDoubleClickEventKey, value); }
			remove { Events.RemoveHandler(_hotSpotDoubleClickEventKey, value); }
		}

		event EventHandler<NativeScintillaEventArgs> INativeScintilla.CallTipClick
		{
			add { Events.AddHandler(_callTipClickEventKeyNative, value); }
			remove { Events.RemoveHandler(_callTipClickEventKeyNative, value); }
		}

		event EventHandler<NativeScintillaEventArgs> INativeScintilla.IndicatorClick
		{
			add { Events.AddHandler(_indicatorClickKeyNative, value); }
			remove { Events.RemoveHandler(_indicatorClickKeyNative, value); }
		}

		event EventHandler<NativeScintillaEventArgs> INativeScintilla.IndicatorRelease
		{
			add { Events.AddHandler(_indicatorReleaseKeyNative, value); }
			remove { Events.RemoveHandler(_indicatorReleaseKeyNative, value); }
		}

		void INativeScintilla.SetModEventMask(int modEventMask)
		{
			_ns.SendMessageDirect(Constants.SCI_SETMODEVENTMASK, modEventMask, 0);
		}

		int INativeScintilla.GetModEventMask()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETMODEVENTMASK, 0, 0);
		}

		void INativeScintilla.SetMouseDwellTime(int mouseDwellTime)
		{
			_ns.SendMessageDirect(Constants.SCI_SETMOUSEDWELLTIME, mouseDwellTime, 0);
		}

		int INativeScintilla.GetMouseDwellTime()
		{
			return _ns.SendMessageDirect(Constants.SCI_GETMOUSEDWELLTIME, 0, 0);
		}

		#endregion
	}
}
