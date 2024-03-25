using PayBridgeAPI.Data;
using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository
{
    public class CorporateBankAccountRepository(PayBridgeDbContext context) : Repository<CorporateBankAccount>(context), ICorporateBankAccountRepository
    {
        private readonly PayBridgeDbContext _context = context;
        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAccount(CorporateBankAccount account)
        {
            _context.Update(account);
            await SaveChanges();
        }
    }
}
