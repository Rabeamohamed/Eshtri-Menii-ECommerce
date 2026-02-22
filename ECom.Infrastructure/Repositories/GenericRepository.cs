using ECom.Core.Interfaces;
using ECom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace ECom.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(T entity)
            =>await _context.Set<T>().AddAsync(entity);
        public async Task<int> CountAsync() 
            => await _context.Set<T>().CountAsync();

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity is not null)
                _context.Set<T>().Remove(entity);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
            => await _context.Set<T>().AsNoTracking().ToListAsync();

        public async Task<IReadOnlyList<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            var query = _context.Set<T>().AsQueryable();
            foreach (var item in includes)
            {
                query = query.Include(item);
            }
            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            return entity;
        }

        public async Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            var query = _context.Set<T>().AsQueryable();
            foreach (var item in includes)
            {
                query = query.Include(item);
            }
            var entity = await query.FirstOrDefaultAsync(x => EF.Property<int>( x, "Id" ) == id);
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
