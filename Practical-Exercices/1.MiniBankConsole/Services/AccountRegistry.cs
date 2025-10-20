using MiniBankConsole.Models;
using MiniBankConsole.Models.Enums;
using MiniBankConsole.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBankConsole.Services
{
    public class AccountRegistry
    {
        private List<BankAccount> _accounts = new ();

        public IReadOnlyList<BankAccount> List() => _accounts;

        public bool TryGet(int id, out BankAccount? account)
        {
            account = _accounts.FirstOrDefault(a => a.Id == id);
            return account != null;
        }

        public BankAccount Create(AccountType type, string owner, decimal openingAmount)
        {
            if (string.IsNullOrWhiteSpace(owner))
            {
                throw new ArgumentException("Owner name cannot be empty.", nameof(owner));
            }

            switch (type)
            {
                case AccountType.Checking:
                    {
                        if (openingAmount < 0)
                            throw new ArgumentException("Opening balance cannot be negative for Checking.");
                        var acc = new CheckingAccount(owner, openingAmount);
                        _accounts.Add(acc);
                        return acc;
                    }
                case AccountType.Savings:
                    {
                        if (openingAmount < 0)
                            throw new ArgumentException("Opening balance cannot be negative for Savings.");
                        var acc = new SavingsAccount(owner, openingAmount);
                        _accounts.Add(acc);
                        return acc;
                    }
                case AccountType.Loan:
                    {
                        if (openingAmount <= 0)
                            throw new ArgumentException("Initial loan must be > 0 for Loan accounts.");
                        var acc = new LoanAccount(owner, openingAmount); 
                        _accounts.Add(acc);
                        return acc;
                    }
                default:
                    throw new NotSupportedException($"Unsupported account type: {type}");
            }
        }

        public bool Deposit(int accountId, decimal amount, out string? error, out decimal accepted)
        {
           error = null;
           accepted = 0;
           if (!TryGet(accountId, out var acc))
           {
                error = "Account not found.";
                return false;
           }
           if (amount <= 0)
           {
                error = "Amount must be > 0.";
                return false;
           }

            var result = acc.Deposit(amount, out accepted);
            if (result == false)
           {
                error = "Deposit not allowed. The loan is already fully repaid or the amount is invalid.";
                return false;
           }

            return true;
        }

        public bool Withdraw(int accountId, decimal amount, out string? error)
        {
            error = null;
            if (!TryGet(accountId, out var acc))
            { 
                error = "Account not found."; 
                return false;
            }
            if (amount <= 0) 
            { 
                error = "Amount must be > 0.";
                return false; 
            }

            var ok = acc.Withdraw(amount, out error);
            return ok;
        }

        public bool PrintStatements(int accountId, out string? error)
        {
            error = null;
            if (!TryGet(accountId, out var acc))
            {
                error = "Account not found.";
                return false;
            }
            if (acc is IStatement statementAccount)
            {
                statementAccount.PrintStatement();
                return true;
            }

            error = "Account does not support statements.";
            return false;
        }

        public int RunMonthEnd()
        {
            int affected = 0;
            foreach (var acc in _accounts.OfType<IInterestBearing>())
            {
                acc.ApplyMonthlyInterest();
                affected++;
            }
            return affected;
        }
    }
}
