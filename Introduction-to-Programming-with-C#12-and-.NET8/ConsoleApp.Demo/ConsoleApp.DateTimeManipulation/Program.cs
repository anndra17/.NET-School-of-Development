Console.WriteLine("********** - DateTime Manipulation - ****-******");

// Empty DateTime object
DateTime date = new DateTime();

// Create a DateTime from date and time
var dateOfBirth = new DateTime(2003, 6, 17);
Console.WriteLine($"My Dob is: {dateOfBirth}");

var exactDateAndTimeOfBirth = new DateTime(2003, 6, 17, 16, 45, 00, DateTimeKind.Local);
Console.WriteLine($"My Dob is: {exactDateAndTimeOfBirth}");

Console.WriteLine($"Day of Week: {dateOfBirth.DayOfWeek}");
Console.WriteLine($"Day of the Year: {dateOfBirth.DayOfYear}");
Console.WriteLine($"Time of Day: {exactDateAndTimeOfBirth.TimeOfDay}");  
Console.WriteLine($"Tick: {exactDateAndTimeOfBirth.Ticks}"); // how many ticks since exactDateAndTimeOfBirth, 10 milion tick = 1 sec
Console.WriteLine($"Kind: {exactDateAndTimeOfBirth.Kind}"); // Local, Utc, Unspecified

// Create a DateTime from current timestamp
DateTime now = DateTime.Now; // the time of my local machine
Console.WriteLine($"Now: {now}");

// Create a DateTime from a String
Console.WriteLine("What is your Dob (yyyy/MM/dd): ");
string dob = Console.ReadLine();

var userDob = DateTime.Parse(dob);
Console.WriteLine($"Your Dob is : {userDob}");

// Change Format of DateTime
Console.WriteLine($"Formatted Date: {userDob.ToString("dd/MM/yyyy")}");
Console.WriteLine($"Formatted Date: {userDob.ToString("MMM, dd-yyyy")}");
Console.WriteLine($"Formatted Date: {userDob.ToString("dd-MM-yyyy")}");
Console.WriteLine($"Formatted Date: {userDob:dd.MM.yyyy}");

// Add Additional Time
Console.WriteLine("One hour from now is: " + now.AddHours(1));
Console.WriteLine("One day from now is: " + now.AddDays(1));
Console.WriteLine("One day ago from now is: " + now.AddDays(-1));


// Ticks from DateTime
Console.WriteLine($"Ticks from now: {now.Ticks}");

Console.WriteLine("*********** -  DateTime Offset manipulation - ***********");
// UTC

// DateTimeOffset and TimeZone Info

Console.WriteLine("********** - Date only and Time only manipulation - ***********");

// DateOnly

// TimeOnly