using System;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class FileGroupFile : SQLServerSchemaBase
    {
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

        public int Size { get; set; }

        public Boolean IsSparse { get; set; }

        public Boolean IsPercentGrowth { get; set; }

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

        public int Growth { get; set; }

        public int MaxSize { get; set; }

        public string PhysicalName { get; set; }

        public int Type { get; set; }

        private string GetNameNewFileGroup(string path)
        {
            string result = "";
            string[] flies = path.Split('\\');
            for (int index = 0; index < flies.Length - 1; index++)
                if (!String.IsNullOrEmpty(flies[index]))
                    result += flies[index] + "\\";
            result += Parent.Parent.Name + "_" + Name + "_DB.ndf";
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
            if (Type != 2)
                return "ALTER DATABASE " + Parent.Parent.FullName + "\r\nADD" + ((Type != 1) ? "" : " LOG") + " FILE ( NAME = N'" + Name + "', FILENAME = N'" + PhysicalName + "' , SIZE = " + Size * 1000 + "KB , FILEGROWTH = " + Growth * 1000 + TypeGrowth + ") TO FILEGROUP " + Parent.FullName + "\r\nGO\r\n";
            else
                return "ALTER DATABASE " + Parent.Parent.FullName + "\r\nADD" + ((Type != 1) ? "" : " LOG") + " FILE ( NAME = N'" + Name + "', FILENAME = N'" + PhysicalName + "') TO FILEGROUP " + Parent.FullName + "\r\nGO\r\n";
        }

        public override string ToSqlAdd()
        {
            if (Type != 2)
                return "ALTER DATABASE " + Parent.Parent.FullName + "\r\nADD" + ((Type != 1) ? "" : " LOG") + " FILE ( NAME = N'" + Name + "', FILENAME = N'" + GetNameNewFileGroup(PhysicalName) + "' , SIZE = " + Size * 1000 + "KB , FILEGROWTH = " + Growth * 1000 + TypeGrowth + ") TO FILEGROUP " + Parent.FullName + "\r\nGO\r\n";
            else
                return "ALTER DATABASE " + Parent.Parent.FullName + "\r\nADD" + ((Type != 1) ? "" : " LOG") + " FILE ( NAME = N'" + Name + "', FILENAME = N'" + GetNameNewFileGroup(PhysicalName) + "') TO FILEGROUP " + Parent.FullName + "\r\nGO\r\n";
        }

        public string ToSQLAlter()
        {
            if (Type != 2)
                return "ALTER DATABASE " + Parent.Parent.FullName + " MODIFY FILE ( NAME = N'" + Name + "', FILENAME = N'" + PhysicalName + "' , SIZE = " + Size * 1000 + "KB , FILEGROWTH = " + Growth * 1000 + TypeGrowth + ")";
            else
                return "ALTER DATABASE " + Parent.Parent.FullName + " MODIFY FILE ( NAME = N'" + Name + "', FILENAME = N'" + PhysicalName + "')";
        }

        public override string ToSqlDrop()
        {
            return "ALTER DATABASE " + Parent.Parent.FullName + " REMOVE FILE " + this.FullName + "\r\nGO\r\n";
        }
    }
}
