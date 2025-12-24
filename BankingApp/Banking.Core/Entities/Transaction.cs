using System;

namespace Banking.Core.Entities
{
    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        TransferIn,
        TransferOut
    }

    public class Transaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string Description { get; set; }
        public string RelatedAccountNumber { get; set; } // For transfers

        public Transaction(decimal amount, TransactionType type, string description = null, string relatedAccountNumber = null)
        {
            Date = DateTime.UtcNow;
            Amount = amount;
            Type = type;
            Description = description;
            RelatedAccountNumber = relatedAccountNumber;
        }
    }
}
