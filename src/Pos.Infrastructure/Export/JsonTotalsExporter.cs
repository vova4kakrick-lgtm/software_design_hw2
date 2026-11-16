using System.Text.Json;
using Pos.Application.Totals;

namespace Pos.Infrastructure.Export;

public sealed class JsonTotalsExporter(string filePath) : ITotalsExporter
{
    public string Export(TotalsReport report)
    {
        var path = Path.GetFullPath(filePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        var document = new
        {
            Rule = report.Mode.ToString().ToLowerInvariant(),
            report.Rounded,
            Totals = report.Totals.Select(t => new
            {
                t.StudentId,
                t.StudentName,
                Scores = t.Lines.Select(l => new { l.CriterionId, l.CriterionName, l.Score, l.MaxScore }),
                t.Final,
            }),
        };

        File.WriteAllText(path, JsonSerializer.Serialize(document, JsonDefaults.Options));
        return path;
    }
}
