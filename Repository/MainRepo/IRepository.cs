using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace PayBridgeAPI.Repository.MainRepo
{
    public interface IRepository<T> where T : class
    {
        Task<IList<T>> GetAllValues(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, bool isTracked = true);
        Task<T> GetValueAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, bool isTracked = true);
        Task CreateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
