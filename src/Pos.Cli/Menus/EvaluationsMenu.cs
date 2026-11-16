using Pos.Application.Evaluations;
using Pos.Cli.Ui;

namespace Pos.Cli.Menus;

internal sealed class EvaluationsMenu(ConsoleIo io, ViewPrinter print, EvaluationService evaluations)
{
    public void Run() =>
        new Menu(io, "Оценки",
        [
            new("1", "Все оценки", () => print.Evaluations(evaluations.ListAll())),
            new("2", "Выставить оценку", Record),
            new("3", "Исправить оценку", Correct),
            new("4", "Отменить оценку", Cancel),
        ]).Run();

    private void Record()
    {
        var (studentId, criterionId) = AskStudentAndCriterion();
        var score = io.AskInt("Балл (целое)");
        var comment = io.Ask("Комментарий (необязательно)");
        var recorded = evaluations.Record(studentId, criterionId, score, comment);
        io.Success($"{recorded.StudentName}: «{recorded.CriterionName}» — {recorded.Score} из {recorded.MaxScore}");
    }

    private void Correct()
    {
        var (studentId, criterionId) = AskStudentAndCriterion();
        var score = io.AskInt("Новый балл (целое)");
        var comment = io.Ask("Комментарий (необязательно)");
        var corrected = evaluations.Correct(studentId, criterionId, score, comment);
        io.Success($"исправлено: «{corrected.CriterionName}» — {corrected.Score} из {corrected.MaxScore}");
    }

    private void Cancel()
    {
        var (studentId, criterionId) = AskStudentAndCriterion();
        evaluations.Cancel(studentId, criterionId);
        io.Success($"оценка студента №{studentId} по критерию №{criterionId} отменена");
    }

    private (int StudentId, int CriterionId) AskStudentAndCriterion() =>
        (io.AskInt("Номер студента"), io.AskInt("Номер критерия"));
}
