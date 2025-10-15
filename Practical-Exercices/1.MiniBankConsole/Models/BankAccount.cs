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
        // Aici as face o lista de tranzactii, eventual o clasa Operations
        // protected List<Operations> Log; { get; protected set; }


        // Constructor
        protected BankAccount(string owner, decimal openingBalance = 0)
        {
            Id = ++_counter;
            Owner = owner;
            Balance = openingBalance;
        }

        // Methods
        public abstract void ViewDetails();
        public virtual bool Withdraw(decimal amount, out string? error)
        {
            error = null;
            if (amount <= 0 || amount > Balance)
            {
                error = "Invalid amount";
                return false;
            }
            Balance -= amount;
            return true;
        }

        public virtual bool Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                return false;
            }
            Balance += amount;
            return true;
        }
    }
}
