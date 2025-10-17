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
        // Properties
        private const decimal _interestRate = 0.01m;

        // Constructor
        public LoanAccount(string owner, decimal initialLoan)
            : base(owner, openingBalance: -Math.Abs(initialLoan)) {}

        // Methods
        public override void Deposit(decimal amount)
        {
            if (amount <= 0 || Balance == 0) { /* Log + return */ return; }
            var owed = -Balance;
            var pay = Math.Min(amount, owed);
            Balance += pay;
            Log.Add($"LOAN REPAY {pay:C}; BAL {Balance:C}");
        }
        //public void Deposit(decimal amount) // Repay loan
        //{
        //    // should i use try catch?
        //    if (amount <= 0)
        //    {
        //        Console.WriteLine("Error: You've entered an invalid deposit amount.");
        //    }
        //    if (Balance == 0)
        //    {
        //        Console.WriteLine("Error: Your loan is already fully repaid.");
        //    }
        //    if (amount > Balance)
        //    {
        //        Console.WriteLine($"Error: The amount exceeds your outstanding loan balance of {Balance.ToString("C")}.");
        //    }
        //    Balance += amount;
        //    Console.WriteLine($"Congratulations! You've just repaid {amount.ToString("C")} from your loan.");
        //}

        protected override bool TryValidateWithdraw(decimal amount, out string? error)
        {
            error = null;
            if (amount <= 0)
            {
                error = "You've entered an invalid withdrawal amount.";
                return false;
            }
            return true;
        }

        
        public void ApplyMonthlyInterest()
        {
            if (Balance < 0)
            {
                var interest = -Math.Abs(Balance) * _interestRate;
                Balance -= interest;
                Log.Add($"INTEREST {interest.ToString("C")}; BAL {Balance.ToString("C")}");

                // DE ADAUGAT IN UI
                //Console.WriteLine($"Applied interest: {interest.ToString("C")}. New loan balance: {Balance.ToString("C")}");
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
