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
        private const decimal _overdraftLimit = -200;
        public decimal OverdraftLimit
        {
            get { return _overdraftLimit; }
        }

        public CheckingAccount(string owner, decimal openingBalance = 0) 
            : base(owner, openingBalance) { }

        protected override bool TryValidateWithdraw(decimal amount, out string? error)
        {
            error = null;
            if (amount <=0)
            {
                error = "You've entered an invalid withdrawal amount.";
                return false;
            }

            decimal newBalance = this.Balance - amount;
            if (newBalance < _overdraftLimit)
            {
                error = "You've exceeded your overdraft limit.";
                return false;
            }

            Log.Add($"OVERDRAFT CHECK PASSED; BALANCE {newBalance.ToString("C")}");
            return true;
        }

        public void PrintStatement()
        {
            Console.WriteLine("CHECKING ACCOUNT STATEMENT");
            foreach (var line in Log)
            {
                Console.WriteLine(line);
            }
        }
    }
}
