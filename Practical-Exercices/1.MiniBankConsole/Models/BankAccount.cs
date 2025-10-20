using MiniBankConsole.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBankConsole.Models
{
    public abstract class BankAccount: ITransactable
    {

        // Properties
        private static int _counter = 0;
        public int Id { get; protected set; }
        public string Owner { get; protected set; }
        public decimal Balance { get; protected set; }
        protected List<string> Log { get; set; }

        // Constructor
        protected BankAccount(string owner, decimal openingBalance = 0)
        {
            Id = ++_counter;
            Owner = owner;
            Balance = openingBalance;
            Log = new List<string>();
        }

        // Methods
        public virtual void Deposit(decimal amount)
        {
            if (amount <= 0)
            { 
                Log.Add($"DEPOSIT FAILED {amount.ToString("C")}"); 
                return; 
            }

            Balance += amount;
            Log.Add($"DEPOSIT {amount.ToString("C")}; BAL {Balance.ToString("C")}");
        }

        public bool Withdraw(decimal amount, out string? error)
        {
            if (TryValidateWithdraw(amount, out error))
            {
                Balance -= amount;
                Log.Add($"WITHDRAW -{amount.ToString("C")}; BAL {Balance.ToString("C")}");
                return true;
            }
            return false;
        }

        protected abstract bool TryValidateWithdraw(decimal amount, out string? error);

    }
}
