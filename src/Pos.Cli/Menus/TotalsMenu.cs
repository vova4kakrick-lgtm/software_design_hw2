using Pos.Application.Totals;
using Pos.Cli.Ui;

namespace Pos.Cli.Menus;

internal sealed class TotalsMenu(ConsoleIo io, ViewPrinter print, TotalsService totals)
{
    public void Run() =>
        new Menu(io, "Итоги и экспорт",
        [
            new("1", "Итоги всех студентов", () => print.Totals(totals.ForAll(AskMode()))),
            new("2", "Экспорт итогов в JSON (округлены до целого)", Export),
        ]).Run();

    private void Export()
    {
        var result = totals.Export(AskMode());
        print.Totals(result.Report);
        io.Success($"итоги выгружены → {result.Location}");
    }

    private TotalMode AskMode()
    {
        io.Line(
            $"Правило итога: 1 — {ViewPrinter.RuleName(TotalMode.Strict, false)}, " +
            $"2 — {ViewPrinter.RuleName(TotalMode.Current, false)}");

        while (true)
        {
            switch (io.AskWithDefault("Правило", "1"))
            {
                case "1":
                    return TotalMode.Strict;
                case "2":
                    return TotalMode.Current;
                default:
                    io.Error("введите 1 или 2");
                    break;
            }
        }
    }
}
