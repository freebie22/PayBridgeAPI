using PayBridgeAPI.Models.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayBridgeAPI.Models.MainModels.ChatModels
{
    public class ChatLine
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ChatLineId { get; set; }
        [Required]  
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }
        [Required]
        public long ChatRoomId { get; set; }
        [ForeignKey(nameof(ChatRoomId))]
        public virtual ChatRoom ChatRoom { get; set; }
        [Required]
        public string TextMessage { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
