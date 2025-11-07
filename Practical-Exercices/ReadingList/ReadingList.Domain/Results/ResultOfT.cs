namespace ReadingList.Domain.Results;

public readonly record struct Result<T>
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public T? Value { get; }

    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Ok(T value) => new Result<T>(true, value, null);

    public static Result<T> Fail(string error)  
            => new (false, default, string.IsNullOrWhiteSpace(error) ? "Unknown error" : error);

    public Result<TResult> Map<TResult>(Func<T, TResult> map)
    {
        if (map is null) throw new ArgumentNullException(nameof(map));

        return IsSuccess ? Result<TResult>.Ok(map(Value!))
                         : Result<TResult>.Fail(Error!);
    }

    public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> bind)
    {
        if (bind is null) throw new ArgumentNullException( nameof(bind));

        return IsSuccess ? bind(Value!) : Result<TResult>.Fail(Error!); 
    }

    public TResult Match<TResult>(Func<T, TResult> onOk, Func<string, TResult> onFail)
    {
        if (onOk is null) throw new ArgumentNullException(nameof(onOk));
        if (onFail is null) throw new ArgumentNullException(nameof(onFail));

        return IsSuccess ? onOk(Value!) : onFail(Error!);
    }

    public void Match(Action<T> onOk, Action<string> onFail)
    {
        if (onOk is null) throw new ArgumentNullException(nameof(onOk));
        if (onFail is null) throw new ArgumentNullException(nameof(onFail));

        if (IsSuccess) 
            onOk(Value!);
        else 
            onFail(Error!);
    }
}
