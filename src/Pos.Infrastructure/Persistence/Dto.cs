namespace Pos.Infrastructure.Persistence;

internal sealed record RubricDto(IReadOnlyList<CriterionDto>? Criteria);

internal sealed record CriterionDto(int Id, string Name, int MaxScore, decimal Weight);

internal sealed record StudentDto(int Id, string Name, IReadOnlyList<EvaluationDto>? Evaluations);

internal sealed record EvaluationDto(int CriterionId, int Score, DateOnly Date, string? Comment);
