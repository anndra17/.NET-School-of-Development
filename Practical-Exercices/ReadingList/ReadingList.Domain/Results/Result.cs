namespace ReadingList.Domain.Results;

public readonly record struct Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }

    private Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Ok() => new Result(true, null);

    public static Result Fail(string error) => new Result(false, error);

    public TResult Match<TResult>(Func<TResult> onOk, Func<string, TResult> onFail)
        => IsSuccess ? onOk() : onFail(Error!);

    public void Match(Action onOk, Action<string> onFail)
    { 
      if (IsSuccess) 
            onOk(); 
      else 
            onFail(Error!); 
    }
}
