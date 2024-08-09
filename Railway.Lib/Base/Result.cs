namespace Railway.Lib.Base;

public class Result
{
    protected internal Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException();
        }

        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    protected internal Result(bool isSuccess, List<Error> errors)
    {
        if (isSuccess && errors.Any())
        {
            throw new InvalidOperationException();
        }

        if (!isSuccess && errors.Contains(Error.None))
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Errors = errors;
        Error = errors.First();
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public List<Error> Errors { get; }

    public static Result Success() => new(true, Error.None);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

    public static Result Failure(List<Error> errors) => new(false, errors);

    public static Result<TValue> Failure<TValue>(List<Error> errors) => new(default, false, errors);

    public static Result Failure(string message, string code, string errorType = null) => new(false, new Error(message, code, errorType));

    public static Result<TValue> Failure<TValue>(string message, string code, string errorType = null) => new(default, false, new Error(message, code, errorType));

    public static Result<TValue> Create<TValue>(TValue? value) => value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

    public static implicit operator Result(Error error) => Failure(error);

    public static implicit operator Result(List<Error> errors) => Failure(errors);

    /// <summary>
    /// Проверка, содержит ли объект результата ошибку
    /// </summary>
    public bool HasError()
    {
        return Errors.Any();
    }

    /// <summary>
    /// Проверка, содержит ли объект результата ошибку с определенным условием
    /// </summary>
    public bool HasError(Func<Error, bool> predicate)
    {
        return HasError(predicate);
    }

    /// <summary>
    /// Проверка, содержит ли объект результата ошибку с определенным условием
    /// </summary>
    public bool HasError(Func<Error, bool> predicate, out IEnumerable<Error> errors)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        return HasError(predicate, out errors);
    }
}
