using PayBridgeAPI.Models.MainModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.Transcations
{
    public class CompanyToCompanyTransaction : Transaction
    {
        [Required]
        public int CompanySenderId { get; set; }
        [ForeignKey(nameof(CompanySenderId))]
        public virtual CorporateBankAccount CompanySender { get; set; }
        [Required]
        public int CompanyReceiverId { get; set; }
        [ForeignKey(nameof(CompanyReceiverId))]
        public virtual CorporateBankAccount CompanyReceiver { get; set; }
        [Required]
        public int ReceiverBankAssetId { get; set; }
        [ForeignKey(nameof(ReceiverBankAssetId))]
        public virtual CompanyBankAsset ReceiverBankAsset { get; set; }
        [Required]
        public int SenderBankAssetId { get; set; }
        [ForeignKey(nameof(SenderBankAssetId))]
        public virtual CompanyBankAsset SenderBankAsset { get; set; }

        [Required]
        public string StripePaymentIntentId { get; set; }
    }
}
