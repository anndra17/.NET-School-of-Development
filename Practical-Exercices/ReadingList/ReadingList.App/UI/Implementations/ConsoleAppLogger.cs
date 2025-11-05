using ReadingList.App.UI.Abstractions;

namespace ReadingList.App.UI.Implementations;

public sealed class ConsoleAppLogger
{
    private readonly IConsole _console;

    public ConsoleAppLogger(IConsole console)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }
    public void Info(string message)
    {
        WithColor(ConsoleColor.Gray, () => _console.WriteLine($"[i] {message}"));
    }

    public void Success(string message)
    {
        WithColor(ConsoleColor.Green, () => _console.WriteLine($"[*] {message}"));
    }

    public void Warn(string message)
    {
        WithColor(ConsoleColor.Yellow, () => _console.WriteLine($"[!] {message}"));
    }

    public void Error(string message)
    {
        WithColor(ConsoleColor.Red, () => _console.WriteLine($"[x] {message}"));
    }

    private static void WithColor(ConsoleColor color, Action writeAction)
    {
        var original = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = color;
            writeAction();
        }
        finally
        {
            Console.ForegroundColor = original;
        }
    }
}
