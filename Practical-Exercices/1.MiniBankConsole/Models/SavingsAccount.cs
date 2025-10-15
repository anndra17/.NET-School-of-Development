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
        public override bool Deposit(decimal amount)
        {
            if (amount > 0)
            {
                Balance += amount;
                Console.WriteLine($"Congratulations! You've just deposited {amount.ToString("C")} into your savings account.");
                return true;
            }
            Console.WriteLine("Error: You've entered an invalid deposit amount.");
            return false;
        }

        public override bool Withdraw(decimal amount, out string? error)
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
            Balance -= amount;
            Console.WriteLine($"Congratulations! You've just withdrawn {amount.ToString("C")} from your savings account.");
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
            if (Balance > 0)
            {
                decimal interest = Balance * _interestRate;
                Balance += interest;
                Console.WriteLine($"Applied interest: {interest:C}. New balance: {Balance:C}");
            }
        }

        public void PrintStatement()
        {

        }
    }
}
