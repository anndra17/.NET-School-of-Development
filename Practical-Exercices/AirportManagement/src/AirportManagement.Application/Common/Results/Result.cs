using AirportManagement.Application.Enums;

namespace AirportManagement.Application.Common.Results;

public class Result
{
    public bool Success { get; init; }
    public ErrorType ErrorType { get; init; }
    public string? ErrorMessage { get; init; }

    public static Result Ok() => new() { Success = true };
    public static Result Fail(ErrorType type, string message) => new() { Success = false, ErrorType = type, ErrorMessage = message };
}
