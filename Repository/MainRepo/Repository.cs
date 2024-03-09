using Microsoft.EntityFrameworkCore;
using PayBridgeAPI.Data;
using PayBridgeAPI.Models.MainModels;
using System.Linq.Expressions;

namespace PayBridgeAPI.Repository.MainRepo
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly PayBridgeDbContext _context;
        internal DbSet<T> _dbSet { get; set; }

        public Repository(PayBridgeDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IList<T>> GetAllValues(Expression<Func<T, bool>> filter = null, bool isTracked = true, string includeProperties = "")
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            if(filter != null)
            {
                query = query.Where(filter);
            }

            if(!isTracked)
            {
                query = query.AsNoTracking();
            }

            if(!string.IsNullOrEmpty(includeProperties))
            {
                var includeProps = includeProperties.Split(new char[] { ',' }, StringSplitOptions.TrimEntries);
                foreach(var prop in includeProps)
                {
                    query = query.Include(prop);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<T> GetValueAsync(Expression<Func<T, bool>> filter = null, bool isTracked = true, string includeProperties = "")
        {
            IQueryable<T> query = _dbSet.AsQueryable<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!isTracked)
            {
                query = query.AsNoTracking();
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                var includeProps = includeProperties.Split(new char[] { ',' }, StringSplitOptions.TrimEntries);
                foreach (var prop in includeProps)
                {
                    query = query.Include(prop);
                }
            }

            return await query.FirstOrDefaultAsync();

        }

        public async Task CreateAsync(T entity)
        {
            if(entity != null)
            {
                await _context.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentNullException(nameof(entity), "Repository Error. An error occured while updating entity.");
            }
        }

        public async Task DeleteAsync(T entity)
        {
            if(entity != null)
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentNullException(nameof(entity), "Repository Error. An error occured while updating entity.");
            }
        }

    }
}
