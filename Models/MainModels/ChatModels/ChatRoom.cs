using PayBridgeAPI.Models.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayBridgeAPI.Models.MainModels.ChatModels
{
    public class ChatRoom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string ChatName { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public int ManagerId { get; set; }
        [ForeignKey(nameof(ManagerId))]
        public virtual Manager Manager { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<ApplicationUser> Participants { get; set; }
        public ICollection<ChatLine> ChatLines { get; set; }
    }
}
