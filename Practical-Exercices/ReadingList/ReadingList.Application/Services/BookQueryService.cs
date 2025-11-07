using ReadingList.Application.Abstractions.Persistence;
using ReadingList.Application.Dtos;
using ReadingList.Domain.Entities;

namespace ReadingList.Application.Services;

public sealed class BookQueryService
{
    private readonly IRepository<Book, int> _repository;

    public BookQueryService(IRepository<Book, int> repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public IReadOnlyList<Book> ListAll()
    {
        return _repository.GetAll().ToList();
    }

    public IReadOnlyList<Book> FilterFinished(bool finished = true)
    {
        var books = _repository.GetAll();
        return books.Where(b => b.Finished == finished).ToList();
    }

    public IReadOnlyList<Book> ByAuthorContains(string input, bool ignoreCase = false)
    {
        if (string.IsNullOrEmpty(input)) return Array.Empty<Book>();
        
        var books = _repository.GetAll();
        if(!ignoreCase)
            return books.Where(b => b.Author?.Contains(input) == true).ToList();

        return books
            .Where(b => b.Author is not null
                        && b.Author.IndexOf(input, StringComparison.OrdinalIgnoreCase) >= 0)
            .ToList();  
    }

    public IReadOnlyList<Book> TopRated(int count)
    {
        if (count <=0) return Array.Empty<Book>();

        var books = _repository.GetAll();
        return books
            .OrderByDescending(b => b.Rating)
            .ThenBy(b => b.Title)
            .Take(count)
            .ToList();
    }

    public BookStatsDto Stats()
    {
        var allBooks = _repository.GetAll().ToList();

        int totalNoBooks = allBooks.Count;
        int finishedBooks = allBooks.Count(b => b.Finished);

        double averageRating = totalNoBooks == 0 ? 0.0 : Math.Round(allBooks.Average(b => b.Rating), 2);

        var pagesByGenre = allBooks
            .GroupBy(b => string.IsNullOrWhiteSpace(b.Genre) ? "Unknown" : b.Genre!)
            .ToDictionary(g => g.Key, g => g.Sum(b => b.Pages));
                                                                    
        var topAuthors = allBooks
            .GroupBy(b => b.Author ?? "Unknown")
            .Select(g => (Author: g.Key, Count: g.Count()))
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Author)
            .Take(3)
            .ToList();

        return new BookStatsDto(
                totalBooks: totalNoBooks,
                finishedBooks: finishedBooks,
                averageRating: averageRating,
                pagesByGenre: pagesByGenre,
                topAuthors: topAuthors);
    }
}
