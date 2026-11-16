using System.Text.Json;
using Pos.Application.Common;
using Pos.Application.Evaluations;
using Pos.Application.Rubrics;
using Pos.Application.Students;
using Pos.Application.Totals;
using Pos.Domain.Services.Totals;
using Pos.Infrastructure.Export;
using Pos.Infrastructure.Persistence;

namespace Pos.Tests.Application;

public sealed class ScenarioTests : IDisposable
{
    private const int Ivanov = 1;
    private const int Hw = 1;
    private const int Exam = 2;

    private readonly string _dir = Directory.CreateTempSubdirectory("pos-tests-").FullName;
    private readonly StudentService _students;
    private readonly RubricService _rubric;
    private readonly EvaluationService _evaluations;
    private readonly TotalsService _totals;

    public ScenarioTests()
    {
        (_students, _rubric, _evaluations, _totals) = Services(_dir);

        _rubric.AddCriterion("Домашнее задание", 10, 0.6m);
        _rubric.AddCriterion("Экзамен", 100, 0.4m);
        _students.Add("Иванов Иван");
    }

    public void Dispose() => Directory.Delete(_dir, recursive: true);

    [Fact]
    public void AddCriterion_AssignsNextNumber()
    {
        var added = _rubric.AddCriterion(" Контрольная ", 10, 0.2m);

        Assert.Equal(new CriterionView(3, "Контрольная", 10, 0.2m), added);
        Assert.Equal(3, _rubric.List().Count);
    }

    [Fact]
    public void AddStudent_AssignsNextNumber()
    {
        var added = _students.Add("Петрова Анна");

        Assert.Equal(new StudentView(2, "Петрова Анна"), added);
    }

    [Fact]
    public void AddStudent_RejectsEmptyName()
    {
        Assert.Throws<UseCaseException>(() => _students.Add(" "));
    }

    [Fact]
    public void RecordEvaluation_StoresTodayDate()
    {
        var recorded = _evaluations.Record(Ivanov, Hw, 8, "-");

        Assert.Equal(TestData.Today, recorded.Date);
        Assert.Equal(8, Assert.Single(_evaluations.ListAll()).Score);
    }

    [Fact]
    public void RecordEvaluation_WithoutComment()
    {
        _evaluations.Record(Ivanov, Hw, 8);

        Assert.Equal(string.Empty, Assert.Single(_evaluations.ListAll()).Comment);
    }

    [Fact]
    public void RecordEvaluation_RejectsUnknownStudent()
    {
        Assert.Throws<UseCaseException>(() => _evaluations.Record(99, Hw, 8));
    }

    [Fact]
    public void RecordEvaluation_RejectsScoreAboveMax()
    {
        Assert.Throws<UseCaseException>(() => _evaluations.Record(Ivanov, Hw, 12));
        Assert.Empty(_evaluations.ListAll());
    }

    [Fact]
    public void RecordEvaluation_RejectsUnknownCriterion()
    {
        Assert.Throws<UseCaseException>(() => _evaluations.Record(Ivanov, 99, 5));
    }

    [Fact]
    public void CorrectEvaluation_ReplacesScoreAndComment()
    {
        _evaluations.Record(Ivanov, Hw, 5);

        _evaluations.Correct(Ivanov, Hw, 9, "пересдал");

        var evaluation = Assert.Single(_evaluations.ListAll());
        Assert.Equal(9, evaluation.Score);
        Assert.Equal("пересдал", evaluation.Comment);
    }

    [Fact]
    public void CancelEvaluation_RemovesItFromTotal()
    {
        _evaluations.Record(Ivanov, Hw, 10);
        _evaluations.Record(Ivanov, Exam, 100);

        _evaluations.Cancel(Ivanov, Exam);

        var total = Assert.Single(_totals.ForAll(TotalMode.Strict).Totals);
        Assert.Equal(6m, total.Final);
        Assert.Null(total.Lines.Single(l => l.CriterionId == Exam).Score);
    }

    [Fact]
    public void Totals_DependOnMode_AndAreNotRounded()
    {
        _evaluations.Record(Ivanov, Hw, 8);

        Assert.Equal(4.8m, Assert.Single(_totals.ForAll(TotalMode.Strict).Totals).Final);
        Assert.Equal(8m, Assert.Single(_totals.ForAll(TotalMode.Current).Totals).Final);
    }

    [Fact]
    public void Totals_WithEmptyRubric_FailAsUseCaseError()
    {
        var (students, _, _, totals) = Services(Path.Combine(_dir, "no-rubric"));
        students.Add("Иванов Иван");

        Assert.Throws<UseCaseException>(() => totals.ForAll(TotalMode.Strict));
    }

    [Fact]
    public void Export_WritesRoundedTotals()
    {
        _students.Add("Петрова Анна");
        _evaluations.Record(Ivanov, Hw, 8);
        _evaluations.Record(Ivanov, Exam, 50);

        var result = _totals.Export(TotalMode.Strict);

        Assert.Equal(Path.Combine(_dir, "totals.json"), result.Location);
        Assert.True(result.Report.Rounded);
        Assert.Equal(7m, result.Report.Totals[0].Final);
        using var json = JsonDocument.Parse(File.ReadAllText(result.Location));
        Assert.Equal("strict", json.RootElement.GetProperty("rule").GetString());
        var totals = json.RootElement.GetProperty("totals");
        Assert.Equal(7, totals[0].GetProperty("final").GetInt32());
        Assert.Equal(50, totals[0].GetProperty("scores")[1].GetProperty("score").GetInt32());
        Assert.Equal(0, totals[1].GetProperty("final").GetInt32());
    }

    [Fact]
    public void Export_RejectsEmptyStudentList()
    {
        var (_, _, _, totals) = Services(Path.Combine(_dir, "empty"));

        Assert.Throws<UseCaseException>(() => totals.Export(TotalMode.Strict));
    }

    private static (StudentService, RubricService, EvaluationService, TotalsService) Services(string dir)
    {
        var rubrics = new JsonRubricRepository(Path.Combine(dir, "rubric.json"));
        var students = new JsonStudentRepository(Path.Combine(dir, "students.json"), rubrics);

        return (
            new StudentService(students),
            new RubricService(rubrics),
            new EvaluationService(students, rubrics, new FixedTime(TestData.Today)),
            new TotalsService(
                students,
                rubrics,
                new TotalCalculator(),
                TestRules.Factory(),
                new JsonTotalsExporter(Path.Combine(dir, "totals.json"))));
    }
}
