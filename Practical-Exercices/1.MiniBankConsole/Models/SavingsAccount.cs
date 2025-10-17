using MiniBankConsole.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBankConsole.Models
{
    public class SavingsAccount: BankAccount, IInterestBearing, IStatement
    {
        // Properties
        private const decimal _interestRate = 0.01m; 

        // Constructor
        public SavingsAccount( string owner, decimal openingBalance) 
            : base(owner, openingBalance) {  }

        // Methods
        protected override bool TryValidateWithdraw(decimal amount, out string? error)
        {
            error = null;
            if (amount <= 0)
            {
                error = "You've entered an invalid withdrawal amount.";
                return false;
            }
            if (Balance == 0 || amount > Balance)
            {
                error = $"Insufficient funds. Your current balance is {Balance.ToString("C")}.";
                return false;
            }
            Log.Add($"SUFFICIENT FUNDS CHECK PASSED; BAL {Balance:C}");
            return true;
        }

        public void ApplyMonthlyInterest()
        {
            if (Balance > 0)
            {
                decimal interest = Balance * _interestRate;
                Balance += interest;
                Log.Add($"MONTHLY INTEREST {interest:C}; BAL {Balance:C}");
            }
        }

        public void PrintStatement()
        {
            foreach (var line in Log)
            {
                Console.WriteLine(line);
            }
        }
    }
}
