using Pos.Application.Common;
using Pos.Domain.Model;
using Pos.Domain.Repositories;
using Pos.Domain.Services.Totals;

namespace Pos.Application.Totals;

public sealed class TotalsService(
    IStudentRepository students,
    IRubricRepository rubrics,
    TotalCalculator calculator,
    TotalRuleFactory rules,
    ITotalsExporter exporter)
{
    /// <summary>Точный итог — для просмотра в консоли.</summary>
    public TotalsReport ForAll(TotalMode mode) => Calculate(mode, rounded: false);

    /// <summary>В ведомость уходит целая оценка, поэтому экспорт округляет итог.</summary>
    public ExportResult Export(TotalMode mode)
    {
        var report = Calculate(mode, rounded: true);
        if (report.Totals.Count == 0)
        {
            throw new UseCaseException("Нет студентов — выгружать нечего");
        }

        return new ExportResult(exporter.Export(report), report);
    }

    private TotalsReport Calculate(TotalMode mode, bool rounded) =>
        DomainErrors.Translate(() =>
        {
            var rubric = rubrics.Get();
            var rule = rules.Create(mode, rounded);
            var totals = students.GetAll().Select(s => ToView(s, rubric, rule)).ToList();

            return new TotalsReport(mode, rounded, totals);
        });

    private TotalView ToView(Student student, Rubric rubric, ITotalStrategy rule)
    {
        var total = calculator.Calculate(student, rubric, rule);

        var lines = total.Lines
            .Select(l => new TotalLineView(
                l.Criterion.Id.Value, l.Criterion.Name, l.Score?.Value, l.Criterion.MaxScore))
            .ToList();

        return new TotalView(student.Id.Value, student.Name, total.Final, lines);
    }
}
