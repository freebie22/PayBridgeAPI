using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.DTO
{
    public class ManagerUpdateDTO
    {
        public int ManagerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Position { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
