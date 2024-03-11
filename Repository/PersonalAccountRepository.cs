using PayBridgeAPI.Data;
using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository
{
    public class PersonalAccountRepository : Repository<PersonalAccountHolder>, IPersonalAccountRepository
    {
        private readonly PayBridgeDbContext _context;
        public PersonalAccountRepository(PayBridgeDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAccount(PersonalAccountHolder account)
        {
            _context.Update(account);
            await SaveChanges();
        }
    }
}
