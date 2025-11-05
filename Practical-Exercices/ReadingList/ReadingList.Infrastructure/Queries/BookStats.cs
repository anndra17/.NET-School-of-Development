namespace ReadingList.Infrastructure.Queries;

public sealed class BookStats
{
    public int TotalBooks { get; }
    public int FinishedBooks { get; }
    public double AverageRating { get; }         
    public IReadOnlyDictionary<string, int> PagesByGenre { get; }
    public IReadOnlyList<(string Author, int Count)> TopAuthors { get; }

    public BookStats(
        int totalBooks,
        int finishedBooks,
        double averageRating,
        IReadOnlyDictionary<string, int> pagesByGenre,
        IReadOnlyList<(string Author, int Count)> topAuthors)
    {
        TotalBooks = totalBooks;
        FinishedBooks = finishedBooks;
        AverageRating = averageRating;
        PagesByGenre = pagesByGenre;
        TopAuthors = topAuthors;
    }
}
