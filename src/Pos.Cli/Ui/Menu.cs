namespace Pos.Cli.Ui;

internal sealed record MenuItem(string Key, string Title, Action Action);

internal sealed class Menu(ConsoleIo io, string title, IReadOnlyList<MenuItem> items, string exitTitle = "Назад")
{
    public void Run()
    {
        while (true)
        {
            io.Header(title);
            foreach (var item in items)
            {
                io.Line($"{item.Key}. {item.Title}");
            }

            io.Line($"0. {exitTitle}");

            var key = io.Ask("Выберите пункт");
            if (key == "0")
            {
                return;
            }

            var chosen = items.FirstOrDefault(i => i.Key == key);
            if (chosen is null)
            {
                io.Error($"нет пункта «{key}»");
                continue;
            }

            io.Safely(chosen.Action);
        }
    }
}
