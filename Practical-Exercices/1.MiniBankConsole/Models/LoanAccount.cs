using MiniBankConsole.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MiniBankConsole.Models
{
    public class LoanAccount: BankAccount, IInterestBearing, IStatement
    {
        private const decimal _interestRate = 0.01m;
        public LoanAccount(string owner, decimal initialLoan)
            : base(owner, openingBalance: -Math.Abs(initialLoan)) {}

        public override bool Deposit(decimal amount, out decimal accepted)
        {
            accepted = 0;
            if (amount <= 0 || Balance == 0) 
            { 
                Log.Add($"DEPOSIT FAILED {amount.ToString("C")}");
                return false; 
            }

            var owed = Math.Abs(Balance);
            var pay = Math.Min(amount, owed);
            Balance += pay;
            accepted = pay;
            Log.Add($"LOAN REPAY {pay.ToString("C")}; BAL -{Math.Abs(Balance):C}");
            return true;
        }
    
        protected override bool TryValidateWithdraw(decimal amount, out string? error)
        {
            error = null;
            if (amount <= 0)
            {
                error = "You've entered an invalid withdrawal amount.";
                return false;
            }
            Log.Add($"LOAN WITHDRAWAL APPROVED;  {amount.ToString("C")}");
            return true;
        }

        public void ApplyMonthlyInterest()
        {
            if (Balance < 0)
            {
                var interest = Math.Abs(Balance) * _interestRate;
                Balance -= interest;
                Log.Add($"INTEREST {interest.ToString("C")}; BAL {Balance.ToString("C")}");
            }
        }

        public void PrintStatement()
        {
            Console.WriteLine("LOAN ACCOUNT STATEMENT");
            foreach (var line in Log)
            {
                Console.WriteLine(line);
            }   
        }
    }
}
