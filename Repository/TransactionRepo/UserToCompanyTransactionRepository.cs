using PayBridgeAPI.Data;
using PayBridgeAPI.Models.Transcations;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository.TransactionRepo
{
    public class UserToCompanyTransactionRepository(PayBridgeDbContext context) : TransactionRepository<UserToCompanyTransaction>(context), IUserToCompanyTransactionRepository
    {
        private readonly PayBridgeDbContext _context = context;
        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTransaction(UserToCompanyTransaction transaction)
        {
            _context.Update(transaction);
            await SaveChanges();
        }
    }
}
