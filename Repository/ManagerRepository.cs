using PayBridgeAPI.Data;
using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository
{
    public class ManagerRepository : Repository<Manager>, IManagerRepository
    {
        private readonly PayBridgeDbContext _context;

        public ManagerRepository(PayBridgeDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Manager manager)
        {
            if(manager != null)
            {
                _context.Update(manager);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentNullException(nameof(manager), "Repository Error. An error occured while updating manager entity.");
            }
        }
    }
}
