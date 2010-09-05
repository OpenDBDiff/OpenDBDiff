using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer2000.Model
{
    public class ConstraintColumn
    {
        private string name;
        private string columnRelationalName;
        private Constraint parent;

        internal ConstraintColumn(Constraint parent, string name)
        {
            this.parent = parent;
            this.name = name;
        }

        internal ConstraintColumn(Constraint parent)
        {
            this.parent = parent;
        }

        public ConstraintColumn Clone()
        {
            ConstraintColumn ccol = new ConstraintColumn(parent);
            ccol.ColumnRelationalName = this.ColumnRelationalName;
            ccol.Name = this.Name;
            return ccol;
        }

        public string ColumnRelationalName
        {
            get { return columnRelationalName; }
            set { columnRelationalName = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string ToXML()
        {
            string xml = "";
            if (parent.Type == Constraint.ConstraintType.PrimaryKey)
            {
                xml += "<COLUMN name=\"" + name + "\"/>\n";
            }
            if (parent.Type == Constraint.ConstraintType.ForeignKey)
            {
                xml += "<COLUMN name=\"" + name + "\" relationalName=\"" + columnRelationalName + "\"/>\n";
            }
            return xml;
        }
    }
}
