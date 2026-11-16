namespace Pos.Domain.Services.Totals;

/// <summary>Текущее правило: учитываются только критерии, по которым оценка уже выставлена.</summary>
public sealed class CurrentTotalStrategy : ITotalStrategy
{
    public decimal Calculate(IReadOnlyList<TotalLine> lines) =>
        WeightedAverage.Of(lines.Where(l => l.Score is not null).ToList());
}
