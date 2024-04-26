using PayBridgeAPI.Data;
using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository.ResponsiblePeopleRepo
{
    public class ResponsiblePeopleRepository(PayBridgeDbContext context) : Repository<ResponsiblePerson>(context), IResponsiblePersonRepository
    {
        private readonly PayBridgeDbContext _context = context;

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateResponsiblePerson(ResponsiblePerson responsiblePerson)
        {
            _context.Update(responsiblePerson);
            await _context.SaveChangesAsync();
        }
    }
}
