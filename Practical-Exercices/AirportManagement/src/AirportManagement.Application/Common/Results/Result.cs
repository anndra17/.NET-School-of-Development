using AirportManagement.Application.Enums;

namespace AirportManagement.Application.Common.Results;

public class Result
{
    public bool Success { get; init; }
    public FlightErrorType? ErrorType { get; init; }
    public string? ErrorMessage { get; init; }

    public static Result Ok() => new() { Success = true };
    public static Result Fail(FlightErrorType type, string message) => new() { Success = false, ErrorType = type, ErrorMessage = message };
}
