namespace Pos.Application.Evaluations;

public sealed record EvaluationView(
    int StudentId,
    string StudentName,
    int CriterionId,
    string CriterionName,
    int Score,
    int MaxScore,
    DateOnly Date,
    string Comment);
