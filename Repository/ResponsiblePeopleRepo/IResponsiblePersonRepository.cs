using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository.ResponsiblePeopleRepo
{
    public interface IResponsiblePersonRepository : IRepository<ResponsiblePerson>
    {
        Task UpdateResponsiblePerson(ResponsiblePerson responsiblePerson);
        Task SaveChangesAsync();
    }
}
