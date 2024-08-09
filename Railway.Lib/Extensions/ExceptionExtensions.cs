using Primitives.Lib.Extensions;

namespace Railway.Lib.Extensions;

public static class ExceptionExtensions
{
    public static List<string> GetMessages(this Exception exception, int level = int.MaxValue)
    {
        var list = new List<string>();
        var counter = 1;
        while (exception != null && counter <= level)
        {
            if(!list.Contains(exception.Message))
                list.Add(exception.Message);

            exception = exception.InnerException;
            counter++;
        }

        return list;
    }

    public static List<string> GetStackTrace(this Exception exception, int level = int.MaxValue)
    {
        var list = new List<string>();
        var counter = 1;
        while (exception != null && counter <= level)
        {
            var lines = GetLinesFromStackTrace(exception);

            if (lines == null || lines.Count == 0)
                return list;

            list.AddRange(lines);

            exception = exception.InnerException;
            counter++;
        }

        return list;
    }

    private static List<string> GetLinesFromStackTrace(Exception exception)
    {
        var results = new List<string>();

        if (string.IsNullOrEmpty(exception?.StackTrace))
            return results;

        var lines = exception.StackTrace.Split(Environment.NewLine)
            .Where(x => x.Contains(":line ")).ToList();

        if (lines.Count == 0)
            return results;

        results.AddRange(lines.Select(GetTextFromLine));

        return results;
    }

    private static string GetTextFromLine(string line)
    {
        var length = line.Length;
        var index = line.IndexOf(") in ", StringComparison.Ordinal);
        var leftPartLength = line.Left(index).Length;
        return line.Right(length - leftPartLength - 5);
    }
}
