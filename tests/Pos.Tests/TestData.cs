using Pos.Application.Totals;
using Pos.Domain.Model;
using Pos.Domain.Services.Totals;

namespace Pos.Tests;

internal static class TestData
{
    public static readonly DateOnly Today = new(2026, 11, 16);

    public static readonly CriterionId Hw = new(1);

    public static readonly CriterionId Exam = new(2);

    /// <summary>№1 — ДЗ (макс. 10, вес 0.6), №2 — экзамен (макс. 100, вес 0.4).</summary>
    public static Rubric Rubric()
    {
        var rubric = new Rubric();
        rubric.AddCriterion("Домашнее задание", 10, new Weight(0.6m));
        rubric.AddCriterion("Экзамен", 100, new Weight(0.4m));
        return rubric;
    }

    public static Student Ivanov() => new(new StudentId(1), "Иванов Иван");

    public static Evaluation Evaluation(Rubric rubric, CriterionId criterionId, int score, string? comment = null) =>
        rubric.Evaluate(criterionId, new Score(score), Today, comment);
}

internal static class TestRules
{
    public static TotalRuleFactory Factory() => new(new StrictTotalStrategy(), new CurrentTotalStrategy());
}

internal sealed class FixedTime(DateOnly today) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => new(today.ToDateTime(new TimeOnly(12, 0)), TimeSpan.Zero);

    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
}
