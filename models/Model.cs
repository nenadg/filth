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
    public class Culture
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        // http://www.w3.org/International/articles/language-tags/
        [Required(ErrorMessage = "Language code is required.")]
        public string Code { get; set; }

        public virtual ICollection<Article> Articles { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; }

        public Culture()
        {
            Blogs = new List<Blog>();
        }
    }

    public class Blog
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "You must name it somehow?!")]
        [Display(Name = "Site name")]
        public string Title { get; set; }

        [Display(Name = "Subtitle (not necessary)")]
        public string Subtitle { get; set; }

        [Display(Name = "Your name")]
        public string Name { get; set; }
        public virtual ICollection<Culture> Cultures { get; set; }
    }

    public class Article
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [Display(Name = "Article title")]
        public string Title { get; set; }

        [Display(Name = "Article")]
        public string Content { get; set; }

        public string Abstract { get; set; }
        public string Author { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Int16 State { get; set; }
        public Int16 CommentsState { get; set; }
        public Int16 MenuOrder { get; set; }
        public Int16 Type { get; set; }

        // relations

        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<File> Files { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        public int CultureId { get; set; }
        public virtual Culture Culture { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [Display(Name = "Category name")]
        public string Name { get; set; }

        public virtual ICollection<Article> Articles { get; set; }

        public Category()
        {
            Articles = new List<Article>();
        }
    }

    public class File
    {
        public int Id { get; set; }

         [Display(Name = "File name")]
        public string Name { get; set; }

         [Display(Name = "File description")]
        public string Description { get; set; }
        public string MimeType { get; set; }
        public Byte[] Bytes { get; set; }
        public int HitCountTotal { get; set; }
        public int HitCountOriginal { get; set; }

        public virtual ICollection<Article> Articles { get; set; }

        public File()
        {
            Articles = new List<Article>();
        }
    }

    public class Comment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [Display(Name = "Your name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "E-mail is required.")]
        [Display(Name = "Your e-mail")]
        public string Mail { get; set; }

        [Display(Name = "Website")]
        public string Web { get; set; }

        public DateTime DateCreated { get; set; }

        [Required(ErrorMessage = "Comment can't be empty.")]
        [Display(Name = "Comment body")]
        public string Content { get; set; }

        public Int16 Status { get; set; }
        public int Parent { get; set; }

        public int ArticleId { get; set; }
        public virtual Article Article { get; set; }
    }


    public class BlogContext : DbContext
    {
        public DbSet<Culture> Cultures { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Profile> Profiles { get; set; }

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

            modelBuilder.Entity<Culture>()
                 .HasMany(b => b.Blogs)
                 .WithMany(c => c.Cultures)
                 .Map(mc =>
                 {
                     mc.ToTable("CultureBlogs");
                     mc.MapLeftKey("CultureId");
                     mc.MapRightKey("BlogId");

                 });

            modelBuilder.Entity<Category>()
                 .HasMany(a => a.Articles)
                 .WithMany(c => c.Categories)
                 .Map(mc =>
                 {
                     mc.ToTable("CategoryArticles");
                     mc.MapLeftKey("CategoryId");
                     mc.MapRightKey("ArticleId");
                 });

            modelBuilder.Entity<File>()
                 .HasMany(a => a.Articles)
                 .WithMany(f => f.Files)
                 .Map(mc =>
                 {
                     mc.ToTable("FileArticles");
                     mc.MapLeftKey("FileId");
                     mc.MapRightKey("ArticleId");
                 });

            modelBuilder.Entity<Profile>()
                .HasOptional(u => u.User)
                .WithOptionalDependent().Map(m=>m.MapKey("UserId"))
                .WillCascadeOnDelete(true);
           
        }
        
   
    }
}