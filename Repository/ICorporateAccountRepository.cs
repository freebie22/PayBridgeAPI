using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository
{
    public interface ICorporateAccountRepository : IRepository<CorporateAccountHolder>
    {
        Task UpdateAccount(CorporateAccountHolder account);
        Task SaveChanges();
    }
}
