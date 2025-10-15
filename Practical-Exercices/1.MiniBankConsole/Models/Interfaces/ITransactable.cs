using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBankConsole.Models.Interfaces
{
    public interface ITransactable
    {
        bool Deposit(decimal amount);
        bool Withdraw(decimal amount, out string? error);
    }
}
