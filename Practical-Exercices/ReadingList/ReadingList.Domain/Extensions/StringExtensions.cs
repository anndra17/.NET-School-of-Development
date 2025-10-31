using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ReadingList.Domain.Extensions;

public static class StringExtensions
{
    public static string NormalizeWhitespace(this string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var normalizedInput = input.Normalize(NormalizationForm.FormC).Replace('\u00A0', ' ');
        normalizedInput = normalizedInput.Trim();
        normalizedInput = Regex.Replace(normalizedInput, @"\s+", " ");

        return normalizedInput;
    }

    public static string ToTitleCaseSafer(this string? input, CultureInfo? culture = null)
    {
        var text = input.NormalizeWhitespace();
        if (text.Length == 0)
            return string.Empty;

        culture ??= CultureInfo.CurrentCulture;
        var lower = text.ToLower(culture);
        var title = culture.TextInfo.ToTitleCase(lower);

        return title;
    }
}
