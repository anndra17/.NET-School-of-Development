using MiniBankConsole.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBankConsole.Models
{
    public class CheckingAccount : BankAccount, IOverdraftPolicy, IStatement
    {
        // Properties
        public const decimal _overdraftLimit = -200;

        // Constructor
        public CheckingAccount(string owner, decimal openingBalance = 0) 
            : base(owner, openingBalance) { }


        // Methods
        public override bool Deposit(decimal amount)
        { 
            if (amount > 0)
            {
                Console.WriteLine($"Congratulations! You've just deposited {amount.ToString("C")}.");
                this.Balance += amount;
                return true;
            }
            Console.WriteLine("Error: You've entered an invalid deposit amount.");
            return false;
        }

        public override bool Withdraw(decimal amount, out string? error)
        {
            error = null;
            if (amount > 0)
            {
                if (CanWithdraw(amount))
                {
                    this.Balance -= amount;
                    Console.WriteLine($"Congratulations! You've just withdraw {amount.ToString("C")} successfully.");
                    return true;
                }
                else
                {
                    error = "You've exceeded your overdraft limit.";
                    return false;
                }
            }
            error = "You've entered an invalid withdrawal amount.";
            return false;
        }

        public override void ViewDetails()
        {
            Console.WriteLine($"Account ID: {Id}");
            Console.WriteLine($"Account Owner: {Owner}");
            Console.WriteLine($"Account Balance: {Balance:C}");
        }
        
        public decimal OverdraftLimit
        {
            get { return _overdraftLimit; }
        }

        public bool CanWithdraw(decimal amount)
        {
            decimal newBalance = this.Balance - amount;
            return newBalance >= _overdraftLimit;
        }

        public void PrintStatement()
        {
           
        }
    }
}
