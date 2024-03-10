using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository
{
    public interface IBankRepository : IRepository<Bank>
    {
        Task UpdateAsync(Bank bank);
        Task SaveChangesAsync();
    }
}
