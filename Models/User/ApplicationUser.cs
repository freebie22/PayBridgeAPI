using Microsoft.AspNetCore.Identity;
using PayBridgeAPI.Models.MainModels.ChatModels;

namespace PayBridgeAPI.Models.User
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public byte[] ProfileImage { get; set; }
        public virtual ICollection<ChatRoom> ChatRooms { get; set; }
    }
}
