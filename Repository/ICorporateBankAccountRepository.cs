using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository
{
    public interface ICorporateBankAccountRepository : IRepository<CorporateBankAccount>
    {
        Task UpdateAccount(CorporateBankAccount account);
        Task SaveChanges();
    }
}
