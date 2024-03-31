using PayBridgeAPI.Models.Transcations;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository.TransactionRepo
{
    public interface IUserToCompanyTransactionRepository : ITranscationRepository<UserToCompanyTransaction>
    {
        Task UpdateTransaction(UserToCompanyTransaction transaction);
        Task SaveChanges();
    }
}
