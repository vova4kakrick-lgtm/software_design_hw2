using Pos.Domain.Model;
using static Pos.Tests.TestData;

namespace Pos.Tests.Domain;

public class RubricTests
{
    private readonly Rubric _rubric = TestData.Rubric();

    [Fact]
    public void AddCriterion_AssignsNextNumber()
    {
        var rubric = new Rubric();

        var hw = rubric.AddCriterion("ДЗ 1", 10, new Weight(0.2m));
        var exam = rubric.AddCriterion("Экзамен", 100, new Weight(0.8m));

        Assert.Equal(new CriterionId(1), hw.Id);
        Assert.Equal(new CriterionId(2), exam.Id);
        Assert.Same(exam, rubric.Find(new CriterionId(2)));
        Assert.Equal(100, exam.MaxScore);
        Assert.Equal(0.8m, exam.Weight.Value);
    }

    [Fact]
    public void Find_ReturnsNullForUnknownNumber()
    {
        Assert.Null(_rubric.Find(new CriterionId(99)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void AddCriterion_RejectsNonPositiveMaxScore(int maxScore)
    {
        Assert.Throws<DomainException>(() => new Rubric().AddCriterion("ДЗ", maxScore, new Weight(0.5m)));
    }

    [Fact]
    public void AddCriterion_RejectsEmptyName()
    {
        Assert.Throws<DomainException>(() => new Rubric().AddCriterion(" ", 10, new Weight(0.5m)));
    }

    [Fact]
    public void Evaluate_ReturnsEvaluationWithAllFields()
    {
        var evaluation = _rubric.Evaluate(Hw, new Score(8), Today, "аккуратно");

        Assert.Equal(Hw, evaluation.CriterionId);
        Assert.Equal(new Score(8), evaluation.Score);
        Assert.Equal(Today, evaluation.Date);
        Assert.Equal("аккуратно", evaluation.Comment);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void Evaluate_AcceptsBoundaryScores(int score)
    {
        Assert.Equal(score, Evaluation(_rubric, Hw, score).Score.Value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    [InlineData(12)]
    public void Evaluate_RejectsScoreOutsideCriterion(int score)
    {
        Assert.Throws<DomainException>(() => Evaluation(_rubric, Hw, score));
    }

    [Fact]
    public void Evaluate_RejectsCriterionNotInRubric()
    {
        Assert.Throws<DomainException>(() => Evaluation(_rubric, new CriterionId(99), 5));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Evaluate_CommentIsOptional(string? comment)
    {
        Assert.Equal(string.Empty, Evaluation(_rubric, Hw, 5, comment).Comment);
    }
}
