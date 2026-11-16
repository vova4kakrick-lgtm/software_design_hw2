namespace Pos.Domain.Services.Totals;

/// <summary>Строгое правило: нет оценки по критерию — по нему 0.</summary>
public sealed class StrictTotalStrategy : ITotalStrategy
{
    public decimal Calculate(IReadOnlyList<TotalLine> lines) => WeightedAverage.Of(lines);
}
