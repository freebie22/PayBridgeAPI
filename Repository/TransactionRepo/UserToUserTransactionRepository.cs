using PayBridgeAPI.Data;
using PayBridgeAPI.Models.Transcations;

namespace PayBridgeAPI.Repository.TransactionRepo
{
    public class UserToUserTransactionRepository : TransactionRepository<UserToUserTransaction>, IUserToUserTransactionRepository
    {
        private readonly PayBridgeDbContext _context;
        public UserToUserTransactionRepository(PayBridgeDbContext context) : base(context) 
        {
            _context = context;
        }
        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTransaction(UserToUserTransaction transaction)
        {
            _context.Update(transaction);
            await SaveChanges();
        }
    }
}
