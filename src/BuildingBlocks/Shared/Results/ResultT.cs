namespace Shared.Results;

public sealed class Result<T> : Result
{
    private readonly T? _value;

    private Result(T value)
        : base(true, Error.None)
    {
        _value = value;
    }

    private Result(Error error)
        : base(false, error)
    {
    }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }

    public static new Result<T> Failure(Error error)
    {
        return new Result<T>(error);
    }

    public static new Result<T> Validation(string code, string message)
    {
        return Failure(Error.Validation(code, message));
    }

    public static new Result<T> Unauthorized(string code, string message)
    {
        return Failure(Error.Unauthorized(code, message));
    }

    public static new Result<T> NotFound(string code, string message)
    {
        return Failure(Error.NotFound(code, message));
    }

    public static new Result<T> Conflict(string code, string message)
    {
        return Failure(Error.Conflict(code, message));
    }
}
