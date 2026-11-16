using Pos.Domain.Model;
using static Pos.Tests.TestData;

namespace Pos.Tests.Domain;

public class StudentTests
{
    private readonly Rubric _rubric = TestData.Rubric();

    [Fact]
    public void Constructor_TrimsName()
    {
        var student = new Student(new StudentId(2), "  Иванов Иван ");

        Assert.Equal(new StudentId(2), student.Id);
        Assert.Equal("Иванов Иван", student.Name);
        Assert.Empty(student.Evaluations);
    }

    [Fact]
    public void Constructor_RejectsEmptyName()
    {
        Assert.Throws<DomainException>(() => new Student(new StudentId(1), " "));
    }

    [Fact]
    public void Record_AddsEvaluation()
    {
        var student = Ivanov();

        student.Record(Evaluation(_rubric, Hw, 8));

        Assert.Equal(8, student.FindEvaluation(Hw)?.Score.Value);
    }

    [Fact]
    public void Record_RejectsSecondEvaluationForSameCriterion()
    {
        var student = Ivanov();
        student.Record(Evaluation(_rubric, Hw, 8));

        Assert.Throws<DomainException>(() => student.Record(Evaluation(_rubric, Hw, 10)));
        Assert.Equal(8, student.FindEvaluation(Hw)?.Score.Value);
    }

    [Fact]
    public void Correct_ReplacesExistingEvaluation()
    {
        var student = Ivanov();
        student.Record(Evaluation(_rubric, Hw, 6));

        student.Correct(Evaluation(_rubric, Hw, 9, "пересдал"));

        var evaluation = Assert.Single(student.Evaluations);
        Assert.Equal(9, evaluation.Score.Value);
        Assert.Equal("пересдал", evaluation.Comment);
    }

    [Fact]
    public void Correct_RejectsMissingEvaluation()
    {
        Assert.Throws<DomainException>(() => Ivanov().Correct(Evaluation(_rubric, Hw, 9)));
    }

    [Fact]
    public void Cancel_RemovesEvaluation()
    {
        var student = Ivanov();
        student.Record(Evaluation(_rubric, Hw, 6));

        student.Cancel(Hw);

        Assert.Null(student.FindEvaluation(Hw));
    }

    [Fact]
    public void Cancel_RejectsMissingEvaluation()
    {
        Assert.Throws<DomainException>(() => Ivanov().Cancel(Hw));
    }
}
