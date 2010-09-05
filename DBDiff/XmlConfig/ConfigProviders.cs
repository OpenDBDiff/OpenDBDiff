using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;

namespace DBDiff.XmlConfig
{
    public class ConfigProviders
    {
        private static Hashtable providers = null;

        public static ConfigProvider GetProvider(string key)
        {
            XmlNodeList nodes;
            if (providers == null)
            {
                XmlDocument xmldom = new XmlDocument();
                xmldom.Load("DBDiffConfig.xml");
                nodes = xmldom.SelectNodes("DBDIFF/Providers/Provider");
                providers = new Hashtable();
                for (int index = 0; index < nodes.Count; index++)
                {
                    ConfigProvider provider = new ConfigProvider();
                    provider.Description = nodes[index].Attributes.GetNamedItem("description").Value;
                    provider.Key = nodes[index].Attributes.GetNamedItem("key").Value;
                    provider.Library = nodes[index].Attributes.GetNamedItem("library").Value ;
                    providers.Add(key, provider);
                }
            }
            return (ConfigProvider)providers[key];
        }
    }
}
