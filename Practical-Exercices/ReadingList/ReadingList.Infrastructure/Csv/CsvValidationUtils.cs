using System.Globalization;

namespace ReadingList.Infrastructure.Csv;

public static class CsvValidationUtils
{
    public const int ExpectedColumnNumber = 8;

    public static bool TryParseId(string input, out int id, out string error)
    {
        error = string.Empty;

        if (!int.TryParse(input.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
        {
            error = $"Invalid Id: '{input}'";
            return false;
        }

        return true;
    }

    public static bool TryParseNonEmpty(string input, string fieldName, out string value, out string error)
    {
        value = Safe(input);
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(value))
        {
            error = $"Empty {fieldName}.";
            return false;
        }

        return true;
    }

    public static bool TryParseYear(string input, out int year, out string error)
    {
        error = string.Empty;

        if (!int.TryParse(input.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out year))
        {
            error = $"Invalid year: '{input}'";
            return false;
        }

        return true;
    }

    public static bool TryParsePages(string input, out int pages, out string error)
    {
        error = string.Empty;

        if (!int.TryParse(input.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out pages) || pages < 0)
        {
            error = $"Invalid number of pages: '{input}'";
            return false;
        }


        return true;
    }

    public static string Safe(string? s) => (s ?? string.Empty).Trim();

    public static bool TryParseFinished(string? s, out bool value)
    {
        value = false;
        var t = (s ?? string.Empty).Trim().Trim('"').ToLowerInvariant();

        switch (t)
        {
            case "y":
            case "yes":
            case "true":
                value = true; return true;
            case "n":
            case "no":
            case "false":
                value = false; return true;
            default:
                return false;
        }
    }

    public static bool TryParseRating(string input, out double rating, out string error)
    {
        error = string.Empty;
        rating = 0.0;

        var ratingText = input?.Trim();
        if (string.IsNullOrWhiteSpace(ratingText))
        {
            rating = 0.0;
            return true;
        }

        if (!TryParseDoubleFlexible(ratingText, out rating))
        {
            error = $"Invalid rating: '{input}' (expected a value between 0 and 5)";
            return false;
        }

        if (rating < 0.0 || rating > 5.0)
        {
            error = $"Invalid rating: '{input}' (expected 0..5)";
            return false;
        }

        return true;
    }

    public static bool TryParseDoubleFlexible(string s, out double value)
    {
        var txt = s.Trim().Trim('"');
        if (double.TryParse(txt, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            return true;
        txt = txt.Replace(',', '.');

        return double.TryParse(txt, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }
}


