using System;
using System.Collections.Generic;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class FileGroupFile : SQLServerSchemaBase
    {
        private int type;
        private string physical_name;
        private int max_size;
        private int growth;
        private int size;
        private Boolean isPercentGrowth;
        private Boolean isSparse;

        public FileGroupFile(ISchemaBase parent)
            : base(parent, Enums.ObjectType.File)
        {
        }

        public override ISchemaBase Clone(ISchemaBase parent)
        {
            FileGroupFile file = new FileGroupFile(parent);
            file.Growth = this.Growth;
            file.Id = this.Id;
            file.IsPercentGrowth = this.IsPercentGrowth;
            file.IsSparse = this.IsSparse;
            file.MaxSize = this.MaxSize;
            file.Name = this.Name;
            file.PhysicalName = this.PhysicalName;
            file.Size = this.Size;
            file.Type = this.Type;
            return file;
        }

        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        public Boolean IsSparse
        {
            get { return isSparse; }
            set { isSparse = value; }
        }

        public Boolean IsPercentGrowth
        {
            get { return isPercentGrowth; }
            set { isPercentGrowth = value; }
        }

        private string TypeGrowth
        {
            get
            {
                if (Growth == 0) 
                    return "";
                else
                    if (IsPercentGrowth) 
                        return "%"; 
                    else 
                        return "KB";
            }
        }

        public int Growth
        {
            get { return growth; }
            set { growth = value; }
        }

        public int MaxSize
        {
            get { return max_size; }
            set { max_size = value; }
        }

        public string PhysicalName
        {
            get { return physical_name; }
            set { physical_name = value; }
        }

        public int Type
        {
            get { return type; }
            set { type = value; }
        }

        private string GetNameNewFileGroup(string path)
        {
            string result = "";
            string[] flies = path.Split('\\');
            for (int index = 0; index < flies.Length - 1; index++)
                result += flies[index] + "\\";
            result += Parent.Parent.Name + "_DB.ndf";
            return result;
        }

        /// <summary>
        /// Compara dos triggers y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(FileGroupFile origen, FileGroupFile destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (origen.Growth != destino.Growth) return false;
            if (origen.IsPercentGrowth != destino.IsPercentGrowth) return false;
            if (origen.IsSparse != destino.IsSparse) return false;
            if (origen.MaxSize != destino.MaxSize) return false;
            if (!origen.PhysicalName.Equals(destino.PhysicalName)) return false;
            return true;
        }

        public override string ToSql()
        {
            if (type != 2)
                return "ALTER DATABASE " + Parent.Parent.FullName + "\r\nADD " + ((Type != 1) ? "" : "LOG") + " FILE ( NAME = N'" + Name + "', FILENAME = N'" + PhysicalName + "' , SIZE = " + Size * 1000 + "KB , FILEGROWTH = " + growth * 1000 + TypeGrowth + ") TO FILEGROUP " + Parent.FullName + "\r\nGO\r\n";
            else
                return "ALTER DATABASE " + Parent.Parent.FullName + "\r\nADD " + ((Type != 1) ? "" : "LOG") + " FILE ( NAME = N'" + Name + "', FILENAME = N'" + PhysicalName + "') TO FILEGROUP " + Parent.FullName + "\r\nGO\r\n";
        }

        public override string ToSqlAdd()
        {
            if (type != 2)
                return "ALTER DATABASE " + Parent.Parent.FullName + "\r\nADD " + ((Type != 1) ? "" : "LOG") + " FILE ( NAME = N'" + Name + "', FILENAME = N'" + GetNameNewFileGroup(PhysicalName) + "' , SIZE = " + Size * 1000 + "KB , FILEGROWTH = " + growth * 1000 + TypeGrowth + ") TO FILEGROUP " + Parent.FullName + "\r\nGO\r\n";
            else
                return "ALTER DATABASE " + Parent.Parent.FullName + "\r\nADD " + ((Type != 1) ? "" : "LOG") + " FILE ( NAME = N'" + Name + "', FILENAME = N'" + GetNameNewFileGroup(PhysicalName) + "') TO FILEGROUP " + Parent.FullName + "\r\nGO\r\n";
        }

        public string ToSQLAlter()
        {
            if (type != 2)
                return "ALTER DATABASE " + Parent.Parent.FullName + " MODIFY FILE ( NAME = N'" + Name + "', FILENAME = N'" + PhysicalName + "' , SIZE = " + Size * 1000 + "KB , FILEGROWTH = " + growth * 1000 + TypeGrowth + ")";
            else
                return "ALTER DATABASE " + Parent.Parent.FullName + " MODIFY FILE ( NAME = N'" + Name + "', FILENAME = N'" + PhysicalName + "')";
        }

        public override string ToSqlDrop()
        {
            return "ALTER DATABASE " + Parent.Parent.FullName + " REMOVE FILE " + this.FullName + "\r\nGO\r\n";
        }
    }
}
