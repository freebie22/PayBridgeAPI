using PayBridgeAPI.Data;
using PayBridgeAPI.Models.Transcations;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository.TransactionRepo
{
    public class CompanyToUserTransactionRepository(PayBridgeDbContext context) : TransactionRepository<CompanyToUserTransaction>(context), ICompanyToUserTransactionRepository
    {
        private readonly PayBridgeDbContext _context = context;
        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTransaction(CompanyToUserTransaction transaction)
        {
            _context.Update(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
