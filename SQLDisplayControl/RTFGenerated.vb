Option Strict Off
Option Explicit On

Imports System.Text.RegularExpressions

Namespace DBDiff
    Friend Class RTFGenerated

        Private Const BASE_FONT As String = "{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fmodern\fprq1\fcharset0 Courier New;}{\f1\fswiss\fcharset0 Arial;}}"
        Private Const BASE_COLOR As String = "{\colortbl ;\red0\green0\blue255;\red255\green0\blue255;\red128\green128\blue128;\red255\green0\blue0;\red0\green128\blue0;}"
        Private Const BASE As String = "\viewkind4\uc1\pard"
        Private ReservedWord As Words
        Private oSQLType As SQLEnum.SQLTypeEnum
        Private Sub FillReserveWord()
            If oSQLType = SQLEnum.SQLTypeEnum.SQLServer Then FillReserveWordSQL()
            If oSQLType = SQLEnum.SQLTypeEnum.MySQL Then FillReserveWordMySQL()
            If oSQLType = SQLEnum.SQLTypeEnum.Sybase Then FillReserveWordSybase()
        End Sub
        Private Sub FillReserveWordSybase()
            ReservedWord = New Words
            'String
            Call ReservedWord.Add("'", Word.WordClassType.StringWord)
            'DDL Commands
            Call ReservedWord.Add("DROP", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ALTER", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("TRUNCATE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("TABLE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("CREATE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("PROCEDURE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("CONSTRAINT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("INDEX", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("GRANT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("VIEW", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("REVOKE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("TRIGGER", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("LOCK", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ALLPAGES", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("DATAPAGES", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("DATAROWS", Word.WordClassType.SQLWord)
            'DML Commands
            Call ReservedWord.Add("CURSOR", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("FETCH", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("CLOSE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("DEALLOCATE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("DECLARE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("SELECT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("FROM", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("JOIN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("INNER", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("LEFT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("RIGHT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("WHERE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("DISTINCT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("LIKE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("HAVING", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("GROUP", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ORDER", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("INSERT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("INTO", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("UPDATE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("DELETE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("EXECUTE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("EXISTS", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("FOREIGN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("PRIMARY", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("KEY", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("AS", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("END", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("SET", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("FOR", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ON", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("TO", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("TOP", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("BY", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("IN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("IF", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("WHILE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("BEGIN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("USE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("GOTO", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("CASE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ELSE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("WHEN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("THEN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("BETWEEN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("CAST", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("RETURN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("COMMIT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ROLLBACK", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("TRANSACTION", Word.WordClassType.SQLWord)
            'Operators
            Call ReservedWord.Add("IS", Word.WordClassType.OperatorWord)
            Call ReservedWord.Add("NOT", Word.WordClassType.OperatorWord)
            Call ReservedWord.Add("AND", Word.WordClassType.OperatorWord)
            Call ReservedWord.Add("OR", Word.WordClassType.OperatorWord)
            Call ReservedWord.Add("NULL", Word.WordClassType.OperatorWord)
        End Sub
        Private Sub FillReserveWordMySQL()
            ReservedWord = New Words
            'String
            Call ReservedWord.Add("'", Word.WordClassType.StringWord)
            'DDL Commands
            Call ReservedWord.Add("DROP", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ALTER", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("TRUNCATE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("TABLE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("CREATE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("PROCEDURE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("CONSTRAINT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("INDEX", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("GRANT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("VIEW", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("REVOKE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("TRIGGER", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ENGINE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ZEROFILL", Word.WordClassType.SQLWord)
            'DML Commands
            Call ReservedWord.Add("CURSOR", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("FETCH", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("CLOSE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("DEALLOCATE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("DECLARE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("SELECT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("FROM", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("JOIN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("INNER", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("LEFT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("RIGHT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("WHERE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("DISTINCT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("LIKE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("HAVING", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("GROUP", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ORDER", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("INSERT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("INTO", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("UPDATE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("DELETE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("EXECUTE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("EXISTS", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("UNSIGNED", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("FOREIGN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("PRIMARY", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("KEY", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("AS", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("END", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("SET", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("FOR", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ON", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("TO", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("TOP", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("BY", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("IN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("IF", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("WHILE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("BEGIN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("USE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("GOTO", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("CASE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ELSE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("WHEN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("THEN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("BETWEEN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("CAST", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("RETURN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("COMMIT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ROLLBACK", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("TRANSACTION", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("SIGNED", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("CHARSET", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("DEFAULT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("CHECKSUM", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("INNODB", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("AUTO_INCREMENT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("MIN_ROWS", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("MAX_ROWS", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("AVG_ROW_LENGTH", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ROW_FORMAT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("COMMENT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("REFERENCES", Word.WordClassType.SQLWord)
            'Operators
            Call ReservedWord.Add("IS", Word.WordClassType.OperatorWord)
            Call ReservedWord.Add("NOT", Word.WordClassType.OperatorWord)
            Call ReservedWord.Add("AND", Word.WordClassType.OperatorWord)
            Call ReservedWord.Add("OR", Word.WordClassType.OperatorWord)
            Call ReservedWord.Add("NULL", Word.WordClassType.OperatorWord)
        End Sub
        Private Sub FillReserveWordSQL()
            ReservedWord = New Words
            'String
            Call ReservedWord.Add("'", Word.WordClassType.StringWord)
            'DDL Commands
            Call ReservedWord.Add("DROP", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ALTER", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("TRUNCATE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("TABLE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("CREATE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("PROCEDURE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("FILEGROUP", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("DATABASE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("READONLY", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("INDEX", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("CLUSTERED", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("NONCLUSTERED", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("CONSTRAINT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("GRANT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("VIEW", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("REVOKE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("TRIGGER", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("DBCC", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("IDENTITY", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("REPLICATION", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("PRIMARY", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ENABLE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("CHECK", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("FOREIGN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("COLUMN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("CONTENT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("DOCUMENT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("PERSISTED", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("MOVE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("FILE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("TYPE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("NOCHECK", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("CALLER", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("RULE", Word.WordClassType.SQLWord)
            'DML Commands
            Call ReservedWord.Add("CURSOR", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("FETCH", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("NEXT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("CLOSE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("DEALLOCATE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("SCROLL", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("DECLARE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("SELECT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("UNION", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("FROM", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("JOIN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("INNER", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("LEFT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("RIGHT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("WHERE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("DISTINCT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("LIKE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("HAVING", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("GROUP", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ORDER", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("INSERT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("INTO", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("VALUES", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("UPDATE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("DELETE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("EXECUTE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("EXEC", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("COLLECTION", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("MAXRECURSION", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("REVERT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("AS", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("END", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("SET", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("FOR", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ON", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("TO", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("TOP", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ALL", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("KEY", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("OFF", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("BY", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("XML", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ADD", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("IN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("IF", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ASC", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("DESC", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("WHILE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("BEGIN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("USE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("GOTO", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("CASE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ELSE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("WHEN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("THEN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("BETWEEN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("CAST", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("RETURN", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("COMMIT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ROLLBACK", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("TRANSACTION", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("PRINT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("DEFAULT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("COLLATE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("TRY", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("CATCH", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("WITH", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("INSTEAD", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("SCHEMA", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("AUTHORIZATION", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("AFTER", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("UNIQUE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ROWGUIDCOL", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("REFERENCES", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("RAISERROR", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("OPTION", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("PAD_INDEX", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("STATISTICS_NORECOMPUTE", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ALLOW_ROW_LOCKS", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("IGNORE_DUP_KEY", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ALLOW_PAGE_LOCKS", Word.WordClassType.SQLWord)
            'SETs
            Call ReservedWord.Add("NOCOUNT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("XACT_ABORT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("NUMERIC_ROUNDABORT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("IDENTITY_INSERT", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ANSI_PADDING", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("QUOTED_IDENTIFIER", Word.WordClassType.SQLWord)
            Call ReservedWord.Add("ANSI_NULLS", Word.WordClassType.SQLWord)
            'Funcions
            Call ReservedWord.Add("GETDATE", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("DATEADD", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("DATEPART", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("REVERSE", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("CHARINDEX", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("PATINDEX", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("LTRIM", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("RTRIM", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("UPPER", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("LOWER", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("ISNULL", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("SPACE", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("CONVERT", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("SUM", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("MIN", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("MAX", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("RANK", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("COUNT", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("COUNT_BIG", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("MIN", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("AVG", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("OBJECT_NAME", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("OBJECT_ID", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("ROUND", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("ABS", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("NEWID", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("NEWSEQUENTIALID", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("IS_MEMBER", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("@@TRANCOUNT", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("@@IDENTITY", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("@@FETCH_STATUS", Word.WordClassType.FunctionWord)
            Call ReservedWord.Add("@@ERROR", Word.WordClassType.FunctionWord)
            'Operators
            Call ReservedWord.Add("IS", Word.WordClassType.OperatorWord)
            Call ReservedWord.Add("NOT", Word.WordClassType.OperatorWord)
            Call ReservedWord.Add("AND", Word.WordClassType.OperatorWord)
            Call ReservedWord.Add("OR", Word.WordClassType.OperatorWord)
            Call ReservedWord.Add("NULL", Word.WordClassType.OperatorWord)
            Call ReservedWord.Add("EXISTS", Word.WordClassType.OperatorWord)
            'SysTables
            Call ReservedWord.Add("SYSOBJECTS", Word.WordClassType.SysTablesWord)
            Call ReservedWord.Add("SYSCOLUMNS", Word.WordClassType.SysTablesWord)
            Call ReservedWord.Add("SYSCOMMENTS", Word.WordClassType.SysTablesWord)
            Call ReservedWord.Add("SYSPROCESSES", Word.WordClassType.SysTablesWord)
            Call ReservedWord.Add("SYSCONSTRAINTS", Word.WordClassType.SysTablesWord)
            Call ReservedWord.Add("SYSINDEXES", Word.WordClassType.SysTablesWord)
            Call ReservedWord.Add("SYSUSERS", Word.WordClassType.SysTablesWord)
            Call ReservedWord.Add("SYSKEYS", Word.WordClassType.SysTablesWord)
            Call ReservedWord.Add("SYSTYPES", Word.WordClassType.SysTablesWord)
            Call ReservedWord.Add("SYSDEPENDS", Word.WordClassType.SysTablesWord)
        End Sub
        Private Function IsNotEndChar(ByRef Char_Renamed As String) As Boolean
            IsNotEndChar = True
            If Char_Renamed = " " Then IsNotEndChar = False
            If Char_Renamed = "(" Then IsNotEndChar = False
            If Char_Renamed = ")" Then IsNotEndChar = False
            If Char_Renamed = vbCr Then IsNotEndChar = False
            If Char_Renamed = vbLf Then IsNotEndChar = False
            If Char_Renamed = vbTab Then IsNotEndChar = False
            If Char_Renamed = "," Then IsNotEndChar = False
            If Char_Renamed = "=" Then IsNotEndChar = False
            If Char_Renamed = "+" Then IsNotEndChar = False
            If Char_Renamed = "-" Then IsNotEndChar = False
            If Char_Renamed = "*" Then IsNotEndChar = False
            If Char_Renamed = "/" Then IsNotEndChar = False
            If Char_Renamed = ";" Then IsNotEndChar = False
            If Char_Renamed = "" Then IsNotEndChar = False
        End Function
        Private Function BuildRTFItem(ByRef Text As String, ByRef Item As Integer) As String
            Dim j As Integer
            Dim iStart As Integer
            Dim sLastChar As String
            Dim sFirstChar As String

            Dim iCount As Integer
            Dim OpenWord As Boolean
            Dim szColor As String = ""

            'Dim Reg As New VBScript_RegExp_55.RegExp


            Dim Matches As MatchCollection
            Dim Match As Match

            Dim Reg As New Regex(ReservedWord.Item(Item).Text, RegexOptions.IgnoreCase + RegexOptions.Multiline)

            'Reg.Options.IgnoreCase = True
            'Reg.IgnoreCase = True
            'Reg.Multiline = True

            'Reg.Pattern = 
            Matches = Reg.Matches(Text)

            iCount = 0
            OpenWord = False
            If ReservedWord.Item(Item).WordType = Word.WordClassType.SQLWord Then szColor = "\cf1\fs20"
            If ReservedWord.Item(Item).WordType = Word.WordClassType.FunctionWord Then szColor = "\cf2\fs20"
            If ReservedWord.Item(Item).WordType = Word.WordClassType.OperatorWord Then szColor = "\cf3\fs20"
            If ReservedWord.Item(Item).WordType = Word.WordClassType.StringWord Then szColor = "\cf4\fs20"
            If ReservedWord.Item(Item).WordType = Word.WordClassType.SysTablesWord Then szColor = "\cf5\fs20"

            For j = 0 To Matches.Count - 1
                Match = Matches.Item(j)

                If Not ReservedWord.Item(Item).IsOpenWordType Then
                    'iStart = Match.FirstIndex + (iCount * 14) + 1
                    iStart = Match.Captures(0).Index + (iCount * 14) + 1
                    sLastChar = Mid(Text, iStart + Match.Length, 1)
                    If iStart > 1 Then sFirstChar = Mid(Text, iStart - 1, 1) Else sFirstChar = ""
                    If Not IsNotEndChar(sFirstChar) And Not IsNotEndChar(sLastChar) Then
                        Text = Left(Text, iStart - 1) & szColor & Match.Value & "\cf0 " & Right(Text, Len(Text) - (iStart - 1 + Match.Length))
                        iCount = iCount + 1
                    End If
                Else
                    iStart = Match.Captures(0).Index + (iCount * 10) + 1
                    If Not OpenWord Then
                        Text = Left(Text, iStart - 1) & "\cf4\fs20 " & Match.Value & Right(Text, Len(Text) - iStart)
                        OpenWord = True
                    Else
                        Text = Left(Text, iStart) & "\cf0\fs20 " & Right(Text, Len(Text) - iStart)
                        OpenWord = False
                    End If
                    iCount = iCount + 1
                End If
            Next
            BuildRTFItem = Text
        End Function
        Public Function BuildRTF(ByRef Text As String) As String
            Dim RTF As String
            Dim i As Integer

            RTF = BASE_FONT & BASE_COLOR & "\cf0\fs20 "
            For i = 1 To ReservedWord.Count
                Text = BuildRTFItem(Text, i)
            Next
            RTF = RTF & Text & "}"
            RTF = Replace(RTF, vbCrLf, "\par ")
            BuildRTF = RTF
        End Function

        Public Sub New(ByVal SQLType As SQLEnum.SQLTypeEnum)
            MyBase.New()
            oSQLType = SQLType
            Call FillReserveWord()
        End Sub

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub
    End Class
End Namespace