namespace Railway.Lib.Base;

/// <summary>
/// Generic объект результата согласно Railway-Oriented концепции
/// </summary>
public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, Error error) : base(isSuccess, error) => _value = value;

    protected internal Result(TValue? value, bool isSuccess, List<Error> errors) : base(isSuccess, errors) => _value = value;

    public TValue Value => IsSuccess ? _value! : throw new InvalidOperationException("Значение недоступно.");

    public TValue ValueOrDefault => IsSuccess ? _value! : default;

    public static implicit operator Result<TValue>(TValue? value) => Create(value);
}
