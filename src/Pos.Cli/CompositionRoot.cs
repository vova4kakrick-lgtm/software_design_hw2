using Microsoft.Extensions.DependencyInjection;
using Pos.Application.Evaluations;
using Pos.Application.Rubrics;
using Pos.Application.Students;
using Pos.Application.Totals;
using Pos.Cli.Demo;
using Pos.Cli.Menus;
using Pos.Cli.Ui;
using Pos.Domain.Repositories;
using Pos.Domain.Services.Totals;
using Pos.Infrastructure.Export;
using Pos.Infrastructure.Persistence;

namespace Pos.Cli;

internal static class CompositionRoot
{
    public static ServiceProvider Build(string dataDirectory)
    {
        var services = new ServiceCollection();

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IRubricRepository>(
            _ => new JsonRubricRepository(Path.Combine(dataDirectory, "rubric.json")));
        services.AddSingleton<IStudentRepository>(sp => new JsonStudentRepository(
            Path.Combine(dataDirectory, "students.json"), sp.GetRequiredService<IRubricRepository>()));
        services.AddSingleton<ITotalsExporter>(
            _ => new JsonTotalsExporter(Path.Combine(dataDirectory, "totals.json")));

        services.AddSingleton<StrictTotalStrategy>();
        services.AddSingleton<CurrentTotalStrategy>();
        services.AddSingleton<TotalRuleFactory>();
        services.AddSingleton<TotalCalculator>();
        services.AddSingleton<StudentService>();
        services.AddSingleton<RubricService>();
        services.AddSingleton<EvaluationService>();
        services.AddSingleton<TotalsService>();

        services.AddSingleton(new ConsoleIo(Console.In, Console.Out));
        services.AddSingleton<ViewPrinter>();
        services.AddSingleton<StudentsMenu>();
        services.AddSingleton<RubricMenu>();
        services.AddSingleton<EvaluationsMenu>();
        services.AddSingleton<TotalsMenu>();
        services.AddSingleton<MainMenu>();
        services.AddSingleton<DemoScenario>();

        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
    }
}
