using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Pos.Cli;
using Pos.Cli.Demo;
using Pos.Cli.Menus;
using Pos.Cli.Ui;

const string DataDirectory = "data";
const string DemoDirectory = "data/demo";

Console.OutputEncoding = Encoding.UTF8;
var demo = args.Contains("--demo");

try
{
    if (demo && Directory.Exists(DemoDirectory))
    {
        Directory.Delete(DemoDirectory, recursive: true);
    }

    using var provider = CompositionRoot.Build(demo ? DemoDirectory : DataDirectory);

    if (demo)
    {
        provider.GetRequiredService<DemoScenario>().Run();
    }
    else
    {
        provider.GetRequiredService<MainMenu>().Run();
    }

    return 0;
}
catch (EndOfInputException)
{
    Console.WriteLine();
    Console.WriteLine("Ввод закончился — выход.");
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Непредвиденная ошибка: {ex.Message}");
    return 1;
}
