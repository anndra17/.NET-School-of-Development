// See https://aka.ms/new-console-template for more information

// Welcome Message
Console.WriteLine("********** - Welcome to the sample calculator! - **********");

// Variable Declarations
int choice = 0;
int num1, num2 = 0;

// Show calculator options / Show menu
while(choice != -1)
{
    Console.WriteLine("Please select an operation (-1 to exit program)");
    Console.WriteLine("1 - Addition");
    Console.WriteLine("2 - Subtraction");
    Console.WriteLine("3 - Multiplication");
    Console.WriteLine("4 - Division");
    Console.WriteLine("5. Fibonacci sequence");

    try
    {
        choice = Convert.ToInt32(Console.ReadLine());
    }
    catch(Exception)
    {
        Console.WriteLine("Invalid input, press any key to try again. ");
        Console.ReadLine();
        Console.Clear();
        continue;
    }

    // Prompt for user input
    try
    {
        Console.WriteLine("Please enter the first number: ");
        num1 = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Please enter the second number: ");
        num2 = Convert.ToInt32(Console.ReadLine());
    }
    catch(Exception)
    {
        Console.WriteLine("Invalid input, press any key to try again. ");
        Console.ReadLine();
        Console.Clear();
        continue;
    }

    // Decide which operation is needed based on selected option
    int answer = 0;
    switch (choice)
    {
        case 1:
            // Addition
            answer = num1 + num2;
            break;
        case 2:
            // Subtraction
            answer = num1 - num2;
            break;
        case 3:
            // Multiplication
            answer = num1 * num2;
            break;
        case 4:
            // Division
            try
            {
                answer = num1 / num2;
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("Cannot divide by zero!");
                Console.WriteLine("Press Enter to continue");
                Console.ReadLine();
                Console.Clear();
                continue;
            }
            break;
        case 5:
            // Fibonacci 
            for (int i = num1; i <= num2; i++)
                answer += i;
            break;
        default:
            Console.WriteLine("Invalid choice");
            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();
            Console.Clear();
            continue;
    }

    // Print output
    Console.WriteLine($"The answer is {answer}");
    Console.WriteLine("Press Enter to continue");
    Console.ReadLine();
    Console.Clear();
}




