using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using DBDiff.Schema.Model;

namespace DBDiff.Schema.SQLServer.Model
{
    public class TableOptions:SchemaList<TableOption,Table> 
    {
        public TableOptions(Table parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Clona el objeto Columns en una nueva instancia.
        /// </summary>
        public TableOptions Clone(Table parent)
        {
            TableOptions options = new TableOptions(parent);
            for (int index = 0; index < this.Count; index++)
            {
                options.Add(this[index].Clone(parent));
            }
            return options;
        }

        /// <summary>
        /// Agrega un objeto columna a la coleccion de columnas.
        /// </summary>
        public void Add(string name, string value)
        {
            TableOption prop = new TableOption(Parent);
            prop.Name = name;
            prop.Value = value;
            prop.Status = Enums.ObjectStatusType.OriginalStatus;
            base.Add(prop);
        }
    }
}
