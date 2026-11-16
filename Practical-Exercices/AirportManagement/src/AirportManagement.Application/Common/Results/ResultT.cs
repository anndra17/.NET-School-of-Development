using AirportManagement.Application.Enums;

namespace AirportManagement.Application.Common.Results;

public class Result<T> : Result
{
    public T? Value { get; init; }

    public static Result<T> Ok(T value) => new() { Success = true, Value = value };
    public new static Result<T> Fail(ErrorType type, string message) => new() { Success = false, ErrorType = type, ErrorMessage = message };

}
