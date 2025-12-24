namespace Banking.API.Models
{
    public class DepositRequest
    {
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
    }

    public class WithdrawRequest
    {
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
    }

    public class TransferRequest
    {
        public string SourceAccountNumber { get; set; }
        public string DestinationAccountNumber { get; set; }
        public decimal Amount { get; set; }
    }

    public class CreateAccountRequest
    {
        public string Owner { get; set; }
        public string AccountType { get; set; } // "Savings" or "Checking"
    }
}
