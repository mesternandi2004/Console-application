using System.Linq.Expressions;

namespace CQW1QQ_HSZF_2024251.Persistence.MsSql.Repository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        void Create(T entity);
        void Delete(T entity);
        IEnumerable<T> Read(Expression<Func<T, bool>> predicate);
        T? Read(object primaryKeyValue);
        IEnumerable<T> ReadAll();
        void Update(T entity);
    }
}