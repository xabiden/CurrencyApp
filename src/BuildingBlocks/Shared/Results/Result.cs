namespace Shared.Results;

public class Result
{
    protected internal Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException("Successful result cannot contain an error.");
        }

        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException("Failed result must contain an error.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public static Result Success()
    {
        return new Result(true, Error.None);
    }

    public static Result Failure(Error error)
    {
        return new Result(false, error);
    }

    public static Result Validation(string code, string message)
    {
        return Failure(Error.Validation(code, message));
    }

    public static Result Unauthorized(string code, string message)
    {
        return Failure(Error.Unauthorized(code, message));
    }

    public static Result NotFound(string code, string message)
    {
        return Failure(Error.NotFound(code, message));
    }

    public static Result Conflict(string code, string message)
    {
        return Failure(Error.Conflict(code, message));
    }
}
