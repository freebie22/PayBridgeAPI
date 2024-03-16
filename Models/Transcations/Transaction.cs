using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.Transcations
{
    public abstract class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        [Required]
        public string TransactionNumber { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string CurrencyCode { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string TransactionType { get; set; }
        [Required]
        public DateTime DateOfTransaction { get; set; } = DateTime.Now;
        public string Description { get; set; } = string.Empty;
        public decimal? Fee { get; set; }
    }
}
