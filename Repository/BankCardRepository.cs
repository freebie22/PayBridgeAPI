using PayBridgeAPI.Data;
using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository
{
    public class BankCardRepository(PayBridgeDbContext context) : Repository<BankCard>(context), IBankCardRepository
    {
        private readonly PayBridgeDbContext _context = context;

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BankCard bankCard)
        {
            _context.Update(bankCard);
            await SaveChangesAsync();
        }
    }
}
