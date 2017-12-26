using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.Schema.SQLServer.Generates.Model
{
    public class FileGroupFiles : List<FileGroupFile>
    {
        private Hashtable hash = new Hashtable();

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="parent">
        /// Objeto Database padre.
        /// </param>
        public FileGroupFiles(FileGroup parent)
        {
            this.Parent = parent;
        }

        /// <summary>
        /// Clona el objeto FileGroups en una nueva instancia.
        /// </summary>
        public FileGroupFiles Clone(FileGroup parentObject)
        {
            FileGroupFiles columns = new FileGroupFiles(parentObject);
            for (int index = 0; index < this.Count; index++)
            {
                columns.Add((FileGroupFile)this[index].Clone(parentObject));
            }
            return columns;
        }

        /// <summary>
        /// Indica si el nombre del FileGroup existe en la coleccion de tablas del objeto.
        /// </summary>
        /// <param name="table">
        /// Nombre de la tabla a buscar.
        /// </param>
        /// <returns></returns>
        public Boolean Find(string table)
        {
            return hash.ContainsKey(table);
        }

        /// <summary>
        /// Agrega un objeto columna a la coleccion de columnas.
        /// </summary>
        public new void Add(FileGroupFile file)
        {
            if (file != null)
            {
                hash.Add(file.FullName, file);
                base.Add(file);
            }
            else
                throw new ArgumentNullException("file");
        }

        public FileGroupFile this[string name]
        {
            get { return (FileGroupFile)hash[name]; }
            set
            {
                hash[name] = value;
                for (int index = 0; index < base.Count; index++)
                {
                    if (((FileGroupFile)base[index]).Name.Equals(name))
                    {
                        base[index] = value;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Devuelve la tabla perteneciente a la coleccion de campos.
        /// </summary>
        public FileGroup Parent { get; private set; }

        public string ToSQL()
        {
            StringBuilder sql = new StringBuilder();
            for (int index = 0; index < this.Count; index++)
            {
                sql.Append(this[index].ToSql());
            }
            return sql.ToString();
        }

        public string ToSQLDrop()
        {
            StringBuilder sql = new StringBuilder();
            for (int index = 0; index < this.Count; index++)
            {
                sql.Append(this[index].ToSqlDrop());
            }
            return sql.ToString();
        }
    }
}
