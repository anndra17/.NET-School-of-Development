using System.Globalization;

namespace ReadingList.Infrastructure.Parsing;

internal static class CsvParsingHelpers
{
    public const int ExpectedColumnCount = 8;

    public static string SafeTrim(string? input)
        => (input ?? string.Empty).Trim().Trim('"');

    public static bool TryParseInt(string input, out int value, out string error)
    {
        error = string.Empty;
        if (!int.TryParse(input.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
        {
            error = $"Invalid integer: '{input}'";
            return false;
        }
        return true;
    }

    public static bool TryParsePositiveInt(string input, out int value, out string error)
    {
        error = string.Empty;
        if (!int.TryParse(input.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value)
            || value < 0)
        {
            error = $"Invalid positive integer: '{input}'";
            return false;
        }
        return true;
    }

    public static bool TryParseBoolean(string? input, out bool value)
    {
        value = false;
        var normalized = SafeTrim(input).ToLowerInvariant();

        switch (normalized)
        {
            case "y":
            case "yes":
            case "true":
            case "1":
                value = true;
                return true;
            case "n":
            case "no":
            case "false":
            case "0":
            case "":
                value = false;
                return true;
            default:
                return false;
        }
    }

    public static bool TryParseDouble(string input, out double value)
    {
        var text = SafeTrim(input);

        if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            return true;
        text = text.Replace(',', '.');

        return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }
}