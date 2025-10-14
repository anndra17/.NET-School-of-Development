using System.Globalization; // for CultureInfo class

Console.WriteLine("********** - DateTime Manipulation - ****-******");

//// Empty DateTime object
//DateTime date = new DateTime();

//// Create a DateTime from date and time
//var dateOfBirth = new DateTime(2003, 6, 17);
//Console.WriteLine($"My Dob is: {dateOfBirth}");

//var exactDateAndTimeOfBirth = new DateTime(2003, 6, 17, 16, 45, 00, DateTimeKind.Local);
//Console.WriteLine($"My Dob is: {exactDateAndTimeOfBirth}");

//Console.WriteLine($"Day of Week: {dateOfBirth.DayOfWeek}");
//Console.WriteLine($"Day of the Year: {dateOfBirth.DayOfYear}");
//Console.WriteLine($"Time of Day: {exactDateAndTimeOfBirth.TimeOfDay}");
//Console.WriteLine($"Tick: {exactDateAndTimeOfBirth.Ticks}"); // how many ticks since exactDateAndTimeOfBirth, 10 milion tick = 1 sec
//Console.WriteLine($"Kind: {exactDateAndTimeOfBirth.Kind}"); // Local, Utc, Unspecified

//// Create a DateTime from current timestamp
DateTime now = DateTime.Now; // the time of my local machine
//Console.WriteLine($"Now: {now}");

//// Create a DateTime from a String
//Console.WriteLine("What is your Dob (yyyy/MM/dd): ");
//string dob = Console.ReadLine();

//var userDob = DateTime.Parse(dob);
//Console.WriteLine($"Your Dob is : {userDob}");

//// Change Format of DateTime
//Console.WriteLine($"Formatted Date: {userDob.ToString("dd/MM/yyyy")}");
//Console.WriteLine($"Formatted Date: {userDob.ToString("MMM, dd-yyyy")}");
//Console.WriteLine($"Formatted Date: {userDob.ToString("dd-MM-yyyy")}");
//Console.WriteLine($"Formatted Date: {userDob:dd.MM.yyyy}");

//// Add Additional Time
//Console.WriteLine("One hour from now is: " + now.AddHours(1));
//Console.WriteLine("One day from now is: " + now.AddDays(1));
//Console.WriteLine("One day ago from now is: " + now.AddDays(-1));


//// Ticks from DateTime
//Console.WriteLine($"Ticks from now: {now.Ticks}");

Console.WriteLine("*********** -  DateTime Offset manipulation - ***********");

//// UTC - Coordinated Universal Time
//var utcNow = DateTime.UtcNow;
//var localNow = DateTime.Now;
//Console.WriteLine($"UTC Time: {utcNow}");
//Console.WriteLine($"Local Time: {localNow}");

//// DateTimeOffset and TimeZone Info
//var tz = TimeZoneInfo.Local.GetUtcOffset(utcNow);
//Console.WriteLine("User Time Zone: " + tz);

//var dto = new DateTimeOffset(localNow, tz);
//Console.WriteLine("User Time Zone with UTC Offset: " + dto);

//var indiaTZ = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
//var indiaDateTime = TimeZoneInfo.ConvertTimeFromUtc(dto.UtcDateTime, indiaTZ);
//Console.WriteLine("India Time Zone: "+ indiaTZ);
//Console.WriteLine($"Action was completed in India at: {indiaDateTime}");

Console.WriteLine("********** - Date only and Time only manipulation - ***********");

// DateOnly
var dateOnly = new DateOnly(2003, 06, 17);
var nextDay = dateOnly.AddDays(1);
var previousDay = dateOnly.AddDays(-1);
var decadeLater = dateOnly.AddYears(10);
var lastMonth = dateOnly.AddMonths(-1);

Console.WriteLine($"The date: {dateOnly}");
Console.WriteLine($"The next day: {nextDay}");
Console.WriteLine($"The previous day: {previousDay}");
Console.WriteLine($"A decade later: {decadeLater}");
Console.WriteLine($"Last month: {lastMonth}");

var DateOnlyFromDateTime = DateOnly.FromDateTime(now);
Console.WriteLine($"DateOnly from DateTime: {DateOnlyFromDateTime}");

Console.WriteLine("What is your Dob (dd.MM.yyyy): ");
string dob = Console.ReadLine();

var theDateOnly = DateOnly.ParseExact(dob, "dd.MM.yyyy", CultureInfo.InvariantCulture);
Console.WriteLine($"Your Dob is : {theDateOnly}");

// TimeOnly
var timeNow = TimeOnly.FromDateTime(now);
Console.WriteLine($"It is now {timeNow}");
Console.WriteLine($"It is now {timeNow:hh:mm tt}");

// Date Comparisons
var date1 = new DateTime(1985, 11, 23);
var date2 = new DateTime(1965, 1, 20);

Console.WriteLine($"Is '{nameof(date1)}' egual? {date1 == date2}");
Console.WriteLine($"Is '{nameof(date1)}' egual? {date1.Equals(date2)}");
Console.WriteLine($"Is '{nameof(date1)}' after? {date1 > date2}");
Console.WriteLine($"Is '{nameof(date1)}' before? {date1 < date2}");
