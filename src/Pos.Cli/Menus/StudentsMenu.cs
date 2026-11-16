using Pos.Application.Students;
using Pos.Cli.Ui;

namespace Pos.Cli.Menus;

internal sealed class StudentsMenu(ConsoleIo io, ViewPrinter print, StudentService students)
{
    public void Run() =>
        new Menu(io, "Студенты",
        [
            new("1", "Список студентов", () => print.Students(students.List())),
            new("2", "Добавить студента", Add),
        ]).Run();

    private void Add()
    {
        var name = io.AskRequired("ФИО");
        var added = students.Add(name);
        io.Success($"добавлен №{added.Id} — {added.Name}");
    }
}
