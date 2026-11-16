using Pos.Domain.Model;
using Pos.Domain.Services.Totals;

namespace Pos.Tests.Domain;

public class TotalCalculatorTests
{
    private readonly TotalCalculator _calculator = new();
    private readonly Rubric _rubric = TestData.Rubric();

    [Fact]
    public void Strict_CountsAllCriteriaByWeight()
    {
        var student = Student(hw: 8, exam: 50);

        var total = _calculator.Calculate(student, _rubric, new StrictTotalStrategy());

        Assert.Equal(6.8m, total.Final);
    }

    [Fact]
    public void Strict_TreatsMissingEvaluationAsZero()
    {
        var student = Student(hw: 8);

        var total = _calculator.Calculate(student, _rubric, new StrictTotalStrategy());

        Assert.Equal(4.8m, total.Final);
    }

    [Fact]
    public void Current_IgnoresMissingEvaluations()
    {
        var student = Student(hw: 8);

        var total = _calculator.Calculate(student, _rubric, new CurrentTotalStrategy());

        Assert.Equal(8m, total.Final);
    }

    [Fact]
    public void Current_IsZeroWhenNothingIsGraded()
    {
        var total = _calculator.Calculate(Student(), _rubric, new CurrentTotalStrategy());

        Assert.Equal(0m, total.Final);
    }

    [Fact]
    public void FullScores_GiveTen()
    {
        var total = _calculator.Calculate(Student(hw: 10, exam: 100), _rubric, new StrictTotalStrategy());

        Assert.Equal(10m, total.Final);
    }

    [Fact]
    public void OnlyRelativeWeightsMatter()
    {
        var rubric = new Rubric();
        rubric.AddCriterion("Домашнее задание", 10, new Weight(3m));
        rubric.AddCriterion("Экзамен", 100, new Weight(2m));
        var student = TestData.Ivanov();
        student.Record(TestData.Evaluation(rubric, TestData.Hw, 8));
        student.Record(TestData.Evaluation(rubric, TestData.Exam, 50));

        var total = _calculator.Calculate(student, rubric, new StrictTotalStrategy());

        Assert.Equal(6.8m, total.Final);
    }

    [Theory]
    [InlineData(8, 50, 7)]
    [InlineData(8, 0, 5)]
    [InlineData(5, 30, 4)]
    public void Rounded_RoundsToInteger(int hw, int exam, int expected)
    {
        var rule = new RoundedTotalStrategy(new StrictTotalStrategy());

        var total = _calculator.Calculate(Student(hw, exam), _rubric, rule);

        Assert.Equal(expected, total.Final);
    }

    [Fact]
    public void Rounded_RoundsHalfUp()
    {
        var rubric = new Rubric();
        rubric.AddCriterion("Экзамен", 20, new Weight(1m));
        var student = TestData.Ivanov();
        student.Record(TestData.Evaluation(rubric, new CriterionId(1), 13));

        var total = _calculator.Calculate(student, rubric, new RoundedTotalStrategy(new StrictTotalStrategy()));

        Assert.Equal(7m, total.Final);
    }

    [Fact]
    public void MaxWeights_DoNotOverflow()
    {
        var rubric = new Rubric();
        for (var i = 0; i < 1000; i++)
        {
            rubric.AddCriterion($"Критерий {i + 1}", int.MaxValue, new Weight(Weight.Max));
        }

        var student = TestData.Ivanov();
        student.Record(TestData.Evaluation(rubric, new CriterionId(1), int.MaxValue));

        var total = _calculator.Calculate(student, rubric, new StrictTotalStrategy());

        Assert.Equal(0.01m, total.Final);
    }

    [Fact]
    public void Lines_HaveLineForEveryCriterion()
    {
        var total = _calculator.Calculate(Student(hw: 8), _rubric, new StrictTotalStrategy());

        Assert.Collection(
            total.Lines,
            line => { Assert.Equal(TestData.Hw, line.Criterion.Id); Assert.Equal(8, line.Score?.Value); },
            line => { Assert.Equal(TestData.Exam, line.Criterion.Id); Assert.Null(line.Score); });
    }

    [Fact]
    public void EmptyRubric_IsRejected()
    {
        Assert.Throws<DomainException>(() =>
            _calculator.Calculate(Student(), new Rubric(), new StrictTotalStrategy()));
    }

    private Student Student(int? hw = null, int? exam = null)
    {
        var student = TestData.Ivanov();
        if (hw is not null)
        {
            student.Record(TestData.Evaluation(_rubric, TestData.Hw, hw.Value));
        }

        if (exam is not null)
        {
            student.Record(TestData.Evaluation(_rubric, TestData.Exam, exam.Value));
        }

        return student;
    }
}
