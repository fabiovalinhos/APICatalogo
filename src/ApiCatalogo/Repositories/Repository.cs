using System.Linq.Expressions;
using ApiCatalogo.Context;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate)
        {

            var query = _context.Set<T>().AsNoTracking();

            if (predicate is not null)
            {
                query = query.Where(predicate);
            }

            return await query.ToListAsync();
        }


        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
        {
            Thread.Sleep(4000); // Simulando uma operação demorada
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }


        public T Create(T entity)
        {
            _context.Set<T>().Add(entity);
            // _context.SaveChanges();
            return entity;
        }

        public T Update(T entity)
        {
            // _context.Set<T>().Update(entity);
            _context.Entry(entity).State = EntityState.Modified;
            // _context.SaveChanges();
            return entity;
        }

        public T Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            // _context.SaveChanges();

            return entity;
        }
    }
}