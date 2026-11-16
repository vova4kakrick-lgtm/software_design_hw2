namespace Pos.Domain.Model;

public sealed record Evaluation
{
    internal Evaluation(CriterionId criterionId, Score score, DateOnly date, string? comment)
    {
        CriterionId = criterionId;
        Score = score;
        Date = date;
        Comment = comment?.Trim() ?? string.Empty;
    }

    public CriterionId CriterionId { get; }

    public Score Score { get; }

    public DateOnly Date { get; }

    /// <summary>Необязательный комментарий; пустая строка, если его нет.</summary>
    public string Comment { get; }
}
