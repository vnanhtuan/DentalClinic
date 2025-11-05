using DentalClinic.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DentalClinic.Infrastructure.Repositories
{
    public class Repository<T>: IRepository<T> where T : class
    {
        protected readonly DentalClinicDbContext _context;
        protected readonly DbSet<T> _dbSet;
        public Repository(DentalClinicDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        protected IQueryable<T> QueryAll()
        {
            return _dbSet.AsTracking();
        }
        protected IQueryable<T> QueryAllAsNoTracking()
        {
            return _dbSet.AsNoTracking();
        }
        public virtual async Task<T?> GetByIdAsync(int id) => 
            await _dbSet.FindAsync(id);
        
        public virtual async Task<IEnumerable<T>> GetAllAsync() =>
            await _dbSet.ToListAsync();

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
            await _dbSet.Where(predicate).ToListAsync();

        public virtual async Task AddAsync(T entity) =>
            await _dbSet.AddAsync(entity);
        public virtual void Update(T entity) =>
            _dbSet.Update(entity);
        public virtual void Delete(T entity) =>
            _dbSet.Remove(entity);
        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();

    }
}
