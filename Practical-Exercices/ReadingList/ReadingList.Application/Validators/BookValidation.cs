using ReadingList.Domain.Results;

namespace ReadingList.Application.Validators;

public static class BookValidation
{
    public const double MinRating = 0.0;
    public const double MaxRating = 5.0;
    public const int MinYear = 1000;

    public static Result ValidateId(int id)
    {
        if (id <= 0)
            return Result.Fail($"Book ID must be positive, got: {id}");

        return Result.Ok();
    }

    public static Result ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Fail("Title cannot be empty");

        if (title.Length > 500)
            return Result.Fail($"Title too long: {title.Length} characters");

        return Result.Ok();
    }

    public static Result ValidateRating(double rating)
    {
        if (rating < MinRating || rating > MaxRating)
            return Result.Fail(
                $"Rating must be between {MinRating} and {MaxRating}, got: {rating}");

        return Result.Ok();
    }

    public static Result ValidateYear(int year)
    {
        if (year < MinYear)
            return Result.Fail(
                $"Year must be greater than {MinYear}");

        return Result.Ok();
    }

    public static Result ValidatePages(int pages)
    {
        if (pages < 0)
            return Result.Fail($"Pages cannot be negative, got: {pages}");

        if (pages > 50000) 
            return Result.Fail($"Pages value seems unrealistic: {pages}");

        return Result.Ok();
    }
}
