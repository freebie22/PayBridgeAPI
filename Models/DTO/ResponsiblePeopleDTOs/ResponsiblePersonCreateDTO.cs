using PayBridgeAPI.Models.User;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.DTO.ResponsiblePeopleDTOs
{
    public class ResponsiblePersonCreateDTO
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string PositionInCompany { get; set; }
        [Required]
        public string Login { get; set; }
    }
}
