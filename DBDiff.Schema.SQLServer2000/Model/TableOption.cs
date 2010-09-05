using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer2000.Model
{
    public class TableOption:SchemaBase
    {
        private string vale;

        public TableOption(Table parent):base("[","]",StatusEnum.ObjectTypeEnum.TableOption)
        {
            this.Parent = parent ;
        }

        /// <summary>
        /// Clona el objeto en una nueva instancia.
        /// </summary>
        public TableOption Clone()
        {
            TableOption option = new TableOption((Table)this.Parent);
            option.Name = this.Name;
            option.Status = this.Status;
            option.Value = this.Value;
            return option;
        }

        public string Value
        {
            get { return vale; }
            set { vale = value; }
        }

        public override string ToSQLDrop()
        {
            if (this.Name.Equals("TextInRow"))
                return "EXEC sp_tableoption " + Parent.Name + ", 'text in row','off'\r\nGO\r\n";
            if (this.Name.Equals("IsPinned"))
                return "EXEC sp_tableoption " + Parent.Name + ", 'pintable','0'\r\nGO\r\n";
            return "";
        }

        public string ToSQL()
        {
            if (this.Name.Equals("TextInRow"))
                return "EXEC sp_tableoption " + Parent.Name + ", 'text in row'," + vale + "\r\nGO\r\n";
            if (this.Name.Equals("IsPinned"))
                return "EXEC sp_tableoption " + Parent.Name + ", 'pintable'," + vale + "\r\nGO\r\n";
            return "";
        }
    }
}
