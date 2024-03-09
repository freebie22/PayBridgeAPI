using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository
{
    public interface IManagerRepository : IRepository<Manager>
    {
        Task UpdateAsync(Manager manager);
        Task SaveChangesAsync();
    }
}
