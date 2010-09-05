using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SQLRichControl
{
    public static class SQLRichProcess
    {
        private static WordList ReservedWord = new WordList();
        private const string BASE_FONT = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fmodern\fprq1\fcharset0 Courier New;}{\f1\fswiss\fcharset0 Arial;}}";
        private const string BASE_COLOR = @"{\colortbl ;\red0\green0\blue255;\red255\green0\blue255;\red128\green128\blue128;\red255\green0\blue0;\red0\green128\blue0;\red128\green0\blue0;}";
        private const string BASE = @"\viewkind4\uc1\pard";
        
        private static void FillReserveWordSQL()
        {
            ReservedWord.Clear();
            //String
            ReservedWord.Add("'", Word.WordClassType.StringWord);
            //DDL Commands            
            ReservedWord.Add("ALTER", Word.WordClassType.SQLWord);
            ReservedWord.Add("APPLICATION", Word.WordClassType.SQLWord);
            ReservedWord.Add("ASSEMBLY", Word.WordClassType.SQLWord);
            ReservedWord.Add("CALLER", Word.WordClassType.SQLWord);
            ReservedWord.Add("CHECK", Word.WordClassType.SQLWord);
            ReservedWord.Add("CASCADE", Word.WordClassType.SQLWord);
            ReservedWord.Add("CLUSTERED", Word.WordClassType.SQLWord);
            ReservedWord.Add("COLUMN", Word.WordClassType.SQLWord);
            ReservedWord.Add("CONSTRAINT", Word.WordClassType.SQLWord);
            ReservedWord.Add("CONTENT", Word.WordClassType.SQLWord);
            ReservedWord.Add("CREATE", Word.WordClassType.SQLWord);
            ReservedWord.Add("DISABLE", Word.WordClassType.SQLWord);
            ReservedWord.Add("DATABASE", Word.WordClassType.SQLWord);
            ReservedWord.Add("DBCC", Word.WordClassType.SQLWord);
            ReservedWord.Add("DROP", Word.WordClassType.SQLWord);
            ReservedWord.Add("ENABLE", Word.WordClassType.SQLWord);
            ReservedWord.Add("EXTERNAL", Word.WordClassType.SQLWord);
            ReservedWord.Add("FILE", Word.WordClassType.SQLWord);
            ReservedWord.Add("FILEGROUP", Word.WordClassType.SQLWord);
            ReservedWord.Add("FILEGROWTH", Word.WordClassType.SQLWord);
            ReservedWord.Add("FILENAME", Word.WordClassType.SQLWord);
            ReservedWord.Add("FOREIGN", Word.WordClassType.SQLWord);            
            ReservedWord.Add("FUNCTION", Word.WordClassType.SQLWord);
            ReservedWord.Add("GRANT", Word.WordClassType.SQLWord);
            ReservedWord.Add("IDENTITY", Word.WordClassType.SQLWord);
            ReservedWord.Add("INDEX", Word.WordClassType.SQLWord);
            ReservedWord.Add("MOVE", Word.WordClassType.SQLWord);
            ReservedWord.Add("NAME", Word.WordClassType.SQLWord);
            ReservedWord.Add("NONCLUSTERED", Word.WordClassType.SQLWord);
            ReservedWord.Add("NOLOCK", Word.WordClassType.SQLWord);
            ReservedWord.Add("OPEN", Word.WordClassType.SQLWord);
            ReservedWord.Add("PERMISSION_SET", Word.WordClassType.SQLWord);
            ReservedWord.Add("PERSISTED", Word.WordClassType.SQLWord);
            ReservedWord.Add("PRIMARY", Word.WordClassType.SQLWord);
            ReservedWord.Add("PROCEDURE", Word.WordClassType.SQLWord);            
            ReservedWord.Add("READONLY", Word.WordClassType.SQLWord);
            ReservedWord.Add("REPLICATION", Word.WordClassType.SQLWord);                        
            ReservedWord.Add("RETURNS", Word.WordClassType.SQLWord);
            ReservedWord.Add("REVOKE", Word.WordClassType.SQLWord);
            ReservedWord.Add("ROLE", Word.WordClassType.SQLWord);
            ReservedWord.Add("SAFE", Word.WordClassType.SQLWord);            
            ReservedWord.Add("SCHEMABINDING", Word.WordClassType.SQLWord);
            ReservedWord.Add("SIZE", Word.WordClassType.SQLWord);
            ReservedWord.Add("SYNONYM", Word.WordClassType.SQLWord);                                    
            ReservedWord.Add("TABLE", Word.WordClassType.SQLWord);
            ReservedWord.Add("TABLOCK", Word.WordClassType.SQLWord);
            ReservedWord.Add("TEXTIMAGE_ON", Word.WordClassType.SQLWord);
            ReservedWord.Add("TYPE", Word.WordClassType.SQLWord);
            ReservedWord.Add("TRIGGER", Word.WordClassType.SQLWord);
            ReservedWord.Add("TRUNCATE", Word.WordClassType.SQLWord);
            ReservedWord.Add("UPDATETEXT", Word.WordClassType.SQLWord);
            ReservedWord.Add("VIEW", Word.WordClassType.SQLWord);
            ReservedWord.Add("XLOCK", Word.WordClassType.SQLWord);
            ReservedWord.Add("DOCUMENT", Word.WordClassType.SQLWord);                                                
            ReservedWord.Add("NOCHECK", Word.WordClassType.SQLWord);            
            ReservedWord.Add("RULE", Word.WordClassType.SQLWord);
            //DML Commands
            ReservedWord.Add("CURSOR", Word.WordClassType.SQLWord);
            ReservedWord.Add("FETCH", Word.WordClassType.SQLWord);
            ReservedWord.Add("NEXT", Word.WordClassType.SQLWord);
            ReservedWord.Add("CLOSE", Word.WordClassType.SQLWord);
            ReservedWord.Add("DEALLOCATE", Word.WordClassType.SQLWord);
            ReservedWord.Add("SCROLL", Word.WordClassType.SQLWord);
            ReservedWord.Add("DECLARE", Word.WordClassType.SQLWord);
            ReservedWord.Add("SELECT", Word.WordClassType.SQLWord);
            ReservedWord.Add("UNION", Word.WordClassType.SQLWord);
            ReservedWord.Add("FROM", Word.WordClassType.SQLWord);
            ReservedWord.Add("JOIN", Word.WordClassType.SQLWord);
            ReservedWord.Add("INNER", Word.WordClassType.SQLWord);
            ReservedWord.Add("LEFT", Word.WordClassType.SQLWord);
            ReservedWord.Add("RIGHT", Word.WordClassType.SQLWord);
            ReservedWord.Add("WHERE", Word.WordClassType.SQLWord);
            ReservedWord.Add("DISTINCT", Word.WordClassType.SQLWord);
            ReservedWord.Add("LIKE", Word.WordClassType.SQLWord);
            ReservedWord.Add("HAVING", Word.WordClassType.SQLWord);
            ReservedWord.Add("GROUP", Word.WordClassType.SQLWord);
            ReservedWord.Add("ORDER", Word.WordClassType.SQLWord);
            ReservedWord.Add("INSERT", Word.WordClassType.SQLWord);
            ReservedWord.Add("INTO", Word.WordClassType.SQLWord);
            ReservedWord.Add("VALUES", Word.WordClassType.SQLWord);
            ReservedWord.Add("UPDATE", Word.WordClassType.SQLWord);
            ReservedWord.Add("DELETE", Word.WordClassType.SQLWord);
            ReservedWord.Add("EXECUTE", Word.WordClassType.SQLWord);
            ReservedWord.Add("EXEC", Word.WordClassType.SQLWord);
            ReservedWord.Add("COLLECTION", Word.WordClassType.SQLWord);
            ReservedWord.Add("MAXRECURSION", Word.WordClassType.SQLWord);
            ReservedWord.Add("REVERT", Word.WordClassType.SQLWord);
            ReservedWord.Add("AS", Word.WordClassType.SQLWord);
            ReservedWord.Add("END", Word.WordClassType.SQLWord);
            ReservedWord.Add("SET", Word.WordClassType.SQLWord);
            ReservedWord.Add("FOR", Word.WordClassType.SQLWord);
            ReservedWord.Add("ON", Word.WordClassType.SQLWord);
            ReservedWord.Add("TO", Word.WordClassType.SQLWord);
            ReservedWord.Add("TOP", Word.WordClassType.SQLWord);
            ReservedWord.Add("ALL", Word.WordClassType.SQLWord);
            ReservedWord.Add("KEY", Word.WordClassType.SQLWord);
            ReservedWord.Add("OFF", Word.WordClassType.SQLWord);
            ReservedWord.Add("BY", Word.WordClassType.SQLWord);
            ReservedWord.Add("XML", Word.WordClassType.SQLWord);
            ReservedWord.Add("ADD", Word.WordClassType.SQLWord);
            ReservedWord.Add("IN", Word.WordClassType.SQLWord);
            ReservedWord.Add("IF", Word.WordClassType.SQLWord);
            ReservedWord.Add("ASC", Word.WordClassType.SQLWord);
            ReservedWord.Add("DESC", Word.WordClassType.SQLWord);
            ReservedWord.Add("WHILE", Word.WordClassType.SQLWord);
            ReservedWord.Add("BEGIN", Word.WordClassType.SQLWord);
            ReservedWord.Add("USE", Word.WordClassType.SQLWord);
            ReservedWord.Add("GOTO", Word.WordClassType.SQLWord);
            ReservedWord.Add("CASE", Word.WordClassType.SQLWord);
            ReservedWord.Add("ELSE", Word.WordClassType.SQLWord);
            ReservedWord.Add("WHEN", Word.WordClassType.SQLWord);
            ReservedWord.Add("THEN", Word.WordClassType.SQLWord);
            ReservedWord.Add("BETWEEN", Word.WordClassType.SQLWord);
            ReservedWord.Add("CAST", Word.WordClassType.SQLWord);
            ReservedWord.Add("RETURN", Word.WordClassType.SQLWord);
            ReservedWord.Add("COMMIT", Word.WordClassType.SQLWord);
            ReservedWord.Add("ROLLBACK", Word.WordClassType.SQLWord);
            ReservedWord.Add("TRANSACTION", Word.WordClassType.SQLWord);
            ReservedWord.Add("PRINT", Word.WordClassType.SQLWord);
            ReservedWord.Add("DEFAULT", Word.WordClassType.SQLWord);
            ReservedWord.Add("COLLATE", Word.WordClassType.SQLWord);
            ReservedWord.Add("TRY", Word.WordClassType.SQLWord);
            ReservedWord.Add("CATCH", Word.WordClassType.SQLWord);
            ReservedWord.Add("WITH", Word.WordClassType.SQLWord);
            ReservedWord.Add("INSTEAD", Word.WordClassType.SQLWord);
            ReservedWord.Add("SCHEMA", Word.WordClassType.SQLWord);
            ReservedWord.Add("AUTHORIZATION", Word.WordClassType.SQLWord);
            ReservedWord.Add("AFTER", Word.WordClassType.SQLWord);
            ReservedWord.Add("UNIQUE", Word.WordClassType.SQLWord);
            ReservedWord.Add("ROWGUIDCOL", Word.WordClassType.SQLWord);
            ReservedWord.Add("REFERENCES", Word.WordClassType.SQLWord);
            ReservedWord.Add("RAISERROR", Word.WordClassType.SQLWord);
            ReservedWord.Add("OPTION", Word.WordClassType.SQLWord);
            ReservedWord.Add("PAD_INDEX", Word.WordClassType.SQLWord);
            ReservedWord.Add("STATISTICS_NORECOMPUTE", Word.WordClassType.SQLWord);
            ReservedWord.Add("ALLOW_ROW_LOCKS", Word.WordClassType.SQLWord);
            ReservedWord.Add("IGNORE_DUP_KEY", Word.WordClassType.SQLWord);
            ReservedWord.Add("ALLOW_PAGE_LOCKS", Word.WordClassType.SQLWord);
            //SETs
            ReservedWord.Add("NOCOUNT", Word.WordClassType.SQLWord);
            ReservedWord.Add("XACT_ABORT", Word.WordClassType.SQLWord);
            ReservedWord.Add("NUMERIC_ROUNDABORT", Word.WordClassType.SQLWord);
            ReservedWord.Add("IDENTITY_INSERT", Word.WordClassType.SQLWord);
            ReservedWord.Add("ANSI_PADDING", Word.WordClassType.SQLWord);
            ReservedWord.Add("QUOTED_IDENTIFIER", Word.WordClassType.SQLWord);
            ReservedWord.Add("ANSI_NULLS", Word.WordClassType.SQLWord);
            //Funcions
            ReservedWord.Add("ABS", Word.WordClassType.FunctionWord);
            ReservedWord.Add("AVG", Word.WordClassType.FunctionWord);
            ReservedWord.Add("CHARINDEX", Word.WordClassType.FunctionWord);
            ReservedWord.Add("DATALENGTH", Word.WordClassType.FunctionWord);
            ReservedWord.Add("DATEADD", Word.WordClassType.FunctionWord);
            ReservedWord.Add("DATEPART", Word.WordClassType.FunctionWord);
            ReservedWord.Add("GETDATE", Word.WordClassType.FunctionWord);
            ReservedWord.Add("GETUTCDATE", Word.WordClassType.FunctionWord);
            ReservedWord.Add("REVERSE", Word.WordClassType.FunctionWord);            
            ReservedWord.Add("PATINDEX", Word.WordClassType.FunctionWord);
            ReservedWord.Add("LTRIM", Word.WordClassType.FunctionWord);
            ReservedWord.Add("RTRIM", Word.WordClassType.FunctionWord);
            ReservedWord.Add("UPPER", Word.WordClassType.FunctionWord);
            ReservedWord.Add("LOWER", Word.WordClassType.FunctionWord);
            ReservedWord.Add("ISNULL", Word.WordClassType.FunctionWord);
            ReservedWord.Add("SPACE", Word.WordClassType.FunctionWord);
            ReservedWord.Add("CONVERT", Word.WordClassType.FunctionWord);
            ReservedWord.Add("SUM", Word.WordClassType.FunctionWord);
            ReservedWord.Add("MIN", Word.WordClassType.FunctionWord);
            ReservedWord.Add("MAX", Word.WordClassType.FunctionWord);
            ReservedWord.Add("RANK", Word.WordClassType.FunctionWord);
            ReservedWord.Add("COUNT", Word.WordClassType.FunctionWord);
            ReservedWord.Add("COUNT_BIG", Word.WordClassType.FunctionWord);            
            ReservedWord.Add("OBJECT_NAME", Word.WordClassType.FunctionWord);
            ReservedWord.Add("OBJECT_ID", Word.WordClassType.FunctionWord);
            ReservedWord.Add("ROUND", Word.WordClassType.FunctionWord);            
            ReservedWord.Add("NEWID", Word.WordClassType.FunctionWord);
            ReservedWord.Add("NEWSEQUENTIALID", Word.WordClassType.FunctionWord);
            ReservedWord.Add("IS_MEMBER", Word.WordClassType.FunctionWord);
            ReservedWord.Add("@@TRANCOUNT", Word.WordClassType.FunctionWord);
            ReservedWord.Add("@@IDENTITY", Word.WordClassType.FunctionWord);
            ReservedWord.Add("@@FETCH_STATUS", Word.WordClassType.FunctionWord);
            ReservedWord.Add("@@ERROR", Word.WordClassType.FunctionWord);
            //Operators
            ReservedWord.Add("IS", Word.WordClassType.OperatorWord);
            ReservedWord.Add("NOT", Word.WordClassType.OperatorWord);
            ReservedWord.Add("AND", Word.WordClassType.OperatorWord);
            ReservedWord.Add("OR", Word.WordClassType.OperatorWord);
            ReservedWord.Add("NULL", Word.WordClassType.OperatorWord);
            ReservedWord.Add("EXISTS", Word.WordClassType.OperatorWord);
            ReservedWord.Add("OUTER", Word.WordClassType.OperatorWord);
            //SysTables
            ReservedWord.Add("SYSOBJECTS", Word.WordClassType.SysTablesWord);
            ReservedWord.Add("SYSCOLUMNS", Word.WordClassType.SysTablesWord);
            ReservedWord.Add("SYSCOMMENTS", Word.WordClassType.SysTablesWord);
            ReservedWord.Add("SYSPROCESSES", Word.WordClassType.SysTablesWord);
            ReservedWord.Add("SYSCONSTRAINTS", Word.WordClassType.SysTablesWord);
            ReservedWord.Add("SYSINDEXES", Word.WordClassType.SysTablesWord);
            ReservedWord.Add("SYSUSERS", Word.WordClassType.SysTablesWord);
            ReservedWord.Add("SYSKEYS", Word.WordClassType.SysTablesWord);
            ReservedWord.Add("SYSTYPES", Word.WordClassType.SysTablesWord);
            ReservedWord.Add("SYSDEPENDS", Word.WordClassType.SysTablesWord);
            //SysProce
            ReservedWord.Add("SP_RENAME", Word.WordClassType.SysProcWord);
            ReservedWord.Add("SP_BINDRULE", Word.WordClassType.SysProcWord);
            ReservedWord.Add("SP_UNBINDRULE ", Word.WordClassType.SysProcWord);
            ReservedWord.Add("SP_BINDEFAULT", Word.WordClassType.SysProcWord);
            ReservedWord.Add("SP_UNBINDEFAULT", Word.WordClassType.SysProcWord);
            ReservedWord.Add("SP_TABLEOPTION", Word.WordClassType.SysProcWord);
            ReservedWord.Add("SP_DELETE_JOB", Word.WordClassType.SysProcWord);
            ReservedWord.Sort();
        }

        public static void FillWords(SQLTextControl.SQLType type)
        {
            if (type == SQLTextControl.SQLType.SQLServer) FillReserveWordSQL();
        }

        private static string GetRTFColor(Word word)
        {
            if (word.Type == Word.WordClassType.SQLWord) return @"\cf1\fs20";
            if (word.Type == Word.WordClassType.FunctionWord) return @"\cf2\fs20";
            if (word.Type == Word.WordClassType.OperatorWord) return @"\cf3\fs20";
            if (word.Type == Word.WordClassType.StringWord) return @"\cf4\fs20";
            if (word.Type == Word.WordClassType.SysTablesWord) return @"\cf5\fs20";
            if (word.Type == Word.WordClassType.SysProcWord) return @"\cf6\fs20";
            return @"\cf1\fs20";
        }

        private static string Format(string RTF)
        {
            RTF = BASE_FONT + BASE_COLOR + @"\cf0\fs20 " + RTF +"}";
            RTF = RTF.Replace("\r\n", @"\par ");
            //RTF = RTF.Replace("\t", @"\tab ");
            return RTF;
        }

        public static string GetTextRTF(string text)
        {
            Regex regex = new Regex(@"\s|\n|\r|\(|\,|\)|\*");
            string rtfColor;
            StringBuilder acum = new StringBuilder();
            int iacum = 0;
            string lastChar = "";
            string line = "";
            bool openString = false;
            bool startWithComillas = false;

            string[] values = regex.Split(text);
            int count = values.Length;

            for (int index = 0; index < count; index++)
            {
                string value = values[index];
                line = "";
                if (!String.IsNullOrEmpty(value))
                {
                    string[] comillas = value.Split('\'');
                    startWithComillas = comillas[0].Equals("");
                    for (int i=0;i<comillas.Length; i++)
                    {
                        if ((((i%2==0) && startWithComillas) || ((i%2==1) && !startWithComillas)) || (comillas[i].Equals("")))
                        {
                            openString = !openString;
                            if (openString)
                                line += @"\cf4\fs20 '" + comillas[i];
                            else
                                line += @"'\cf0\fs20 " + comillas[i];
                        }
                        else
                        {
                            Word word = ReservedWord[comillas[i].ToUpper()];
                            if ((word != null) && (!openString))
                            {
                                rtfColor = GetRTFColor(word);
                                line += rtfColor + comillas[i] + @"\cf0 ";
                            }
                            else
                                line += comillas[i];
                        }
                    }
                }
                else
                    line = value;
                iacum += value.Length + 1;
                if (iacum < text.Length)                
                    lastChar = text.Substring(iacum-1, 1);
                else
                    lastChar = "";
                
                acum.Append(line + lastChar);
            }

            return Format(acum.ToString());
        }
    }
}
