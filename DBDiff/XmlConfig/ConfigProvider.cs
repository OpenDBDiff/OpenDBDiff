using System;
using System.Collections.Generic;
using System.Text;

namespace DBDiff.XmlConfig
{
    public class ConfigProvider
    {
        private string description;
        private string key;
        private string library;

        public string Library
        {
            get { return library; }
            set { library = value; }
        }

        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
    }
}
