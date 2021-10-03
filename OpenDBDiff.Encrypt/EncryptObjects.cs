using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDBDiff.Encrypt
{
    public class EncryptObjects
    {
        public string[] ConnectionStrings { get; set; }
        public List<KeyValuePair<string,string>> OperationSummary { get; set; }

        public EncryptObjects()
        {
            if (OperationSummary == null)
                OperationSummary = new List<KeyValuePair<string, string>>();
        }

       public void EncryptProcedures()
        {
            foreach (string operatingConnectionString in ConnectionStrings)
            {
                SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder(operatingConnectionString);

                SqlConnectionInfo sc = new SqlConnectionInfo();
                sc.ServerName = sb.DataSource;
                sc.DatabaseName = sb.InitialCatalog;

                if (sb.IntegratedSecurity)
                    sc.UseIntegratedSecurity = true;
                else
                {
                    sc.UserName = sb.UserID;
                    sc.Password = sb.Password;
                }

                //Connect to the local, default instance of SQL Server. 
                ServerConnection objServerCOnnection = new ServerConnection(sc);

                Server srv = new Server();
                try // Check to see if server connection details are ok.
                {
                    srv = new Server(objServerCOnnection);
                    if (srv == null)
                    {
                        OperationSummary.Add(new KeyValuePair<string, string>(operatingConnectionString, "The connection to the specified SQL Server could not be established."));
                        continue;
                    }
                }
                catch (Exception SqlConnectionException)
                {
                    OperationSummary.Add(new KeyValuePair<string, string>(operatingConnectionString, SqlConnectionException.Message));
                    continue;
                }

                Database db = new Database();
                try // Check to see if database exists.
                {
                    db = srv.Databases[sc.DatabaseName];
                    if (db == null)
                    {
                        OperationSummary.Add(new KeyValuePair<string, string>(operatingConnectionString, string.Format("The specified database {0} does not seem to be existing on the server.", db.Name)));
                        continue;
                    }
                }
                catch (Exception SqlConnectionException)
                {
                    OperationSummary.Add(new KeyValuePair<string, string>(operatingConnectionString, SqlConnectionException.Message));
                    continue;
                }
                string allSP = "";

                List<KeyValuePair<string, string>> erringProcs = new List<KeyValuePair<string, string>>();

                for (int i = 0; i < db.StoredProcedures.Count; i++)
                {
                    //Define a StoredProcedure object variable by supplying the parent database 
                    //and name arguments in the constructor. 
                    StoredProcedure sp;
                    sp = new StoredProcedure();
                    sp = db.StoredProcedures[i];
                    if (!sp.IsSystemObject)// Exclude System stored procedures
                    {
                        if (!sp.IsEncrypted) // Exclude already encrypted stored procedures
                        {
                            try
                            {
                                string text = "";// = sp.TextBody;
                                sp.TextMode = false;
                                sp.IsEncrypted = true;
                                sp.TextMode = true;
                                sp.Alter();

                                sp = null;
                                text = null;
                            }
                            catch (Exception FailedProcException)
                            {
                                erringProcs.Add(new KeyValuePair<string, string>(sp.Name, FailedProcException.Message));
                            }
                        }
                    }
                }

                if (erringProcs.Count < 1)
                {
                    OperationSummary.Add(new KeyValuePair<string, string>(operatingConnectionString, "All Procedures have been encrypted."));
                }
                else
                {
                    OperationSummary.Add(new KeyValuePair<string, string>(operatingConnectionString,string.Join(",", erringProcs.ToArray())));
                }
            }
        }
    }
}
