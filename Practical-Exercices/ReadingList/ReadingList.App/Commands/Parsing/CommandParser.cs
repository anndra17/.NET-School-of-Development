using ReadingList.App.Commands.Models;
using ReadingList.Infrastructure.Enums;
using System.Globalization;

namespace ReadingList.App.Commands.Parsing;

public sealed class CommandParser
{
    public ParseResult Parse(string? line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return ParseResult.Fail("Empty command. Type 'help'.");

        var tokens = Tokenize(line);
        if (tokens.Count == 0)
            return ParseResult.Fail("Empty command. Type 'help'.");

        var verb = tokens[0].ToLowerInvariant();
        var args = tokens.Skip(1).ToList();

        try
        {
            return verb switch
            {
                "help" => ParseResult.Ok(new HelpCommand()),
                "exit" => ParseResult.Ok(new ExitCommand()),

                "import" => ParseImport(args),
                "list" => ParseList(args),
                "filter" => ParseFilter(args),
                "by" => ParseBy(args),
                "top" => ParseTop(args),
                "stats" => ParseResult.Ok(new StatsCommand()),
                "mark" => ParseMark(args),
                "rate" => ParseRate(args),
                "export" => ParseExport(args),

                _ => ParseResult.Fail("Unknown command. Try 'help'.")
            };
        }
        catch (FormatException fe)
        {
            return ParseResult.Fail(fe.Message);
        }
    }

    private static ParseResult ParseImport(IReadOnlyList<string> args)
    {
        if (args.Count == 0)
            return ParseResult.Fail("Usage: import <file1.csv> [file2.csv ...]");

        return ParseResult.Ok(new ImportCommand(args.ToList()));
    }

    private static ParseResult ParseList(IReadOnlyList<string> args)
    {
        // list all
        if (args.Count == 0 || (args.Count == 1 && args[0].Equals("all", StringComparison.OrdinalIgnoreCase)))
            return ParseResult.Ok(new ListAllCommand());

        return ParseResult.Fail("Usage: list all");
    }

    private static ParseResult ParseFilter(IReadOnlyList<string> args)
    {
        // filter finished  | filter notfinished
        if (args.Count == 1 && args[0].Equals("finished", StringComparison.OrdinalIgnoreCase))
            return ParseResult.Ok(new FilterFinishedCommand(true));

        if (args.Count == 1 && (args[0].Equals("notfinished", StringComparison.OrdinalIgnoreCase)
                             || args[0].Equals("unfinished", StringComparison.OrdinalIgnoreCase)))
            return ParseResult.Ok(new FilterFinishedCommand(false));

        return ParseResult.Fail("Usage: filter finished | filter notfinished");
    }

    private static ParseResult ParseBy(IReadOnlyList<string> args)
    {
        // by author <text> [--ignore-case]
        if (args.Count >= 2 && args[0].Equals("author", StringComparison.OrdinalIgnoreCase))
        {
            var text = args[1];
            var ignoreCase = args.Count >= 3 && args[2].Equals("--ignore-case", StringComparison.OrdinalIgnoreCase);
            return ParseResult.Ok(new ByAuthorCommand(text, ignoreCase));
        }

        return ParseResult.Fail("Usage: by author <text> [--ignore-case]");
    }

    private static ParseResult ParseTop(IReadOnlyList<string> args)
    {
        // top rated <n>
        if (args.Count == 2 && args[0].Equals("rated", StringComparison.OrdinalIgnoreCase))
        {
            if (!int.TryParse(args[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) || n <= 0)
                throw new FormatException("The number after 'top rated' must be a positive integer.");

            return ParseResult.Ok(new TopRatedCommand(n));
        }

        return ParseResult.Fail("Usage: top rated <n>");
    }

    private static ParseResult ParseMark(IReadOnlyList<string> args)
    {
        // mark finished <id>
        if (args.Count == 2 && args[0].Equals("finished", StringComparison.OrdinalIgnoreCase))
        {
            if (!int.TryParse(args[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
                throw new FormatException("Book id must be an integer.");

            return ParseResult.Ok(new MarkFinishedCommand(id));
        }

        return ParseResult.Fail("Usage: mark finished <id>");
    }

    private static ParseResult ParseRate(IReadOnlyList<string> args)
    {
        // rate <id> <0-5>
        if (args.Count == 2)
        {
            if (!int.TryParse(args[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
                throw new FormatException("Book id must be an integer.");

            if (!double.TryParse(args[1].Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out var rating))
                throw new FormatException("Rating must be a number between 0 and 5.");

            return ParseResult.Ok(new RateCommand(id, rating));
        }

        return ParseResult.Fail("Usage: rate <id> <0-5>");
    }

    private static ParseResult ParseExport(IReadOnlyList<string> args)
    {
        // export json <path> | export csv <path>
        if (args.Count == 2)
        {
            var fmt = args[0].ToLowerInvariant() switch
            {
                "json" => ExportFormat.Json,
                "csv" => ExportFormat.Csv,
                _ => throw new FormatException("Export format must be 'json' or 'csv'.")
            };
            var path = args[1];
            return ParseResult.Ok(new ExportCommand(fmt, path));
        }

        return ParseResult.Fail("Usage: export json <path> | export csv <path>");
    }

    private static List<string> Tokenize(string input)
    {
        var tokens = new List<string>();
        var sb = new System.Text.StringBuilder();
        bool inQuotes = false;

        foreach (var ch in input)
        {
            if (ch == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (char.IsWhiteSpace(ch) && !inQuotes)
            {
                if (sb.Length > 0) { tokens.Add(sb.ToString()); sb.Clear(); }
            }
            else
            {
                sb.Append(ch);
            }
        }

        if (sb.Length > 0) tokens.Add(sb.ToString());

        return tokens;
    } 
}
