using ReadingList.Application.Dtos;
using ReadingList.CLI.UI.Abstractions;
using System.Globalization;

namespace ReadingList.CLI.UI.Rendering;

public sealed class BookStatsRenderer
{
    private readonly IConsole _console;
    private readonly IConsoleNotifier _consoleNotifier;

    public BookStatsRenderer(IConsole console, IConsoleNotifier consoleNotifier)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _consoleNotifier = consoleNotifier ?? throw new ArgumentNullException(nameof(consoleNotifier));
    }

    public void Render(BookStatsDto stats)
    {
        stats ??= new BookStatsDto(
            totalBooks: 0,
            finishedBooks: 0,
            averageRating: 0,
            pagesByGenre: new Dictionary<string, int>(),
            topAuthors: new List<(string Author, int Count)>()
            );

        _consoleNotifier.Info("Statistics");
        _console.WriteLine();

        _console.WriteLine($"  Total books    : {stats.TotalBooks.ToString(CultureInfo.InvariantCulture)}");
        _console.WriteLine($"  Finished books : {stats.FinishedBooks.ToString(CultureInfo.InvariantCulture)}");
        _console.WriteLine($"  Avg. rating    : {stats.AverageRating.ToString("0.00", CultureInfo.InvariantCulture)}");
        _console.WriteLine();

        _consoleNotifier.Info("Pages by genre:");
        if (stats.PagesByGenre is null || stats.PagesByGenre.Count == 0)
        {
            _console.WriteLine("  (none)");
        }
        else
        {
            const int genreW = 18;
            _console.WriteLine(Pad("  Genre", genreW) + " Pages");
            _console.WriteLine(new string('-', genreW + 1 + 10));
            foreach (var kv in stats.PagesByGenre.OrderBy(k => k.Key))
            {
                var pagesTxt = kv.Value.ToString(CultureInfo.InvariantCulture);
                _console.WriteLine(Pad("  " + (kv.Key ?? ""), genreW) + " " + pagesTxt);
            }
        }
        _console.WriteLine();

        _consoleNotifier.Info("Top 3 authors (by book count):");
        if (stats.TopAuthors is null || stats.TopAuthors.Count == 0)
        {
            _console.WriteLine("  (none)");
        }
        else
        {
            int rank = 1;
            foreach (var item in stats.TopAuthors)
            {
                _console.WriteLine($"  {rank}. {item.Author} — {item.Count.ToString(CultureInfo.InvariantCulture)}");
                rank++;
            }
        }
        _console.WriteLine();
    }

    private static string Pad(string text, int width)
    {
        if (text.Length == width) return text;
        if (text.Length < width) return text.PadRight(width);
        return text[..Math.Max(0, width - 1)] + "…";
    }
}
