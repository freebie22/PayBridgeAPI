using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.DTO
{
    public class UserDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImage { get; set; }
    }
}
