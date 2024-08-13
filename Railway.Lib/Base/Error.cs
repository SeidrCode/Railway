using System.Text.Json.Serialization;
using Primitives.Lib.Extensions;
using Railway.Lib.Constants;
using Railway.Lib.Extensions;

namespace Railway.Lib.Base;

/// <summary>
/// Класс олицетворяющий объект ошибки
/// </summary>
public record Error
{
    private Error()
    {
        Metadata = new Dictionary<string, object>();
        Details = new List<string>();
        StackTraces = new List<string>();
    }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    /// <param name="code">Код ошибки</param>
    /// <param name="errorType">Тип ошибки: BusinessError или SystemError</param>
    public Error(string message) : this()
    {
        Message = message;
    }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    /// <param name="code">Код ошибки</param>
    /// <param name="errorType">Тип ошибки: BusinessError или SystemError</param>
    public Error(string message, string code, string errorType = null) : this()
    {
        Message = message;
        Code = code;

        ErrorTypeValidate(errorType);
    }

    private void ErrorTypeValidate(string errorType)
    {
        if (string.IsNullOrEmpty(errorType))
            return;

        if (errorType is ErrorTypes.BusinessError or ErrorTypes.SystemError)
        {
            Metadata.Add("ErrorType", errorType);
        }
        else
        {
            throw new ArgumentException("Некорректный тип ошибки. Допустимые типы: BusinessError или SystemError.");
        }
    }

    /// <summary>
    /// Текст ошибки
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Код ошибки
    /// </summary>
    public string Code { get; protected set; }

    /// <summary>
    /// Время ошибки
    /// </summary>
    public DateTime Timestamp => DateTime.Now;

    /// <summary>
    /// Дополнительные реквизиты ошибки
    /// </summary>
    public Dictionary<string, object> Metadata { get; }

    /// <summary>
    /// Список причин ошибки
    /// </summary>
    public List<string> Details { get; }

    /// <summary>
    /// Стек трейс ошибки
    /// </summary>
    public List<string> StackTraces { get; }

    /// <summary>
    /// Добавляет причину ошибки
    /// </summary>
    public Error WithDetails(string details)
    {
        if (!string.IsNullOrEmpty(details))
            Details.Add(details);
        return this;
    }

    public Error WithErrorCode(string code)
    {
        Code = code;
        return this;
    }

    public Error WithErrorType(string errorType)
    {
        ErrorTypeValidate(errorType);
        return this;
    }

    /// <summary>
    /// Добавляет причину ошибки в виде исключения
    /// </summary>
    public Error CausedBy(Exception exception)
    {
        if (exception == null)
            throw new ArgumentNullException(nameof(exception));

        Details.AddRange(exception.GetMessages());
        StackTraces.AddRange(exception.GetStackTrace());
        return this;
    }

    /// <summary>
    /// Добавляет реквизиты ошибки
    /// </summary>
    public Error WithMetadata(string metadataName, object metadataValue)
    {
        if (string.IsNullOrEmpty(metadataName))
            throw new ArgumentNullException(nameof(metadataName));

        if (metadataValue == null)
            throw new ArgumentNullException(nameof(metadataValue));

        if (Metadata.ContainsKey(metadataName))
            Metadata[metadataName] = metadataValue;
        else
            Metadata.Add(metadataName, metadataValue);
        return this;
    }

    /// <summary>
    /// Добавляет реквизиты ошибки
    /// </summary>
    public Error WithMetadata(Dictionary<string, object> metadata)
    {
        foreach (var metadataItem in metadata)
            WithMetadata(metadataItem.Key, metadataItem.Value);

        return this;
    }

    public bool HasMetadataKey(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        return Metadata.ContainsKey(key);
    }

    public bool HasMetadata(string key, Func<object, bool> predicate)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        if (Metadata.TryGetValue(key, out object actualValue))
            return predicate(actualValue);

        return false;
    }

    public static readonly Error None = new(string.Empty, string.Empty);

    public static readonly Error NullValue = new("Error.NullValue", "Ошибка. Ожидаемое значение равно null.");

    public static readonly Error NotFound = new("Error.NotFound", "Ошибка. Ожидаемое значение не найдено.");

    public static implicit operator Result(Error error) => Result.Failure(error);

    public string GetErrorTypeFromMetadata()
    {
        var errorTypeObject = Metadata.Keys.FirstOrDefault(x => x == "ErrorType");
        return errorTypeObject != null
            ? Metadata["ErrorType"].ToString()
            : ErrorTypes.SystemError;
    }

    public override string ToString()
    {
        var print = new ErrorForPrint(this);

        return print.ToJson();
    }

    private record ErrorForPrint(Error Error)
    {
        [JsonPropertyName("code")]
        public string Code => Error.Code;

        [JsonPropertyName("message")]
        public string Message => Error.Message;

        [JsonPropertyName("timestamp")]
        public string Timestamp => Error.Timestamp.ToString("dd-MM-yyyTHH:mm:ss.fff");

        [JsonPropertyName("metadata"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string[] Metadata => Error.Metadata.Select(x => StringExtensions.GetStringOrEmpty(x.Key, x.Value)).ToArray();

        [JsonPropertyName("details"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string[] Details => Error.Details.Distinct().ToArray();

        [JsonPropertyName("stackTraces"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string[] StackTraces => Error.StackTraces.ToArray();
    }
}
