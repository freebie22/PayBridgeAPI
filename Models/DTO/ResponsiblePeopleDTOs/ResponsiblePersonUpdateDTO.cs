using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.DTO.ResponsiblePeopleDTOs
{
    public class ResponsiblePersonUpdateDTO
    {
        public int ResponsiblePersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string PositionInCompany { get; set; }
        public bool IsActive { get; set; }
    }
}
