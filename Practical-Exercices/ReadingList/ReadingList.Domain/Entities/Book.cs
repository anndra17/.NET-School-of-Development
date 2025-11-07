using ReadingList.Domain.Results;

namespace ReadingList.Domain.Entities;

public class Book
{
    public int Id { get; }
    public string Title { get; private set; }
    public string Author { get; private set; }
    public int Year { get; private set; }
    public int Pages { get; private set; }
    public string Genre { get; private set; }
    public double Rating { get; private set; }
    public bool Finished { get; private set; }

    private Book(int id, string title, string author, int year, int pages, string genre, double rating, bool finished)
    {
        Id = id;
        Title = title;
        Author = author;
        Year = year;
        Pages = pages;
        Genre = genre;
        Finished = finished;
        Rating = rating;
    }

    public static Result<Book> Create(int id, string title, string author, int year, int pages, string genre, double rating, bool finished = false)
    {
        var validationError = ValidateParameters(id, title, author, year, pages, genre, rating);
        if (validationError is not null)
            return Result<Book>.Fail(validationError);

        title = title.Trim();
        author = author.Trim();
        genre = genre.Trim();

        var book = new Book(id, title, author, year, pages, genre, rating, finished);
        return Result<Book>.Ok(book);
    }

    private static string? ValidateParameters(int id, string title, string author, int year, int pages, string genre, double rating)
    {
        if (id <= 0)
            return "Id must be greater than 0.";

        if (string.IsNullOrWhiteSpace(title))
            return "Title cannot be empty.";

        if (string.IsNullOrWhiteSpace(author))
            return "Author cannot be empty.";

        if (string.IsNullOrWhiteSpace(genre))
            return "Genre cannot be empty.";

        if (pages <= 0)
            return "Pages must be greater than 0.";

        if (year <= 0)
            return "Year must be greater than 0.";

        if (double.IsNaN(rating) || double.IsInfinity(rating) || rating < 0 || rating > 5)
            return "Rating must be between 0 and 5.";

        return null;
    }

    public void MarkFinished()
    {
         Finished = true;
    }

    public Result SetRating(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value) || value < 0 || value > 5)
            return Result.Fail("Rating must be between 0 and 5.");

        Rating = value;
        return Result.Ok();
    }
}
