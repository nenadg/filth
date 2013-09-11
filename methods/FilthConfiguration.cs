using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;
using DevOne.Security.Cryptography.BCrypt;
using System.Data.Entity;
using filth.models;

namespace filth.methods
{

    public class FilthConfiguration 
    {
        public ConnectionStringState CheckConnection()
        {
            if (ConfigurationManager.ConnectionStrings["filth1connection"] == null)
                return ConnectionStringState.Absent;
            else
            {
                try
                {
                    ConnectionStringSettings connectionstringsets = ConfigurationManager.ConnectionStrings["filth1connection"];

                    using (var connection = new SqlConnection(connectionstringsets.ConnectionString))
                    {
                        connection.Open();
                        return ConnectionStringState.Present;
                    }
                }
                catch
                {
                    return ConnectionStringState.Invalid;
                }
            }
        }

        public void CreateConnectionString(ServerConfiguration serverConfiguration)
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            ConnectionStringSettings connectionstringsets = new ConnectionStringSettings();
            connectionstringsets.Name = "filth1connection";

            // create connection string
            if (serverConfiguration.Live)
            {
                connectionstringsets.ConnectionString = "Data Source=" + serverConfiguration.ServerName + ";Initial Catalog=" + serverConfiguration.Catalog + ";Integrated Security=false;User ID=" + serverConfiguration.Username + ";Password=" + serverConfiguration.Password + ";multipleactiveresultsets=True;App=EntityFramework;"; //Encrypt=yes
                try
                {
                    using (var connection = new SqlConnection(connectionstringsets.ConnectionString))
                    {
                        connection.Open();
                        // most hostings doesn't allow this
                        // var command = connection.CreateCommand();
                        // command.CommandText = "CREATE DATABASE " + database;
                        // command.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else
                connectionstringsets.ConnectionString = "Data Source=.\\SQLEXPRESS;AttachDBFilename=|DataDirectory|filthdb_" + new Random().Next() + ".mdf" + ";User Instance=true;Integrated Security=true;multipleactiveresultsets=True;App=EntityFramework";

            connectionstringsets.ProviderName = "System.Data.SqlClient";

            config.ConnectionStrings.ConnectionStrings.Add(connectionstringsets);

            config.Save();

        }

        public void RemoveConnectionString()
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            config.ConnectionStrings.ConnectionStrings.Remove("filth1connection");
            config.Save();
        }
    }
}