using ReadingList.CLI.UI.Abstractions;

namespace ReadingList.CLI.Adapters;

public sealed class SystemConsoleAdapter: IConsole
{
    public void Write(string text) => Console.Write(text);

    public void WriteLine(string text) => Console.WriteLine(text);

    public void WriteLine() => Console.WriteLine();

    public Task<string?> ReadLineAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Console.ReadLine(), cancellationToken);
    }

    public async Task<bool> PromptYesNoAsync(
            string question,
            bool defaultYes = false,
            CancellationToken cancellationToken = default)
    {
        var hint = defaultYes ? "Y/n" : "y/N";
        while (true)
        {
            Write($"{question} ({hint}): ");
            var input = await ReadLineAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(input))
                return defaultYes;

            input = input.Trim();
            if (input.Equals("y", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("yes", StringComparison.OrdinalIgnoreCase))
                return true;

            if (input.Equals("n", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("no", StringComparison.OrdinalIgnoreCase))
                return false;

            WriteLine("Please type 'y' or 'n'.");
        }
    }
}
