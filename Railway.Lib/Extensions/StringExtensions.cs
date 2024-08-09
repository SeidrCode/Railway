namespace Railway.Lib.Extensions;

public static class StringExtensions
{
    private const string _line = "/";

    /// <summary>
    /// Возращает указанное количество знаков с начала строки
    /// </summary>
    /// <param name="text">Строка</param>
    /// <param name="length">Количество знаков</param>
    public static string Left(this string text, int length)
    {
        if (string.IsNullOrEmpty(text)) return text;

        length = Math.Abs(length);

        return text.Length <= length ? text : text.Substring(0, length);
    }

    /// <summary>
    /// Возращает указанное количество знаков с конца строки
    /// </summary>
    /// <param name="text">Строка</param>
    /// <param name="length">Количество знаков</param>
    /// <exception cref="ArgumentException"></exception>
    public static string Right(this string text, int length)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        if (length < 1)
            throw new ArgumentException($"{nameof(length)} должен быть > 0");

        return text.Length <= length ? text : text[^length..];
    }

    /// <summary>
    /// Добавляет сегмент к строке url
    /// </summary>
    /// <param name="baseAddress">Базовый url</param>
    /// <param name="segment">Добавляемый сегмент</param>
    /// <returns></returns>
    public static Uri AddUrlSegment(this Uri baseAddress, string segment)
    {
        return new Uri(baseAddress, segment);
    }

    /// <summary>
    /// Преобразует ключ-значение в строку {key}={value}
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetStringOrEmpty(string key, object? value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        var valueText = value.ToJson();

        return valueText == string.Empty ? string.Empty : $"{key}='{valueText}'";
    }
}
