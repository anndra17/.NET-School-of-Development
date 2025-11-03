using System.Text;

namespace ReadingList.Infrastructure.Import;

internal static class CsvTokenizer
{

    public static string[] Split(
        string line,
        char delimiter = ',',
        char quoteChar = '"',
        bool trimUnquotedFields = false)
    {
        if (line is null) return Array.Empty<string>();

        var result = new List<string>();
        int i = 0;

        int end = line.Length;
        while (end > 0 && (line[end - 1] == '\r' || line[end - 1] == '\n'))
            end--;

        while (i < end)
        {
            result.Add(ReadField(line, ref i, end, delimiter, quoteChar, trimUnquotedFields));

            if (i < end && line[i] == delimiter)
                i++;
        }

        if (i == end && end > 0 && line[end - 1] == delimiter)
            result.Add(string.Empty);

        return result.ToArray();
    }

    private static string ReadField(
        string s,
        ref int i,
        int end,
        char delimiter,
        char quoteChar,
        bool trimUnquoted)
    {
        var sb = new StringBuilder();
        bool quoted = false;

        if (i < end && s[i] == quoteChar)
        {
            quoted = true;
            i++;
        }

        if (quoted)
        {
            while (i < end)
            {
                char c = s[i];

                if (c == quoteChar)
                {
                    if (i + 1 < end && s[i + 1] == quoteChar)
                    {
                        sb.Append(quoteChar);
                        i += 2;
                        continue;
                    }

                    i++;

                    while (i < end && (s[i] == ' ' || s[i] == '\t'))
                        i++;

                    if (i < end && s[i] != delimiter)
                        throw new FormatException("Invalid character after closing quote in CSV field.");

                    break; 
                }
                else
                {
                    sb.Append(c);
                    i++;
                }
            }

            if (i >= end) 
                throw new FormatException("Unbalanced quotes in CSV field.");
        }
        else
        {
            int start = i;
            while (i < end)
            {
                char c = s[i];
                if (c == delimiter) break;
                if (c == '\r' || c == '\n') break;
                i++;
            }

            if (trimUnquoted)
            {
                int len = i - start;
                while (len > 0 && char.IsWhiteSpace(s[start])) { start++; len--; }
                while (len > 0 && char.IsWhiteSpace(s[start + len - 1])) { len--; }

                if (len > 0) sb.Append(s, start, len);
            }
            else
            {
                sb.Append(s, start, i - start);
            }
        }

        return sb.ToString();
    }
}
