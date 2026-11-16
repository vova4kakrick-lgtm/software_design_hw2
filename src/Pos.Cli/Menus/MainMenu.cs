using Pos.Cli.Ui;

namespace Pos.Cli.Menus;

internal sealed class MainMenu(
    ConsoleIo io,
    StudentsMenu students,
    RubricMenu rubric,
    EvaluationsMenu evaluations,
    TotalsMenu totals)
{
    public void Run()
    {
        io.Line("ПОС — преподавательская оценка студентов");

        new Menu(io, "Главное меню",
        [
            new("1", "Студенты", students.Run),
            new("2", "Рубрикатор (критерии)", rubric.Run),
            new("3", "Оценки", evaluations.Run),
            new("4", "Итоги и экспорт", totals.Run),
        ], exitTitle: "Выход").Run();
    }
}
