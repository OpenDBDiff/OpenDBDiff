using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SQLRichControl
{
    public partial class SQLTextControl : UserControl
    {
        [DllImport("user32.dll")]
        private static extern bool LockWindowUpdate(IntPtr hWndLock);

        public enum SQLType
        {
            SQLServer = 1,
            MySQL = 2,
            Sybase = 3,
            Oracle = 4
        }

        private SQLType type = SQLType.SQLServer;
        private string sql;
        private Boolean showSintax;

        public Boolean ShowSintax
        {
            get { return showSintax; }
            set { showSintax = value; }
        }

        public SQLType Type
        {
            get { return type; }
            set 
            { 
                type = value;
                SQLRichProcess.FillWords(type);
            }
        }

        public SQLTextControl()
        {
            InitializeComponent();            
        }

        public override string Text
        {
            get
            {
                return sql;
            }
            set
            {
                sql = value;
                if (showSintax)
                {
                    LockWindowUpdate(richTextBox1.Handle);
                    richTextBox1.Rtf = SQLRichProcess.GetTextRTF(sql);
                    LockWindowUpdate(IntPtr.Zero);
                }
                else
                    richTextBox1.Text = sql;
                richTextBox1.SelectionStart = 1;
            }
        }

    }
}
