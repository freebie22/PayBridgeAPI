using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository
{
    public interface IPersonalAccountRepository : IRepository<PersonalAccountHolder>
    {
        Task UpdateAccount(PersonalAccountHolder account);
        Task SaveChanges();
    }
}
