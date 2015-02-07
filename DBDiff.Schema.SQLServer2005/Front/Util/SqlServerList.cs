using System.Collections.Generic;
using System.Data;
using System.Data.Sql;

namespace DBDiff.Schema.SQLServer.Generates.Front.Util
{
    internal static class SqlServerList
    {
       public static List<string> Get()
       {
             SqlDataSourceEnumerator sqlSource = SqlDataSourceEnumerator.Instance;
             DataTable dt = sqlSource.GetDataSources();

             List<string> serverList = new List<string>();
             string serverName = null;
             string instanceName = null;

             foreach(DataRow dr in dt.Rows)
             {
                 serverName = dr["ServerName"].ToString();
                 instanceName = dr["InstanceName"] != null ? dr["InstanceName"].ToString() : null;

                 if (string.IsNullOrEmpty(instanceName))
                     serverList.Add(serverName);
                 else
                     serverList.Add(string.Format("{0}\\{1}", serverName, instanceName));
             }

             return serverList;
       }
    }
}
