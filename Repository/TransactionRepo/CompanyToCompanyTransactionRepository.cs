using PayBridgeAPI.Data;
using PayBridgeAPI.Models.Transcations;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository.TransactionRepo
{
    public class CompanyToCompanyTransactionRepository(PayBridgeDbContext context) : TransactionRepository<CompanyToCompanyTransaction>(context), ICompanyToCompanyTransactionRepository
    {
        private readonly PayBridgeDbContext _context = context;
        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTransaction(CompanyToCompanyTransaction transaction)
        {
           _context.Update(transaction);
            await SaveChanges();
        }
    }
}
