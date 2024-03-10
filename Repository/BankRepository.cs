using PayBridgeAPI.Data;
using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository
{
    public class BankRepository :  Repository<Bank>, IBankRepository
    {
        private readonly PayBridgeDbContext _context;

        public BankRepository(PayBridgeDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Bank bank)
        {
            _context.Update(bank);
            await _context.SaveChangesAsync();
        }
    }
}
