namespace ReadingList.App.Commands.Parsing;

public sealed class ParseResult
{
    public object? Command { get; }
    public bool IsSuccess { get; }
    public string? Error { get; }

    private ParseResult(object? command, bool succes, string?error)
    {
        Command = command;
        IsSuccess = succes;
        Error = error;
    }

    public static ParseResult Ok(object command) => new(command, true, null);

    public static ParseResult Fail(string message) => new(null, false, message);

}
