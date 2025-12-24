using System;
using System.Collections.Generic;

namespace Banking.Core.Entities
{
    public enum AccountType
    {
        Savings,
        Checking
    }

    public class Account
    {
        public string AccountNumber { get; set; }
        public string Owner { get; set; }
        public decimal Balance { get; private set; }
        public AccountType Type { get; set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        public Account(string accountNumber, string owner, AccountType type)
        {
            AccountNumber = accountNumber;
            Owner = owner;
            Type = type;
            Balance = 0;
        }

        public void UpdateBalance(decimal amount)
        {
            Balance += amount;
        }
    }
}
