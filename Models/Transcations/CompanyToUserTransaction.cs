using PayBridgeAPI.Models.MainModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.Transcations
{
    public class CompanyToUserTransaction : Transaction
    {
        [Required]
        public int CompanySenderId { get; set; }
        [ForeignKey(nameof(CompanySenderId))]
        public virtual CorporateBankAccount CompanySender { get; set; }
        [Required]
        public int ReceiverId { get; set; }
        [ForeignKey(nameof(ReceiverId))]
        public virtual PersonalBankAccount Receiver { get; set; }
        [Required]
        public int ReceiverBankCardId { get; set; }
        [ForeignKey(nameof(ReceiverBankCardId))]
        public virtual BankCard BankCard { get; set; }
        [Required]
        public int SenderBankAssetId { get; set; }
        [ForeignKey(nameof(SenderBankAssetId))]
        public virtual CompanyBankAsset SenderBankAsset { get; set; }
    }
}
