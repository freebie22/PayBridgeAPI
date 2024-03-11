using PayBridgeAPI.Data;
using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository
{
    public class PersonalBankAccountRepository(PayBridgeDbContext context) : Repository<PersonalBankAccount>(context), IPersonalBankAccountRepository
    {
        private readonly PayBridgeDbContext _context = context;

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAccount(PersonalBankAccount account)
        {
            _context.Update(account);
            await SaveChanges();
        }
    }
}
