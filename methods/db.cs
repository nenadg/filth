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
using filth.models;

namespace filth.methods
{
    public interface IFilthConfiguration
    {
        ConnectionStringState CheckConnection();
        void CreateConnectionString(ServerConfiguration serverConfiguration);
        void RemoveConnectionString();
        void CreateDatabase(Blog context);
        void CreateUser(User user, Role role);
        void CreateRole(Role role);

        bool DoWeHaveAnyUsers();
        bool ValidateLogin(User user);
 
    }

    public class FilthConfiguration : IFilthConfiguration
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

        // TODO : ubaciti provajdere za druge baze
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

        public void CreateDatabase(Blog context)
        {
            using (var db = new BlogContext())
            {
                db.Blogs.Add(context);
                db.SaveChanges();
            }
        }

        public void CreateUser(User user, Role role)
        {
            using (var db = new BlogContext())
            {
                if (!DoesUserExist(user))
                {
                    CreateRole(role);
                    CreateUser(user);

                    // attach 'user' object to context
                    // to notify ef that such object already exist
                    db.Users.Attach(user);

                    db.Roles.FirstOrDefault(u => u.Name == role.Name).Users.Add(user);

                    Profile profile = new Profile();
                    profile.Name = db.Blogs.FirstOrDefault().Name;
                    profile.User = user;

                    db.Profiles.Add(profile);

                    db.SaveChanges();
                }
                else
                    throw new Exception("User named " + user.Username + " is already here.");
            }
        }

        private void CreateUser(User user)
        {
            using (var db = new BlogContext())
            {
                // no need explaining why I use bcrypt 
                // but in case you wonder, this is just the thread you need - https://news.ycombinator.com/item?id=4073477
                string salt = BCryptHelper.GenerateSalt(10);
                user.Password = BCryptHelper.HashPassword(user.Password, salt);
                user.Salt = salt;

                db.Users.Add(user);
                db.SaveChanges();
            }
        }

        public void CreateRole(Role role)
        {
            using (var db = new BlogContext())
            {
                if (!DoesRoleExist(role))
                {
                    db.Roles.Add(role);
                    db.SaveChanges();
                }
            }
        }

        public bool DoWeHaveAnyUsers()
        {
            using (var db = new BlogContext())
            {
                if (db.Users.Count() == 0 && db.Roles.Count() == 0)
                    return false;
                else
                    return true;

            }
        }

        public bool ValidateLogin(User user)
        {
            using (var db = new BlogContext())
            {
                if (!DoesUserExist(user))
                    return false;
                else
                {
                    string hashed = db.Users.FirstOrDefault(u => u.Username == user.Username).Password;
                    if (BCryptHelper.CheckPassword(user.Password, hashed))
                        return true;
                    else
                        return false;
                }
                    
            }
        }

        private bool DoesUserExist(User user)
        {
            using (var db = new BlogContext())
            {
                if (db.Users.Any(u => u.Username == user.Username))
                    return true;
                else
                    return false;
            }
        }  

        private bool DoesRoleExist(Role role)
        {
            using (var db = new BlogContext())
            {
                if (db.Roles.Any(u => u.Name == role.Name))
                    return true;
                else
                    return false;
            }
        }

    }
}