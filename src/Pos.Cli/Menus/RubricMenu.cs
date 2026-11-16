using Pos.Application.Rubrics;
using Pos.Cli.Ui;

namespace Pos.Cli.Menus;

internal sealed class RubricMenu(ConsoleIo io, ViewPrinter print, RubricService rubric)
{
    public void Run() =>
        new Menu(io, "Рубрикатор (критерии оценивания)",
        [
            new("1", "Список критериев", () => print.Criteria(rubric.List())),
            new("2", "Добавить критерий", Add),
        ]).Run();

    private void Add()
    {
        var name = io.AskRequired("Название");
        var maxScore = io.AskInt("Максимальный балл (целое)");
        var weight = io.AskDecimal("Вес в итоге (например 0.3 или 30, не больше 1000)");
        var added = rubric.AddCriterion(name, maxScore, weight);
        io.Success(
            $"добавлен №{added.Id} «{added.Name}»: максимум {added.MaxScore}, вес {ViewPrinter.Number(added.Weight)}");
    }
}
