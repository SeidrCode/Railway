using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Railway.Lib.Extensions;

public static class JsonExtensions
{
    /// <summary>
    /// Опции Json с игнорированием нулевых значений, включают русско-английскую кодировку и camel case
    /// </summary>
    public static readonly JsonSerializerOptions IgnoreNullJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
    };

    /// <summary>
    /// Опции Json по умолчанию включают русско-английскую кодировку и camel case
    /// </summary>
    public static readonly JsonSerializerOptions BaseJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
    };

    /// <summary>
    /// Преобразование объекта в json строку
    /// </summary>
    /// <param name="obj">Объект для преобразования в json строку</param>
    /// <param name="ignoreNullValues">Опция игнорирования нулевых значений. По умолчанию включена.</param>
    public static string ToJson(this object obj, bool ignoreNullValues = true)
    {
        return ignoreNullValues
            ? JsonSerializer.Serialize(obj, IgnoreNullJsonOptions)
            : JsonSerializer.Serialize(obj, BaseJsonOptions);
    }

    /// <summary>
    /// Получение http контента в виде строки и десериализация в T
    /// </summary>
    public static async Task<T?> ReadAsJsonAsync<T>(this HttpContent content)
    {
        var json = await content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json);
    }
}
