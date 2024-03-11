using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository
{
    public interface IPersonalBankAccountRepository : IRepository<PersonalBankAccount>
    {
        Task UpdateAccount(PersonalBankAccount account);
        Task SaveChanges();
    }
}
