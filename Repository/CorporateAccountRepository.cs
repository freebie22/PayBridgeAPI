using PayBridgeAPI.Data;
using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository
{
    public class CorporateAccountRepository : Repository<CorporateAccountHolder>, ICorporateAccountRepository
    {
        private readonly PayBridgeDbContext _context;
        public CorporateAccountRepository(PayBridgeDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAccount(CorporateAccountHolder account)
        {
            _context.Update(account);
            await SaveChanges();
        }
    }
}
