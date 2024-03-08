using Microsoft.AspNetCore.Identity;

namespace PayBridgeAPI.Models.User
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public byte[] ProfileImage { get; set; }
    }
}
