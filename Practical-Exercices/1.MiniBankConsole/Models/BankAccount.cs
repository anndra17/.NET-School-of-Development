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
        public string AccountType 
        { 
            get
            {
                return this.GetType().Name;
            }
        }

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
            // Basic validation for deposit amount
            if (amount > 0)
            {
                // Update balance
                Balance += amount;

                // Log the transaction
                Console.WriteLine($"Congratulations! You've just deposited {amount.ToString("C")}.");
            }

            Console.WriteLine("Error: You've entered an invalid deposit amount.");
        }

        public bool Withdraw(decimal amount, out string? error)
        {
            if (TryValidateWithdraw(amount, out error))
            {
                Balance -= amount;
                return true;
            }
            return false;
        }

        protected abstract bool TryValidateWithdraw(decimal amount, out string? error);
        protected abstract void ViewDetails();

    }
}
