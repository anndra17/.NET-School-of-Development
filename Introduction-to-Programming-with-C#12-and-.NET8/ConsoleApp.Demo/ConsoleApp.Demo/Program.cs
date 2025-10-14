using System.Globalization;

// Variable Declarations and Types
const int retirementAge = 65;


string? firstName = string.Empty;
string lastName = string.Empty;
int age; // default 0
DateOnly dob = new DateOnly(); 
decimal salary; // default 0.0m
char gender= char.MinValue; 
bool working = true; // by default false

// Prompt the user for imput
Console.Write("Please eneter your first name: ");
firstName = Console.ReadLine();

Console.Write("Please eneter your last name: ");
lastName = Console.ReadLine();

Console.Write("Please eneter your date of birth (dd mm yyyy): ");
dob = DateOnly.ParseExact(Console.ReadLine(), "dd mm yyyy", CultureInfo.InvariantCulture);
age = DateTime.Now.Year - dob.Year;

Console.Write("Please eneter your salary: ");
salary = decimal.Parse(Console.ReadLine());

Console.Write("Please eneter your gender (M or F): ");
gender = Convert.ToChar(Console.ReadLine());

Console.Write("Are you currently working? (true or false): ");
working = Convert.ToBoolean(Console.ReadLine());

// Process the data
int workingYearsRemaining = retirementAge - age;
var estimatedRetirementYear = DateTime.Now.AddYears(workingYearsRemaining);

// Output the results to the user
Console.WriteLine($"Full Name: {firstName} {lastName}");
Console.WriteLine($"Age: {age}");
Console.WriteLine($"Date of Birth: {dob}");
Console.WriteLine($"Salary: {salary.ToString("C")}");
Console.WriteLine($"Gender: {gender}");
Console.WriteLine($"Currently Working: {working}");
Console.WriteLine($"Estimated Retirement Year: {estimatedRetirementYear.Year}");
Console.WriteLine($"Years until retirement: {workingYearsRemaining}");