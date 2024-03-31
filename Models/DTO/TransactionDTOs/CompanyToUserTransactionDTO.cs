﻿namespace PayBridgeAPI.Models.DTO.TransactionDTOs
{
    public class CompanyToUserTransactionDTO
    {
        public int TransactionId { get; set; }
        public string TransactionUniqueNumber { get; set; }
        public string SenderCompanyShortName { get; set; }
        public string Sender_CBA_IBANNumber { get; set; }
        public string SenderBankEmitent { get; set; }
        public string ReceiverCredentials { get; set; }
        public string ReceiverBankCardNumber { get; set; }
        public string ReceiverBankEmitent { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public string DateOfTransaction { get; set; }
        public string Description { get; set; }
        public decimal? Fee { get; set; }
        public string Status { get; set; }
    }
}
