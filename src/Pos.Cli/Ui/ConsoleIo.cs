using System.Globalization;
using Pos.Application.Common;

namespace Pos.Cli.Ui;

internal sealed class ConsoleIo(TextReader input, TextWriter output)
{
    public string Ask(string prompt)
    {
        output.Write($"{prompt}: ");
        var line = input.ReadLine() ?? throw new EndOfInputException();
        return line.Trim();
    }

    public string AskRequired(string prompt)
    {
        while (true)
        {
            var answer = Ask(prompt);
            if (answer.Length > 0)
            {
                return answer;
            }

            Error("Значение не может быть пустым");
        }
    }

    public string AskWithDefault(string prompt, string defaultValue)
    {
        var answer = Ask($"{prompt} [{defaultValue}]");
        return answer.Length == 0 ? defaultValue : answer;
    }

    public decimal AskDecimal(string prompt)
    {
        while (true)
        {
            var answer = Ask(prompt).Replace(',', '.');
            if (decimal.TryParse(answer, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }

            Error("Нужно число, например 7.5 или 7,5");
        }
    }

    public int AskInt(string prompt)
    {
        while (true)
        {
            var answer = Ask(prompt);
            if (int.TryParse(answer, out var value))
            {
                return value;
            }

            Error("Нужно целое число, например 8");
        }
    }

    public void Header(string text)
    {
        output.WriteLine();
        output.WriteLine($"=== {text} ===");
    }

    public void Line(string text = "") => output.WriteLine(text);

    public void Success(string text) => output.WriteLine($"OK: {text}");

    public void Error(string text) => output.WriteLine($"Ошибка: {text}");

    public void Table(IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyList<string>> rows)
    {
        if (rows.Count == 0)
        {
            output.WriteLine("(пусто)");
            return;
        }

        var widths = headers
            .Select((h, i) => Math.Max(h.Length, rows.Max(r => r[i].Length)))
            .ToArray();

        string Format(IReadOnlyList<string> cells) =>
            string.Join(" | ", cells.Select((c, i) => c.PadRight(widths[i]))).TrimEnd();

        output.WriteLine(Format(headers));
        output.WriteLine(string.Join("-+-", widths.Select(w => new string('-', w))));
        foreach (var row in rows)
        {
            output.WriteLine(Format(row));
        }
    }

    public void Safely(Action action)
    {
        try
        {
            action();
        }
        catch (UseCaseException ex)
        {
            Error(ex.Message);
        }
        catch (Exception ex) when (ex is InvalidDataException or IOException or UnauthorizedAccessException)
        {
            Error($"не удалось работать с файлом — {ex.Message}");
        }
        catch (Exception ex) when (ex is not EndOfInputException)
        {
            // Страховка: неожиданная ошибка одного действия не должна ронять всё приложение.
            Error($"непредвиденная ошибка — {ex.Message}");
        }
    }
}
