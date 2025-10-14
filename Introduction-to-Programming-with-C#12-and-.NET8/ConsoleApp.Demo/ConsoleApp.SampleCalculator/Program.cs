// See https://aka.ms/new-console-template for more information

// Variable Declarations
using System.Net.Http.Headers;
using System.Numerics;

int choice = 0;
int num1, num2 = 0;

// Show calculator options / Show menu
while(choice != -1)
{
    try
    {
        // Welcome message
        PrintMenu();

        choice = Convert.ToInt32(Console.ReadLine());

        if (choice == -1)
            break;
      
        // Prompt for user input
        Console.WriteLine("Please enter the first number: ");
        num1 = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Please enter the second number: ");
        num2 = Convert.ToInt32(Console.ReadLine());
        

        // Decide which operation is needed based on selected option
        int answer = 0;
        switch (choice)
        {
            case 1:
                answer = AddNumbers(num1, num2);
                break;
            case 2:
                answer = SubstractNumbers(num1, num2);
                break;
            case 3:
                answer = Product(num1, num2);
                break;
            case 4:
                    answer = Division(num1, num2);
                break;
            case 5:
                answer = Fibbonaci(num1, num2);
                
                break;
            default:
                throw new Exception("Invalid Menu Item Selected");
        }

        // Print output
        Console.WriteLine($"The answer is {answer}");

    }
    catch (DivideByZeroException)
    {
        Console.WriteLine("Cannot divide by zero!");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
    finally
    {
        Console.WriteLine("Press any key to continue. ");
        Console.ReadLine();
        Console.Clear();
    }
}
// Methods definitions


int Fibbonaci(int num1, int num2)
{
    int answer = 0;
    for (int i = num1; i <= num2; i++)
        answer += i;
    return answer;
}

int Division(int num1, int num2)
{
    return num1 / num2;
}

int Product(int num1, int num2)
{
    return num1 * num2;
}

int SubstractNumbers(int num1, int num2)
{
    return num1 - num2;
}

int AddNumbers(int num1, int num2)
{
    return num1 + num2;
}

void PrintMenu()
{
    Console.Clear();
    Console.WriteLine("********** - Welcome to the sample calculator! - **********");
    Console.WriteLine("Please select an operation (-1 to exit program)");
    Console.WriteLine("1 - Addition");
    Console.WriteLine("2 - Subtraction");
    Console.WriteLine("3 - Multiplication");
    Console.WriteLine("4 - Division");
    Console.WriteLine("5. Fibonacci sequence");
}