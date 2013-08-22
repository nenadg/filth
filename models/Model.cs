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
    public class Blog
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "You must name it somehow?!")]
        [Display(Name = "Site name")]
        public string Title { get; set; }

        [Display(Name = "Your name")]
        public string BloggerName { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime DateCreated { get; set; }
        public string Content { get; set; }
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }

    public class BlogContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public BlogContext()
            : base("name=filth1connection") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Role>()
                .HasMany(u => u.Users)
                .WithMany(t => t.Roles)
                .Map(mc =>
                {
                    mc.ToTable("RoleUsers");
                    mc.MapLeftKey("RoleId");
                    mc.MapRightKey("UserId");
                    
                });
 
            // later: configure model with fluent API
           
        }
        
   
    }
}