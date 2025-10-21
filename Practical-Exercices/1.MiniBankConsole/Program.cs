using System;
using MiniBankConsole.Services;
using MiniBankConsole.Models.Enums;

class Program
{
    static void Main()
    {
        var registry = new AccountRegistry();
        int choice = 0;


        while (choice != 7)
        {
            try 
            {
                PrintMenu();

                if (choice == 7) 
                    break;

                Console.Write("Select: ");
                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid choice.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        var accounts = registry.List();
                        if (!accounts.Any())
                        {
                            Console.WriteLine("No accounts found.");
                            break;
                        }
                        else
                        {
                            foreach (var a in registry.List())
                                Console.WriteLine(
                                    $"#{a.Id} {a.Owner} {a.AccountType} BALANCE {(a.Balance < 0 ? "-" : "")}{Math.Abs(a.Balance).ToString("C")}"
                                );
                        }
                        break;

                    case 2:
                        Console.Write("Type 1/2/3 (1 - Checking, 2 - Savings, 3 - Loan): ");
                        if (!int.TryParse(Console.ReadLine(), out var t) || t < 1 || t > 3) 
                        { 
                            Console.WriteLine("Invalid type."); 
                            break; 
                        }

                        Console.Write("Owner: "); 
                        var owner = Console.ReadLine();

                        Console.Write("Opening amount: ");
                        if (!decimal.TryParse(Console.ReadLine(), out var open)) 
                        { 
                            Console.WriteLine("Invalid amount."); 
                            break; 
                        }

                        try
                        {
                            var acc = registry.Create((AccountType)t, owner!, open);
                            Console.WriteLine(
                                $"You've succesfully created an {acc.AccountType} account. Details: id #{acc.Id}  owner: {acc.Owner} Balance: {(acc.Balance < 0 ? "-" : "")}{Math.Abs(acc.Balance).ToString("C")}");
                        }
                        catch (Exception ex)
                        { Console.WriteLine(ex.Message); }
                        break;

                    case 3:
                        Console.Write("Account id: ");
                        if (!int.TryParse(Console.ReadLine(), out var idD)) 
                        { 
                            Console.WriteLine("Invalid id."); 
                            break; 
                        }
                        Console.Write("Amount: ");
                        if (!decimal.TryParse(Console.ReadLine(), out var amtD))
                        { 
                            Console.WriteLine("Invalid amount."); 
                            break; 
                        }
                        if (!registry.Deposit(idD, amtD, out var errD, out var accepted))
                            Console.WriteLine(errD);
                        else if (accepted < amtD)
                            Console.WriteLine($"You've deposited {accepted.ToString("C")} into account #{idD}. The rest was ignored because the loan was fully repaid.");
                        else
                            Console.WriteLine($"You've deposited {amtD.ToString("C")} into account #{idD}.");
                        break;

                    case 4:
                        Console.Write("Account id: ");
                        if (!int.TryParse(Console.ReadLine(), out var idW)) { Console.WriteLine("Invalid id."); break; }
                        Console.Write("Amount: ");
                        if (!decimal.TryParse(Console.ReadLine(), out var amtW)) { Console.WriteLine("Invalid amount."); break; }
                        if (!registry.Withdraw(idW, amtW, out var errW)) Console.WriteLine(errW);
                        else Console.WriteLine($"You've withdrawn {amtW.ToString("C")} from account #{idW}.");
                        break;

                    case 5:
                        Console.Write("Account id: ");
                        if (!int.TryParse(Console.ReadLine(), out var idS)) { Console.WriteLine("Invalid id."); break; }
                        if (!registry.PrintStatements(idS, out var errS)) Console.WriteLine(errS);
                        break;

                    case 6:
                        var n = registry.RunMonthEnd();
                        Console.WriteLine($"Month-end applied to {n} interest-bearing accounts.");
                        break;
                    case 7:
                        Console.WriteLine("Exiting...");
                        break;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

                Console.WriteLine();
            } 
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            } 
            finally
            {
                Console.WriteLine("Press any key to continue. ");
                Console.ReadLine();
                Console.Clear();
            }

           
        }
    }

    private static void PrintMenu()
    {
        Console.Clear();
        Console.WriteLine("=== MINIBANK ===");
        Console.WriteLine("1. List accounts");
        Console.WriteLine("2. Create account");
        Console.WriteLine("3. Deposit");
        Console.WriteLine("4. Withdraw");
        Console.WriteLine("5. View statement");
        Console.WriteLine("6. Run month-end");
        Console.WriteLine("7. Exit");
    }
}
