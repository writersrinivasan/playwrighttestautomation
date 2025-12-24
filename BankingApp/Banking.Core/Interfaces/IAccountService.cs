using Banking.Core.Entities;

namespace Banking.Core.Interfaces
{
    public interface IAccountService
    {
        void Deposit(Account account, decimal amount);
        void Withdraw(Account account, decimal amount);
        void Transfer(Account source, Account destination, decimal amount);
    }
}
