using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Web;
using System.Linq.Expressions;
using DevOne.Security.Cryptography.BCrypt;
using filth.models;

namespace filth.methods
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        TEntity Get(object id);
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");

        bool Any(Expression<Func<TEntity, bool>> filter);

        int Count();
        void Insert(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entity);
        void Update(TEntity entity);

        void Save();

    }
    
    public interface ISetup : IDisposable
    {
        void CreateBlog(Blog context);
        void CreateUser(User user, Role role);

        bool ValidateLogin(User user);
        string GenerateSessionKey(Session session, string key);//string username, string key, string useragent, string ip);
        User ValidateUser(string key);
        void RemoveSessionKey(string key);
    }

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        internal BlogContext _context;
        internal DbSet<TEntity> dbSet;

        internal BlogContext context
        {
            get
            {
                if (this._context == null)
                    this._context = new BlogContext();

                return this._context;
            }
        }

        internal Repository()
        {
            this.dbSet = context.Set<TEntity>();
        }

        public virtual bool Any(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> query = dbSet;
            return query.Any(filter);
        }

        public virtual int Count()
        {
            return dbSet.Count();
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
                query = query.Where(filter);


            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
                return orderBy(query).ToList();
            else
                return query.ToList();
        }

        public virtual TEntity Get(object id)
        {
            return dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entity)
        {
            if (context.Entry(entity).State == EntityState.Detached)
                dbSet.Attach(entity);

            dbSet.Remove(entity);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }

    public class Setup : ISetup
    {
        private BlogContext _context;
        private bool disposed = false;

        internal BlogContext context
        {
            get
            {
                if (this._context == null)
                    this._context = new BlogContext();

                return this._context;
            }
        }

        public void CreateBlog(Blog blog)
        {
            context.Blogs.Add(blog);
            context.SaveChanges();
        }

        public void CreateUser(User user, Role role)
        {
            if (context.Users.Count() == 0 && context.Roles.Count() == 0)
            {
                // no need explaining why I use bcrypt 
                // but in case you wonder, this is just the thread you need - https://news.ycombinator.com/item?id=4073477
                string salt = BCryptHelper.GenerateSalt(10);
                user.Password = BCryptHelper.HashPassword(user.Password, salt);
                user.Salt = salt;

                context.Roles.Add(role);
                context.Users.Add(user);
                context.SaveChanges();

                context.Users.Attach(user);
                context.Roles.FirstOrDefault(u => u.Name == role.Name).Users.Add(user);

                Profile profile = new Profile { Name = context.Blogs.FirstOrDefault().Name, User = user };


                context.Profiles.Add(profile);
                context.SaveChanges();
            }
            else
                throw new Exception("It seems that you've already registred master user for this instance.");//"User named " + user.Username + " is already here.");

        }

        public bool ValidateLogin(User user)
        {
            if (context.Users.Any(u => u.Username == user.Username))
            {
                string hashed = context.Users.FirstOrDefault(u => u.Username == user.Username).Password;
                if (BCryptHelper.CheckPassword(user.Password, hashed))
                    return true;
                else
                    return false;
            }
            else
                return false;

        }

        // Ovde ubaciti Session za atribut umjesto ovih zajebancija ...
        public string GenerateSessionKey(Session session, string key)//string username, string key, string useragent, string ip, Datet)
        {
            // randomly generated key will be hashed with user's current password's salt
            User user = context.Users.FirstOrDefault(u => u.Username == session.User.Username);

            if (user.Sessions.Any(u => u.UserAgent == session.UserAgent && u.IP == session.IP && u.Key != null && u.Authorised))
            {
                return null;
            }
            else
            {
                string salt = user.Salt;
                string encrypted = BCryptHelper.HashPassword(key, salt).Remove(0, salt.Length);

                if (session.User.Remember)
                    session.Expires = DateTime.Now.AddMonths(12);

                session.User = user;
                session.Key = encrypted;

                context.Sessions.Add(session);
                context.SaveChanges();

                context.Entry(user).State = EntityState.Modified;
                context.Users.Attach(user);
                user.Sessions.Add(session);

                context.SaveChanges();

                return encrypted;
            }
        }

        public User ValidateUser(string key)
        {
            Session session = context.Sessions.FirstOrDefault(s => s.Key == key);
            if (session != null && session.Authorised)
                return session.User;

            return null;
        }

        public void RemoveSessionKey(string key)
        {
            Session session = context.Sessions.FirstOrDefault(s => s.Key == key);
            
            context.Entry(session).State = EntityState.Modified;
            context.Sessions.Attach(session);
            session.Authorised = false; 

            context.SaveChanges();

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
    
}