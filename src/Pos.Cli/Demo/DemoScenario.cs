using Pos.Application.Evaluations;
using Pos.Application.Rubrics;
using Pos.Application.Students;
using Pos.Application.Totals;
using Pos.Cli.Ui;

namespace Pos.Cli.Demo;

/// <summary>Сквозной сценарий из ТЗ: студенты и критерии → оценки → итог → экспорт итога.</summary>
internal sealed class DemoScenario(
    ConsoleIo io,
    ViewPrinter print,
    StudentService students,
    RubricService rubric,
    EvaluationService evaluations,
    TotalsService totals)
{
    public void Run()
    {
        io.Header("Демо: сквозной сценарий ПОС");

        io.Header("1. Студенты и критерии");
        var ivanov = students.Add("Иванов Иван");
        var petrova = students.Add("Петрова Анна");
        var sidorov = students.Add("Сидоров Пётр");
        print.Students(students.List());

        var hw1 = rubric.AddCriterion("ДЗ 1", 10, 0.2m);
        var hw2 = rubric.AddCriterion("ДЗ 2", 10, 0.2m);
        var cw = rubric.AddCriterion("Контрольная", 10, 0.2m);
        var exam = rubric.AddCriterion("Экзамен", 100, 0.4m);
        print.Criteria(rubric.List());

        io.Header("2. Оценки");
        evaluations.Record(ivanov.Id, hw1.Id, 9, "аккуратное решение");
        evaluations.Record(ivanov.Id, hw2.Id, 8);
        evaluations.Record(ivanov.Id, cw.Id, 7);
        evaluations.Record(ivanov.Id, exam.Id, 81);
        evaluations.Record(petrova.Id, hw1.Id, 10);
        evaluations.Record(petrova.Id, hw2.Id, 8);
        evaluations.Record(petrova.Id, cw.Id, 9);
        evaluations.Record(petrova.Id, exam.Id, 95);
        evaluations.Record(sidorov.Id, hw1.Id, 5);
        evaluations.Record(sidorov.Id, hw2.Id, 7, "сдал с опозданием");
        print.Evaluations(evaluations.ListAll());

        io.Header("3. Правила домена нарушить нельзя");
        Attempt("12 из 10", () => evaluations.Record(sidorov.Id, cw.Id, 12));
        Attempt("критерий, которого нет в рубрикаторе", () => evaluations.Record(sidorov.Id, 99, 5));
        Attempt("вторая оценка по тому же критерию", () => evaluations.Record(ivanov.Id, hw1.Id, 10));
        Attempt("критерий с нулевым весом", () => rubric.AddCriterion("Посещаемость", 10, 0m));

        io.Header("4. Исправление и отмена оценки");
        var corrected = evaluations.Correct(sidorov.Id, hw1.Id, 6, "пересдал");
        io.Success($"{corrected.StudentName}: «{corrected.CriterionName}» исправлено на {corrected.Score}");
        evaluations.Record(sidorov.Id, cw.Id, 4);
        evaluations.Cancel(sidorov.Id, cw.Id);
        io.Success($"{corrected.StudentName}: оценка за «{cw.Name}» выставлена по ошибке и отменена");

        io.Header("5. Итог: строгое правило");
        print.Totals(totals.ForAll(TotalMode.Strict));

        io.Header("6. Итог: текущее правило");
        print.Totals(totals.ForAll(TotalMode.Current));

        io.Header("7. Экспорт итога (в ведомость — целая оценка)");
        var exported = totals.Export(TotalMode.Strict);
        print.Totals(exported.Report);
        io.Success($"итоги выгружены → {exported.Location}");
    }

    private void Attempt(string what, Action action)
    {
        io.Line($"Попытка: {what}");
        io.Safely(action);
    }
}
