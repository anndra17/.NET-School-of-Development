// Variable Declarations and Types

string? firstName = string.Empty;
string? lastName = string.Empty;
int age; // default 0
int retirementAge = 65;
decimal salary; // default 0.0m
char gender; // default char.MinValue;
bool working; // by default false

// Prompt the user for imput
Console.Write("Please eneter your first name: ");
firstName = Console.ReadLine();

Console.Write("Please eneter your last name: ");
lastName = Console.ReadLine();

Console.Write("Please eneter your age: ");
age = int.Parse(Console.ReadLine());

Console.Write("Please eneter your salary: ");
salary = decimal.Parse(Console.ReadLine()!);

Console.Write("Please eneter your gender (M or F): ");
gender = Convert.ToChar(Console.ReadLine());

Console.Write("Are you currently working? (true or false): ");
working = Convert.ToBoolean(Console.ReadLine());

// Process the data
int workingYearsRemaining = retirementAge - age;

// Output the results to the user
Console.WriteLine($"Hello {firstName} {lastName}, you have {workingYearsRemaining} years left until you reach the age of {retirementAge}.");
Console.WriteLine($"Your salary is {salary:C}, \0 You are employed: {working.ToString()}.");
Console.WriteLine($"Your gender is {gender}");