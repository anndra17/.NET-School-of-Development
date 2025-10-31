using ReadingList.Domain.Entities;

namespace ReadingList.Domain.Extensions;

public static class EnumerableBookExtensions
{
    public static double AverageRating(this IEnumerable<Book> source, bool ignoreZero = false)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        double sum = 0.0;
        int count = 0;

        foreach (var book in source)
        {
            var r = book?.Rating ?? 0.0;
            if (ignoreZero && r <= 0.0) continue;

            sum += r;
            count++;
        }

        return count == 0 ? 0.0 : sum / count;
    }

    public static IEnumerable<Book> TopRated(this IEnumerable<Book> source, int n, bool ignoreZero = false)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));
        if (n <= 0)
            return Enumerable.Empty<Book>();

        var query = ignoreZero ? source.Where(b => b.Rating > 0.0) : source;
        var ordered = query
            .OrderByDescending(b => b.Rating)
            .ThenBy(b => b.Title)
            .ThenBy(b => b.Id)
            .Take(n);

        return ordered;
    }
}
