using System;
using Banking.Core.Entities;
using Banking.Core.Exceptions;
using Banking.Core.Interfaces;

namespace Banking.Core.Services
{
    public class AccountService : IAccountService
    {
        public void Deposit(Account account, decimal amount)
        {
            if (amount <= 0)
            {
                throw new ValidationException("Deposit amount must be positive.");
            }

            account.UpdateBalance(amount);
            account.Transactions.Add(new Transaction(amount, TransactionType.Deposit, "Cash Deposit"));
        }

        public void Withdraw(Account account, decimal amount)
        {
            if (amount <= 0)
            {
                throw new ValidationException("Withdrawal amount must be positive.");
            }

            if (account.Balance < amount)
            {
                throw new ValidationException("Insufficient funds.");
            }

            account.UpdateBalance(-amount);
            account.Transactions.Add(new Transaction(-amount, TransactionType.Withdrawal, "Cash Withdrawal"));
        }

        public void Transfer(Account source, Account destination, decimal amount)
        {
            if (source == null || destination == null)
            {
                throw new ValidationException("Source and destination accounts must handle valid.");
            }

            if (amount <= 0)
            {
                throw new ValidationException("Transfer amount must be positive.");
            }

            if (source.AccountNumber == destination.AccountNumber)
            {
                throw new ValidationException("Cannot transfer to the same account.");
            }

            if (source.Balance < amount)
            {
                throw new ValidationException("Insufficient funds for transfer.");
            }

            // Debit Source
            source.UpdateBalance(-amount);
            source.Transactions.Add(new Transaction(-amount, TransactionType.TransferOut, $"Transfer to {destination.AccountNumber}", destination.AccountNumber));

            // Credit Destination
            destination.UpdateBalance(amount);
            destination.Transactions.Add(new Transaction(amount, TransactionType.TransferIn, $"Transfer from {source.AccountNumber}", source.AccountNumber));
        }
    }
}
