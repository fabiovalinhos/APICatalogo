using System.Linq.Expressions;

namespace ApiCatalogo.Repositories
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate);

        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);

        T Create(T entity);

        T Update(T entity);

        T Delete(T entity);
    }
}