using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository
{
    public interface IBankCardRepository : IRepository<BankCard>
    {
        Task UpdateAsync(BankCard bankCard);
        Task SaveChangesAsync();
    }
}
