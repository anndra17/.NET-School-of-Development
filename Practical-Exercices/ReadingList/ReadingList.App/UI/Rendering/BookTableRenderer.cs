using ReadingList.CLI.UI.Abstractions;
using ReadingList.Domain.Entities;
using System.Globalization;

namespace ReadingList.CLI.UI.Rendering;

public sealed class BookTableRenderer
{
    private readonly IConsole _console;

    public BookTableRenderer(IConsole console)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    public void Render(IReadOnlyList<Book> books)
    {
        const int idW = 4, titleW = 32, authorW = 22, yearW = 6, finW = 9, ratingW = 7;

        WriteSeparator(idW, titleW, authorW, yearW, finW, ratingW);
        _console.WriteLine(
            Pad("Id", idW) + " " +
            Pad("Title", titleW) + " " +
            Pad("Author", authorW) + " " +
            Pad("Year", yearW) + " " +
            Pad("Finished", finW) + " " +
            Pad("Rating", ratingW));
        WriteSeparator(idW, titleW, authorW, yearW, finW, ratingW);

        foreach (var b in books)
        {
            var id = b.Id.ToString(CultureInfo.InvariantCulture);
            var title = b.Title ?? "";
            var author = b.Author ?? "";
            var year = b.Year.ToString(CultureInfo.InvariantCulture);
            var finished = b.Finished ? "yes" : "no";
            var rating = b.Rating.ToString("0.##", CultureInfo.InvariantCulture);

            _console.WriteLine(
                Pad(id, idW) + " " +
                Pad(title, titleW) + " " +
                Pad(author, authorW) + " " +
                Pad(year, yearW) + " " +
                Pad(finished, finW) + " " +
                Pad(rating, ratingW));
        }

        WriteSeparator(idW, titleW, authorW, yearW, finW, ratingW);
        _console.WriteLine(); 
    }

    private void WriteSeparator(params int[] widths)
    {
        var total = widths.Sum() + (widths.Length - 1); 
        _console.WriteLine(new string('-', total));
    }

    private static string Pad(string text, int width)
    {
        if (text.Length == width) return text;
        if (text.Length < width) return text.PadRight(width);

        return text.Substring(0, Math.Max(0, width - 1)) + "…";
    }
}
