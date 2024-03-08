using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.DTO
{
    public class RegistrationRequestDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Role { get; set; }
        public IFormFile ProfileImage { get; set; }
    }
}
