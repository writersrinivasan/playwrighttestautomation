using Microsoft.AspNetCore.Mvc;
using Banking.Core.Entities;
using Banking.Core.Interfaces;
using Banking.Core.Exceptions;
using Banking.API.Models;
using System.Collections.Generic;
using System.Linq;

namespace Banking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly List<Account> _accounts; // In-memory store

        public AccountsController(IAccountService accountService, List<Account> accounts)
        {
            _accountService = accountService;
            _accounts = accounts;
        }

        [HttpPost]
        public IActionResult CreateAccount([FromBody] CreateAccountRequest request)
        {
            if (!Enum.TryParse<AccountType>(request.AccountType, true, out var type))
            {
                return BadRequest("Invalid account type.");
            }

            var account = new Account(Guid.NewGuid().ToString().Substring(0, 8), request.Owner, type);
            _accounts.Add(account);
            return CreatedAtAction(nameof(GetAccount), new { accountNumber = account.AccountNumber }, account);
        }

        [HttpGet("{accountNumber}")]
        public IActionResult GetAccount(string accountNumber)
        {
            var account = _accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            if (account == null) return NotFound();
            return Ok(account);
        }

        [HttpPost("deposit")]
        public IActionResult Deposit([FromBody] DepositRequest request)
        {
            var account = _accounts.FirstOrDefault(a => a.AccountNumber == request.AccountNumber);
            if (account == null) return NotFound("Account not found.");

            try
            {
                _accountService.Deposit(account, request.Amount);
                return Ok(account);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("withdraw")]
        public IActionResult Withdraw([FromBody] WithdrawRequest request)
        {
            var account = _accounts.FirstOrDefault(a => a.AccountNumber == request.AccountNumber);
            if (account == null) return NotFound("Account not found.");

            try
            {
                _accountService.Withdraw(account, request.Amount);
                return Ok(account);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("transfer")]
        public IActionResult Transfer([FromBody] TransferRequest request)
        {
            var source = _accounts.FirstOrDefault(a => a.AccountNumber == request.SourceAccountNumber);
            var dest = _accounts.FirstOrDefault(a => a.AccountNumber == request.DestinationAccountNumber);

            if (source == null || dest == null) return NotFound("One or both accounts not found.");

            try
            {
                _accountService.Transfer(source, dest, request.Amount);
                return Ok(new { Message = "Transfer successful", SourceBalance = source.Balance, DestinationBalance = dest.Balance });
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
