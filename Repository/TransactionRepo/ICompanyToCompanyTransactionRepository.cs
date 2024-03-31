using PayBridgeAPI.Models.Transcations;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository.TransactionRepo
{
    public interface ICompanyToCompanyTransactionRepository : ITranscationRepository<CompanyToCompanyTransaction>
    {
        Task UpdateTransaction(CompanyToCompanyTransaction transaction);
        Task SaveChanges();
    }
}
