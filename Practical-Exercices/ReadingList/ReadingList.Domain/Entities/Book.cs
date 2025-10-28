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

    public static Book Create(int id, string title, string author, int year, int pages, string genre, double rating, bool finished = false)
    {
        ValidateBookParameters(id, title, author, year, pages, genre, rating);

        title = title.Trim();
        author = author.Trim();
        genre = genre.Trim();

        return new Book(id, title, author, year, pages, genre, rating, finished);
    }

    private static void ValidateBookParameters(int id, string title, string author, int year, int pages, string genre, double rating)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than 0.");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Author cannot be empty.", nameof(author));

        if (string.IsNullOrWhiteSpace(genre))
            throw new ArgumentException("Genre cannot be empty.", nameof(genre));

        if (pages <= 0)
            throw new ArgumentOutOfRangeException(nameof(pages), "Pages must be greater than 0.");

        if (year <= 0)
            throw new ArgumentOutOfRangeException(nameof(year), "Year must be greater than 0.");

        if (double.IsNaN(rating) || double.IsInfinity(rating) || rating < 0 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 0 and 5.");
    }

    public void MarkFinished()
    {
         Finished = true;
    }

    public void SetRating(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value) || value < 0 || value > 5)
            throw new ArgumentOutOfRangeException(nameof(value), "Rating must be between 0 and 5.");

        Rating = value;
    }

}
