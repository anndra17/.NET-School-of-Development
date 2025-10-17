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
        protected override bool TryValidateWithdraw(decimal amount, out string? error)
        {
            error = null;
            if (amount <=0)
            {
                error = "You've entered an invalid withdrawal amount.";
                return false;
            }

            // Check if the withdrawal would exceed the overdraft limit
            decimal newBalance = this.Balance - amount;
            if (newBalance < _overdraftLimit)
            {
                error = "You've exceeded your overdraft limit.";
                return false;
            }
            Console.WriteLine($"Congratulations! You've just withdraw {amount.ToString("C")} successfully.");
            return true;
        }

        
        public decimal OverdraftLimit
        {
            get { return _overdraftLimit; }
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
