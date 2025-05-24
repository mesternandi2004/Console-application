using Castle.Components.DictionaryAdapter;
using CQW1QQ_HSZF_2024251.Persistence.MsSql.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CQW1QQ_HSZF_2024251.Persistence.MsSql.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // Create
        public virtual void Create(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        // Read All
        public virtual IEnumerable<T> ReadAll()
        {
            return _dbSet;
        }

        // Read by Condition
        public virtual IEnumerable<T> Read(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToList();
        }

        // Get by ID (Assumes entities have a property named "Id")
        public virtual T? Read(object primaryKeyValue)
        {
            return _dbSet.Find(primaryKeyValue);
        }

        // Update
        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }

        // Delete
        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }
    }
}
