ScintillaNet 2.0
-----------------
This version of ScintillaNET will work with Visual Studio 2005 and 2008 including express editions. ScintillaNET relies on the unmanaged dll SciLexer.dll. If ScintillaNET can't find this dll you will get "Window class name is not valid" exceptions and basically nothing will work. My suggestion is to copy SciLexer.dll to your \Windows\System32 folder on the development PC. When deploying your software, this isn't necessary. Instead just make sure SciLexer.dll is in the same folder as ScintillaNet.dll.

The ScintillaNET solution also includes the SCide sample project. If you wish to run the solution make sure to set the startup project to SCide. Otherwise Visual Studio will show a Control Test program that throws NullReferenceExceptions because some of the Controls in the ScintillaNET project aren't meant to be instantiated  outside the Scintilla Context.

Be sure and do a full solution build before attempting to view any of the SCide forms or you will encounter errors.