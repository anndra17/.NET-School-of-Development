using MiniBankConsole.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public override bool Deposit(decimal amount) // Repay loan
        { 
            if (amount <= 0)
            {
                Console.WriteLine("Error: You've entered an invalid deposit amount.");
                return false;
            }
            if (Balance == 0)
            {
                Console.WriteLine("Error: Your loan is already fully repaid.");
                return false;
            }
            if (amount > Balance)
            {
                Console.WriteLine($"Error: The amount exceeds your outstanding loan balance of {Balance.ToString("C")}.");
                return false;
            }
            Balance += amount;
            Console.WriteLine($"Congratulations! You've just repaid {amount.ToString("C")} from your loan.");
            return true;
        }

        public override bool Withdraw(decimal amount, out string? error) // borrow more
        {
            error = null;
            if (amount <= 0)
            {
                error = "Error: You've entered an invalid withdrawal amount.";
                return false;
            }
            Balance -= amount;
            Console.WriteLine($"Congratulations! You've just borrowed {amount.ToString("C")} successfully. Your current loan is {Balance.ToString("C")}.");
            return true;
        }

        public override void ViewDetails()
        {
            Console.WriteLine($"Account ID: {Id}");
            Console.WriteLine($"Account Owner: {Owner}");
            Console.WriteLine($"Account Balance: {Balance:C}");
        }
        
        public void ApplyMonthlyInterest()
        {
            if (Balance < 0)
            {
                decimal interest = Balance * _interestRate;
                Balance += interest; 
                Console.WriteLine($"Applied interest: {interest:C}. New loan balance: {Balance:C}");
            }
        }
        public void PrintStatement()
        {

        }
    }
}
