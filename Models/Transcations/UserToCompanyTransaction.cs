using PayBridgeAPI.Models.MainModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayBridgeAPI.Models.Transcations
{
    public class UserToCompanyTransaction : Transaction
    {
        [Required]
        public int SenderId { get; set; }
        [ForeignKey(nameof(SenderId))]
        public virtual PersonalBankAccount Sender { get; set; }
        [Required]
        public int CompanyReceiverId { get; set; }
        [ForeignKey(nameof(CompanyReceiverId))]
        public virtual CorporateBankAccount CompanyReceiver { get; set; }
        [Required]
        public int SenderBankCardId { get; set; }
        [ForeignKey(nameof(SenderBankCardId))]
        public virtual BankCard BankCard { get; set; }

        [Required]
        public string StripePaymentIntentId { get; set; }

    }
}
