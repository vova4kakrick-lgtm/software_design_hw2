using System.Globalization;
using Pos.Application.Evaluations;
using Pos.Application.Rubrics;
using Pos.Application.Students;
using Pos.Application.Totals;

namespace Pos.Cli.Ui;

internal sealed class ViewPrinter(ConsoleIo io)
{
    public static string Number(decimal value) => value.ToString("0.##", CultureInfo.InvariantCulture);

    public static string RuleName(TotalMode mode, bool rounded)
    {
        var name = mode switch
        {
            TotalMode.Strict => "строгое (нет оценки = 0)",
            TotalMode.Current => "текущее (только выставленные оценки)",
            _ => mode.ToString(),
        };

        return rounded ? $"{name}, округлено до целого" : name;
    }

    public void Students(IReadOnlyList<StudentView> students) =>
        io.Table(
            ["№", "ФИО"],
            students.Select(s => Row(s.Id.ToString(), s.Name)).ToList());

    public void Criteria(IReadOnlyList<CriterionView> criteria) =>
        io.Table(
            ["№", "Критерий", "Макс.", "Вес"],
            criteria.Select(c => Row(c.Id.ToString(), c.Name, c.MaxScore.ToString(), Number(c.Weight))).ToList());

    public void Evaluations(IReadOnlyList<EvaluationView> evaluations) =>
        io.Table(
            ["Студент", "Критерий", "Балл", "Дата", "Комментарий"],
            evaluations.Select(e => Row(
                $"№{e.StudentId} {e.StudentName}",
                $"№{e.CriterionId} {e.CriterionName}",
                $"{e.Score} / {e.MaxScore}",
                e.Date.ToString("dd.MM.yyyy"),
                e.Comment)).ToList());

    /// <summary>Итог с разбивкой: по столбцу на критерий, «—» — оценки нет.</summary>
    public void Totals(TotalsReport report)
    {
        io.Line($"Правило: {RuleName(report.Mode, report.Rounded)}");

        var criteria = report.Totals.Count > 0
            ? report.Totals[0].Lines.Select(l => $"№{l.CriterionId} {l.CriterionName}").ToList()
            : [];

        io.Table(
            ["№", "ФИО", .. criteria, "Итог"],
            report.Totals.Select(t => Row(
            [
                t.StudentId.ToString(),
                t.StudentName,
                .. t.Lines.Select(l => l.Score is null ? "—" : $"{l.Score}/{l.MaxScore}"),
                Number(t.Final),
            ])).ToList());
    }

    private static IReadOnlyList<string> Row(params string[] cells) => cells;
}
