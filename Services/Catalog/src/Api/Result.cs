using System;

namespace Api;

public record Result
{
    public Exception? Exception { get; }
    public string? Error { get; }

    public bool IsSuccess { get; }

    public static Result Success() => new();
    public static Result Failure(string error, Exception? exception = null) => new(error, exception);
    public static Result Failure(Exception exception) => new(exception.Message, exception);

    protected Result()
    {
        IsSuccess = true;
    }

    protected Result(string error, Exception? exception = null)
    {
        IsSuccess = false;
        Error = error;
        Exception = exception;
    }
}

public sealed record Result<T> : Result
{
    public T? Value { get; }

    public static Result<T> Success(T value) => new(value);
    public static new Result<T> Failure(string error, Exception? exception = null) => new(error, exception);
    public static new Result<T> Failure(Exception exception) => new(exception.Message, exception);

    Result(T value)
    {
        Value = value;
    }

    Result(string error, Exception? exception) : base(error, exception)
    {
    }
}
