using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Configuration;
using System.Web.Configuration;
using System.ComponentModel.DataAnnotations;

namespace filth.models
{

    public enum ConnectionStringState
    {
        Present = 0,
        Absent = 1,
        Invalid = 2
    }

    public class ServerConfiguration
    {
        [Display(Name = "Live installation (If you run this instance on production server, check this).")] 
        public bool Live { get; set; }
        
        [Required(ErrorMessage = "You have to connect to some server.")]
        [Display(Name = "Server address")]
        public string ServerName { get; set; }

        [Required(ErrorMessage = "Enter some name (eg. MyHappyDatabase).")]
        [Display(Name = "Database name")]
        public string Catalog { get; set; }

        [Required(ErrorMessage = "I can't connect if you don't give me the username.")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

    }

    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You must specify your favourite user name.")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Enter your password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Salt { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
        
    }

    public class Role
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You can't have role named <blank>.")]
        [Display(Name = "Role name")]
        public string Name { get; set; }
        
        public virtual ICollection<User> Users { get; set; }

        public Role()
        {
            Users = new List<User>();
        }
    }

    public class Profile
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter your name or whatever you like.")]
        [Display(Name = "Name (eg. John Smith)")]
        public string Name { get; set; }

        [Display(Name = "Tell me something about you...")]
        public string Overview { get; set; }

        public Byte[] Image { get; set; }

        public virtual User User { get; set; }
    }


}