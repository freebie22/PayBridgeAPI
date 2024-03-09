using System.Linq.Expressions;

namespace PayBridgeAPI.Repository.MainRepo
{
    public interface IRepository<T> where T : class
    {
        Task<IList<T>> GetAllValues(Expression<Func<T, bool>> filter = null, bool isTracked = true, string includeProperties = "");
        Task<T> GetValueAsync(Expression<Func<T, bool>> filter = null, bool isTracked = true, string includeProperties = "");
        Task CreateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
