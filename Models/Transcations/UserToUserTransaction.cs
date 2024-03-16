using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Models.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayBridgeAPI.Models.Transcations
{
    public class UserToUserTransaction : Transaction
    {
        [Required]
        public int SenderId { get; set; }
        [ForeignKey(nameof(SenderId))]
        public virtual PersonalBankAccount Sender { get; set; }
        [Required]
        public int ReceiverId { get; set; }
        [ForeignKey(nameof(ReceiverId))]
        public virtual PersonalBankAccount Receiver { get; set; }
        [Required]
        public int SenderBankCardId { get; set; }
        [ForeignKey(nameof(SenderBankCardId))]
        public virtual BankCard SenderBankCard { get; set; }
        [Required]
        public int ReceiverBankCardId { get; set; }
        [ForeignKey(nameof(ReceiverBankCardId))]
        public virtual BankCard ReceiverBankCard { get; set; }
        [Required]
        public string StripePaymentIntentId { get; set; }
    }
}
