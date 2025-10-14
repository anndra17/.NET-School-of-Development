// Initialize with a regular string literal.
string s1 = "This is a literal string";  // alias for System.String (recommended)
String s2 = "This is a literal string"; // Same as above, using System.String

// Declare without initializing. (posible null exception)
string s3;

// Initialize to null. (possible null exception)
string? s4 = null; // nullable string

// Initialize as an empty string
string s5 = string.Empty; // preferred way to represent an empty string
string s6 = ""; // also valid, but I can insert a space by mistake

// Escape sequences and characters
// She said, "Hello, World!"
string sentence = "She said, \"Hello, World!\" \r\n This is the next line";

// Verbatim string literal (ignores escape sequences)
string oldPath = "C\\program files\\programfolder";
string NEWPath = @"C\program files\programfolder";

// Use a const string to prevent modification to a string
const string path = "C\\program files\\programfolder";

// path = "new value"; ILLEGAL OPERATION AGAINST A CONSTANT
// s1 = "new string";

// Raw string literals
string rawLiteral = """She said, "Hello, World!" """;
string rawLiteral1 = """
    She said, "Hello, Wold!"
    This is the next line
    And another one
    Path = "C\program files\programfolder";
    """; // To have multiple rows i have to put """ on separate rows

// Review concatenation and interpolation
s1 = s1 + s2;
s1 += s2;
string newString = $"{s1} {21} Some random literal text";
string newString1 = s1 + $"{s1} {21} Some random literal text";
string newString2 = String.Format("Literal string {0} {1}", s1, s2); // start counting from 0

/* String manipulation methods */

// Null or empty checks

// - find the length of a string
Console.WriteLine($"{nameof(s1)} has the length of {s1.Length}");
Console.WriteLine($"{nameof(s2)} has the length of {s2.Length}");
// Console.WriteLine($"{nameof(s4)} has the length of {s4.Length}"); // Will throw a null exception


if (!string.IsNullOrEmpty(s4))
{
    Console.WriteLine($"{nameof(s4)} has the length of {s4.Length}");
}

if(!string.IsNullOrEmpty(s5))
{
    Console.WriteLine($"{nameof(s5)} has the length of {s5.Length}");
}


// Substrings
string subString = s1.Substring(5); // from position 5 to the end, start counting from 0
Console.WriteLine($"{nameof(subString)} : {subString}");
subString = s1.Substring(5, 10); // startIndex=5, length=10 including startIndex
Console.WriteLine($"{nameof(subString)} : {subString}");

// Splitting strings
var stringToSplit = s2.Split(' ');
for (int i = 0;i < stringToSplit.Length; i++)
{
    Console.WriteLine($"Word {i}: {stringToSplit[i]}");
}

// Replace
string replacements = s1.Replace('s', 'V');
Console.WriteLine($"{nameof(replacements)} : {replacements}");

string replacements1 = s1.Replace("string", "chicken");
Console.WriteLine($"{nameof(replacements1)} : {replacements1}");

// Convert to string
string salary = 459797987.02.ToString();
int value = 765675869;
string stringValue = value.ToString();
bool chosen = true;
chosen.ToString();

// Changing Formatting
Console.WriteLine($"{nameof(salary)} : {salary:C}"); // :C - currency format
Console.WriteLine(nameof(salary) + ":" + value.ToString("C")); // "C" - currency format

