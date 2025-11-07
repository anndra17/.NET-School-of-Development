using ReadingList.CLI.Commands.Models;
using ReadingList.CLI.Commands.Routing.Abstractions;
using ReadingList.CLI.UI.Abstractions;

namespace ReadingList.CLI.Commands.Routing;

public sealed class HelpHandler : IHelpHandler
{
    private readonly IConsoleNotifier _consoleNotifier;
    private readonly IConsole _console;

    public HelpHandler(IConsoleNotifier consoleNotifier, IConsole console)
    {
        _consoleNotifier = consoleNotifier ?? throw new ArgumentNullException(nameof(consoleNotifier));
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    public Task HandleAsync(HelpCommand command, CancellationToken ct)
    {
        _consoleNotifier.Info("Reading List – available commands:");
        EmptyLine();

        Section("Import");
        Line("import <file1.csv> [file2.csv ...]");
        Description("Imports one or more CSV files in parallel.");
        EmptyLine();

        Section("Listing & Filters");
        Line("list all");
        Description("Lists all books.");
        EmptyLine();

        Line("filter finished | filter notfinished");
        Description("Shows only finished or unfinished books.");
        EmptyLine();

        Line("by author <text> [--ignore-case]");
        Description("Filters books where Author contains <text> (case-insensitive).");
        EmptyLine();

        Line("top rated <n>");
        Description("Shows top <n> books by rating (descending).");
        EmptyLine();

        Section("Updates");
        Line("mark finished <id>");
        Description("Marks a book as finished.");
        EmptyLine();

        Line("rate <id> <0-5>");
        Description("Sets a rating between 0 and 5.");
        EmptyLine();

        Section("Statistics");
        Line("stats");
        Description("Prints totals: books, finished, avg rating, pages by genre, top 3 authors.");
        EmptyLine();

        Section("Export");
        Line("export json <path> | export csv <path>");
        Description("Exports current collection; asks before overwriting existing files.");
        EmptyLine();

        Section("Help & Exit");
        Line("help");
        Description("Shows this help.");
        EmptyLine();

        Line("exit");
        Description("Exits the application.");
        EmptyLine();

        return Task.CompletedTask;
    }

    private void Section(string title)
    {
        _console.WriteLine(title);
        _console.WriteLine(new string('-', Math.Max(6, title.Length)));
    }

    private void Line(string text) => _console.WriteLine($"  {text}");
    
    private void Description(string text) => _console.WriteLine($"    - {text}");

    private void EmptyLine() => _console.WriteLine();
}
