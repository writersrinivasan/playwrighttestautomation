using System;
using System.Collections.Generic;
using Xunit;
using Banking.Core.Entities;
using Banking.Core.Services;
using Banking.Core.Exceptions;

namespace Banking.Tests
{
    public class AccountServiceTests
    {
        private readonly AccountService _accountService;

        public AccountServiceTests()
        {
            _accountService = new AccountService();
        }

        [Fact]
        public void Deposit_PositiveAmount_IncreasesBalance()
        {
            // Arrange
            var account = new Account("123", "User", AccountType.Checking);
            decimal initialBalance = account.Balance;
            decimal depositAmount = 100m;

            // Act
            _accountService.Deposit(account, depositAmount);

            // Assert
            Assert.Equal(initialBalance + depositAmount, account.Balance);
            Assert.Single(account.Transactions);
            Assert.Equal(TransactionType.Deposit, account.Transactions[0].Type);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public void Deposit_NonPositiveAmount_ThrowsValidationException(decimal amount)
        {
            // Arrange
            var account = new Account("123", "User", AccountType.Checking);

            // Act & Assert
            var ex = Assert.Throws<ValidationException>(() => _accountService.Deposit(account, amount));
            Assert.Equal("Deposit amount must be positive.", ex.Message);
        }

        [Fact]
        public void Withdraw_ValidAmount_DecreasesBalance()
        {
            // Arrange
            var account = new Account("123", "User", AccountType.Checking);
            _accountService.Deposit(account, 200m); // Seed funds
            decimal initialBalance = account.Balance;
            decimal withdrawAmount = 50m;

            // Act
            _accountService.Withdraw(account, withdrawAmount);

            // Assert
            Assert.Equal(initialBalance - withdrawAmount, account.Balance);
            Assert.Equal(2, account.Transactions.Count); // Deposit + Withdraw
            Assert.Equal(TransactionType.Withdrawal, account.Transactions[1].Type);
        }

        [Fact]
        public void Withdraw_InsufficientFunds_ThrowsValidationException()
        {
            // Arrange
            var account = new Account("123", "User", AccountType.Checking);
            _accountService.Deposit(account, 50m); 

            // Act & Assert
            var ex = Assert.Throws<ValidationException>(() => _accountService.Withdraw(account, 100m));
            Assert.Equal("Insufficient funds.", ex.Message);
        }

        [Fact]
        public void Transfer_ValidAmount_MovesMoney()
        {
            // Arrange
            var source = new Account("A1", "Source", AccountType.Checking);
            var dest = new Account("A2", "Dest", AccountType.Savings);
            _accountService.Deposit(source, 500m);

            // Act
            _accountService.Transfer(source, dest, 200m);

            // Assert
            Assert.Equal(300m, source.Balance);
            Assert.Equal(200m, dest.Balance);
            Assert.Equal(TransactionType.TransferOut, source.Transactions.Last().Type);
            Assert.Equal(TransactionType.TransferIn, dest.Transactions.Last().Type);
        }

        [Fact]
        public void Transfer_ToSameAccount_ThrowsValidationException()
        {
             // Arrange
            var source = new Account("A1", "Source", AccountType.Checking);
            
            // Act & Assert
            var ex = Assert.Throws<ValidationException>(() => _accountService.Transfer(source, source, 100m));
            Assert.Equal("Cannot transfer to the same account.", ex.Message);
        }
    }
}
