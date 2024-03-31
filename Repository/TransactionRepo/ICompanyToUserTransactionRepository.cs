using PayBridgeAPI.Models.Transcations;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository.TransactionRepo
{
    public interface ICompanyToUserTransactionRepository : ITranscationRepository<CompanyToUserTransaction>
    {
        Task UpdateTransaction(CompanyToUserTransaction transaction);
        Task SaveChanges();
    }
}
